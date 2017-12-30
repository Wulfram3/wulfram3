using UnityEditor;
using UnityEngine;

public class AutoSaverEditor : EditorWindow
{
    [MenuItem("Window/AutoSaver")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(AutoSaverEditor), false, "AutoSaver");
    }

    void Update()
    {
        Repaint();
    }

    void OnGUI()
    {
        #region AutoSaver button
        if(GUILayout.Button((AutoSaver.IsEnabled) ? "AutoSaver: ON" : "AutoSaver: OFF"))
        {
            if (AutoSaver.IsEnabled) AutoSaver.DeactivateAutosaver();
            else AutoSaver.ActivateAutosaver();
        }
        #endregion
        #region AutoSaver Debug button
        if (GUILayout.Button((AutoSaver.IsDebugEnabled) ? "AutoSaver Debug: ON" : "AutoSaver Debug: OFF"))
        {
            if (AutoSaver.IsDebugEnabled) AutoSaver.IsDebugEnabled = false;
            else AutoSaver.IsDebugEnabled = true;
        }
        #endregion

        #region Assets saver and Save on play buttons
        EditorGUILayout.BeginHorizontal();

        EditorGUILayout.BeginVertical();
        if (GUILayout.Button((AutoSaver.AutosaveAssets) ? "Assets saver: ON" : "Assets saver: OFF"))
        {
            if (AutoSaver.AutosaveAssets) AutoSaver.AutosaveAssets = false;
            else AutoSaver.AutosaveAssets = true;
        }
        EditorGUILayout.EndVertical();

        EditorGUILayout.BeginVertical();
        if (GUILayout.Button((AutoSaver.SaveOnPlay) ? "Save on play: ON" : "Save on play: OFF"))
        {
            if (AutoSaver.SaveOnPlay) AutoSaver.SaveOnPlay = false;
            else AutoSaver.SaveOnPlay = true;
        }
        EditorGUILayout.EndVertical();

        EditorGUILayout.EndHorizontal();
        #endregion

        #region Interval slider
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.BeginVertical();
        AutoSaver.AutosaveInterval = EditorGUILayout.IntSlider("AutoSave Interval:", AutoSaver.AutosaveInterval, 1, 60);
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.EndVertical();
        #endregion

        #region Save and Delete buttons
        EditorGUILayout.BeginHorizontal(GUI.skin.box);

        EditorGUILayout.BeginVertical();
        if (GUILayout.Button("Save prefs"))
        {
            SaveEditorPrefs();
        }
        EditorGUILayout.EndVertical();

        EditorGUILayout.BeginVertical();
        if (GUILayout.Button("Delete prefs"))
        {
            DeleteEditorPrefs();
        }
        EditorGUILayout.EndVertical();

        EditorGUILayout.EndHorizontal();
        #endregion
    }

    #region Private methods
    private void SaveEditorPrefs()
    {
        EditorPrefs.SetBool("UNITY_AUTOSAVER_prefsExist", true);
        EditorPrefs.SetBool("UNITY_AUTOSAVER_isEnabled", AutoSaver.IsEnabled);
        EditorPrefs.SetBool("UNITY_AUTOSAVER_isDebugEnabled", AutoSaver.IsDebugEnabled);
        EditorPrefs.SetBool("UNITY_AUTOSAVER_autosaveAssets", AutoSaver.AutosaveAssets);
        EditorPrefs.SetInt("UNITY_AUTOSAVER_autosaveInterval", AutoSaver.AutosaveInterval);

        Debug.Log("AutoSaver: settings saved!");
    }

    private void DeleteEditorPrefs()
    {
        EditorPrefs.DeleteKey("UNITY_AUTOSAVER_prefsExist");
        EditorPrefs.DeleteKey("UNITY_AUTOSAVER_isEnabled");
        EditorPrefs.DeleteKey("UNITY_AUTOSAVER_isDebugEnabled");
        EditorPrefs.DeleteKey("UNITY_AUTOSAVER_autosaveAssets");
        EditorPrefs.DeleteKey("UNITY_AUTOSAVER_autosaveInterval");

        Debug.Log("AutoSaver: settings deleted!");
    }
    #endregion
}