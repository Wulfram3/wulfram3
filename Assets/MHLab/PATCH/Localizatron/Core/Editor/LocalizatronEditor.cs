using UnityEngine;
using UnityEditor;
using System.Collections;
using System.IO;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using MHLab.PATCH.Settings;

public enum CONTEXT {
	FILES_MANAGER,
	EDIT_FILE
}

public enum EDITOR_EVENT {
	NONE,
	ADD_ENTRY
}

public class LocalizatronEditor : EditorWindow {
	protected static bool IS_DEBUG = true;

	protected static List<string> langFiles;
	protected string newFileName;
	protected string selectedFile;
	protected Dictionary<string, string> localeDict;
	private CONTEXT _context = CONTEXT.FILES_MANAGER;
	private static string logStream = "";

	Queue<Action> guiEvents = new Queue<Action>();

	[MenuItem ("Window/Localizatron")] 
	static void OpenLocalizationWindow() {
		LocalizatronEditor.GetLocalizationFiles();
		EditorWindow.GetWindow(typeof(LocalizatronEditor), false, "Localizatron");
	}

	void OnGUI() {
		switch(this._context) {
			case CONTEXT.FILES_MANAGER:
				this.FileManagerWindow();
				break;
			case CONTEXT.EDIT_FILE:
				this.EditFileWindow();
				break;
		}
		if(LocalizatronEditor.IS_DEBUG) {
			EditorGUILayout.TextArea(LocalizatronEditor.logStream, GUILayout.MaxHeight(50), GUILayout.MaxWidth(450), GUILayout.ExpandHeight(false));
		}
	}

	
	void Update() {
		while (guiEvents.Count > 0)
		{
			this.guiEvents.Dequeue().Invoke();
		}
		Repaint ();
	}

	public static void Log(string message) {
		LocalizatronEditor.logStream = "[Localizatron " + DateTime.Now.ToString() + "]: " + message + "\n" + LocalizatronEditor.logStream;
	}

	protected static void GetLocalizationFiles() {
		try {
			LocalizatronEditor.langFiles = new List<string>();
			if(!Directory.Exists(Settings.LANGUAGE_PATH + Path.DirectorySeparatorChar)) {
				Directory.CreateDirectory(Settings.LANGUAGE_PATH + Path.DirectorySeparatorChar);
			}
			string[] paths = Directory.GetFiles(Settings.LANGUAGE_PATH);
			foreach (string path in paths) {
				if(path.Contains(Settings.LANGUAGE_EXTENSION) && !path.Contains(".meta")) {
					LocalizatronEditor.langFiles.Add(path);
				}
			}
			LocalizatronEditor.Log("Localization files loaded successfully!");
		}
		catch(Exception e) {
			if(IS_DEBUG)
				LocalizatronEditor.Log("Failed to inizialize files: " + e.Message);
		}
	}

	protected static void SaveLocalizationFile(string fileName, string content) {
		System.IO.File.WriteAllText(Application.dataPath + Settings.SAVING_LANGUAGE_PATH + fileName + Settings.LANGUAGE_EXTENSION, content);
		LocalizatronEditor.Log("Localization files (" + fileName + ") saved!");
	}

	protected static void DeleteLocalizationFile(string filePath) {
		System.IO.File.Delete(Application.dataPath + Settings.SAVING_LANGUAGE_PATH + Path.GetFileName(filePath));
		LocalizatronEditor.Log("Localization files (" + Path.GetFileName(filePath) + ") deleted!");
	}

	protected void OnDeleteFileInFileManagerWindow(string entry) {
		LocalizatronEditor.langFiles.Remove(entry);
		LocalizatronEditor.DeleteLocalizationFile(entry);
	}

	protected Vector2 scrollPositionFileManager = Vector2.zero;

