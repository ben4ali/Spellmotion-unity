using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AbilityIconUI : MonoBehaviour
{
	[Header("Links")]
	public BaseSpell spell;
	public Image iconImage;
	public Image cooldownOverlay;
	public TextMeshProUGUI cooldownText;

	[Header("Colors")]
	public Color readyColor = Color.white;
	public Color cooldownColor = new Color(0.5f, 0.5f, 0.5f, 1f); // grayscale tint

	private void Reset()
	{
		// auto-assign children if dropped fresh
		if (iconImage == null) iconImage = GetComponent<Image>();
		if (cooldownOverlay == null)
		{
			Transform t = transform.Find("CooldownOverlay");
			if (t) cooldownOverlay = t.GetComponent<Image>();
		}
		if (cooldownText == null)
		{
			Transform t = transform.Find("CooldownText");
			if (t) cooldownText = t.GetComponent<TextMeshProUGUI>();
		}
	}

	private void Update()
	{
		if (spell == null) return;

		float remain = spell.CooldownRemaining;
		bool ready = remain <= 0.001f;

		// Icon coloration
		if (iconImage != null)
			iconImage.color = ready ? readyColor : cooldownColor;

		// Overlay fill / visibility
		if (cooldownOverlay != null)
		{
			if (remain > 0f && spell.cooldown > 0f)
			{
				cooldownOverlay.enabled = true;

				// If overlay set to Filled radial, we can use normalized
				cooldownOverlay.type = Image.Type.Filled;
				cooldownOverlay.fillMethod = Image.FillMethod.Radial360;
				cooldownOverlay.fillAmount = remain / spell.cooldown;
			}
			else
			{
				cooldownOverlay.enabled = false;
			}
		}

		// Text countdown
		if (cooldownText != null)
		{
			if (remain > 0f)
			{
				cooldownText.gameObject.SetActive(true);
				cooldownText.text = Mathf.CeilToInt(remain).ToString();
			}
			else
			{
				cooldownText.gameObject.SetActive(false);
			}
		}
	}
}
