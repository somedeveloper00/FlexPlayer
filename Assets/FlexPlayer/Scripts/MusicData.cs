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
		public int rate;
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

		public void UnloadMetaData() {
			Debug.Log( $"unloaded meta data for {title}" );
			title = null;
			artist = null;
			loaded = false;
		}

		public void LoadMetaData() {
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
			loaded = true;
		}

		public static MusicData Deserialize(string data) {
			var split = data.Split( (char)0x1f ); // unit separator char. plain text cannot contain them
			return new MusicData( split[0] ) {
				title = split[1],
				artist = split[2],
				rate = int.Parse( split[3] )
			};
		}
		
		public string Serialize() => $"{path}{(char)0x1f}{title}{(char)0x1f}{artist}{(char)0x1f}{rate}";
	}
}