using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using TagLib;
using TagLib.Id3v2;
using UnityEngine;
using File = TagLib.File;
using Tag = TagLib.Tag;

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
		public DateTime lastTimeModified;

		public bool Loaded { get; private set; }
		
		TagLib.Id3v2.Tag _tag;

		public MusicData(string path) {
			this.path = path;
		}

		public Texture2D getIcon() {
			
			Texture2D icon = null;
			if ( _tag.Pictures.Length > 0 ) {
				var bin = _tag.Pictures[0].Data.Data;
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
			Loaded = false;
			_tag = null;
		}

		public void LoadMetaData() {
			if ( Loaded ) return;
			loadFromTag();
		}

		void loadFromTag() {
			// artist
			var file = File.Create( path );
			if ( !file.TagTypes.HasFlag( TagTypes.Id3v2 ) ) return;
			_tag = (TagLib.Id3v2.Tag)file.GetTag( TagTypes.Id3v2 );

			artist = _tag.AlbumArtists.FirstOrDefault( s => !string.IsNullOrEmpty( s ) );
			if ( string.IsNullOrEmpty( artist ) ) {
				artist = _tag.Composers.FirstOrDefault( s => !string.IsNullOrEmpty( s ) );
			}

			// title
			title = _tag.Title;
			if ( string.IsNullOrEmpty( title ) ) {
				title = Path.GetFileNameWithoutExtension( path );
			}

			// rate
			var popularimeterFrame = PopularimeterFrame.Get( _tag, "FlexPlayer", true );
			rate = popularimeterFrame.Rating;
			Loaded = true;
		}

		/// <summary>
		/// Deserializes the data, if valid, it'll return the <see cref="MusicData"/> back, otherwise it'll return null
		/// </summary>
		public static MusicData Deserialize(string data) {
			var split = data.Split( (char)0x1f ); // unit separator char. plain text cannot contain them
			
#region validation checks
			// data integrity
			if ( split.Length != 5 ) return null;
			// existence of file
			if ( !System.IO.File.Exists( split[0] ) ) return null;
			// modification time of file
			if ( new FileInfo( split[0] ).LastWriteTimeUtc.Ticks != long.Parse( split[4] ) ) return null;
#endregion
			
			return new MusicData( split[0] ) {
				title = split[1],
				artist = split[2],
				rate = int.Parse( split[3] ),
				lastTimeModified = new DateTime( long.Parse( split[4] ) ),
				Loaded = true
			};
		}

		public string Serialize() =>
			$"{path}{(char)0x1f}{title}{(char)0x1f}{artist}{(char)0x1f}{rate}{(char)0x1f}{lastTimeModified.Ticks}";
	}
}