using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
	public float LogoFadeInTime;
	public float LogoWaitingTime;
	public float LogoFadeOutTime;
	public float MenuHiderFadeOutTime;
	public GameObject DevHint;
	public Image LogoCover;
	public Image MenuCover;

	private float startingTimer = -1;
	private int phase = 0;

	private AudioSource audioSource;

	// Use this for initialization
	void Start ()
	{
		Cursor.visible = false;
		startingTimer = LogoFadeInTime;
		DevHint.SetActive(false);
		LogoCover.gameObject.SetActive(true);
		MenuCover.gameObject.SetActive(true);
		audioSource = GetComponent<AudioSource>();
		if (Time.timeScale < 0.001f)
		{
			LogoCover.transform.parent.gameObject.SetActive(false);
			Time.timeScale = 1;
			MenuCover.gameObject.SetActive(true);
			phase = 3;
			startingTimer = MenuHiderFadeOutTime;
			audioSource.Play();
		}
	}
	
	// Update is called once per frame
	void Update ()
	{
		HandleStartingAnimations();
	}

	private void HandleStartingAnimations()
	{
		if(startingTimer > 0)
		{
			startingTimer -= Time.deltaTime;
			if(phase == 0)
			{
				float a = Mathf.Lerp(1, 0, 1.0f - (startingTimer / LogoFadeInTime));
				LogoCover.color = new Color(0, 0, 0, a);
				if(startingTimer < 0.001f)
				{
					phase = 1;
					startingTimer = LogoWaitingTime;
				}
			}
			else if(phase == 1)
			{
				if (startingTimer < 0.001f)
				{
					phase = 2;
					startingTimer = LogoFadeOutTime;
				}
			}
			else if(phase == 2)
			{
				float a = Mathf.Lerp(0, 1, 1.0f - (startingTimer / LogoFadeOutTime));
				LogoCover.color = new Color(0, 0, 0, a);
				if (startingTimer < 0.001f)
				{
					phase = 3;
					startingTimer = MenuHiderFadeOutTime;
					LogoCover.transform.parent.gameObject.SetActive(false);
					audioSource.Play();
				}
			}
			else if(phase == 3)
			{
				float a = Mathf.Lerp(1, 0, 1.0f - (startingTimer / MenuHiderFadeOutTime));
				MenuCover.color = new Color(0, 0, 0, a);
				if (startingTimer < 0.001f)
				{
					MenuCover.gameObject.SetActive(false);
					Cursor.visible = true;
					DevHint.SetActive(true);
				}
			}
		}
	}

	public void Intro()
	{
		if (!Cursor.visible) return;
		SceneManager.LoadScene("Intro");
	}

	public void SeaOfWords()
	{
		if (!Cursor.visible) return;
		SceneManager.LoadScene("SeaOfWords");
	}

	public void SeaOfTime()
	{
		if (!Cursor.visible) return;
		SceneManager.LoadScene("SeaOfTime");
	}

	public void End()
	{
		if (!Cursor.visible) return;
		SceneManager.LoadScene("End");
	}

	public void Exit()
	{
		if (!Cursor.visible) return;
		Application.Quit();
	}

}