	protected void FileManagerWindow() {
		try {
			EditorGUILayout.LabelField("Localization files", EditorStyles.boldLabel);
			
			EditorGUILayout.Space();
			
			EditorGUILayout.BeginScrollView(this.scrollPositionFileManager, GUILayout.MaxWidth(450), GUILayout.MaxHeight(450), GUILayout.ExpandHeight(false));
			foreach(string entry in LocalizatronEditor.langFiles) {
				EditorGUILayout.BeginHorizontal();
				
				EditorGUILayout.BeginVertical();
				EditorGUILayout.TextField(Path.GetFileName(entry).Replace(Settings.LANGUAGE_EXTENSION, ""));
				EditorGUILayout.EndVertical();
				
				EditorGUILayout.BeginVertical();
				if(GUILayout.Button("Delete", EditorStyles.miniButtonLeft, GUILayout.Width(45))){
					OnDeleteFileInFileManagerWindow(entry);
				}
				EditorGUILayout.EndVertical();

				EditorGUILayout.BeginVertical();
				if(GUILayout.Button("Edit", EditorStyles.miniButtonRight, GUILayout.Width(45))){
					this.selectedFile = Path.GetFileName(entry).Replace(Settings.LANGUAGE_EXTENSION, "");
					this.localeDict = this.inEditorLoadLanguageTable(Application.dataPath + Settings.SAVING_LANGUAGE_PATH + this.selectedFile + Settings.LANGUAGE_EXTENSION);
					if(this.localeDict.Count == 0) {
						this.localeDict.Add("Entry-" + DateTime.Now.Ticks, "");
					}
					this._context = CONTEXT.EDIT_FILE;
					LocalizatronEditor.Log("Selected localization file: " + this.selectedFile);
				}
				EditorGUILayout.EndVertical();
				
				EditorGUILayout.EndHorizontal();
			}
			
			EditorGUILayout.Space();
			
			EditorGUILayout.BeginHorizontal();
			
			EditorGUILayout.BeginVertical();
			this.newFileName = EditorGUILayout.TextField(this.newFileName);
			EditorGUILayout.EndVertical(); 
			
			EditorGUILayout.BeginVertical();
			if(GUILayout.Button("Add new localization file")){
				if(this.newFileName != "" && this.newFileName != null) {
					LocalizatronEditor.langFiles.Add(Settings.LANGUAGE_PATH + this.newFileName);
					LocalizatronEditor.SaveLocalizationFile(this.newFileName, "<key></key><value></value>");
					this.newFileName = "";
				}
				else {
					LocalizatronEditor.Log("You cannot add an unnamed file!");
				}
			}
			EditorGUILayout.EndVertical();
			EditorGUILayout.EndHorizontal();
			
			EditorGUILayout.EndScrollView();
		}
		catch {
			if(LocalizatronEditor.IS_DEBUG) 
				LocalizatronEditor.Log("Updated missing ref of deleted file");
			EditorGUILayout.EndScrollView();
		}
	}

	protected string fileContent;
	protected Dictionary<string, string> finalContentDict = new Dictionary<string, string>();
	protected Vector2 scrollPosition = Vector2.zero;
	protected string keyFilter = "";
	protected string valueFilter = "";

