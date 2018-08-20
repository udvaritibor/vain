using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CollapsingLeftSide : MonoBehaviour
{
	public float MovementSpeed;

	private AudioSource audioSource;

	private string sceneName;

	private Player player;

	private int phase;

	public void StartTheCollapse()
	{
		player = GameObject.Find("Player").GetComponent<Player>();
		sceneName = SceneManager.GetActiveScene().name;
		if (sceneName == "SeaOfWords")
		{
			audioSource.time = 3f;
		}
		audioSource.Play();
	}

	public bool IsMusicOver()
	{
		return !audioSource.isPlaying;
	}

	void Awake()
	{
		audioSource = GetComponent<AudioSource>();
	}

	void Update()
	{
		if (audioSource.isPlaying)
		{
			if (sceneName == "SeaOfWords")
			{
				if (transform.position.x > 155 && transform.position.x < 172 && phase == 0)
				{
					phase = 1;
					MovementSpeed *= 0.6f;
					player.MovementSpeed *= 0.6f;
					player.FlapStrengthX *= 0.6f;
				}
				if (transform.position.x > 172 && phase == 1)
				{
					phase = 2;
					MovementSpeed *= 3f;
					player.MovementSpeed *= 3f;
					player.FlapStrengthX *= 3f;
				}
				if (transform.position.x > 385 && phase == 2)
				{
					phase = 3;
					MovementSpeed *= 0.5f;
					player.MovementSpeed *= 0.5f;
					player.FlapStrengthX *= 0.5f;
				}
			}
			transform.position = transform.position + new Vector3(MovementSpeed * Time.deltaTime, 0);
		}
	}

	void OnTriggerEnter2D(Collider2D other)
	{
		if(other.tag == "Player")
		{
			Player player = other.GetComponent<Player>();
			player.Die();
			audioSource.Stop();
		}
	}
}
