using UnityEngine;

namespace D3TEditor.TimeTracking
{
	[System.Serializable]
	public class TimeSample
	{
		public float activeEditTime = 0;
		public float unfocussedEditTime = 0;
		public float playmodeTime = 0;

		public float CombinedTime => activeEditTime + unfocussedEditTime + playmodeTime;

		public void Increase(float delta)
		{
			if(Application.isPlaying)
			{
				playmodeTime += delta;
			}
			else
			{
				bool focussed = UnityEditorInternal.InternalEditorUtility.isApplicationActive;
				if(focussed) activeEditTime += delta;
				else unfocussedEditTime += delta;
			}
		}
	}
}
