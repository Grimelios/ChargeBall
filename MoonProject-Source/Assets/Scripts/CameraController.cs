using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
	private enum CameraModes
	{
		Fixed,
		Follow,
		Interpolate
	}

	private Transform playerTransform;

	private CameraModes mode = CameraModes.Follow;

	private void Awake()
	{
		DontDestroyOnLoad(this);
	}

	private void Start()
	{
		// This assumes the player is already active in the scene (created by the player spawn controller).
		playerTransform = GameObject.FindWithTag("Player").transform;
	}

	private void Update()
	{
		switch (mode)
		{
			case CameraModes.Follow:
				Vector2 playerPosition = playerTransform.position;
				
				transform.position = new Vector3(playerPosition.x, playerPosition.y, -10);

				break;

			case CameraModes.Interpolate:
				break;
		}
	}
}