	protected void EditFileWindow() {
		try {
			EditorGUILayout.LabelField("Editing locale: " + this.selectedFile, EditorStyles.boldLabel);
			
			EditorGUILayout.Space();

			this.scrollPosition = EditorGUILayout.BeginScrollView(this.scrollPosition, GUILayout.MaxWidth(450), GUILayout.MaxHeight(450), GUILayout.ExpandHeight(false));

			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.BeginVertical();
			EditorGUILayout.LabelField("Key Filter", EditorStyles.boldLabel);
			EditorGUILayout.EndVertical();
			EditorGUILayout.BeginVertical();
			this.keyFilter = EditorGUILayout.TextField(this.keyFilter);
			EditorGUILayout.EndVertical();
			EditorGUILayout.EndHorizontal();

			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.BeginVertical();
			EditorGUILayout.LabelField("Value Filter", EditorStyles.boldLabel);
			EditorGUILayout.EndVertical();
			EditorGUILayout.BeginVertical();
			this.valueFilter = EditorGUILayout.TextField(this.valueFilter);
			EditorGUILayout.EndVertical();
			EditorGUILayout.EndHorizontal();

			EditorGUILayout.Space();

			EditorGUILayout.BeginHorizontal();
				EditorGUILayout.BeginVertical();
					EditorGUILayout.LabelField("Key", EditorStyles.boldLabel);
				EditorGUILayout.EndVertical();
				EditorGUILayout.BeginVertical();
					EditorGUILayout.LabelField("Value", EditorStyles.boldLabel);
				EditorGUILayout.EndVertical();
			EditorGUILayout.EndHorizontal();


			if(this.localeDict == null) {
				this.localeDict = this.inEditorLoadLanguageTable(Application.dataPath + Settings.SAVING_LANGUAGE_PATH + this.selectedFile + Settings.LANGUAGE_EXTENSION);
			}

			bool isFiltered = false;

			foreach(KeyValuePair<string, string> pair in this.localeDict) {
				string tmpKey = "", tmpValue = "";
				if(this.keyFilter != "" || this.valueFilter != "") {
					isFiltered = true;
					if(this.keyFilter != "" && this.valueFilter != "") {
						if(pair.Key.ToLower().Contains(this.keyFilter.ToLower()) && pair.Value.ToLower().Contains(this.valueFilter.ToLower())) {
							EditorGUILayout.BeginHorizontal();
							
							EditorGUILayout.BeginVertical();
							tmpKey = EditorGUILayout.TextField(pair.Key);
							EditorGUILayout.EndVertical();
							
							EditorGUILayout.BeginVertical();
							tmpValue = EditorGUILayout.TextField(pair.Value);
							EditorGUILayout.EndVertical();
							
							EditorGUILayout.EndHorizontal(); 
						} 
					}
					else if(this.keyFilter != "") {
						if(pair.Key.ToLower().Contains(this.keyFilter.ToLower())) {
							EditorGUILayout.BeginHorizontal();
							
							EditorGUILayout.BeginVertical();
							tmpKey = EditorGUILayout.TextField(pair.Key);
							EditorGUILayout.EndVertical();
							
							EditorGUILayout.BeginVertical();
							tmpValue = EditorGUILayout.TextField(pair.Value);
							EditorGUILayout.EndVertical();
							
							EditorGUILayout.EndHorizontal(); 
						} 
					}
					else if(this.valueFilter != "") {
						if(pair.Value.ToLower().Contains(this.valueFilter.ToLower())) {
							EditorGUILayout.BeginHorizontal();
							
							EditorGUILayout.BeginVertical();
							tmpKey = EditorGUILayout.TextField(pair.Key);
							EditorGUILayout.EndVertical();
							
							EditorGUILayout.BeginVertical();
							tmpValue = EditorGUILayout.TextField(pair.Value);
							EditorGUILayout.EndVertical();
							
							EditorGUILayout.EndHorizontal(); 
						}
					}
				}
				else {
					EditorGUILayout.BeginHorizontal();
						
						EditorGUILayout.BeginVertical();
							tmpKey = EditorGUILayout.TextField(pair.Key);
						EditorGUILayout.EndVertical();
					
						EditorGUILayout.BeginVertical();
							tmpValue = EditorGUILayout.TextField(pair.Value);
						EditorGUILayout.EndVertical();
					
					EditorGUILayout.EndHorizontal();
				}
				if(this.finalContentDict.ContainsKey(pair.Key)) {
					this.finalContentDict.Remove(pair.Key);
				}
				if(tmpKey != "")
					this.finalContentDict[tmpKey] = tmpValue;
			}

			if(!isFiltered)
				this.localeDict = new Dictionary<string, string>(this.finalContentDict);
			
			EditorGUILayout.Space();

			EditorGUILayout.BeginHorizontal();
			
			EditorGUILayout.BeginVertical();
			if(GUILayout.Button("Add entry")){
				guiEvents.Enqueue(()=>
				{				
					try {
						this.localeDict.Add("Entry-" + DateTime.Now.Ticks , "");
					}
					catch(Exception e) {
						if(LocalizatronEditor.IS_DEBUG) 
							LocalizatronEditor.Log(e.Message);
					}
				});
			}
			EditorGUILayout.EndVertical();
			
			EditorGUILayout.EndHorizontal();
			
			EditorGUILayout.BeginHorizontal();

			EditorGUILayout.BeginVertical();
			if(GUILayout.Button("< Back")){
				this.selectedFile = "";
				this.localeDict = null;
				this.finalContentDict.Clear();
				this._context = CONTEXT.FILES_MANAGER;
			}
			EditorGUILayout.EndVertical();

			EditorGUILayout.BeginVertical();
			if(GUILayout.Button("Save locale")){
				string finalContent = "";

				foreach(KeyValuePair<string, string> kv in this.finalContentDict) {
					finalContent += "<key>" + kv.Key + "</key><value>" + kv.Value + "</value>\n";
				}
				LocalizatronEditor.SaveLocalizationFile(this.selectedFile, finalContent);
			}
			EditorGUILayout.EndVertical();

			EditorGUILayout.EndHorizontal();

			EditorGUILayout.EndScrollView();
		}
		catch(Exception e) {
			if(LocalizatronEditor.IS_DEBUG) 
				LocalizatronEditor.Log (e.Message);
			EditorGUILayout.EndScrollView();
		}
	}
	
	public Dictionary<string, string> inEditorLoadLanguageTable(string fileName) {
		try {
			StreamReader reader = new StreamReader (fileName);
			string languageFileContent = reader.ReadToEnd();
			reader.Close();
			
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
}
