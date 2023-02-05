using System.Collections;
using UnityEngine;

namespace DefenseNodes.Towers
{
	public class Bomber : TowerBase
	{
		public float attackFrequency = 0.1f;
		public float attackDamage = 0.5f;
		
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

					for (int i = EnemiesInRange.Count - 1; i > 0; i--)
					{
						EnemiesInRange[i].takeDamage(attackDamage);
					}

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