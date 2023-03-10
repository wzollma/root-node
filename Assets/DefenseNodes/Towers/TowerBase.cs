using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace DefenseNodes.Towers
{
	public enum Appearance
	{
		Healthy = 0,
		Damaged,
	}
	
	public abstract class TowerBase : MonoBehaviour
	{

		[SerializeField] protected int cost = 1;
		public int Cost => cost;

		[SerializeField] protected String towerName;
		public String TowerName => towerName;

		[SerializeField] protected float initialHealth = 10;
		public float InitialHealth => initialHealth;

		public MeshFilter meshFilter;
		[SerializeField] private Mesh healthyMesh;
		[SerializeField] private Mesh damagedMesh;
		[SerializeField] private Transform meshPivot;

		// for audio
		[SerializeField] internal string attackSoundName;
		internal string hitSoundName;
		
		public void OnNodeTakesDamage(float health)
		{
			if (health > 5)
			{
				meshFilter.mesh = healthyMesh;
			}
			else
			{
				meshFilter.mesh = damagedMesh;
			}


			Vector3 randLook = new Vector3(Random.value - 0.5f, 1, Random.value - 0.5f);
			meshPivot.rotation = Quaternion.LookRotation(Vector3.forward, randLook);
		}
		
		protected readonly List<Enemy> EnemiesInRange = new List<Enemy>(10);
		
		private void OnTriggerEnter(Collider other)
		{
			Enemy enemy = other.GetComponent<Enemy>();
			EnemiesInRange.Add(enemy);
			enemy.OnDestroyed += delegate { EnemiesInRange.Remove(enemy); };
		}

		protected virtual void Update()
		{
			meshPivot.rotation = Quaternion.Lerp(meshPivot.rotation, Quaternion.identity, Time.deltaTime * 30);
		}

		private void OnTriggerExit(Collider other)
		{
			EnemiesInRange.Remove(other.GetComponent<Enemy>());
		}

		private void OnValidate()
		{
			meshFilter.mesh = healthyMesh;
		}
	}
}