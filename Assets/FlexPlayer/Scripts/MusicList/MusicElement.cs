using System;
using System.Security.Cryptography;
using System.Threading.Tasks;
using FlexPlayer;
using FlexPlayer.Pool;
using FlexPlayer.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace FlexPlayer.MusicList
{
	public class MusicElement : MonoBehaviour, IPoolable
	{
		public RectTransform rectTransform { get; private set; }
		
		[SerializeField] RawImage cover;
		[SerializeField] TMP_Text title;
		[SerializeField] TMP_Text artist;
		[SerializeField] Image playStopImage;
		[SerializeField] MusicElementStar[] stars;

		Action<MusicData> _onPlay;
		Coroutine disable_coroutine;
		bool _isActive;

		public MusicData Data { get; private set; }


		public bool IsActive() => _isActive;
		
		public void Setup(MusicData data, Action<MusicData> onPlay) {
			Data = data;
			_onPlay = onPlay;

			for ( int i = 0; i < stars.Length; i++ ) {
				var index = i;
				stars[i].button.onClick.RemoveAllListeners();
				stars[i].button.onClick.AddListener(() => {
					Data.rate = index + 1;
					MusicIOUtils.SaveCachedAsync();
					setStars();
				});
			}
		}

		public void PlayMusic() {
			_onPlay?.Invoke( Data );
			Data.playing = !Data.playing;
			updatePlayImg();
		}

		public void Init() {
			rectTransform = GetComponent<RectTransform>();
			foreach ( var star in stars ) star.Init();
		}

		public async void Activate() {
			if ( Data.IsLoading ) {
				while (Data.IsLoading) await Task.Yield();
			}

			if ( !Data.Loaded ) {
				await Data.LoadMetaData();
			}
			assignFromData();
			gameObject.SetActive( true );
			_isActive = true;
		}

		public void Deactivate() {
			gameObject.SetActive( false );
			_isActive = false;
		}

		public IPoolable Duplicate() {
			var go = Instantiate( gameObject, transform.parent );
			var element = go.GetComponent<MusicElement>();
			element.Init();
			return element;
		}

		void assignFromData() {
			title.text = Data.title;
			artist.text = Data.artist;
			updatePlayImg();
			setStars();
		}
		
		void setStars() {
			for ( int i = 0; i < 5; i++ ) {
				stars[i].image.sprite = Data.rate > i ? SpriteInventory.Instance.star_fill : SpriteInventory.Instance.star_empty;
			}
		}

		void updatePlayImg() {
			playStopImage.sprite = Data.playing ? SpriteInventory.Instance.stop : SpriteInventory.Instance.play;
		}
	}
}
