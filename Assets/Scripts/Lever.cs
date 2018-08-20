using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lever : MonoBehaviour
{
	public Sprite ActivatedVersion;
	public GameObject TargetObject;
	public AudioClip LeverPull;

	private bool activated;
	private new SpriteRenderer renderer;
	private AudioSource audioSource;

	void Start ()
	{
		renderer = GetComponent<SpriteRenderer>();
		audioSource = GetComponent<AudioSource>();
	}

	void OnTriggerEnter2D(Collider2D other)
	{
		if(!activated)
		{
			activated = true;
			renderer.sprite = ActivatedVersion;
			TargetObject.SetActive(!TargetObject.activeSelf);
			audioSource.PlayOneShot(LeverPull);
		}
	}
}
