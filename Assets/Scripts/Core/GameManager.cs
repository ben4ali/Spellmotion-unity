using UnityEngine;

public class GameManager : MonoBehaviour
{
	public static GameManager Instance;

	[Header("References")]
	public NexusController nexus;

	public bool gameOver = false;
	public MageAnimator mageAnimator;

	public void Victory()
	{
		if (gameOver) return;
		gameOver = true;
		mageAnimator?.PlayVictory();
		Debug.Log("Victory!");
		Time.timeScale = 0f;
	}

	public void GameOver()
	{
		if (gameOver) return;
		gameOver = true;
		mageAnimator?.PlayLost();
		Debug.Log("Game Over!");
		Time.timeScale = 0f;
	}
	private void Awake()
	{
		if (Instance == null) Instance = this;
		else Destroy(gameObject);
	}

	public Vector3 NexusPos => nexus != null ? nexus.transform.position : Vector3.zero;
}
