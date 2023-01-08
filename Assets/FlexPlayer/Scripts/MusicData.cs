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

		public bool Loaded { get; private set; }
		public bool IsLoading { get; private set; }
		
		bool cache_loaded;
		
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

		public async Task LoadMetaData() {
			if ( Loaded ) return;
			if ( IsLoading ) return;
			IsLoading = true;
			if ( cache_loaded ) {
				// load in another thread
				await Task.Run( load_from_tag );
				IsLoading = false;
				return;
			}

			load_from_tag();
			IsLoading = false;

			void load_from_tag() {
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
		}

		public static MusicData Deserialize(string data) {
			var split = data.Split( (char)0x1f ); // unit separator char. plain text cannot contain them
			return new MusicData( split[0] ) {
				title = split[1],
				artist = split[2],
				rate = int.Parse( split[3] ),
				cache_loaded = true
			};
		}
		
		public string Serialize() => $"{path}{(char)0x1f}{title}{(char)0x1f}{artist}{(char)0x1f}{rate}";
	}
}