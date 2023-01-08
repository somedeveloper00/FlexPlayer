using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace FlexPlayer.MusicList
{
    public class MusicList : MonoBehaviour
    {
        [SerializeField] MusicElementPoolList poolList;
        [SerializeField] RectTransform container;
        [SerializeField] float elementHeight;
        
        [Header("Settings")]
        [SerializeField] int maxItems = 100;
        [SerializeField] bool playable = true;

        List<Item> items = new List<Item>( 128 );
        Action on_unity_thread;
        readonly Stopwatch sw = new Stopwatch();
        int items_pool_indexer;

        void Start() {
            StartCoroutine( safe_coroutine() );
            for ( int i = 0; i < poolList.Count; i++ ) {
                poolList[i].Init();
            }
        }

        void Update() {
            const long max_ms_extra = 8; // extra load in less than specified milliseconds
            sw.Restart();

            int max = Screen.height + 500;
            int min = 0 - 500;
            for ( int i = 0; i < items.Count; i++ ) {
                
                // check if inside screen and setup views from pool
                var y = container.position.y + items[i].y;
                // draw debug line for Y and min and max
                Debug.DrawLine( new Vector3( 0, y, 0 ), new Vector3( 100, y, 0 ), Color.red );
                Debug.DrawLine( new Vector3( 0, min, 0 ), new Vector3( 100, min, 0 ), Color.green );
                Debug.DrawLine( new Vector3( 0, max, 0 ), new Vector3( 100, max, 0 ), Color.blue );
                var visible = !(y > max || y < min);
                if ( visible ) {
                    if ( items[i].current == null ) {
                        Debug.Log( $"taking from pool for {items[i].data.path}" );
                        // take new from pool
                        items[i].current = poolList.GetInactive();
                        items[i].current.Setup( items[i].data, Play );
                        items[i].current.Activate();
                        items[i].current.rectTransform.position = new Vector2(
                            items[i].current.rectTransform.position.x,
                            y );
                    }
                }
                else {
                    if ( items[i].current != null ) { // return to pool
                        items[i].current.Deactivate();
                        items[i].current = null;
                    }
                }

                // // load extra items
                // if ( sw.ElapsedMilliseconds < max_ms_extra ) {
                //     if ( !items[i].data.Loaded && !items[i].data.IsLoading ) {
                //         items[i].data.LoadMetaData();
                //     }
                // }
            }
        }

        
        /// <summary>
        /// adds music data array to the displaying list, given the array's length
        /// </summary>
        public void Add(MusicData[] musicDatas, int count) {
            Debug.Log( $"added {count} musics" );
            for ( int i = 0; i < count; i++ ) 
                add( musicDatas[i] );
        }

        /// <summary>
        /// Removes an element from the displaying list
        /// </summary>
        public void Remove(MusicData musicData) {
            for ( int i = 0; i < items.Count; i++ ) {
                if ( items[i].data == musicData ) {
                    if ( items[i].current != null ) items[i].current.Deactivate();
                    items.RemoveAt( i );
                    return;
                }
            }
        }

        /// <summary>
        /// clears the displaying list
        /// </summary>
        public void Clear() {
            for ( int i = 0; i < items.Count; i++ ) {
                if ( items[i].current != null )
                    items[i].current.Deactivate();
            }
            items.Clear();
        }


        void add(MusicData data) {
            if ( items.Count >= maxItems )
                return;
            on_unity_thread += () => {
                Item item = new ();
                item.data = data;
                item.y = -items.Count * elementHeight;
                items.Add( item );
                // expand container size for scroll-view
                setContainerSize( items.Count );
            };
        }

        void setContainerSize(int itemsCount) {
            container.sizeDelta = new Vector2( container.sizeDelta.x, (1 + itemsCount) * elementHeight );
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
            setContainerSize( poolList.Count );
        }
        
        IEnumerator safe_coroutine() {
            while (true) {
                yield return null;
                on_unity_thread?.Invoke();
                on_unity_thread = null;
            }
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