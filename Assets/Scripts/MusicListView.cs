using System;
using System.Collections.Generic;
using FlexPlayer.Utils;
using UnityEngine;

namespace FlexPlayer
{
    public class MusicListView : MonoBehaviour
    {
        [SerializeField] MusicSelectable itemSample;
        [SerializeField] Transform listParent;

        List<MusicSelectable> selectables = new List<MusicSelectable>();


        void Start() {
            UpdateList();
        }

        async void UpdateList()
        {
            // destroy old items
            foreach (var selectable in selectables)
                Destroy(selectable.gameObject);
            selectables.Clear();

            // create new items
            var musicDatas = await MusicIOUtils.GetAllMusicsAsync();
            foreach (var musicData in musicDatas) {
                var ins = Instantiate(itemSample, listParent);
                ins.Setup(musicData);
                selectables.Add(ins);
            }
        }
    }
}