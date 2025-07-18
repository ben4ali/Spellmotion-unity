using UnityEngine;
using System.Collections;

public class WaveManager : MonoBehaviour
{
	[Header("Wave Settings")]
	public int totalWaves = Constants.TOTAL_WAVES;
	public float minSpawnInterval = Constants.ENEMY_SPAWN_MIN_INTERVAL;
	public float maxSpawnInterval = Constants.ENEMY_SPAWN_MAX_INTERVAL;
	public Transform spawnPoint;
	public GameObject[] enemyPrefabs;

	private int currentWave = 0;
	private bool spawning = false;
	private int enemiesAlive = 0;
	public WaveUIManager waveUI;
	private int enemiesInCurrentWave = 0;
	private int enemiesSpawnedThisWave = 0;

	private void Start()
	{
		if (waveUI == null)
		{
			waveUI = FindObjectOfType<WaveUIManager>();
			waveUI.SetTotalWaves(totalWaves);
		}
		StartCoroutine(StartNextWave());
	}

	private IEnumerator StartNextWave()
	{
		yield return new WaitForSeconds(2f);
		while (currentWave < totalWaves && !GameManager.Instance.gameOver)
		{
			currentWave++;
			Debug.Log($"Wave {currentWave}/{totalWaves} starting...");

			if (waveUI != null)
			{
				waveUI.SetWave(currentWave);
				waveUI.SetEnemiesRemaining(0);
			}

			yield return StartCoroutine(SpawnWave(currentWave));
			while (enemiesAlive > 0) yield return null;
			Debug.Log($"Wave {currentWave} cleared!");
			yield return new WaitForSeconds(3f);
		}

		if (!GameManager.Instance.gameOver)
		{
			GameManager.Instance.Victory();
		}
	}

	private IEnumerator SpawnWave(int waveNumber)
	{
		spawning = true;
		int enemyCount = 3 + (waveNumber * 2);
		enemiesInCurrentWave = enemyCount;

		for (int i = 0; i < enemyCount; i++)
		{
			SpawnEnemy();
			if (waveUI != null)
			{
				waveUI.SetEnemiesRemaining(enemiesAlive);
			}
			yield return new WaitForSeconds(Random.Range(minSpawnInterval, maxSpawnInterval));
		}
		spawning = false;
	}

	private void SpawnEnemy()
	{
		GameObject prefab = enemyPrefabs[Random.Range(0, enemyPrefabs.Length)];
		GameObject enemy = Instantiate(prefab, spawnPoint.position, Quaternion.identity);
		enemiesAlive++;

		EnemyController controller = enemy.GetComponent<EnemyController>();
		controller.OnEnemyDeath += () => enemiesAlive--;
	}
}
