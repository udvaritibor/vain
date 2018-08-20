using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
	public GameObject EscPanel;

	public AudioClip ButtonHover;
	public AudioClip ButtonClick;

	public bool IsEscMenuShown
	{
		get { return EscPanel.activeSelf; }
	}

	private AudioSource audioSource;

	public void ShowEscMenu()
	{
		EscPanel.SetActive(true);
		Time.timeScale = 0;
		audioSource.PlayOneShot(ButtonHover);
	}

	public void HideEscMenu()
	{
		EscPanel.SetActive(false);
		Time.timeScale = 1;
		audioSource.PlayOneShot(ButtonHover);
	}

	public void MyUpdate(Player player)
	{
		if(player.BirdMechanics)
		{
			UpdateBirdMechanicsInterface(player.MaxBirdFlaps, player.RemainingBirdFlaps);
		}

		if(EscPanel.activeSelf)
		{
			HandleEscPanel();
		}
		
	}

	private void Awake()
	{
		audioSource = GetComponent<AudioSource>();
	}

	void UpdateBirdMechanicsInterface(int maxBirdFlaps, int remainingBirdFlaps)
	{
		//todo
	}

	void HandleEscPanel()
	{
		if (Input.GetKeyDown(KeyCode.Return))
		{
			EscPanel.GetComponent<EscPanel>().HitButton();
			audioSource.PlayOneShot(ButtonClick);
		}

		int dir = 0;
		if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
		{
			dir = -1;
		}
		else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
		{
			dir = 1;
		}
		if(dir != 0)
		{
			EscPanel.GetComponent<EscPanel>().MoveSelection(dir);
			audioSource.PlayOneShot(ButtonHover);
		}
		
		
	}
}
