using UnityEngine;
using System.Collections;

public class MenuManager : MonoBehaviour
{
	MenuScript current;
	GameObject guiBackgroundCam;
	bool drawMenu = true;

	// Use this for initialization
	void Start () 
	{
		current = GetComponent<MainMenuScript>();
		guiBackgroundCam = transform.Find("GUICamera").gameObject;
		guiBackgroundCam.SetActive(true);
	}
	
	void OnGUI()
	{
		current.GUIMethod();
	}
	
	// Update is called once per frame
	void Update () 
	{


		if(drawMenu)
			current = current.GetNextMenu();
	}

	public void EnableMenuBackgroundCamera(bool enable)
	{
		guiBackgroundCam.SetActive(enable);
	}

	public void SetCurrent(MenuScript menu)
	{
		current = menu;
	}

	public void Enable(MenuScript nextMenu)
	{
		if(nextMenu)
			current = nextMenu;
		drawMenu = true;
	}

	public void Enable()
	{
		Enable(null);
	}

	public void Disable()
	{
		EnableMenuBackgroundCamera(false);
		current = null;
	}
}
