﻿using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

namespace FlexPlayer
{
	public class MusicPlayer : MonoBehaviour
	{
		public static MusicPlayer instance;
		
		[SerializeField] AudioSource audioSource;

		void Awake() {
			if ( instance == null ) {
				instance = this;
			}
			else {
				Destroy( gameObject );
			}
		}

		public void Play(MusicData musicData) {
			StartCoroutine( play( musicData ) );
		}

		IEnumerator play(MusicData musicData) {
			
			Debug.Log( "Playing: " + musicData.path );
			string path = musicData.path;
			var req = UnityWebRequestMultimedia.GetAudioClip( $"file://{path}", AudioType.MPEG );
			yield return req.SendWebRequest();
			if ( req.result == UnityWebRequest.Result.Success) {
				audioSource.clip = DownloadHandlerAudioClip.GetContent( req );
				audioSource.Play();
				Debug.Log( "Play: " + musicData.title + " Success" );
			}
			else {
				Debug.LogError( req.error );
			}
		}
	}
}