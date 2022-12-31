using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace FlexPlayer
{
    public class MusicSelectable : MonoBehaviour
    {
        [SerializeField] Image cover;
        [SerializeField] TMP_Text title;
        [SerializeField] TMP_Text artist;


        public void Setup(MusicData data) {
            if (data.icon != null) {
                cover.sprite = Sprite.Create(data.icon, new Rect(0, 0, data.icon.width, data.icon.height), Vector2.zero);
            }
            title.text = data.title;
            artist.text = data.artist;
            Debug.Log($"music {title} view is set up.");
        }
    }
}
