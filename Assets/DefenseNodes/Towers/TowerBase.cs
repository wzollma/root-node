using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

namespace DefenseNodes.Towers
{
	public abstract class TowerBase : MonoBehaviour
	{
		public float MaxHealth { get; protected set; }
		public float Health { get; protected set; }

		public float Damage(float amount)
		{
			Health -= amount;

			return Health;
		}

		public void SetHealth(float health)
		{
			Health = Mathf.Clamp(health, 0, MaxHealth);
			if (Health < MaxHealth / 2f)
			{
				meshFilter.mesh = damagedMesh;
			}
			else
			{
				meshFilter.mesh = healthyMesh;
			}
		}

		[SerializeField] private MeshFilter meshFilter;
		[SerializeField] private Mesh healthyMesh;
		[SerializeField] private Mesh damagedMesh;
		
		protected readonly List<Enemy> EnemiesInRange = new List<Enemy>(10);
		
		private void OnTriggerEnter(Collider other)
		{
			if (!other.CompareTag("Enemy"))
				return;
			
			EnemiesInRange.Add(other.GetComponent<Enemy>());
		}

		private void OnTriggerExit(Collider other)
		{
			if (!other.CompareTag("Enemy"))
				return;
			
			EnemiesInRange.Remove(other.GetComponent<Enemy>());
		}
	}
}