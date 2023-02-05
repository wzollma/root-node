using System;
using System.Collections;
using UnityEngine;

namespace DefenseNodes.Towers
{
	public class Shooter : TowerBase
	{
		public float attackFrequency = 0.1f;

		private Coroutine _attackCoroutine;

		private void Start()
		{
			attackCooldown = UnityEngine.Random.Range(attackSoundFrequencyRange.x, attackSoundFrequencyRange.y);

			_attackCoroutine = StartCoroutine(Attack());
		}

		private IEnumerator Attack()
		{
			while (true)
			{			
				if (EnemiesInRange.Count > 0)
				{
					if (attackSoundName != null && attackSoundName.Length > 0 && Time.time - lastTimeAttackSound > attackCooldown)
					{
						AudioManager.PlayNoOverlap(attackSoundName);
						lastTimeAttackSound = Time.time;
					}

					EnemiesInRange[0].takeDamage(0.5f);

					if (hitSoundName != null && hitSoundName.Length > 0)
						AudioManager.PlayNoOverlap(hitSoundName);
				}

				yield return new WaitForSeconds(attackFrequency);
			}
		}

		private void OnDestroy()
		{
			StopCoroutine(_attackCoroutine);
		}
	}
}