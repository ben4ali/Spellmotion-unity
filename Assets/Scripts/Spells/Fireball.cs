using UnityEngine;
using DG.Tweening;

public class Fireball : BaseSpell
{
	[Header("Gameplay")]
	public int damage = 25;
	public float aoeRadius = 3f;

	[Header("Casting")]
	public Transform castOrigin;
	public float travelSpeed = 10f;

	[Header("FX Prefabs")]
	public GameObject fxProjectilePrefab;
	public GameObject fxImpactPrefab;
	public float fxProjectileLifetime = 5f;
	public float fxImpactLifetime = 2f;

	public override void Cast()
	{
		EnemyController target = EnemyUtils.GetClosestEnemyToNexus();
		if (target == null)
		{
			Debug.Log("[Fireball] Aucun ennemi trouvé.");
			return;
		}

		Vector3 spawnPos = castOrigin != null ? castOrigin.position : transform.position;
		Vector3 targetPos = EnemyUtils.GetEnemyCenter(target);

		GameObject proj = null;
		if (fxProjectilePrefab != null)
		{
			proj = Object.Instantiate(fxProjectilePrefab, spawnPos, Quaternion.identity);
			Object.Destroy(proj, fxProjectileLifetime);
		}

		float dist = Vector3.Distance(spawnPos, targetPos);
		float travelDuration = Mathf.Min(0.75f, Mathf.Max(0.05f, dist / travelSpeed));

		if (proj != null)
		{
			proj.transform.DOMove(targetPos, travelDuration)
					.SetEase(Ease.Linear)
					.OnComplete(() =>
					{
						Impact(targetPos);
						Object.Destroy(proj);
					});
		}
		else
		{
			Impact(targetPos);
		}

	}

	private void Impact(Vector3 pos)
	{
		if (fxImpactPrefab != null)
		{
			GameObject fx = Object.Instantiate(fxImpactPrefab, pos, Quaternion.identity);
			CameraShake.Instance.Shake(1f, 0.4f);
			Object.Destroy(fx, fxImpactLifetime);
		}

		Collider[] hits = Physics.OverlapSphere(pos, aoeRadius);
		int hitCount = 0;
		foreach (var hit in hits)
		{
			EnemyController enemy = hit.GetComponent<EnemyController>();
			if (enemy != null)
			{
				enemy.TakeDamage(damage);
				hitCount++;
			}
		}
		Debug.Log($"[Fireball] Impact au {pos}. Ennemis touchés: {hitCount}");
	}
}
