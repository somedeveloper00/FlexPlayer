using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using UnityEngine;
using UnityEngine.Android;
using Debug = UnityEngine.Debug;

namespace FlexPlayer.Utils
{
	public static class MusicIOUtils
	{
		public static List<string> paths = new() {
			Environment.GetFolderPath( Environment.SpecialFolder.MyMusic ),
			Environment.GetFolderPath( Environment.SpecialFolder.CommonMusic ),
			"/storage/emulated/0/Music",
		};

		public struct FindSettings
		{ }

		public static IEnumerator GetAllMusics(Action<MusicData> onFind, FindSettings settings = default) {

			// grant permission to read files
			if ( !Permission.HasUserAuthorizedPermission( Permission.ExternalStorageRead ) ) {
				Debug.Log( "no permission. asking now..." );
				bool can_continue = false;
				PermissionCallbacks callback = new PermissionCallbacks();
				callback.PermissionGranted += (_) => can_continue = true;
				callback.PermissionDenied += (_) => Debug.Log( "lol" );
				callback.PermissionDeniedAndDontAskAgain += (_) => Debug.Log( "lollll" );
				Permission.RequestUserPermission( Permission.ExternalStorageRead, callback );

				while (!can_continue) yield return null;

				if ( !Permission.HasUserAuthorizedPermission( Permission.ExternalStorageRead ) ) {
					Debug.Log( "didn't have permission" );
					yield break;
				}
			}

			int frameTime = Mathf.FloorToInt( 1f / 60 * 1000 );

			var st = new Stopwatch();
			st.Start();
			int c = 0;

			foreach ( var path in paths ) {
				if ( !Directory.Exists( path ) ) {
					Debug.Log( $"directory {path} doesn't exist. skipping." );
					continue;
				}

				Debug.Log( $"searching folder {path}" );
				var enumOptions = new EnumerationOptions {
					RecurseSubdirectories = true,
				};
				IEnumerable<string> filePaths;
				try {
					// filePaths = Directory.GetFiles( path, "*.mp3", SearchOption.AllDirectories );
					filePaths = Directory.EnumerateFiles( path, "*.mp3", enumOptions );
				}
				catch (Exception e) {
					Debug.LogException( e );
					throw;
				}
				foreach ( var filePath in filePaths ) {
					Debug.Log( $"processing {filePath}" );
					var musicData = new MusicData( filePath );
					onFind?.Invoke( musicData );
					// if took more than 1 frame, yield
					c++;
					if ( st.ElapsedMilliseconds > frameTime ) {
						Debug.Log( $"{c}. {st.ElapsedMilliseconds}" );
						yield return null;
						c = 0;
						st.Restart();
					}
				}
			}

			st.Stop();
			Debug.Log( $"total elapsed: {st.ElapsedMilliseconds}" );
		}
	}
}