using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.IO;
using MHLab.PATCH.Settings;

public class Localizatron : Singleton<Localizatron> {
	public Localizatron() {}

	// Private features
	private string	_languagePath;
	private string 	_currentLanguage;
	private Dictionary<string,string> languageTable;

	// Methods

	public Dictionary<string, string> GetLanguageTable() {
		return this.languageTable;
	}

	public bool SetLanguage(string language) {
		if (Regex.IsMatch(language, @"^[a-z]{2}_[A-Z]{2}$")) {
			this._currentLanguage = language;
			this._languagePath = /*Settings.LANGUAGE_PATH +*/ this._currentLanguage /*+ Settings.LANGUAGE_EXTENSION*/;
			this.languageTable = this.loadLanguageTable(this._languagePath);
			Debug.Log ("[Localizatron] Locale loaded at: " + this._languagePath);
			return true;
		} 
		else {
			return false;
		}
	}

	public string GetCurrentLanguage() {
		return this._currentLanguage;
	}

	public string Translate(string key) {
		if (this.languageTable != null) {
			if (this.languageTable.ContainsKey (key)) {
				return this.languageTable[key];
			} else {
				return key;
			}
		} else 
		{
			return key;
		}
	}

	public Dictionary<string, string> loadLanguageTable(string fileName) {
		try {
			TextAsset file = Resources.Load<TextAsset>("Localizatron/Locale/" + fileName);

			string languageFileContent = file.text;


			Dictionary<string, string> languageDict = new Dictionary<string, string>();
			Regex regexKey = new Regex(@"<key>(.*?)</key>");
			Regex regexValue = new Regex(@"<value>(.*?)</value>");
			MatchCollection keysMatchCollection = regexKey.Matches(languageFileContent);
			MatchCollection valuesMatchCollection = regexValue.Matches(languageFileContent);
			IEnumerator keysEnum = keysMatchCollection.GetEnumerator();
			IEnumerator valuesEnum = valuesMatchCollection.GetEnumerator();

			while(keysEnum.MoveNext()) {
				valuesEnum.MoveNext();
				languageDict.Add(keysEnum.Current.ToString().Replace("<key>", "").Replace("</key>", ""), 
				                 valuesEnum.Current.ToString().Replace("<value>", "").Replace("</value>", ""));
			}

			return languageDict;
		}
		catch(FileNotFoundException e) {
			Debug.Log ( e.Message );
			return null;
		}
	}

	private void Init() {
		// Set language to default
		this.SetLanguage (Settings.LANGUAGE_DEFAULT);
	}

	void Awake () {
		this.Init();
	}
}
