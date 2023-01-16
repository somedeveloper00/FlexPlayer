using System;
using UnityEditor;

namespace FlexPlayer.MusicList
{
	[CustomEditor( typeof(MusicList) )]
	public class MusicListEditor : UnityEditor.Editor
	{
		MusicList ins;

		void OnEnable() {
			ins = target as MusicList;
		}

		public override void OnInspectorGUI() {
			base.OnInspectorGUI();
			EditorGUILayout.Space( 10 );
			EditorGUI.BeginDisabledGroup( true );
			EditorGUI.EndDisabledGroup();
		}
	}
}