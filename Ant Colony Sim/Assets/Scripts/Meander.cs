using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Handles the movement of the ants - based on Rock Ants
 * (Temnothorax rugatulus).
 * -    78% of the time turn one direction then the next
 *      after roughly three times the length of the ant.
 * -    22% of the time is random movement.
*/

public class Meander : MonoBehaviour
{
	private Rigidbody2D npcRigidbody;

	[SerializeField] private float speed = 5f;
	[SerializeField] private float npcLength = .01f;

	private Vector2 startPosition;
	private float maxTravelDistance;

	private float randomTravelAngle;
	private float randomRotationAngle;

	void Awake()
	{
		npcRigidbody = GetComponent<Rigidbody2D>();
		maxTravelDistance = 3 * npcLength;
	}

	void Start()
	{
		startPosition = this.transform.position;

		// Set random travel angle
		randomTravelAngle = Random.Range(0f, 360f);
		this.transform.rotation = Quaternion.Euler(0, 0, randomTravelAngle);

		// Set random rotation angle
		randomRotationAngle = Random.Range(20f, 90f);
	}

	void Update()
	{
		// Move NPC in the direction they are facing
		Vector2 forward = new Vector2(this.transform.up.x, this.transform.up.y);
		npcRigidbody.velocity = forward * speed;

		if (Vector2.Distance(npcRigidbody.position, startPosition) >= maxTravelDistance)
		{
			// 
			if (randomTravelAngle < 360f)
			{
				randomTravelAngle += randomRotationAngle;
				this.transform.rotation = Quaternion.Euler(0, 0, randomTravelAngle);
			}
			else if (randomTravelAngle > 360f)
			{
				randomTravelAngle -= randomRotationAngle;
				this.transform.rotation = Quaternion.Euler(0, 0, randomTravelAngle);
			}

			// Update start position
			startPosition = npcRigidbody.position;
		}
	}
}
