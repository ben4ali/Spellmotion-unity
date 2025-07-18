using UnityEngine;

public class SpellBook : MonoBehaviour
{
	public ArcaneBlast arcaneBlast;
	public Fireball fireball;
	public Lightning lightning;
	public IceSpikes iceSpikes;
	public Heal heal;

	[Header("Animation")]
	public MageAnimator mageAnimator;

	public void CastArcaneBlast() => TryCast("ArcaneBlast", arcaneBlast, () => mageAnimator?.PlayArcaneBlast());
	public void CastFireball() => TryCast("Fireball", fireball, () => mageAnimator?.PlayFireball());
	public void CastLightning() => TryCast("Lightning", lightning, () => mageAnimator?.PlayLightning());
	public void CastIceSpikes() => TryCast("IceSpikes", iceSpikes, () => mageAnimator?.PlayIceSpikes());
	public void CastHeal() => TryCast("Heal", heal, () => mageAnimator?.PlayHeal());


	private void TryCast(string label, BaseSpell spell, System.Action playAnim)
	{
		if (spell == null)
		{
			Debug.LogWarning($"[SpellBook] {label}: référence NULL.");
			mageAnimator?.PlayNo();
			return;
		}

		if (!spell.CanCast())
		{
			Debug.Log($"[SpellBook] {label}: déjà en cooldown ({spell.cooldown}s).");
			mageAnimator?.PlayNo();
			return;
		}

		bool began = spell.BeginCast();
		if (!began)
		{
			Debug.Log($"[SpellBook] {label}: BeginCast() a échoué (déjà en cooldown).");
			mageAnimator?.PlayNo();
			return;
		}

		playAnim?.Invoke();

		spell.Cast();
		Debug.Log($"[SpellBook] {label}: Cast lancé. Cooldown démarré ({spell.cooldown}s).");
	}
}
