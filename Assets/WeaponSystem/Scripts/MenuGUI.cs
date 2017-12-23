using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

namespace HWRWeaponSystem
{
	public class MenuGUI : MonoBehaviour
	{

		public Texture2D logo;
		public GUISkin skin;

		void Start ()
		{
	
		}

		void Update ()
		{
			MouseLock.MouseLocked = false;
		}

		void OnGUI ()
		{
			if (skin)
				GUI.skin = skin;
		
			GUI.skin.button.fontSize = 18;
			GUI.DrawTexture (new Rect (Screen.width / 2 - logo.width / 2, 10, logo.width, logo.height), logo);
			if (GUI.Button (new Rect (Screen.width / 2 - 400, 300, 200, 30), "Demo 1"))
				SceneManager.LoadScene ("Demo1");
			if (GUI.Button (new Rect (Screen.width / 2 - 400, 340, 200, 30), "Demo 2"))
				SceneManager.LoadScene ("Demo2");
			if (GUI.Button (new Rect (Screen.width / 2 - 400, 380, 200, 30), "3D First Person"))
				SceneManager.LoadScene ("Demo3");
			if (GUI.Button (new Rect (Screen.width / 2 - 400, 420, 200, 30), "2D Side Scrolling"))
				SceneManager.LoadScene ("Demo2D");
			if (GUI.Button (new Rect (Screen.width / 2 - 400, 460, 200, 30), "Tower Defend"))
				SceneManager.LoadScene ("TD");
		
			if (GUI.Button (new Rect (Screen.width - 320, Screen.height - 50, 300, 30), "Get this project"))
				Application.OpenURL ("https://www.assetstore.unity3d.com/#/content/7676");

		
		
		}
	}
}