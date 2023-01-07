using UnityEngine;

namespace FlexPlayer
{
	[CreateAssetMenu( menuName = "Startups", order = 0 )]
	public class Startups : ScriptableObject
	{
		public int fps;

		void OnEnable() {
			Application.targetFrameRate = fps;
		}
	}
}