using UnityEngine;
using TMPro;

public class WaveUIManager : MonoBehaviour
{
	[Header("UI References")]
	public TextMeshProUGUI waveText;        // Affiche la wave
	public TextMeshProUGUI enemiesText;     // Affiche le nombre d'ennemis restants
	public int totalWaves = 0;

	private void Awake()
	{
		if (waveText == null || enemiesText == null)
			Debug.LogWarning("[WaveUIManager] Assurez-vous d'assigner WaveText et EnemiesText dans l'Inspector.");
	}

	public void SetTotalWaves(int total)
	{
		totalWaves = total;
	}

	public void SetWave(int waveNumber)
	{
		if (waveText != null)
			waveText.text = $"Wave: {waveNumber} / {totalWaves}";
	}

	public void SetEnemiesRemaining(int count)
	{
		if (enemiesText != null)
			enemiesText.text = $"Enemies Left: {count}";
	}
}
