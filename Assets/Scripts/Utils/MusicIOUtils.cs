using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace FlexPlayer.Utils
{
    public static class MusicIOUtils
    {
        public static List<string> paths = new List<string>(){
            Environment.GetFolderPath(Environment.SpecialFolder.MyMusic),
            Environment.GetFolderPath(Environment.SpecialFolder.CommonMusic),
        };

        public static async Task<List<MusicData>> GetAllMusicsAsync()
        {
            List<MusicData> results = new ();
            foreach (var path in paths)
            {
                var enumOptions = new EnumerationOptions();
                enumOptions.RecurseSubdirectories = true;
                var filesEnum = Directory.EnumerateFiles(path, "*.mp3", enumOptions);
                foreach (var file in filesEnum)
                {
                    results.Add(new MusicData()
                    {
                        artist = "artist",
                        icon = null,
                        title = new FileInfo(file).Name
                    });
                    await Task.Yield();
                }
            }
            return results;
        }
    }
}
