using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;

/*
 * Handles the movement of the ants - based on Rock Ants
 * (Temnothorax rugatulus).
*/

public class Meander : MonoBehaviour
{
	private Rigidbody2D npcRigidbody;
	private float npcLength;

	[SerializeField] private float speed = 5f;
	[SerializeField] private AnimationCurve distribution;

	private Vector2 startPosition;
	private float maxTravelDistance;

	private float randomRotationAngle;

	private bool turnLeft;

	void Awake()
	{
		npcRigidbody = GetComponent<Rigidbody2D>();

		// Calculate length of NPC
		CalculateNPCLength();

		// Set max travel distance of NPC
		maxTravelDistance = 3 * npcLength;
	}

	void Start()
	{
		startPosition = this.transform.position;

		// Set random rotation angle
		randomRotationAngle = Random.Range(-180, 180f);
		this.transform.rotation = Quaternion.Euler(0, 0, randomRotationAngle);

		// Set direction NPC will turn first
		SetFirstTurnDirection();
	}

	void Update()
	{
		DetectObstacles();

		// Move NPC in the direction they are facing
		Vector2 forward = new Vector2(this.transform.up.x, this.transform.up.y);
		npcRigidbody.velocity = forward * speed;

		if (Vector2.Distance(npcRigidbody.position, startPosition) >= maxTravelDistance)
		{
			if (turnLeft)
			{
				randomRotationAngle -= GetRandomRotationAngle() * 180;
				this.transform.rotation = Quaternion.Euler(0, 0, randomRotationAngle);
				turnLeft = false;

				// Reset start position
				startPosition = this.transform.position;
			}
			else	// Turn right
			{
				randomRotationAngle += GetRandomRotationAngle() * 180;
				this.transform.rotation = Quaternion.Euler(0, 0, randomRotationAngle);
				turnLeft = true;

				// Reset start position
				startPosition = this.transform.position;
			}
		}
	}

	private void SetFirstTurnDirection()
	{
		float randomDirection = Random.Range(0f, 1f);

		if (randomDirection < 0.5f)
		{
			turnLeft = false;
		}
		else
		{
			turnLeft = true;
		}
	}

	private float GetRandomRotationAngle()
	{
		float randomRotationAngle = Random.Range(0f, 1f);

		return distribution.Evaluate(randomRotationAngle);
	}

	private void CalculateNPCLength()
	{
		Renderer renderer = this.GetComponentInChildren<Renderer>();

		if (renderer != null)
		{
			npcLength = renderer.bounds.size.y;
		}
		else
		{
			Debug.Log($"No renderer found on: {this.gameObject.name}");
		}
	}

	private void DetectObstacles()
	{
		// Perform raycast
		RaycastHit2D hit = Physics2D.Raycast(this.transform.position, this.transform.up, npcLength);
		Debug.DrawRay(this.transform.position, this.transform.up);

		// Check if raycast hit
		if (hit.collider != null)
		{
			if (hit.collider.gameObject.transform.parent != null && hit.collider.gameObject.transform.parent.CompareTag("Obstacle"))
			{
				Debug.Log($"Obstacle detected: {hit.collider.gameObject.transform.parent.name}");

				// Calculate the direction away from the obstacle
				Vector2 directionAwayFromObstacle = this.transform.position - hit.transform.position;

				directionAwayFromObstacle.Normalize();

				// Calculate the angle to rotate the NPC
				float angle = Mathf.Atan2(directionAwayFromObstacle.y, directionAwayFromObstacle.x) * Mathf.Rad2Deg;

				// Apply rotation
				this.transform.rotation = Quaternion.Euler(0, 0, angle);
			}
		}
	}
}
