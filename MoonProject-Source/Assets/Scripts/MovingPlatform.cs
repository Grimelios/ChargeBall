using System.Collections;
using System.Collections.Generic;
using Assets.Classes;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
	public enum PlatformMovementTypes
	{
		Vertical,
		Horizontal,
		Unassigned
	}

	private Rigidbody2D body;
	private Vector2 spawnPosition;
	private Vector2 targetPosition;

	private bool active;
	private bool moving;
	private bool movingAway = true;

	private float elapsed;

	public PlatformMovementTypes movementType;

	public int movementDistance;

	public float movementDuration;
	public float stallDuration;

	private void Awake()
	{
		body = GetComponent<Rigidbody2D>();
		spawnPosition = transform.position;
		targetPosition = spawnPosition + (movementType == PlatformMovementTypes.Horizontal
			? new Vector2(movementDistance, 0)
			: new Vector2(0, movementDistance));
	}

	private void OnCollisionEnter2D(Collision2D collision)
	{
		// Only downward collisions activate the platform.
		if (!active && collision.GetContact(0).normal.y == -1)
		{
			active = true;
			moving = true;

			Debug.Log("Platform activated.");
		}
	}

	private void FixedUpdate()
	{
		// Moving platforms start stationary, then, when activated, move towards their target location over a preset
		// duration, hold the target position, then cycle back.
		if (!active)
		{
			return;
		}

		elapsed += Time.deltaTime;

		if (moving)
		{
			Vector2 start = movingAway ? spawnPosition : targetPosition;
			Vector2 end = movingAway ? targetPosition : spawnPosition;

			if (elapsed >= movementDuration)
			{
				body.position = end;
				moving = false;
				active = movingAway;
				movingAway = !movingAway;
				elapsed = 0;
			}
			else
			{
				body.position = Vector2.Lerp(start, end, EaseFunctions.Ease(elapsed / movementDuration,
					EaseTypes.QuadraticInOut));
			}
		}
		else if (elapsed > stallDuration)
		{
			elapsed = 0;
			moving = true;
		}
	}
}
