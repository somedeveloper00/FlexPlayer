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
		[SerializeField] Canvas canvas;
		
		MusicData _data;
		Action<MusicData> _onPlay;
		Coroutine disable_coroutine;

		public bool IsShown => canvas.enabled;
		public bool IsLoaded { get; private set; } = false;

		public void Setup(MusicData data, Action<MusicData> onPlay) {
			_data = data;
			_onPlay = onPlay;

			rectTransform = GetComponent<RectTransform>();
		}

		public void Show() {
			if ( _data == null ) return;
			LoadData();
			canvas.enabled = true;
			title.text = _data.title;
			artist.text = _data.artist;
			updatePlayImg();
			for ( int i = 0; i < 5; i++ ) {
				stars[i].sprite = _data.stars > i ? SpriteInventory.Instance.star_fill : SpriteInventory.Instance.star_empty;
			}
			IsLoaded = true;

			// stop hiding process
			if ( disable_coroutine != null )
				CoroutineMaker.stopCoroutine( disable_coroutine );
		}

		public void LoadData() {
			if ( IsLoaded ) return;
			_data.LoadData();
			IsLoaded = true;
		}

		void updatePlayImg() {
			playStopImage.sprite = _data.playing ? SpriteInventory.Instance.stop : SpriteInventory.Instance.play;
		}

		public void Hide() {
			
			canvas.enabled = false;
			return;
			if ( _data == null ) return;
			if ( !IsLoaded ) return;
			
			
			if ( disable_coroutine != null )
				CoroutineMaker.stopCoroutine( disable_coroutine );
			disable_coroutine = CoroutineMaker.startCoroutine( coroutine() );

			// wait for some time, if still disabled, then unload
			IEnumerator coroutine() {
				yield return new WaitForSecondsRealtime( 60 );
				_data.UnloadData();
				IsLoaded = false;
			}
		}

		public void Play() {
			_onPlay?.Invoke( _data );
			_data.playing = !_data.playing;
			updatePlayImg();
		}
	}
}