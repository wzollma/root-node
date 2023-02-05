using System;
using UnityEngine;

namespace DefenseNodes.Towers
{
	public class Shooter : TowerBase
	{
		private void FixedUpdate()
		{
			if(EnemiesInRange.Count > 0)
				EnemiesInRange[0].takeDamage(0.5f);
		}
	}
}