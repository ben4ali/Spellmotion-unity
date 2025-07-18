using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class NexusHealthUI : MonoBehaviour
{
	[Header("UI References")]
	public Slider healthSlider;
	public TextMeshProUGUI healthText;

	[Header("Target")]
	public NexusController nexus;

	private Camera mainCam;

	void Start()
	{
		if (nexus == null)
			nexus = FindObjectOfType<NexusController>();

		if (nexus != null)
			healthSlider.maxValue = nexus.maxHP;

		mainCam = Camera.main;
	}

	void Update()
	{
		if (nexus == null) return;

		healthSlider.value = nexus.currentHP;
		healthText.text = $"{nexus.currentHP} / {nexus.maxHP}";

		if (mainCam != null)
			transform.LookAt(transform.position + mainCam.transform.forward);
	}
}
