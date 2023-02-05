using System;
using System.Collections;
using UnityEngine;

namespace DefenseNodes.Towers
{
	public class Shooter : TowerBase
	{
		public float attackFrequency = 0.1f;
		public float attackDamage = 0.5f;

		private LineRenderer _lineRenderer;
		private Coroutine _attackCoroutine;

		private void Awake()
		{
			_lineRenderer = GetComponent<LineRenderer>();
		}

		private void Start()
		{
			_attackCoroutine = StartCoroutine(Attack());
			
			_lineRenderer.SetPosition(0, transform.position + Vector3.up);
			_lineRenderer.SetPosition(1, transform.position + Vector3.up);
		}

		private float _lineAlpha;

		private IEnumerator Attack()
		{
			while (true)
			{			
				if (EnemiesInRange.Count > 0)
				{
					if (attackSoundName != null && attackSoundName.Length > 0)
					{
						AudioManager.PlayNoOverlap(attackSoundName);
						lastTimeAttackSound = Time.time;
					}

					EnemiesInRange[0].takeDamage(attackDamage);
					
					_lineRenderer.SetPosition(1, EnemiesInRange[0].transform.position);

					if (hitSoundName != null && hitSoundName.Length > 0)
						AudioManager.PlayNoOverlap(hitSoundName);
				}
				
				yield return new WaitForSeconds(0.1f);
				
				_lineRenderer.SetPosition(1, transform.position);

				yield return new WaitForSeconds(attackFrequency - 0.1f);
			}
		}

		private void OnDestroy()
		{
			StopCoroutine(_attackCoroutine);
		}
	}
}