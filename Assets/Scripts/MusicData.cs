using System;
using Microsoft.Unity.VisualStudio.Editor;
using UnityEngine;

namespace FlexPlayer
{
    [Serializable]
    public class MusicData
    {
        public string path;
        public Texture2D icon;
        public string title;
        public string artist;
    }
}
