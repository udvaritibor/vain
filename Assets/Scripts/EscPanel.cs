using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EscPanel : MonoBehaviour
{
	public List<Image> MenuElements;
	public List<Sprite> MenuElementNormalSprites;
	public List<Sprite> MenuElementSelectedSprites;

	private int selectedIndex = 0;

	public void HitButton()
	{
		//this is pretty badly designed, every other code is flexible, and this is hardcoded. Delagates would do the job
		if (selectedIndex == 0)
		{
			SceneManager.LoadScene(SceneManager.GetActiveScene().name);
		}

		else if (selectedIndex == 1)
		{
			SceneManager.LoadScene("Menu");
		}
	}

	public void MoveSelection(int dir)
	{
		int newIndex = selectedIndex + dir;
		if (newIndex == MenuElements.Count)
		{
			newIndex = 0;
		}
		else if (newIndex == -1)
		{
			newIndex = MenuElements.Count - 1;
		}
		MenuElements[selectedIndex].sprite = MenuElementNormalSprites[selectedIndex];
		MenuElements[newIndex].sprite = MenuElementSelectedSprites[newIndex];
		selectedIndex = newIndex;
	}

	void Start ()
	{
		MenuElements[selectedIndex].sprite = MenuElementSelectedSprites[selectedIndex];
	}
}
