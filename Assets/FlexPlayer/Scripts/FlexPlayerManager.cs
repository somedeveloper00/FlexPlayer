using FlexPlayer.Utils;
using UnityEngine;

namespace FlexPlayer
{
	public class FlexPlayerManager : MonoBehaviour
	{
		public static FlexPlayerManager Instance;
		[SerializeField] MusicList.MusicList mainList;

		void Awake() {
			if ( Instance != null ) Destroy( gameObject );
			Instance = this;
			DontDestroyOnLoad( gameObject );
		}

		void Start() {
			showMainList();
		}

		void showMainList() {
			MusicIOUtils.GetAllMusics( (batch, count) => mainList.Add( batch, count ) );
		}

	}
}