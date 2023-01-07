using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using FlexPlayer.Utils;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace FlexPlayer
{
    public class MusicListView : MonoBehaviour
    {
        [SerializeField] List<MusicElement> itemSamples;
        
        [SerializeField] RectTransform container;
        [SerializeField] float elementHeightDiff;
        [SerializeField] int maxItems = 100;
        
        List<MusicElement> elements = new List<MusicElement>();

        Action on_unity_thread;
        Stopwatch sw = new Stopwatch();

        void Start() {
            StartCoroutine( coroutine() );
            UpdateList();
        }

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

        void UpdateList() {
            // destroy old items
            foreach (var selectable in elements)
                Destroy(selectable.gameObject);
            elements.Clear();

            // create new items
            StartCoroutine( MusicIOUtils.GetAllMusics( onMusicFind ) );
            
        }

        void onMusicFind(MusicData data) {
            if ( elements.Count >= maxItems )
                return;
            on_unity_thread += () => {
                MusicElement ins;
                if ( itemSamples.Count > 0 ) {
                    // take from last
                    ins = itemSamples[^1];
                    itemSamples.RemoveAt( itemSamples.Count - 1 );
                }
                else {
                    ins = Instantiate( elements[0], container );
                }

                ins.Setup( data, Play );
                ins.rectTransform.anchoredPosition = new Vector3( ins.rectTransform.anchoredPosition.x, -elements.Count * elementHeightDiff, 0 );
                container.sizeDelta = new Vector2( container.sizeDelta.x, (1 + elements.Count) * elementHeightDiff );

                elements.Add( ins );
            };
        }

        void Play(MusicData musicData) {
            MusicPlayer.instance.Play( musicData );
        }

        IEnumerator coroutine() {
            while (true) {
                yield return null;
                on_unity_thread?.Invoke();
                on_unity_thread = null;
            }
        }
    }
}