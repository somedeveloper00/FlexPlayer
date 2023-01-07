using System;
using System.Collections;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace FlexPlayer
{
	public class MusicElement : MonoBehaviour
	{
		public RectTransform rectTransform { get; private set; }
		
		[SerializeField] RawImage cover;
		[SerializeField] TMP_Text title;
		[SerializeField] TMP_Text artist;
		[SerializeField] Image playStopImage;
		[SerializeField] Image[] stars;

		MusicData _data;
		Action<MusicData> _onPlay;
		bool loaded = false;
		bool waiting_unload = false;

		public void Setup(MusicData data, Action<MusicData> onPlay) {
			_data = data;
			_onPlay = onPlay;

			rectTransform = GetComponent<RectTransform>();
			
			if (gameObject.activeSelf) OnEnable();
		}

		void OnEnable() {
			if ( _data == null ) return;
			if ( loaded ) return;
			_data.LoadData();
			title.text = _data.title;
			artist.text = _data.artist;
			updatePlayImg();
			for ( int i = 0; i < 5; i++ ) {
				stars[i].sprite = _data.stars > i ? SpriteInventory.Instance.star_fill : SpriteInventory.Instance.star_empty;
			}
			loaded = true;
		}

		void updatePlayImg() {
			playStopImage.sprite = _data.playing ? SpriteInventory.Instance.stop : SpriteInventory.Instance.play;
		}

		void OnDisable() {
			return;
			if ( _data == null ) return;
			if ( !loaded ) return;
			if ( waiting_unload ) return;
			CoroutineMaker.startCoroutine( coroutine() );
			
			// wait for some time, if still disabled, then unload
			IEnumerator coroutine() {
				waiting_unload = true;
				yield return new WaitForSecondsRealtime( 60 );
				if ( !gameObject.activeSelf ) {
					_data.UnloadData();
					loaded = false;
				}
			}
		}

		public void Play() {
			_onPlay?.Invoke( _data );
			_data.playing = !_data.playing;
			updatePlayImg();
		}
	}
}