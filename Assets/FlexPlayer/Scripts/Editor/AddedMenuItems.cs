using System.Diagnostics;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace FlexPlayer.Editor
{
	public static class AddedMenuItems
	{
		[MenuItem("Tools/PersistentDataPath/Open")]
		static void openPersistentDataPath() {
			Process.Start( Application.persistentDataPath );
		}

		[MenuItem( "Tools/PersistentDataPath/Clear" )]
		static void clearPersistentDataPath() {
			Directory.Delete( Application.persistentDataPath, true );
		}
	}
}