using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Classes;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
	private const float AngularDrag = 0.6f;
	private const float BarZ = -0.1f;

	private Rigidbody2D body;
	private Vector2 launchVector;
	private Vector2 previousMouseScreenPosition;
	private ChargeBarController chargeBar;
	private SpringValue spring;
	private AudioSource launchSound;
	
	private bool charging;
	private bool inputLocked;
	private bool justLaunched;

	// The ball's spin is manually changed when colliding with a vertical wall. These two variables help determine
	// when that manual correction is needed.
	private bool hitWallThroughNormalCollision;

	private float elapsed;
	private float previousVelocityX;

	public ChargeBarController chargeBarPrefab;
	public Vector2 maximumSpeed;

	public float barOffset;
	public float bouncedAngularVelocityFactor;
	public float launchAngularVelocityFactor;
	public float wallBounceSpeedFactor;
	public float forcedReversalRange;
	public float maximumChargeTime;
	public float maximumSquishiness;
	public float minimumPitch;
	public float maximumPitch;

	public int minimumForce;
	public int maximumForce;

	public int TotalJumps { get; private set; }
	public int TotalDeaths { get; private set; }

	public DateTime SpawnTime { get; private set; }

	private void Awake()
	{
		DontDestroyOnLoad(this);

		body = GetComponent<Rigidbody2D>();
		body.angularDrag = AngularDrag;
		chargeBar = Instantiate(chargeBarPrefab);
		spring = new SpringValue(1);
		launchSound = GetComponent<AudioSource>();
		SpawnTime = DateTime.Now;
	}

	private void OnCollisionEnter2D(Collision2D collision)
	{
		Vector2 normal = collision.GetContact(0).normal;

		if (Mathf.Abs(normal.x) == 1)
		{
			Vector2 v = body.velocity;
			v.x *= wallBounceSpeedFactor;
			body.velocity = v;
			hitWallThroughNormalCollision = true;

			OnWallCollision(normal);
		}
	}
	
	private void OnTriggerEnter2D(Collider2D collider)
	{
		if (collider.gameObject.CompareTag("Hazard"))
		{
			Respawn(collider.gameObject.GetComponent<HazardController>().respawnPoint.position);
		}
	}

	private void OnWallCollision(Vector2 normal)
	{
		body.angularVelocity = -normal.x * Mathf.Abs(body.velocity.x) * bouncedAngularVelocityFactor;
	}

	private void Respawn(Vector2 respawnPoint)
	{
		transform.position = respawnPoint;
		body.velocity = Vector2.zero;
		body.angularVelocity = 0;
		charging = false;
		chargeBar.Charge = 0;

		TotalDeaths++;
	}

	private void Update()
	{
		// Quitting code would ordinarily be managed in a menu, but for this project, it's easier to put it here (since
		// there's one persistent copy of the player and the player is present in every scene).
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			Application.Quit();
		}

		if (inputLocked)
		{
			return;
		}

		justLaunched = false;

		Vector2 mousePosition = Input.mousePosition;
		Vector2 playerPosition = transform.position;

		// The mouse overrides joystick aim if the mouse was moved this frame.
		if (mousePosition != previousMouseScreenPosition)
		{
			Vector2 worldPosition = Camera.main.ScreenToWorldPoint(mousePosition);

			previousMouseScreenPosition = mousePosition;
			launchVector = (worldPosition - playerPosition).normalized;
		}
		else
		{
			Vector2 joystickVector = new Vector2(Input.GetAxis("Joystick X"), -Input.GetAxis("Joystick Y"));

			if (joystickVector != Vector2.zero)
			{
				launchVector = joystickVector.normalized;
			}
		}

		if (charging)
		{
			elapsed += Time.deltaTime;
			elapsed = Mathf.Min(elapsed, maximumChargeTime);

			float t = EaseFunctions.Ease(elapsed / maximumChargeTime, EaseTypes.QuadraticOut);

			chargeBar.Charge = t;
			spring.targetValue = 1 - maximumSquishiness * t;

			if (Input.GetButtonUp("Launch") && launchVector != Vector2.zero)
			{
				Launch();
			}
		}
		else if (Input.GetButtonDown("Launch"))
		{
			elapsed = 0;
			charging = true;
		}

		float launchRotation = Mathf.Atan2(launchVector.y, launchVector.x) * Mathf.Rad2Deg;

		Vector2 barPosition = playerPosition + launchVector * barOffset;

		Transform arrowTransform = chargeBar.transform;
		arrowTransform.position = new Vector3(barPosition.x, barPosition.y, BarZ);
		arrowTransform.rotation = Quaternion.Euler(0, 0, launchRotation);

		spring.Update();
		transform.localScale = new Vector3(spring.value, spring.value, 1);
	}

	private void Launch()
	{
		Vector2 v = body.velocity;

		float angle = Mathf.Abs(Mathf.Atan2(launchVector.y, launchVector.x));
		float halfReversalRange = forcedReversalRange / 2;

		if (angle > Mathf.PI / 2)
		{
			angle = Mathf.PI - angle;
		}

		float t = EaseFunctions.Ease(elapsed / maximumChargeTime, EaseTypes.QuadraticOut);
		float force = (maximumForce - minimumForce) * t + minimumForce;

		// This correction mean it's easier to quickly reverse X direction through a quick, weak launch.
		if (Math.Sign(v.x * launchVector.x) <= 0 && angle <= halfReversalRange)
		{
			v.x = 0;

			// This spin correction is comparable to the special case in FixedUpdate.
			body.angularVelocity = -launchVector.x * force * launchAngularVelocityFactor;
		}

		// If a launch is aimed upwards while falling, the falling speed is negated. This makes it easier to stay
		// airborne.
		if (launchVector.y > 0 && v.y < 0)
		{
			v.y = 0;
		}

		body.velocity = v;
		body.AddForce(launchVector * force);
		TotalJumps++;
		justLaunched = true;
		charging = false;
		chargeBar.Charge = 0;
		spring.targetValue = 1;

		launchSound.pitch = t * (maximumPitch - minimumPitch) + minimumPitch;
		launchSound.Play();
	}

	private void FixedUpdate()
	{
		// When the ball hits a vertical wall, its angular velocity is forcibly set to rotate away from the wall.
		// However, when the ball is rolling along the ground and then hits a wall, a new collision isn't registered
		// (through either OnCollisionEnter2D or OnCollisionStay2D). This code accounts for this special case by
		// manually checking whether the ball's velocity swapped directions when not a result of 1) a "normal" wall
		// collision, or 2) activating a launch.
		//
		// It's possible there's an easy fix to this problem, but in the interest of time, this solution works.
		Vector2 v = body.velocity;

		if (!(justLaunched || hitWallThroughNormalCollision))
		{
			int sign = Math.Sign(v.x);

			if (Math.Sign(sign * previousVelocityX) == -1)
			{
				OnWallCollision(new Vector2(sign, 0));
			}
		}

		// With unlimited launches, it's too easy to reach speeds too fast to reasonably control. Restricting max
		// speed helps alleviate this.
		if (Mathf.Abs(v.x) > maximumSpeed.x)
		{
			v.x = maximumSpeed.x * Mathf.Sign(v.x);
		}

		if (Mathf.Abs(v.y) > maximumSpeed.y)
		{
			v.y = maximumSpeed.y * Mathf.Sign(v.y);
		}

		body.velocity = v;
		previousVelocityX = v.x;
		hitWallThroughNormalCollision = false;
	}

	public void Lock()
	{
		if (charging)
		{
			charging = false;
			chargeBar.Hide();
		}

		inputLocked = true;
	}
}
