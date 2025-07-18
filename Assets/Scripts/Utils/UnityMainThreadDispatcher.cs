using System;
using System.Collections.Generic;
using UnityEngine;

public class UnityMainThreadDispatcher : MonoBehaviour
{
	private static readonly Queue<Action> _executionQueue = new Queue<Action>();
	private static UnityMainThreadDispatcher _instance;

	public static UnityMainThreadDispatcher Instance()
	{
		if (_instance == null)
		{
			Debug.LogError("[UnityMainThreadDispatcher] Pas trouvé dans la scène !");
			throw new Exception("Ajoute UnityMainThreadDispatcher dans la scène avant d'utiliser Enqueue.");
		}
		return _instance;
	}

	private void Awake()
	{
		if (_instance == null)
		{
			_instance = this;
			DontDestroyOnLoad(gameObject);
		}
		else
		{
			Destroy(gameObject);
		}
	}

	public void Enqueue(Action action)
	{
		if (action == null) return;
		lock (_executionQueue)
		{
			_executionQueue.Enqueue(action);
		}
	}

	private void Update()
	{
		lock (_executionQueue)
		{
			while (_executionQueue.Count > 0)
			{
				var action = _executionQueue.Dequeue();
				action?.Invoke();
			}
		}
	}
}
