using UnityEngine;
using System.Collections;

public abstract class MenuScript : MonoBehaviour
{
	//this is the function that will be called on the currently active menu
	public abstract void GUIMethod();
	
	//this function should return this if the menu doesn't change to a different menu, otherwise it should return a MenuScript that the MenuManager should be printing from
	public abstract MenuScript GetNextMenu();

	protected void BeginScreenCentering()
	{
		BeginCentering (new Rect(0, 0, Screen.width, Screen.height));
	}

	protected void EndScreenCentering()
	{
		EndCentering ();
	}

	protected void BeginCentering(Rect rect)
	{
		GUILayout.BeginArea(rect);
		GUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();
		GUILayout.BeginVertical();
		GUILayout.FlexibleSpace();
	}

	protected void EndCentering()
	{
		GUILayout.FlexibleSpace();
		GUILayout.EndVertical();
		GUILayout.FlexibleSpace();
		GUILayout.EndHorizontal();
		GUILayout.EndArea();
	}

	protected Texture2D MakeTex(int width, int height, Color col)	
	{
		Color[] pix = new Color[width*height];
		for(int i = 0; i < pix.Length; i++)
			pix[i] = col;

		Texture2D result = new Texture2D(width, height);
		result.SetPixels(pix);
		result.Apply();

		return result;
		
	}
}
