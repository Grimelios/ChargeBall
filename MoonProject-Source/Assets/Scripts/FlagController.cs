using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FlagController : MonoBehaviour
{
	private NumericCycleController launchDisplay;
	private NumericCycleController deathDisplay;
	private Text timeDisplay;

	private void Start()
	{
		Canvas canvas = FindObjectOfType<Canvas>();
		NumericCycleController[] cyclers = canvas.GetComponentsInChildren<NumericCycleController>();

		launchDisplay = cyclers[0];
		deathDisplay = cyclers[1];
		timeDisplay = canvas.GetComponentsInChildren<Text>()[2];
	}

	private void OnTriggerEnter2D(Collider2D collider)
	{
		// The player is the only moving object that can touch the flag.
		PlayerController player = collider.gameObject.GetComponent<PlayerController>();
		player.Lock();

		TimeSpan completionTime = DateTime.Now - player.SpawnTime;

		string timeString = string.Format("{0:D2}.{1:D3}", completionTime.Seconds, completionTime.Milliseconds);

		if (completionTime.Hours > 0)
		{
			timeString = completionTime.Hours + string.Format(":{0:D2}", completionTime.Minutes) + timeString;
		}
		else
		{
			timeString = completionTime.Minutes + ":" + timeString;
		}

		launchDisplay.Refresh(player.TotalJumps);
		deathDisplay.Refresh(player.TotalDeaths);
		timeDisplay.text = timeString;
	}
}
