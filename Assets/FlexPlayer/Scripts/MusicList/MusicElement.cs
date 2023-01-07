using System;
using FlexPlayer;
using FlexPlayer.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace FlexPlayer.MusicList
{
	public class MusicElement : MonoBehaviour
	{
		public RectTransform rectTransform { get; private set; }
		
		[SerializeField] RawImage cover;
		[SerializeField] TMP_Text title;
		[SerializeField] TMP_Text artist;
		[SerializeField] Image playStopImage;
		[SerializeField] MusicElementStar[] stars;

		Action<MusicData> _onPlay;
		Coroutine disable_coroutine;

		public MusicData Data { get; private set; }
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
			Data.LoadMetaData();
			loaded = true;
		}


		public void Setup(MusicData data, Action<MusicData> onPlay) {
			gameObject.SetActive( true );
			Data = data;
			_onPlay = onPlay;

			rectTransform = GetComponent<RectTransform>();
			for ( int i = 0; i < stars.Length; i++ ) {
				var index = i;
				stars[i].button.onClick.AddListener(() => {
					Data.rate = index + 1;
					MusicIOUtils.SaveCachedAsync();
					setStars();
				});
			}
		}

		void setStars() {
			for ( int i = 0; i < 5; i++ ) {
				stars[i].image.sprite = Data.rate > i ? SpriteInventory.Instance.star_fill : SpriteInventory.Instance.star_empty;
			}
		}
		void Show() {
			gameObject.SetActive( true );
			title.text = Data.title;
			artist.text = Data.artist;
			updatePlayImg();
			setStars();
		}

		void updatePlayImg() {
			playStopImage.sprite = Data.playing ? SpriteInventory.Instance.stop : SpriteInventory.Instance.play;
		}

		public void Play() {
			_onPlay?.Invoke( Data );
			Data.playing = !Data.playing;
			updatePlayImg();
		}
	}
}