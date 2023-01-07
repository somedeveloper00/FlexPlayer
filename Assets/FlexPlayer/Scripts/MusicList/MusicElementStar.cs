using UnityEngine;
using UnityEngine.UI;

namespace FlexPlayer.MusicList
{
	public class MusicElementStar : MonoBehaviour
	{
		public Button button { get; private set; }
		public Image image { get; private set; }

		void OnEnable() {
			button = GetComponent<Button>();
			image = GetComponent<Image>();
		}
	}
}