using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpawnController : MonoBehaviour
{
	public GameObject playerPrefab;

	private void Awake()
	{
		if (GameObject.FindWithTag("Player") == null)
		{
			Instantiate(playerPrefab, transform.position, Quaternion.identity);
		}
	}
}
