using UnityEngine;

public class InputManager : MonoBehaviour
{
	[Header("Debug")]
	[SerializeField] private bool debugLogging = true;

	private SpellBook spellBook;

	private void Start()
	{
		spellBook = FindObjectOfType<SpellBook>();
		if (spellBook == null)
		{
			Debug.LogError("[InputManager] Aucun SpellBook trouvé dans la scène. Les inputs ne feront rien.");
		}
		else if (debugLogging)
		{
			Debug.Log("[InputManager] SpellBook détecté, prêt à recevoir les inputs.");
		}
	}

	private void Update()
	{
		if (GameManager.Instance != null && GameManager.Instance.gameOver) return;
		if (spellBook == null) return;

		if (Input.GetKeyDown(KeyCode.Alpha1))
			HandleSpellInput("ArcaneBlast", spellBook.arcaneBlast, spellBook.CastArcaneBlast);

		if (Input.GetKeyDown(KeyCode.Alpha2))
			HandleSpellInput("Fireball", spellBook.fireball, spellBook.CastFireball);

		if (Input.GetKeyDown(KeyCode.Alpha3))
			HandleSpellInput("Lightning", spellBook.lightning, spellBook.CastLightning);

		if (Input.GetKeyDown(KeyCode.Alpha4))
			HandleSpellInput("IceSpikes", spellBook.iceSpikes, spellBook.CastIceSpikes);

		if (Input.GetKeyDown(KeyCode.Alpha5))
			HandleSpellInput("Heal", spellBook.heal, spellBook.CastHeal);
	}

	private void HandleSpellInput(string label, BaseSpell spellRef, System.Action castAction)
	{
		if (!debugLogging)
		{
			if (spellRef != null && spellRef.CanCast())
				castAction?.Invoke();
			return;
		}

		if (spellRef == null)
		{
			Debug.LogWarning($"[InputManager] Touche pour {label} pressée, mais le sort n'est pas assigné dans le SpellBook.");
			return;
		}

		if (!spellRef.CanCast())
		{
			Debug.Log($"[InputManager] {label} pressé mais encore en cooldown ({spellRef.cooldown}s).");
			return;
		}

		Debug.Log($"[InputManager] {label} cast !");
		castAction?.Invoke();
	}
}
