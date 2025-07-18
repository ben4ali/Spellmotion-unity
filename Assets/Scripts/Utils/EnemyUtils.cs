using UnityEngine;
using System.Collections.Generic;

public static class EnemyUtils
{
	public static EnemyController GetClosestEnemyToNexus()
	{
		NexusController nexus = Object.FindObjectOfType<NexusController>();
		if (nexus == null) return null;

		EnemyController[] enemies = Object.FindObjectsOfType<EnemyController>();
		EnemyController closest = null;
		float minDist = float.MaxValue;

		foreach (var enemy in enemies)
		{
			if (enemy.IsDead) continue;

			float dist = Vector3.Distance(nexus.transform.position, enemy.transform.position);
			if (dist < minDist)
			{
				minDist = dist;
				closest = enemy;
			}
		}

		return closest;
	}

	public static Vector3 GetEnemyCenter(EnemyController enemy)
	{
		if (enemy == null) return Vector3.zero;

		Collider col = enemy.GetComponentInChildren<Collider>();
		return col != null ? col.bounds.center : enemy.transform.position;
	}


	public static EnemyController[] GetEnemiesInRadius(Vector3 center, float radius)
	{
		Collider[] colliders = Physics.OverlapSphere(center, radius);
		List<EnemyController> enemies = new List<EnemyController>();

		foreach (var col in colliders)
		{
			if (!col.CompareTag("Enemy")) continue;

			EnemyController enemy = col.GetComponentInParent<EnemyController>();
			if (enemy != null && !enemy.IsDead)
			{
				enemies.Add(enemy);
			}
		}

		return enemies.ToArray();
	}
}
