using System.Collections;
using System.Collections.Generic;
using Assets.Classes;
using UnityEngine;

public class GateController : MonoBehaviour
{
	private const float ZValue = 0.1f;

	public enum GateStates
	{
		Closed,
		Stutter,
		Holding,
		Opening,
		Open
	}

	private Vector2 originalPosition;
	private Vector2 basePosition;
	private Vector2 targetPosition;

	private float height;
	private float currentDuration;
	private float elapsed;

	public float stutterDistance;
	public float stutterDuration;
	public float holdTime;
	public float openDuration;

	private GateStates state = GateStates.Closed;

	private void Awake()
	{
		height = GetComponent<BoxCollider2D>().size.y;
	}

	private void Update()
	{
		if (state == GateStates.Closed || state == GateStates.Open)
		{
			return;
		}

		elapsed += Time.deltaTime;

		if (elapsed >= currentDuration)
		{
			if (state != GateStates.Holding)
			{
				transform.position = new Vector3(targetPosition.x, targetPosition.y, ZValue);
			}

			elapsed -= currentDuration;

			switch (state)
			{
				case GateStates.Stutter:
					state = GateStates.Holding;
					currentDuration = holdTime;

					break;

				case GateStates.Holding:
					state = GateStates.Opening;
					basePosition = targetPosition;
					targetPosition = originalPosition + new Vector2(0, height);
					currentDuration = openDuration;

					break;

				case GateStates.Opening:
					state = GateStates.Open;

					return;
			}
		}

		if (state != GateStates.Holding)
		{
			float amount = elapsed / currentDuration;

			EaseTypes easeType = state == GateStates.Stutter ? EaseTypes.CubicOut : EaseTypes.CubicIn;

			Vector2 p = Vector2.Lerp(basePosition, targetPosition, EaseFunctions.Ease(amount, easeType));

			// Adding a small Z value causes gates to render behind the tilemap as they open.
			transform.position = new Vector3(p.x, p.y, ZValue);
		}
	}

	public void Open()
	{
		state = GateStates.Stutter;
		originalPosition = transform.position;
		basePosition = originalPosition;
		targetPosition = originalPosition + new Vector2(0, stutterDistance);
		currentDuration = stutterDuration;
	}
}
