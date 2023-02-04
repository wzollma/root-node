using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Cameraman
{
	public class CameraRig : MonoBehaviour
	{
		private void Update()
		{
			transform.eulerAngles += Vector3.up * (Input.mouseScrollDelta.y * 10);
		}
	}
}