using UnityEngine;
using UnityEngine.EventSystems;

namespace DefenseNodes
{
	public class CameraRef : MonoBehaviour
	{
		public static Camera Main { get; private set; }
		public static PhysicsRaycaster Raycaster { get; private set; }

		private void Start()
		{
			Main = GetComponent<Camera>();
			Raycaster = GetComponent<PhysicsRaycaster>();
		}
	}
}