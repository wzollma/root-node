using System;
using System.Collections.Generic;
using UnityEngine;

namespace DefenseNodes.Tree
{
	public class TreeBase : MonoBehaviour
	{
		protected List<GameObject> EnemiesInRange = new List<GameObject>(10);
		
		private void OnTriggerEnter(Collider other)
		{
			if (!other.CompareTag("Enemy"))
				return;
			
			EnemiesInRange.Add(other.gameObject);
		}

		private void OnTriggerExit(Collider other)
		{
			if (!other.CompareTag("Enemy"))
				return;
			
			EnemiesInRange.Remove(other.gameObject);
		}
	}
}