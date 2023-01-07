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
		Coroutine disable_coroutine;

		public bool loaded { get; private set; }
		
		bool visible;
		
		public bool IsVisible {
			get => visible;
			set {
				visible = value;
				if ( visible ) {
					if (!loaded) Load();
					Show();
				}
			}
		}

		public void Load() {
			_data.LoadData();
			loaded = true;
		}


		public void Setup(MusicData data, Action<MusicData> onPlay) {
			_data = data;
			_onPlay = onPlay;

			rectTransform = GetComponent<RectTransform>();
		}

		void Show() {
			gameObject.SetActive( true );
			title.text = _data.title;
			artist.text = _data.artist;
			updatePlayImg();
			for ( int i = 0; i < 5; i++ ) {
				stars[i].sprite = _data.stars > i ? SpriteInventory.Instance.star_fill : SpriteInventory.Instance.star_empty;
			}
		}

		void updatePlayImg() {
			playStopImage.sprite = _data.playing ? SpriteInventory.Instance.stop : SpriteInventory.Instance.play;
		}

		public void Play() {
			_onPlay?.Invoke( _data );
			_data.playing = !_data.playing;
			updatePlayImg();
		}
	}
}