using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchController : MonoBehaviour
{
	private SpriteRenderer sprite;
	private AudioSource activationSound;

	public Color idleColor;
	public Color activatedColor;

	public GateController linkedGate;

	private void Awake()
	{
		sprite = GetComponent<SpriteRenderer>();
		sprite.color = idleColor;
		activationSound = GetComponent<AudioSource>();
	}

	private void OnTriggerEnter2D()
	{
		sprite.color = activatedColor;
		linkedGate.Open();
		activationSound.Play();

		// Once the gate has been triggered to open, the switch can safely be disabled.
		GetComponent<Collider2D>().enabled = false;
	}
}
