using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class ChargeBarController : MonoBehaviour
{
	private SpriteRenderer sprite;
	private Material material;
	
	// Charge is restricted to the 0-1 (between minimum charge and maximum charge).
	public float Charge
	{
		set { material.SetFloat("charge", value); }
	}

	private void Awake()
	{
		DontDestroyOnLoad(this);

		material = GetComponent<SpriteRenderer>().material;
		sprite = GetComponent<SpriteRenderer>();
		Charge = 0;
	}

	public void Hide()
	{
		sprite.enabled = false;
	}
}
