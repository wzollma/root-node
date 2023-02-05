using System;
using UnityEngine;

namespace DefenseNodes.Towers
{
	public class Shooter : TowerBase
	{
		private void FixedUpdate()
		{
			foreach (Enemy enemy in EnemiesInRange)
			{
				// hurt
			}
		}
	}
}