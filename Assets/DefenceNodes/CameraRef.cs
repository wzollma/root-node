using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CameraRef : MonoBehaviour
{
	public static Camera Main;
	public static PhysicsRaycaster Raycaster;

	private void Awake()
	{
		Main = GetComponent<Camera>();
		Raycaster = GetComponent<PhysicsRaycaster>();
	}
}