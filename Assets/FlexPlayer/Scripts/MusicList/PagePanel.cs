using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace FlexPlayer.MusicList
{
	public class PagePanel : MonoBehaviour
	{
		[SerializeField] MusicList musicList;
		[SerializeField] Button pageButtonSample;
		[SerializeField] string pageButtonTextFormat = "Page {0}";

		List<Button> _buttons = new List<Button>();

		public void SetButtonCount(int count) {
			if ( _buttons.Count > count ) {
				for ( int i = count; i < _buttons.Count; i++ ) {
					Destroy( _buttons[i].gameObject );
				}
			}
			else if ( _buttons.Count < count ) {
				for ( int i = _buttons.Count; i < count; i++ ) {
					var button = Instantiate( pageButtonSample, pageButtonSample.transform.parent );
					button.gameObject.SetActive( true );
					var index = i;
					button.onClick.AddListener( () => onPageButtonClick( index ) );
					button.GetComponentInChildren<TMP_Text>().text = string.Format( pageButtonTextFormat, index + 1 ); 
					_buttons.Add( button );
				}
			}
		}

		void onPageButtonClick(int number) {
			musicList.GotoPage( number );
		}
	}
}