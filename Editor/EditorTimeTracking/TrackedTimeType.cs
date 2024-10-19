﻿using System;
using UnityEngine;

namespace D3TEditor.TimeTracking
{
	[Flags]
	public enum TrackedTimeType
	{
		ActiveEditorTime = 1 << 0,
		UnfocusedEditorTime = 1 << 1,
		PlaymodeTime = 1 << 2,
		InactiveTime = 1 << 3,

		All = ActiveEditorTime | UnfocusedEditorTime | PlaymodeTime | InactiveTime
	}
}
