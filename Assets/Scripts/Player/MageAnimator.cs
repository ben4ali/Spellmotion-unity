using UnityEngine;
using System.Collections;

public class MageAnimator : MonoBehaviour
{
	[SerializeField] private Animator animator;

	[Header("Maintain Durations")]
	public float arcaneBlastHold = 0.9f;
	public float firehold = 0.75f;
	public float iceSpikesHold = 0.5f;

	public float healHold = 1f;

	private Coroutine holdRoutine;

	private void Awake()
	{
		if (animator == null) animator = GetComponentInChildren<Animator>();
		if (animator == null)
			Debug.LogError("[MageAnimator] Aucun Animator trouvé.");
		else
			Debug.Log("[MageAnimator] Animator trouvé et prêt.");
	}

	// Sorts
	public void PlayArcaneBlast()
	{
		Debug.Log("[MageAnimator] Lancement animation: ArcaneBlast");
		Trigger("ArcaneBlast");
		StartHold(arcaneBlastHold);
	}

	public void PlayFireball()
	{
		Debug.Log("[MageAnimator] Lancement animation: Fireball");
		Trigger("Fireball");
		StartHold(firehold);
	}

	public void PlayLightning()
	{
		Debug.Log("[MageAnimator] Lancement animation: Lightning");
		Trigger("Lightning");

	}

	public void PlayIceSpikes()
	{
		Debug.Log("[MageAnimator] Lancement animation: IceSpikes");
		Trigger("IceSpikes");
		StartHold(iceSpikesHold);
	}

	public void PlayHeal()
	{
		Debug.Log("[MageAnimator] Lancement animation: Heal");
		Trigger("Heal");
		StartHold(healHold);
	}

	public void PlayNo()
	{
		Debug.Log("[MageAnimator] Animation: No (Cooldown)");
		Trigger("No");
	}

	public void PlayVictory()
	{
		Debug.Log("[MageAnimator] Animation: Victory");
		Trigger("Victory");
	}

	public void PlayLost()
	{
		Debug.Log("[MageAnimator] Animation: Lost");
		Trigger("Lost");
	}

	// Gestion du maintien
	private void StartHold(float duration)
	{
		if (holdRoutine != null)
		{
			StopCoroutine(holdRoutine);
			Debug.Log("[MageAnimator] Ancien maintien annulé.");
		}

		Debug.Log($"[MageAnimator] Début maintien pendant {duration}s");
		holdRoutine = StartCoroutine(HoldThenIdle(duration));
	}

	private IEnumerator HoldThenIdle(float t)
	{
		yield return new WaitForSeconds(t);
		Debug.Log("[MageAnimator] Maintien terminé → Retour Idle01");
		GoIdle();
		holdRoutine = null;
	}

	private void GoIdle()
	{
		if (animator == null) return;
		Debug.Log("[MageAnimator] Force transition → Idle01");
		animator.CrossFade("Idle01", 0.1f);
	}

	private void Trigger(string name)
	{
		if (animator == null)
		{
			Debug.LogWarning($"[MageAnimator] Impossible de déclencher {name}, Animator introuvable.");
			return;
		}

		Debug.Log($"[MageAnimator] Trigger envoyé: {name}");
		animator.ResetTrigger(name);
		animator.SetTrigger(name);
	}
}
