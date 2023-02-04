using UnityEngine;

namespace DefenseNodes
{
	public class RandomYRot : MonoBehaviour
	{
		private void Start()
		{
			transform.eulerAngles = Vector3.up * Random.value * 360f;
		}
	}
}