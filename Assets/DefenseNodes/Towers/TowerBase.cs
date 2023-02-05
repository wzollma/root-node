using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

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

		[SerializeField] private MeshFilter meshFilter;
		[SerializeField] private Mesh healthyMesh;
		[SerializeField] private Mesh damagedMesh;

		public void SetAppearance(Appearance appearance)
		{
			switch (appearance)
			{
				case Appearance.Healthy:
					meshFilter.mesh = healthyMesh;
					break;
				
				case Appearance.Damaged :
					meshFilter.mesh = damagedMesh;
					break;
			}
		}
		
		protected readonly List<Enemy> EnemiesInRange = new List<Enemy>(10);
		
		private void OnTriggerEnter(Collider other)
		{
			Enemy enemy = other.GetComponent<Enemy>();
			EnemiesInRange.Add(enemy);
			enemy.OnDestroyed += delegate { EnemiesInRange.Remove(enemy); };
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