using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Levitate : MonoBehaviour
{
	public float amplitude = 0.5f;
	public float duration = 2f;

	private Vector3 startPosition;
	void Start()
	{
		startPosition = transform.position;
		StartLevitation();
	}

	void StartLevitation()
	{
		transform.DOMoveY(startPosition.y + amplitude, duration)
				.SetLoops(-1, LoopType.Yoyo)
				.SetEase(Ease.InOutSine);
	}
}