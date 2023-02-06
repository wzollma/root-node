using System;
using System.Collections;
using UnityEngine;

namespace DefenseNodes.Towers
{
	public class Slower : TowerBase
	{

        protected override void Update()
		{
			base.Update();
			
			if (EnemiesInRange.Count > 0)
				AudioManager.PlayNoOverlap(attackSoundName);

			//Debug.Log(EnemiesInRange.Count);
			for (int i = 0; i < EnemiesInRange.Count; i++)
			{
				Enemy enemy = EnemiesInRange[i];
				enemy.MultiplySpeedNextMove(0.5f);
			}
		}
	}
}