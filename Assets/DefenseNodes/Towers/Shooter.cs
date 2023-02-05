using System;
using System.Collections;
using UnityEngine;

namespace DefenseNodes.Towers
{
	public class Shooter : TowerBase
	{
		public float attackFrequency = 0.1f;

		private Coroutine _attackCoroutine;

		Vector2 attackSoundFrequencyRange = new Vector2(.6f, 2.5f);
		float attackCooldown; // random value within above range (provides auditory variation)
		float lastTimeAttackSound = -100; // arbitrarily low number so sound could play on start

		private void Start()
		{
			attackCooldown = UnityEngine.Random.Range(attackSoundFrequencyRange.x, attackSoundFrequencyRange.y);

			_attackCoroutine = StartCoroutine(Attack());
		}

		private IEnumerator Attack()
		{
			while (true)
			{
				if (Time.time - lastTimeAttackSound > attackCooldown) {
					AudioManager.PlayNoOverlap(attackSoundName);
					lastTimeAttackSound = Time.time;
				}					

				if (EnemiesInRange.Count > 0)
				{
					EnemiesInRange[0].takeDamage(0.5f);

					if (hitSoundName.Length > 0)
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