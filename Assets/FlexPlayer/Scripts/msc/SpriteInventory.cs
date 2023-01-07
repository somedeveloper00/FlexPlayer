using System;
using UnityEngine;

namespace FlexPlayer
{
	[CreateAssetMenu( menuName = "SpriteInventory", order = 0 )]
	public class SpriteInventory : ScriptableObject
	{
		public static SpriteInventory Instance;
		
		public Sprite play, stop, star_fill, star_empty;

		void OnEnable() {
			Debug.Log( "sprite inventory loaded" );
			Instance = this;
		}
	}
}