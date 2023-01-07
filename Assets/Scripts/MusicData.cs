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

		bool loaded;
		
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
			title = null;
			artist = null;
			loaded = false;
		}

		public void LoadData() {
			if ( loaded ) return;
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
			loaded = true;
		}
	}
}