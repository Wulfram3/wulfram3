using System;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_5_3_OR_NEWER
using UnityEngine.SceneManagement;
#endif

public class CommandLineChecker : MonoBehaviour
{
    #region Public fields
    public string LaunchArgument = "default";
    public int LoadLevelId = 1;
    #endregion
    #region Private fields
    private List<string> CommandArgs;
    private GUIState _guiState = GUIState.NONE;
    #endregion
    #region Private methods
    private void GetCommandLineArgs()
    {
        string[] args = Environment.GetCommandLineArgs();
        CommandArgs = new List<string>();
        foreach (string entry in args)
        {
            CommandArgs.Add(entry);
        }
    }

    private bool CheckLaunchArg()
    {
        foreach(string entry in CommandArgs)
        {
            if(entry.Contains("-LaunchArg=" + LaunchArgument))
            {
                return true;
            }
        }
        return false;
    }

    private static Vector2 windowSize = new Vector2(250, 150);
    private Rect windowRect0 = new Rect((Screen.width / 2) - (windowSize.x / 2), (Screen.height / 2) - (windowSize.y / 2), windowSize.x, windowSize.y);
    private void GUICommandLineCheckingFailed()
    {
        windowRect0 = GUILayout.Window(0, windowRect0, WindowFunction, "Launch argument fails!");
    }

    private void WindowFunction(int id)
    {
        GUILayout.Label("Launch your game with Launcher! You can't open your game without it! Application will now quit!");
        if(GUILayout.Button("OK"))
        {
            Application.Quit();
        }
        
        GUI.DragWindow(new Rect((Screen.width / 2) - (windowSize.x / 2) - 20, (Screen.height / 2) - (windowSize.y / 2) - 20, windowSize.x, windowSize.y + 20));
    }
    #endregion
    #region MonoBehaviour's methods
    void Start()
    {
        GetCommandLineArgs();
        if(CheckLaunchArg())
        {
#if !UNITY_5_3_OR_NEWER
            Application.LoadLevel(LoadLevelId);
#else
            SceneManager.LoadScene(LoadLevelId);
#endif

        }
        else
        {
            _guiState = GUIState.COMMAND_LINE_CHECKING_FAILED;
        }
    }

    void OnGUI()
    {
        switch(_guiState)
        {
            case GUIState.NONE:
                break;
            case GUIState.COMMAND_LINE_CHECKING_FAILED:
                GUICommandLineCheckingFailed();
                break;
        }
    }
#endregion
}