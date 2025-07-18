using UnityEngine;

public class Lightning : BaseSpell
{
	[Header("Damage")]
	public int damage = 20;
	[Tooltip("Rayon de l'impact en mètres.")]
	public float radius = 5f;

	[Header("FX")]
	public GameObject fxPrefab;
	public float fxDuration = 2f;
	public bool scaleFxToRadius = false;

	public override void Cast()
	{
		EnemyController target = EnemyUtils.GetClosestEnemyToNexus();
		if (target == null)
		{
			Debug.Log("[Lightning] Aucun ennemi.");
			return;
		}

		Vector3 strikePos = EnemyUtils.GetEnemyCenter(target);

		if (fxPrefab != null)
		{
			GameObject fx = Instantiate(fxPrefab, strikePos, Quaternion.identity);
			CameraShake.Instance.Shake(0.2f, 0.2f);
			if (scaleFxToRadius) fx.transform.localScale = Vector3.one * (radius * 2f);
			Destroy(fx, fxDuration);
		}

		EnemyController[] enemiesHit = EnemyUtils.GetEnemiesInRadius(strikePos, radius);
		foreach (var enemy in enemiesHit)
		{
			enemy.TakeDamage(damage);
		}

		Debug.Log($"[Lightning] Impact ({radius}m) | Dégâts: {damage} | Ennemis touchés: {enemiesHit.Length}");
	}
}
