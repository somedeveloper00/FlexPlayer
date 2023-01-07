using UnityEngine;

namespace FlexPlayer
{
	[CreateAssetMenu( menuName = "Initials", order = 0 )]
	public class Initials : ScriptableObject
	{
		public int fps;

		void OnEnable() {
			Application.targetFrameRate = fps;
		}
	}
}