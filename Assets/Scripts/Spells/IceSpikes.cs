using UnityEngine;
using System.Collections;

public class IceSpikes : BaseSpell
{
	[Header("Damage & Effect")]
	public int damage = 15;
	public float forwardOffset = -4f;
	public float effectRange = 10f;
	public float effectWidth = 6f;
	public float slowPercent = 0.5f;
	public float slowDuration = 2f;

	[Header("FX")]
	public GameObject fxPrefab;
	public float fxDuration = 2f;

	public override void Cast()
	{
		EnemyController target = EnemyUtils.GetClosestEnemyToNexus();
		if (target == null)
		{
			Debug.Log("[IceSpikes] Aucun ennemi.");
			return;
		}

		Vector3 targetPos = target.transform.position;
		NexusController nexus = FindObjectOfType<NexusController>();
		if (nexus == null)
		{
			Debug.LogError("[IceSpikes] Nexus introuvable.");
			return;
		}

		Vector3 dirToNexus = (nexus.transform.position - targetPos).normalized;
		Vector3 fxPos = targetPos - dirToNexus * forwardOffset;

		if (fxPrefab != null)
		{
			GameObject fx = Instantiate(fxPrefab, fxPos, Quaternion.LookRotation(dirToNexus));
			CameraShake.Instance.Shake(1f, 0.7f);
			fx.transform.Rotate(0, 90f, 0);
			Destroy(fx, fxDuration);
		}

		Vector3 boxCenter = fxPos - dirToNexus * (effectRange / 2f);
		Vector3 halfExtents = new Vector3(effectWidth / 2f, 2f, effectRange / 2f);
		Collider[] hits = Physics.OverlapBox(boxCenter, halfExtents, Quaternion.LookRotation(dirToNexus));

		int count = 0;
		foreach (var hit in hits)
		{
			if (hit.CompareTag("Enemy"))
			{
				EnemyController enemy = hit.GetComponentInParent<EnemyController>();
				if (enemy != null && !enemy.IsDead)
				{
					enemy.TakeDamage(damage);
					ApplySlow(enemy);
					count++;
				}
			}
		}

		Debug.Log($"[IceSpikes] Ennemis touchés: {count} | Zone: {effectWidth}x{effectRange} | Slow {slowPercent * 100}% pendant {slowDuration}s");

	}

	private void ApplySlow(EnemyController enemy)
	{
		StartCoroutine(SlowCoroutine(enemy));
	}

	private IEnumerator SlowCoroutine(EnemyController enemy)
	{
		float originalSpeed = enemy.stats.speed;
		enemy.stats.speed *= (1f - slowPercent);

		yield return new WaitForSeconds(slowDuration);

		if (!enemy.IsDead)
		{
			enemy.stats.speed = originalSpeed;
		}
	}
}
