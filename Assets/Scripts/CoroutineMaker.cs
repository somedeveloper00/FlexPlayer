using System.Collections;
using UnityEngine;

namespace FlexPlayer
{
	public class CoroutineMaker : MonoBehaviour
	{
		static CoroutineMaker instance;


		public static Coroutine startCoroutine(IEnumerator coroutine) {
			createInstance();
			return instance.StartCoroutine( coroutine );
		}
		public static void stopCoroutine(Coroutine coroutine) {
			createInstance();
			instance.StopCoroutine( coroutine );
		}

		public static void stopAllCoroutines() {
			createInstance();
			instance.StopAllCoroutines();
		}

		static void createInstance() {
			if ( instance == null ) {
				GameObject go = new GameObject( "CoroutineMaker" );
				instance = go.AddComponent<CoroutineMaker>();
			}
		}

	}
}