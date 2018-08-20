using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
	public Transform SceneTitle;
	public CollapsingLeftSide CollapsingLeftSide;
	public Player player;

	public float FadeInTime = 3.5f;
	public float FadeOutDelay; //this is the only thing which should be varied. (based on text length)
	public float FadeOutTime = 3.5f;

	private float sceneStartTimer = -1;
	private int sceneStartPhase = 0;

	public float EndingFadeOutTime = 5f;
	public Image EndFade;

	private float sceneEndTimer = -1;
	private UIManager uiManager;

	void Start ()
	{
		Cursor.visible = false;
		//set screen to previous levels fade out color
		sceneStartTimer = FadeInTime;
		uiManager = GetComponent<UIManager>();
		uiManager.HideEscMenu();
		EndFade.gameObject.SetActive(true);

	}

	void Update ()
	{
		if(player.IsDead)
		{
			if (!uiManager.IsEscMenuShown)
			{
				uiManager.ShowEscMenu();
			}
		}
		else if(Input.GetKeyDown(KeyCode.Escape))
		{
			if(uiManager.IsEscMenuShown)
			{
				uiManager.HideEscMenu();
			}
			else
			{
				uiManager.ShowEscMenu();
			}
		}

		uiManager.MyUpdate(player);

		HandleSceneStartAnimations();
		HandleSceneEndingAnimations();
	}

	void HandleSceneStartAnimations()
	{
		if (sceneStartTimer > 0)
		{
			sceneStartTimer -= Time.deltaTime;
			if (sceneStartPhase == 0) //fade in the title or the big
			{
				float a = Mathf.Lerp(1, 0, 1.0f - (sceneStartTimer / FadeInTime));
				Color newColor = EndFade.color;
				newColor.a = a;
				EndFade.color = newColor;

				if (sceneStartTimer < 0.0001f) //fade in complete
				{
					sceneStartPhase = 1;
					sceneStartTimer = FadeOutDelay;
				}
			}
			else if (sceneStartPhase == 1) //waiting for the player to read second text
			{
				if (sceneStartTimer < 0.0001f) //fade in complete
				{
					sceneStartPhase = 2;
					sceneStartTimer = FadeOutTime;
				}
			}
			else
			{
				if (sceneStartTimer < 0.0001f)
				{
					player.canMove = true;
					SceneTitle.gameObject.SetActive(false);
					CollapsingLeftSide.StartTheCollapse();
				}
			}
		}
	}

	void HandleSceneEndingAnimations()
	{
		if(!SceneTitle.gameObject.activeSelf && CollapsingLeftSide.IsMusicOver() && !player.IsDead)
		{
			//Time.timeScale = 0; //debug so I can see where the music ends.
			///*
			if (sceneEndTimer < 0) //music has ended -> start fading
			{
				sceneEndTimer = EndingFadeOutTime;
			}
			if (sceneEndTimer > 0) //fading is in progress
			{
				sceneEndTimer -= Time.deltaTime;
				float a = Mathf.Lerp(0, 1, 1.0f - (sceneEndTimer / EndingFadeOutTime));
				Color newColor = EndFade.color;
				newColor.a = a;
				EndFade.color = newColor;
				if (sceneEndTimer < 0.001f) //fading has ended
				{
					if(SceneManager.GetActiveScene().name == "SeaOfWords")
					{
						SceneManager.LoadScene("SeaOfTime");
					}
					else
					{
						SceneManager.LoadScene("End");
					}
				}
			}
			//*/
		}
	}
}
