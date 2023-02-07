using System.Collections;
using UnityEngine;

namespace DefenseNodes.Towers
{
	public class Bomber : TowerBase
	{
		public float attackFrequency = 0.1f;
		public float attackDamage = 0.5f;

		[SerializeField] private ParticleSystem _particles;

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
					// plays parent and all child particles
					_particles.Play(true);

					if (attackSoundName != null && attackSoundName.Length > 0)
						AudioManager.PlayNoOverlap(attackSoundName);

					for (int i = EnemiesInRange.Count - 1; i >= 0; i--)
					{
						EnemiesInRange[i].takeDamage(attackDamage);

					}

					if (hitSoundName != null && hitSoundName.Length > 0)
						AudioManager.PlayNoOverlap(hitSoundName);

					// waits a frame so the particle system effectively starts the particle emission
					// before I disable further emission
					yield return null;
				}

				// stops parent and all child particles from emitting
				_particles.Stop(true, ParticleSystemStopBehavior.StopEmitting);

				yield return new WaitForSeconds(attackFrequency);
			}
		}

		private void OnDestroy()
		{
			StopCoroutine(_attackCoroutine);
		}
	}
}