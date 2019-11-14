using System.Collections;
using System.Collections.Generic;
using Assets.Classes;
using UnityEngine;
using UnityEngine.UI;

public class NumericCycleController : MonoBehaviour
{
	private const int MinimumRate = 4;

	private Text text;

	private int currentValue;
	private int targetValue;

	private float accumulator;

	public string label;

	private void Awake()
	{
		text = GetComponent<Text>();
	}

	public void Refresh(int targetValue)
	{
		if (targetValue == 0)
		{
			text.text = label + ": 0";
			text.color = new Color(255, 215, 0);

			return;
		}
		
		this.targetValue = targetValue;

		currentValue = 0;
	}

	private void Update()
	{
		if (currentValue == targetValue)
		{
			return;
		}
		
		// At first, I tried using pre-defined threshold classes to determine the numeric rate (similar to the
		// player's charge thresholds). I later realized it's much simpler to just use the current value as the
		// rate, which also ensures a smooth slowdown as the target value approaches.
		accumulator += Mathf.Max((targetValue - currentValue), MinimumRate) * Time.deltaTime;
		currentValue = Mathf.Min((int)Mathf.Floor(accumulator), targetValue);
		text.text = label + ": " + currentValue;
	}
}
