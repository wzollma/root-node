using System.Collections;
using UnityEngine;

namespace DefenseNodes.Towers
{
	public class Bomber : TowerBase
	{
		public float attackFrequency = 0.1f;
		public float attackDamage = 0.5f;

		[SerializeField] private ParticleSystem _particles1;
		[SerializeField] private ParticleSystem _particles2;

		private Coroutine _attackCoroutine;

		private void Start()
		{
			_attackCoroutine = StartCoroutine(Attack());
		}

		private IEnumerator Attack()
		{
			while (true)
			{			
				if (EnemiesInRange.Count > 0)
				{
					_particles1.enableEmission = true;
					_particles2.enableEmission = true;

					if (attackSoundName != null && attackSoundName.Length > 0)
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
				} else
                {
					_particles1.enableEmission = false;
					_particles2.enableEmission = false;
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