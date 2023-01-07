using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace FlexPlayer.MusicList
{
    public class MusicList : MonoBehaviour
    {
        [SerializeField] List<MusicElement> itemsPool;
        [SerializeField] RectTransform container;
        [SerializeField] float elementHeight;
        
        [Header("Settings")]
        [SerializeField] int maxItems = 100;
        [SerializeField] bool playable = true;
        
        List<MusicElement> elements = new ();

        Action on_unity_thread;
        Stopwatch sw = new Stopwatch();

        /// <summary>
        /// adds music data array to the displaying list, given the array's length
        /// </summary>
        public void Add(MusicData[] musicDatas, int count) {
            for ( int i = 0; i < count; i++ ) 
                add( musicDatas[i] );
        }

        /// <summary>
        /// Removes an element from the displaying list
        /// </summary>
        public void Remove(MusicData musicData) {
            for ( int i = 0; i < elements.Count; i++ ) {
                if ( elements[i].Data == musicData ) {
                    elements[i].gameObject.SetActive( false );
                    itemsPool.Add( elements[i] );
                    elements.RemoveAt( i );
                }
            }
        }

        /// <summary>
        /// clears the displaying list
        /// </summary>
        public void Clear() {
            for ( int i = 0; i < elements.Count; i++ ) {
                itemsPool.Add( elements[i] );
                elements[i].gameObject.SetActive( false );
            }

            elements.Clear();
        }


        void Start() => StartCoroutine( safe_coroutine() );

        void Update() {
            const long max_ms_extra = 8; // extra load in less than specified milliseconds
            sw.Restart();

            int max = Screen.height + 500;
            int min = 0 - 500;
            for ( int i = 0; i < elements.Count; i++ ) {
                // check if inside screen
                var y = elements[i].rectTransform.position.y;
                var visible = !(y > max || y < min);
                if ( elements[i].IsVisible != visible ) {
                    elements[i].IsVisible = visible;
                }

                // load extra items
                if ( sw.ElapsedMilliseconds < max_ms_extra ) {
                    if ( !elements[i].loaded ) {
                        elements[i].Load();
                    }
                }
            }
        }

        void add(MusicData data) {
            if ( elements.Count >= maxItems )
                return;
            on_unity_thread += () => {
                MusicElement ins;
                if ( itemsPool.Count > 0 ) {
                    // take from last
                    ins = itemsPool[^1];
                    itemsPool.RemoveAt( itemsPool.Count - 1 );
                }
                else {
                    ins = Instantiate( elements[0], container );
                }

                ins.Setup( data, Play );
                ins.rectTransform.anchoredPosition = new Vector3( ins.rectTransform.anchoredPosition.x, -elements.Count * elementHeight, 0 );
                container.sizeDelta = new Vector2( container.sizeDelta.x, (1 + elements.Count) * elementHeight );

                elements.Add( ins );
            };
        }

        void Play(MusicData musicData) {
            if ( playable ) {
                MusicPlayer.Player.instance.Play( musicData );
            }
        }

        IEnumerator safe_coroutine() {
            while (true) {
                yield return null;
                on_unity_thread?.Invoke();
                on_unity_thread = null;
            }
        }
    }
}