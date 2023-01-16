using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;

namespace FlexPlayer.MusicList
{
    public class MusicList : MonoBehaviour
    {
        [SerializeField] MusicElementPoolList poolList;
        [SerializeField] RectTransform container;
        [SerializeField] float elementHeight;

        [Header( "Page" )] 
        [SerializeField] PagePanel pagePanel;
        [SerializeField] TMP_Text pageIndicatorTxt;
        [SerializeField] string pageIndicatorFormat = "Page {0}/{1}";
        [SerializeField] Button pageNext, pagePrevious;
        
        [Header( "Settings" )] 
        [SerializeField] int itemsPerPage = 100;
        [SerializeField] float minY, maxY;
        [SerializeField] bool playable = true;
        
        [Header("Animations")]
        [SerializeField] Animator animator;
        [SerializeField] string pageOpenAnimName = "page-open";
        [SerializeField] string pageCloseAnimName = "page-close";

        List<Page> _pages = new List<Page>( 4 );
        int _currentPageIndex = 0;
        bool _pagePanelOpen;
        
        Action on_unity_thread;


        void Start() {
            StartCoroutine( safe_coroutine() );
            for ( int i = 0; i < poolList.Count; i++ ) {
                poolList[i].Init();
            }
        }

        void Update() {
            if ( _pages.Count == 0 ) return;
            const long max_ms_extra = 8; // extra load in less than specified milliseconds

            var items = _pages[_currentPageIndex].items;

            // check for items to deactivate
            for ( int i = 0; i < items.Count; i++ ) {
                var y = container.anchoredPosition.y + items[i].y;
                var visible = !(y > maxY || y < minY);
                if ( !visible ) {
                    if ( items[i].current != null ) { // return to pool
                        items[i].current.Deactivate();
                        items[i].current = null;
                    }
                }
            }

            // check for items to activate
            for ( int i = 0; i < items.Count; i++ ) {
                var y = container.anchoredPosition.y + items[i].y;
                var visible = !(y > maxY || y < minY);
                if ( visible ) {
                    if ( items[i].current == null ) {
                        // take new from pool
                        items[i].current = poolList.GetInactive();
                        items[i].current.Setup( items[i].data, Play );
                        items[i].current.Activate();
                        items[i].current.rectTransform.anchoredPosition = new Vector2(
                            items[i].current.rectTransform.anchoredPosition.x,
                            items[i].y );
                    }
                }
            }
        }


        /// <summary>
        /// adds music data array to the displaying list, given the array's length
        /// </summary>
        public void Add(MusicData[] musicDatas, int count) {
            Debug.Log( $"added {count} musics" );
            on_unity_thread += () => {
                for ( int i = 0; i < count; i++ )
                    add( musicDatas[i] );
                updatePageView();
            };
        }

        /// <summary>
        /// Removes an element from the displaying list
        /// </summary>
        public void Remove(MusicData musicData) {
            for ( int k = 0; k < _pages.Count; k++ ) {
                var items = _pages[k].items;
                for ( int i = 0; i < items.Count; i++ ) {
                    if ( items[i].data == musicData ) {
                        if ( items[i].current != null ) items[i].current.Deactivate();
                        items.RemoveAt( i );
                        return;
                    }
                }
            }

            updatePageView();
            updateContainerSize();
        }

        /// <summary>
        /// clears the displaying list
        /// </summary>
        public void Clear() {
            foreach ( var page in _pages ) {
                var items = page.items;
                for ( int i = 0; i < items.Count; i++ ) {
                    if ( items[i].current != null )
                        items[i].current.Deactivate();
                }

                items.Clear();
            }

            updatePageView();
            updateContainerSize();
        }

        public void TogglePagePannel() {
            if ( _pagePanelOpen ) {
                animator.Play( pageCloseAnimName );
                _pagePanelOpen = false;
            }
            else {
                animator.Play( pageOpenAnimName );
                _pagePanelOpen = true;
            }
        }
        
        public void GotoPage(int number) {
            if ( number < 0 || number > _pages.Count - 1 ) {
                return;
            }
            // remove all views from views in the page
            var items = _pages[_currentPageIndex].items;
            for ( int i = 0; i < items.Count; i++ ) {
                items[i].current?.Deactivate();
                items[i].current = null;
            }
            _currentPageIndex = number;
            updatePageView();
            updateContainerSize();
            
        }
        
        public void NextPage() => GotoPage( _currentPageIndex + 1 );
        public void PreviousPage() => GotoPage( _currentPageIndex - 1 );

        void updatePageView() {
            pageIndicatorTxt.text = string.Format( pageIndicatorFormat, _currentPageIndex + 1, _pages.Count );
            pageNext.interactable = _pages.Count - 1 > _currentPageIndex;
            pagePrevious.interactable = _currentPageIndex > 0;
        }

        void add(MusicData data) {
            Item item = new();
            if ( _pages.Count == 0 || _pages[^1].items.Count == itemsPerPage ) {
                // new page 
                _pages.Add( new Page() );
                pagePanel.SetButtonCount( _pages.Count );
            }

            var page = _pages[^1];
            item.data = data;
            item.y = -page.items.Count * elementHeight;
            page.items.Add( item );
            // expand container size for scroll-view
            updateContainerSize();
        }

        void updateContainerSize() {
            container.sizeDelta =
                new Vector2( container.sizeDelta.x, _pages[_currentPageIndex].items.Count * elementHeight );
        }

        void Play(MusicData musicData) {
            if ( playable ) {
                MusicPlayer.Player.instance.Play( musicData );
            }
        }

        [ContextMenu("Place Pools In Position")]
        void placePoolsInPosition() {
            for ( int i = 0; i < poolList.Count; i++ ) {
                poolList[i].GetComponent<RectTransform>().anchoredPosition = new Vector2(
                    poolList[i].GetComponent<RectTransform>().anchoredPosition.x,
                    -i * elementHeight );
            }
        }
        
        IEnumerator safe_coroutine() {
            while (true) {
                yield return null;
                on_unity_thread?.Invoke();
                on_unity_thread = null;
            }
        }

        [Serializable]
        class Page
        {
            public List<Item> items = new List<Item>();
        }
        
        [Serializable]
        class Item
        {
            public MusicElement current;
            public MusicData data;
            public float y;
        }
    }
}