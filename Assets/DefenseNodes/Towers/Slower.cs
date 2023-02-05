using System;
using System.Collections;
using UnityEngine;

namespace DefenseNodes.Towers
{
	public class Slower : TowerBase
	{
        private void Start()
        {
			attackCooldown = UnityEngine.Random.Range(attackSoundFrequencyRange.x, attackSoundFrequencyRange.y);
		}

        private void Update()
		{
			if (EnemiesInRange.Count > 0 && Time.time - lastTimeAttackSound > attackCooldown)
			{
				AudioManager.PlayNoOverlap(attackSoundName);
				lastTimeAttackSound = Time.time;
			}

			Debug.Log(EnemiesInRange.Count);
			for (int i = 0; i < EnemiesInRange.Count; i++)
			{
				Enemy enemy = EnemiesInRange[i];
				enemy.MultiplySpeedNextMove(0.5f);
			}
		}
	}
}