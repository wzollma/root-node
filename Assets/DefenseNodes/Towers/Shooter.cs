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
			_attackCoroutine = StartCoroutine(Attack());
		}

		private IEnumerator Attack()
		{
			while (true)
			{
				if (EnemiesInRange.Count > 0)
				{
					EnemiesInRange[0].takeDamage(0.5f);
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