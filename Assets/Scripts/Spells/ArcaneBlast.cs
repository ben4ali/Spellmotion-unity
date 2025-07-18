using UnityEngine;
using System.Collections;

public class ArcaneBlast : BaseSpell
{
	[Header("Damage")]
	public int damage = 30;
	public float radius = 2.5f;

	[Header("Timing")]
	public float delayBeforeDamage = 0.75f;

	[Header("FX")]
	public GameObject fxImpactPrefab;
	public float fxImpactLifetime = 1.5f;

	private Coroutine castRoutine;

	public override void Cast()
	{
		if (castRoutine != null) StopCoroutine(castRoutine);
		castRoutine = StartCoroutine(CastSequence());
	}

	private IEnumerator CastSequence()
	{
		EnemyController target = EnemyUtils.GetClosestEnemyToNexus();
		if (target == null)
		{
			Debug.Log("[ArcaneBlast] Aucun ennemi.");
			yield break;
		}

		Vector3 blastPos = EnemyUtils.GetEnemyCenter(target);

		if (fxImpactPrefab != null)
		{
			GameObject impactFx = Instantiate(fxImpactPrefab, blastPos, Quaternion.identity);
			Destroy(impactFx, fxImpactLifetime);
		}

		yield return new WaitForSeconds(delayBeforeDamage);

		EnemyController[] enemiesHit = EnemyUtils.GetEnemiesInRadius(blastPos, radius);
		foreach (var enemy in enemiesHit)
		{
			enemy.TakeDamage(damage);
		}

		Debug.Log($"[ArcaneBlast] AOE appliquée | Dégâts: {damage} | Ennemis touchés: {enemiesHit.Length}");
		castRoutine = null;
	}
}
