using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Android;
using Debug = UnityEngine.Debug;

namespace FlexPlayer.Utils
{
	public static class MusicIOUtils
	{
		public static List<string> musicPaths = new() {
			Environment.GetFolderPath( Environment.SpecialFolder.MyMusic ),
			Environment.GetFolderPath( Environment.SpecialFolder.CommonMusic ),
			"/storage/emulated/0/Music",
		};
		
		static string cachePath = Path.Combine( Application.persistentDataPath, "_cache.dat" );

		public static List<MusicData> MusicDatas = new List<MusicData>( 512 );

		static bool saving = false;
		static bool cancel_saving = false;

		public class FindSettings
		{
			public int batchCount = 256;
		}

		public delegate void OnBatchFound(MusicData[] batch, int count);

		public static async void GetAllMusics(OnBatchFound onBatchFind, FindSettings settings = null) {

			MusicDatas.Clear();
			var st = new Stopwatch();
			st.Start();
			
			settings ??= new FindSettings();
			MusicData[] batch = new MusicData[settings.batchCount];
			int batch_index = 0;
			
			// grant permission to read files
			if ( !Permission.HasUserAuthorizedPermission( Permission.ExternalStorageRead ) ) {
				Debug.Log( "no permission. asking now..." );
				bool can_continue = false;
				PermissionCallbacks callback = new PermissionCallbacks();
				callback.PermissionGranted += (_) => can_continue = true;
				callback.PermissionDenied += (_) => Debug.Log( "lol" );
				callback.PermissionDeniedAndDontAskAgain += (_) => Debug.Log( "lollll" );
				Permission.RequestUserPermission( Permission.ExternalStorageRead, callback );
				
				while ( !can_continue ) await Task.Yield();

				if ( !Permission.HasUserAuthorizedPermission( Permission.ExternalStorageRead ) ) {
					Debug.Log( "didn't have permission" );
					return;
				}
			}
			
			// read caches
			if ( !Directory.Exists( Application.persistentDataPath ) )
				Directory.CreateDirectory( Application.persistentDataPath );
			using (var file = new FileStream( cachePath, FileMode.OpenOrCreate, FileAccess.Read )) {
				using (var reader = new StreamReader( file )) {
					while (!reader.EndOfStream) {
						var line = await reader.ReadLineAsync();
						var musicData = MusicData.Deserialize( line );
						if ( System.IO.File.Exists( musicData.path ) ) {
							MusicDatas.Add( musicData );
							batch[batch_index++] = musicData;
							if ( batch_index == batch.Length ) {
								onBatchFind( batch, batch.Length );
								batch_index = 0;
							}
						}
					} 
				}
			}

			// read files for new ones
			bool foundNewMusic = false;
			foreach ( var path in musicPaths ) {
				if ( !Directory.Exists( path ) ) {
					Debug.Log( $"directory {path} doesn't exist. skipping." );
					continue;
				}

				try {
					var filePaths = Directory.EnumerateFiles( path, "*.mp3", SearchOption.AllDirectories );
					int c = 0;
					foreach ( var filePath in filePaths ) {
						c++;
						// check if already in cache
						if ( isInCache( filePath ) ) continue;
						var musicData = new MusicData( filePath );
						MusicDatas.Add( musicData );
						foundNewMusic = true;
						batch[batch_index++] = musicData;
						if ( batch_index == batch.Length ) {
							onBatchFind( batch, batch.Length );
							batch_index = 0;
						}
						// Debug.Log( $"processed new file {filePath}" );
					}
					Debug.Log( $"folder {path}: {c} files" );
				}
				catch (Exception e) {
					Debug.LogException( e );
					throw;
				}
			}

			if ( batch_index > 0 ) {
				onBatchFind( batch, batch_index );
			}

			st.Stop();
			Debug.Log( $"total elapsed: {st.ElapsedMilliseconds}. found {MusicDatas.Count} musics" );
			if ( foundNewMusic )
				SaveCachedAsync();
		}

		public static async void SaveCachedAsync() {
			// cancel previous task and start a new one
			if ( saving ) {
				cancel_saving = true;
				while (saving) await Task.Yield();
			}

			var so = new Stopwatch();
			so.Start();
			saving = true;
			await using var file = new FileStream( cachePath, FileMode.OpenOrCreate, FileAccess.Write );
			await using var writer = new StreamWriter( file );
			foreach ( var data in MusicDatas ) {
				await writer.WriteLineAsync( data.Serialize() );
				if ( cancel_saving ) {
					cancel_saving = false;
					saving = false;
					return;
				}
			}
			so.Stop();
			Debug.Log( $"saved {MusicDatas.Count} musics to {cachePath}. {so.ElapsedMilliseconds}ms" );
			saving = false;
		}
		
		static bool isInCache(string path) {
			for ( int i = 0; i < MusicDatas.Count; i++ ) {
				if ( MusicDatas[i].path == path ) return true;
			}
			return false;
		}
	}
}