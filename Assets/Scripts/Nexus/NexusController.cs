using UnityEngine;

public class NexusController : MonoBehaviour
{
	public int maxHP = 500;
	public int currentHP;

	private void Start()
	{
		currentHP = maxHP;
	}

	public void TakeDamage(int amount)
	{
		currentHP -= amount;
		Debug.Log("Nexus HP: " + currentHP);
		if (currentHP <= 0)
		{
			Debug.Log("Game Over!");
		}
	}

	public void Heal(int amount)
	{
		currentHP = Mathf.Min(currentHP + amount, maxHP);
		Debug.Log("Nexus healed. HP: " + currentHP);
	}

	public bool IsDead()
	{
		return currentHP <= 0;
	}

}
