using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LastSceneManager : MonoBehaviour
{

	public float ScrollSpeed;

	public RectTransform Text;

	private AudioSource audioSource;

	void Start ()
	{
		audioSource = GetComponent<AudioSource>();
	}
	

	void Update ()
	{

		Text.localPosition = new Vector2(0, Text.localPosition.y + Time.deltaTime * ScrollSpeed);

		if (Input.GetKeyDown(KeyCode.Return))
		{
			SceneManager.LoadScene("Menu");
		}

		if (!audioSource.isPlaying)
		{
			Time.timeScale = 0;
			SceneManager.LoadScene("Menu");
		}
	}
}
