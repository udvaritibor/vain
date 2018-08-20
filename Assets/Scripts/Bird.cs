using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bird : MonoBehaviour
{
	public float MovementSpeed;
	public float FrameTime;
	public List<Sprite> Frames;


	private bool firstSeen;
	private float animationTimer = -1;
	private new SpriteRenderer renderer;
	private int frameID = 0;

	void Start ()
	{
		renderer = GetComponent<SpriteRenderer>();
	}
	
	void Update ()
	{
		if (!firstSeen)
		{
			if(renderer.isVisible)
			{
				firstSeen = true;
			}
			else
			{
				return;
			}
		}
		HandleMovement();
		HandleAnimation();
	}

	void HandleMovement()
	{
		transform.position = transform.position + new Vector3(MovementSpeed * Time.deltaTime, 0, 0);
	}

	void HandleAnimation()
	{
		animationTimer -= Time.deltaTime;
		if (animationTimer < 0)
		{
			renderer.sprite = Frames[frameID++];
			if (frameID == Frames.Count)
			{
				frameID = 0;
			}
			animationTimer = FrameTime;
		}
	}
}
