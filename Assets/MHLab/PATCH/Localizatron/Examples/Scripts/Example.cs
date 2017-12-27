using UnityEngine;
using System.Collections;

public class Example : MonoBehaviour {

	private float _screenMiddleX;
	private float _screenMiddleY;

	void Start() {
		this._screenMiddleX = Screen.width / 2;
		this._screenMiddleY = Screen.height / 2;
	}

	void OnGUI() {
		if(GUI.Button(new Rect(10f, 10f, 50f, 22f), "it-IT")) {
			Localizatron.Instance.SetLanguage("it-IT");
		}
		if(GUI.Button(new Rect(70f, 10f, 50f, 22f), "en-EN")) {
			Localizatron.Instance.SetLanguage("en-EN");
		}
		GUI.Label(new Rect((this._screenMiddleX - 50f), (this._screenMiddleY - 11f), 100f, 22f), Localizatron.Instance.Translate("Hello World"));
	}
}
