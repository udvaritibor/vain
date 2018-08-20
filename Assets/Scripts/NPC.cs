using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class NPC : MonoBehaviour
{
	private AudioSource audioSource;
	private GameObject bubble;
	private TextMeshPro text;

	public float TextDelayTime;
	public float FrameTime;
	public List<Sprite> Frames;

	private float animationTimer = -1;
	private new SpriteRenderer renderer;
	private int frameID = 0;

	private float textDelay = -1;

	void Start ()
	{
		audioSource = GetComponent<AudioSource>();
		bubble = transform.GetChild(0).gameObject;
		text = bubble.transform.GetChild(0).GetComponent<TextMeshPro>();
		bubble.SetActive(false);
		renderer = GetComponent<SpriteRenderer>();
	}
	

	void Update ()
	{
		HandleAnimation();
		if(renderer.isVisible && textDelay < 0)
		{
			textDelay = TextDelayTime;
		}
		if(textDelay > 0)
		{
			textDelay -= Time.deltaTime;
			if(textDelay < 0)
			{
				bubble.SetActive(true);
			}
		}
		//todo: play sound effect for text
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
