using UnityEngine;

namespace D3TEditor.TimeTracking
{
	[System.Serializable]
	public class TimeSample
	{
		public float editTime = 0;
		public float unfocussedEditTime = 0;
		public float playTime = 0;

		public void Increase(float delta)
		{
			if(Application.isPlaying)
			{
				playTime += delta;
			}
			else
			{
				bool focussed = UnityEditorInternal.InternalEditorUtility.isApplicationActive;
				if(focussed) editTime += delta;
				else unfocussedEditTime += delta;
			}
		}
	}
}
