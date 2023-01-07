using System;
using System.IO;
using System.Linq;
using TagLib;
using UnityEngine;
using File = TagLib.File;

namespace FlexPlayer
{
	[Serializable]
	public class MusicData
	{
		public string path;
		public string title;
		public string artist;
		public int stars;
		public bool playing;
		
		public Texture2D small_icon;

		public MusicData(string path) {
			this.path = path;
		}

		public Texture2D getIcon() {
			var _file = File.Create( path, ReadStyle.Average );
			Texture2D icon = null;
			if ( _file.Tag.Pictures.Length > 0 ) {
				var bin = _file.Tag.Pictures[0].Data.Data;
				try {
					icon = new Texture2D( 64, 64 );
					icon.LoadImage( bin );
				}
				catch {
					// ignored
				}
			}

			return icon;
		}

		public void UnloadData() {
			Debug.Log( $"unloaded {title}" );
			small_icon = null;
			title = null;
			artist = null;
		}

		public void LoadData() {
			using var file = File.Create( path, ReadStyle.None );
			file.GetTag( TagTypes.AudibleMetadata );
			
			// artist
			artist = file.Tag.AlbumArtists.FirstOrDefault( s => !string.IsNullOrEmpty( s ) );
			if ( string.IsNullOrEmpty( artist ) ) {
				artist = file.Tag.Composers.FirstOrDefault( s => !string.IsNullOrEmpty( s ) );
			}

			// title
			title = file.Tag.Title;
			if ( string.IsNullOrEmpty( title ) ) {
				title = Path.GetFileNameWithoutExtension( path );
			}

			Debug.Log( $"loaded {title}" );
			// small icon
			// if ( file.Tag.Pictures.Length > 0 ) {
			// 	var bin = file.Tag.Pictures[0].Data.Data;
			// 	try {
			// 		const int size = 64;
			// 		small_icon = new Texture2D( size, size );
			// 		small_icon.LoadImage( bin );
			// 		var rt = RenderTexture.GetTemporary( size, size, 24 );
			// 		Graphics.Blit( small_icon, rt );
			// 		RenderTexture.active = rt;
			// 		small_icon.ReadPixels( new Rect( 0, 0, size, size ), 0, 0, false );
			// 		small_icon.Apply();
			// 		RenderTexture.ReleaseTemporary( rt );
			// 	}
			// 	catch (Exception e) {
			// 		Debug.LogException( e );
			// 	}
			// }
			// else {
			// 	small_icon = null;
			// }
		}
	}
}