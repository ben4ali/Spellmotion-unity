using UnityEngine;

public class Heal : BaseSpell
{
	public int healAmount = 50;
	public NexusController nexus;
	public GameObject fxPrefab;
	public float fxDuration = 2f;

	public override void Cast()
	{
		nexus.Heal(healAmount);
		Debug.Log("Nexus healed by " + healAmount);

		if (fxPrefab != null)
		{
			GameObject fx = Instantiate(fxPrefab, nexus.transform.position, Quaternion.identity);
			Destroy(fx, fxDuration);
		}

	}
}
