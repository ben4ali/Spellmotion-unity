using UnityEngine;

public abstract class BaseSpell : MonoBehaviour
{
	[Header("Base Spell")]
	public string spellName;
	public float cooldown = 0f;

	protected bool isOnCooldown;
	private float cooldownEndTime;

	public abstract void Cast();

	public bool CanCast() => !isOnCooldown;

	public bool BeginCast()
	{
		if (isOnCooldown) return false;
		if (cooldown > 0f)
		{
			isOnCooldown = true;
			cooldownEndTime = Time.time + cooldown;
			Invoke(nameof(ResetCooldown), cooldown);
		}
		return true;
	}

	protected void CancelCooldown()
	{
		if (!isOnCooldown) return;
		isOnCooldown = false;
		cooldownEndTime = 0f;
		CancelInvoke(nameof(ResetCooldown));
	}

	private void ResetCooldown()
	{
		isOnCooldown = false;
		cooldownEndTime = 0f;
	}

	public float CooldownRemaining
	{
		get
		{
			if (!isOnCooldown) return 0f;
			float t = cooldownEndTime - Time.time;
			return t > 0f ? t : 0f;
		}
	}

	public float CooldownNormalized
	{
		get
		{
			if (cooldown <= 0f) return 1f;
			return Mathf.Clamp01(1f - (CooldownRemaining / cooldown));
		}
	}
}
