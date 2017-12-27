using UnityEngine;
using UnityEditor;
using System;
using System.Threading;
using System.IO;
using System.Collections.Generic;
using MHLab.PATCH;
using MHLab.PATCH.Compression;
using MHLab.PATCH.Settings;
using MHLab.PATCH.Uploader;

public class PatchBuilderWindow : EditorWindow
{
    #region GUI Context
    protected enum GUIContext
    {
        PATCH_BUILDING
    }
    #endregion

    #region Members
    PatchManager m_patchManager;
    GUIContext m_guiContext;

    private bool _isInitialized = false;
    //static PatchBuilderWindow _currentWindowInstance;
    #endregion

    #region Members for Patch Builder context
    protected enum PatchBuilderStatus
    {
        IDLE,
        IS_BUILDING
    }

    Vector2 _newVersionWindowScrollPosition = Vector2.zero;
    private int _majorVersionNumber = 0;
    private int _minorVersionNumber = 0;
    private int _maintenanceNumber = 0;
    private int _buildNumber = 0;

    private PatchBuilderStatus _patchBuilderStatus = PatchBuilderStatus.IDLE;

    private string _mainLog = "";
    private float _mainProgress = 0.0f;
    private float _mainProgressBarMax = 0.0f;
    private string _detailLog = "";
    private float _detailProgress = 0.0f;
    private float _detailProgressBarMax = 0.0f;
    private string _endLog = "";
    private GUIStyle _endLogStyle = new GUIStyle();
    private string _lastVersion;
    private GUIStyle _lastVersionStyle = new GUIStyle();

    private string[] _currentVersions;
    private string[] _currentPatches;

    // Patches builder
    Vector2 _patchesBuilderWindowScrollPosition = Vector2.zero;
    private int _versionFromDropdownIndex;
    private int _versionToDropdownIndex;
    private int _compressionTypeDropdownIndex = 0;
    public string[] _compressionTypes = { "ZIP", "TAR", "TARGZ" };
    private string _mainLogPatchBuild = "";
    private float _mainProgressPatchBuild = 0.0f;
    private float _mainProgressBarMaxPatchBuild = 0.0f;
    private string _detailLogPatchBuild = "";
    private float _detailProgressPatchBuild = 0.0f;
    private float _detailProgressBarMaxPatchBuild = 0.0f;
    private string _endLogPatchBuild = "";

    // Deploy
    Vector2 _deployWindowScrollPosition = Vector2.zero;
    private int _deployCompressionTypeDropdownIndex = 0;
    private int _versionToDeployDropdownIndex;
    private int _launcherSceneToDeployDropdownIndex;
    private int _platformToDeployDropdownIndex;
    private string[] _platformsAvailable;
    private List<string> _scenesInBuildSettings = new List<string>();
    private string _launcherCustomName = "Launcher";

    // Upload to FTP/SFTP
    Vector2 _uploadWindowScrollPosition = Vector2.zero;
    private int _versionToUploadDropdownIndex;
    private int _patchToUploadDropdownIndex;
    private bool _isSFTP = false;
    private string _uploadHost;
    private int _uploadHostPort = 21;
    private string _uploadUsername;
    private string _uploadPassword;
    private string _buildsRemotePath;
	private string _patchesRemotePath;
	private string _patcherRemotePath;

    private Thread _patchBuilderThread;
    #endregion
    
    [MenuItem("Window/P.A.T.C.H./Patch Builder")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(PatchBuilderWindow), false, "P.A.T.C.H.");
    }

    [MenuItem("Window/P.A.T.C.H./Open workspace folder")]
    public static void ShowWorkspaceFolder()
    {
        System.Diagnostics.Process.Start(Directory.GetParent(Application.dataPath).FullName + Path.DirectorySeparatorChar + "PATCH");
    }

    void Init()
    {
        if(!_isInitialized)
        {
            SettingsManager.APP_PATH = Directory.GetParent(Application.dataPath).FullName + Path.DirectorySeparatorChar + "PATCH";
            SettingsManager.RegeneratePaths();

            m_patchManager = new PatchManager();
            m_guiContext = GUIContext.PATCH_BUILDING;


            _endLogStyle.fontSize = 8;
            _endLogStyle.normal.textColor = Color.green;

            //_lastVersionStyle.fontSize = 10;
            _lastVersionStyle.normal.textColor = Color.yellow;
            _lastVersionStyle.fontStyle = FontStyle.Italic;
            _lastVersionStyle.alignment = TextAnchor.MiddleLeft;
            _lastVersion = m_patchManager.GetLastVersion();

            LoadEditorPrefs();

            _currentVersions = m_patchManager.GetCurrentVersions();
            _currentPatches = m_patchManager.GetCurrentPatches();

            _platformsAvailable = Enum.GetNames(typeof(BuildTarget));

            UpdateScenesToBuild();

            UpdateLastVersionGUI(_lastVersion);
            
            _isInitialized = true;
        }
    }

    #region Callbacks for New version
    void OnSetMainProgressBar(int min, int max)
    {
        
        _mainProgressBarMax = max;
        _mainProgress = 0;
    }

    void OnSetDetailProgressBar(int min, int max)
    {
        _detailProgressBarMax = max;
        _detailProgress = 0;
    }

    void OnIncreaseMainProgressBar()
    {
        _mainProgress++;
    }

    void OnIncreaseDetailProgressBar()
    {
        _detailProgress++;
    }

    void OnLog(string main, string detail)
    {
        _mainLog = main;
        _detailLog = detail;
    }

    void OnError(string main, string detail, Exception e)
    {
        _mainLog = main;
        _detailLog = detail;
    }

    void OnFatalError(string main, string detail, Exception e)
    {
        _mainLog = main;
        _detailLog = detail;
    }

    void OnTaskStarted(string message)
    {
        _patchBuilderStatus = PatchBuilderStatus.IS_BUILDING;
        _endLog = message;
    }

    void OnTaskCompleted(string message)
    {
        _lastVersion = m_patchManager.GetLastVersion();
        _currentVersions = m_patchManager.GetVersions().ToArray();
        UpdateLastVersionGUI(_lastVersion);
        _endLog = message;
        _patchBuilderStatus = PatchBuilderStatus.IDLE;
    }
    #endregion

    #region Callbacks for Patch build
    void OnSetMainProgressBarPatchBuild(int min, int max)
    {
        _mainProgressBarMaxPatchBuild = max;
        _mainProgressPatchBuild = 0;
    }

    void OnSetDetailProgressBarPatchBuild(int min, int max)
    {
        _detailProgressBarMaxPatchBuild = max;
        _detailProgressPatchBuild = 0;
    }

    void OnIncreaseMainProgressBarPatchBuild()
    {
        _mainProgressPatchBuild++;
    }

    void OnIncreaseDetailProgressBarPatchBuild()
    {
        _detailProgressPatchBuild++;
    }

    void OnLogPatchBuild(string main, string detail)
    {
        _mainLogPatchBuild = main;
        _detailLogPatchBuild = detail;
    }

    void OnErrorPatchBuild(string main, string detail, Exception e)
    {
        _mainLogPatchBuild = main;
        _detailLogPatchBuild = detail;
    }

    void OnFatalErrorPatchBuild(string main, string detail, Exception e)
    {
        _mainLogPatchBuild = main;
        _detailLogPatchBuild = detail;
    }

    void OnTaskStartedPatchBuild(string message)
    {
        _patchBuilderStatus = PatchBuilderStatus.IS_BUILDING;
        _endLogPatchBuild = message;
    }

    void OnTaskCompletedPatchBuild(string message)
    {
        _currentPatches = m_patchManager.GetCurrentPatches();
        UpdateLastVersionGUI(_lastVersion);
        _endLogPatchBuild = message;
        _patchBuilderStatus = PatchBuilderStatus.IDLE;
    }
    #endregion

    void OnGUI()
    {
        if(_isInitialized)
        {
            switch(m_guiContext)
            {
                case GUIContext.PATCH_BUILDING:
                    OnPatchBuildingContext();
                    break;
            }
        }
    }

    void Update()
    {
        Init();
        Repaint();
    }

    private void LoadEditorPrefs()
    {
        if(EditorPrefs.HasKey("UNITY_PATCH_SFTP_prefsExist"))
        {
            if(EditorPrefs.HasKey("UNITY_PATCH_SFTP_isSFTP"))
            {
                _isSFTP = EditorPrefs.GetBool("UNITY_PATCH_SFTP_isSFTP");
            }
            if (EditorPrefs.HasKey("UNITY_PATCH_SFTP_host"))
            {
                _uploadHost = EditorPrefs.GetString("UNITY_PATCH_SFTP_host");
            }
            if (EditorPrefs.HasKey("UNITY_PATCH_SFTP_host"))
            {
                _uploadHostPort = EditorPrefs.GetInt("UNITY_PATCH_SFTP_port");
            }
            if (EditorPrefs.HasKey("UNITY_PATCH_SFTP_username"))
            {
                _uploadUsername = EditorPrefs.GetString("UNITY_PATCH_SFTP_username");
            }
            if (EditorPrefs.HasKey("UNITY_PATCH_SFTP_password"))
            {
                _uploadPassword = EditorPrefs.GetString("UNITY_PATCH_SFTP_password");
            }
            if (EditorPrefs.HasKey("UNITY_PATCH_SFTP_buildsRemotePath"))
            {
                _buildsRemotePath = EditorPrefs.GetString("UNITY_PATCH_SFTP_buildsRemotePath");
            }
            if (EditorPrefs.HasKey("UNITY_PATCH_SFTP_patchesRemotePath"))
            {
                _patchesRemotePath = EditorPrefs.GetString("UNITY_PATCH_SFTP_patchesRemotePath");
            }
			if (EditorPrefs.HasKey("UNITY_PATCH_SFTP_patcherRemotePath"))
			{
				_patcherRemotePath = EditorPrefs.GetString("UNITY_PATCH_SFTP_patcherRemotePath");
			}
        }
    }

    private void SaveEditorPrefs()
    {
        EditorPrefs.SetBool("UNITY_PATCH_SFTP_prefsExist", true);
        EditorPrefs.SetBool("UNITY_PATCH_SFTP_isSFTP", _isSFTP);
        EditorPrefs.SetString("UNITY_PATCH_SFTP_host", _uploadHost);
        EditorPrefs.SetInt("UNITY_PATCH_SFTP_port", _uploadHostPort);
        EditorPrefs.SetString("UNITY_PATCH_SFTP_username", _uploadUsername);
        EditorPrefs.SetString("UNITY_PATCH_SFTP_password", _uploadPassword);
        EditorPrefs.SetString("UNITY_PATCH_SFTP_buildsRemotePath", _buildsRemotePath);
		EditorPrefs.SetString("UNITY_PATCH_SFTP_patchesRemotePath", _patchesRemotePath);
		EditorPrefs.SetString("UNITY_PATCH_SFTP_patcherRemotePath", _patcherRemotePath);
    }

    private void DeleteEditorPrefs()
    {
        EditorPrefs.DeleteKey("UNITY_PATCH_SFTP_prefsExist");
        EditorPrefs.DeleteKey("UNITY_PATCH_SFTP_isSFTP");
        EditorPrefs.DeleteKey("UNITY_PATCH_SFTP_host");
        EditorPrefs.DeleteKey("UNITY_PATCH_SFTP_port");
        EditorPrefs.DeleteKey("UNITY_PATCH_SFTP_username");
        EditorPrefs.DeleteKey("UNITY_PATCH_SFTP_password");
        EditorPrefs.DeleteKey("UNITY_PATCH_SFTP_buildsRemotePath");
		EditorPrefs.DeleteKey("UNITY_PATCH_SFTP_patchesRemotePath");
		EditorPrefs.DeleteKey("UNITY_PATCH_SFTP_patcherRemotePath");

        _isSFTP = false;
        _uploadHost = "";
        _uploadHostPort = 21;
        _uploadUsername = "";
        _uploadPassword = "";
        _buildsRemotePath = "";
		_patchesRemotePath = "";
		_patcherRemotePath = "";
	}

    private void UpdateLastVersionGUI(string version)
    {
        if (version != "none")
        {
            MHLab.PATCH.Version v = new MHLab.PATCH.Version(version);
            
            _majorVersionNumber = v.Major;
            _minorVersionNumber = v.Minor;
            _maintenanceNumber = v.Revision;
            _buildNumber = v.Build + 1;

            _versionFromDropdownIndex = _currentVersions.Length - 2;
            _versionToDropdownIndex = _currentVersions.Length - 1;
        }
    }

    private void UpdateScenesToBuild()
    {
        foreach (EditorBuildSettingsScene scene in EditorBuildSettings.scenes)
        {
            _scenesInBuildSettings.Add(scene.path);
        }
    }

    protected void OnPatchBuildingContext()
    {
        try
        {
            #region Create new version
            EditorGUILayout.LabelField("New version builder", EditorStyles.boldLabel);

            EditorGUILayout.Space();

            _newVersionWindowScrollPosition = EditorGUILayout.BeginScrollView(_newVersionWindowScrollPosition, GUILayout.MaxHeight(450), GUILayout.ExpandHeight(false));

            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.BeginVertical();
            EditorGUILayout.LabelField("Last version:", GUILayout.MaxWidth(100));
            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginVertical();
            EditorGUILayout.LabelField(_lastVersion, _lastVersionStyle);
            EditorGUILayout.EndVertical();

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal(GUI.skin.box);

            EditorGUILayout.BeginVertical();
            EditorGUILayout.LabelField("New version", GUILayout.MaxWidth(100));
            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginVertical();
            _majorVersionNumber = EditorGUILayout.IntField(_majorVersionNumber, GUILayout.MaxWidth(25));
            EditorGUILayout.EndVertical();
            EditorGUILayout.BeginVertical();
            _minorVersionNumber = EditorGUILayout.IntField(_minorVersionNumber, GUILayout.MaxWidth(25));
            EditorGUILayout.EndVertical();
            EditorGUILayout.BeginVertical();
            _maintenanceNumber = EditorGUILayout.IntField(_maintenanceNumber, GUILayout.MaxWidth(25));
            EditorGUILayout.EndVertical();
            EditorGUILayout.BeginVertical();
            _buildNumber = EditorGUILayout.IntField(_buildNumber, GUILayout.MaxWidth(25));
            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginVertical();
            GUI.enabled = _patchBuilderStatus == PatchBuilderStatus.IDLE;
            if (GUILayout.Button("New version"))
            {
                string newVersion = _majorVersionNumber + "." + _minorVersionNumber + "."
                    + _maintenanceNumber + "." + _buildNumber;
                m_patchManager.SetOnSetMainProgressBarAction(OnSetMainProgressBar);
                m_patchManager.SetOnSetDetailProgressBarAction(OnSetDetailProgressBar);
                m_patchManager.SetOnIncreaseMainProgressBarAction(OnIncreaseMainProgressBar);
                m_patchManager.SetOnIncreaseDetailProgressBarAction(OnIncreaseDetailProgressBar);
                m_patchManager.SetOnLogAction(OnLog);
                m_patchManager.SetOnErrorAction(OnError);
                m_patchManager.SetOnFatalErrorAction(OnFatalError);
                m_patchManager.SetOnTaskStartedAction(OnTaskStarted);
                m_patchManager.SetOnTaskCompletedAction(OnTaskCompleted);
                //m_patchManager.BuildNewVersion(newVersion);

				IEnumerable<string> files = FileManager.GetFiles(SettingsManager.PATCHER_FILES_PATH, "*", SearchOption.AllDirectories);
				foreach (string entry in files)
				{
                    string currentFile = entry.Replace(SettingsManager.PATCHER_FILES_PATH, SettingsManager.CURRENT_BUILD_PATH);
                    if (!FileManager.FileExists(currentFile))
                        if (!FileManager.DirectoryExists(Path.GetDirectoryName(currentFile)))
                            FileManager.CreateDirectory(Path.GetDirectoryName(currentFile));
						FileManager.FileCopy(entry, currentFile, false);
				}

                _patchBuilderThread = new Thread(() => m_patchManager.BuildNewVersion(newVersion));
                _patchBuilderThread.Start();
            }
            GUI.enabled = true;
            EditorGUILayout.EndVertical();

            EditorGUILayout.EndHorizontal();


            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.BeginVertical();
            EditorGUI.ProgressBar(new Rect(3, 45, position.width - 6, 20), _detailProgress / _detailProgressBarMax, _detailLog);
            EditorGUI.ProgressBar(new Rect(3, 70, position.width - 6, 20), _mainProgress / _mainProgressBarMax, _mainLog);
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.EndVertical();

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Separator();

            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.BeginVertical();
            EditorGUILayout.LabelField(_endLog, _endLogStyle);
            EditorGUILayout.EndVertical();

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.EndScrollView();
            #endregion

            #region Create patch
            EditorGUILayout.LabelField("Patch builder", EditorStyles.boldLabel);

            EditorGUILayout.Space();

            _patchesBuilderWindowScrollPosition = EditorGUILayout.BeginScrollView(_patchesBuilderWindowScrollPosition, GUILayout.MaxHeight(450), GUILayout.ExpandHeight(false));

            if (_currentVersions != null && _currentVersions.Length > 1)
            {
                EditorGUILayout.BeginHorizontal();

                EditorGUILayout.BeginVertical();
                EditorGUILayout.LabelField("Compression type:", GUILayout.MaxWidth(150));
                EditorGUILayout.EndVertical();

                EditorGUILayout.BeginVertical();
                _compressionTypeDropdownIndex = EditorGUILayout.Popup(_compressionTypeDropdownIndex, _compressionTypes, GUILayout.MinWidth(60));
                EditorGUILayout.EndVertical();

                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();

                EditorGUILayout.BeginVertical();
                EditorGUILayout.LabelField("Version from:", GUILayout.MaxWidth(100));
                EditorGUILayout.EndVertical();

                EditorGUILayout.BeginVertical();
                _versionFromDropdownIndex = EditorGUILayout.Popup(_versionFromDropdownIndex, _currentVersions, GUILayout.MinWidth(60));
                EditorGUILayout.EndVertical();

                EditorGUILayout.BeginVertical();
                EditorGUILayout.LabelField("Version to:", GUILayout.MaxWidth(100));
                EditorGUILayout.EndVertical();

                EditorGUILayout.BeginVertical();
                _versionToDropdownIndex = EditorGUILayout.Popup(_versionToDropdownIndex, _currentVersions, GUILayout.MinWidth(60));
                EditorGUILayout.EndVertical();

                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();

                GUI.enabled = _patchBuilderStatus == PatchBuilderStatus.IDLE;
                if (GUILayout.Button("Build patch"))
                {
                    m_patchManager.SetOnSetMainProgressBarAction(OnSetMainProgressBarPatchBuild);
                    m_patchManager.SetOnSetDetailProgressBarAction(OnSetDetailProgressBarPatchBuild);
                    m_patchManager.SetOnIncreaseMainProgressBarAction(OnIncreaseMainProgressBarPatchBuild);
                    m_patchManager.SetOnIncreaseDetailProgressBarAction(OnIncreaseDetailProgressBarPatchBuild);
                    m_patchManager.SetOnLogAction(OnLogPatchBuild);
                    m_patchManager.SetOnErrorAction(OnErrorPatchBuild);
                    m_patchManager.SetOnFatalErrorAction(OnFatalErrorPatchBuild);
                    m_patchManager.SetOnTaskStartedAction(OnTaskStartedPatchBuild);
                    m_patchManager.SetOnTaskCompletedAction(OnTaskCompletedPatchBuild);
                    //m_patchManager.BuildPatch(_currentVersions[_versionFromDropdownIndex], _currentVersions[_versionToDropdownIndex], (CompressionType)Enum.Parse(typeof(CompressionType), _compressionTypes[_compressionTypeDropdownIndex]));
                    _patchBuilderThread = new Thread(
                        () => m_patchManager.BuildPatch(_currentVersions[_versionFromDropdownIndex], _currentVersions[_versionToDropdownIndex], (CompressionType)Enum.Parse(typeof(CompressionType), _compressionTypes[_compressionTypeDropdownIndex]))
                    );
                    _patchBuilderThread.Start();
                }
                GUI.enabled = true;

                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();

                EditorGUILayout.BeginVertical();
                EditorGUI.ProgressBar(new Rect(3, 60, position.width - 6, 20), _detailProgressPatchBuild / _detailProgressBarMaxPatchBuild, _detailLogPatchBuild);
                EditorGUI.ProgressBar(new Rect(3, 85, position.width - 6, 20), _mainProgressPatchBuild / _mainProgressBarMaxPatchBuild, _mainLogPatchBuild);
                EditorGUILayout.Space();
                EditorGUILayout.Space();
                EditorGUILayout.Space();
                EditorGUILayout.Space();
                EditorGUILayout.Space();
                EditorGUILayout.Space();
                EditorGUILayout.Space();
                EditorGUILayout.Space();
                EditorGUILayout.EndVertical();

                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();

                EditorGUILayout.BeginVertical();
                EditorGUILayout.LabelField(_endLogPatchBuild, _endLogStyle);
                EditorGUILayout.EndVertical();

                EditorGUILayout.EndHorizontal();
            }
            else
            {
                EditorGUILayout.BeginHorizontal(GUI.skin.box);

                EditorGUILayout.BeginVertical();
                EditorGUILayout.LabelField("There are not enough builds to create patches!");
                EditorGUILayout.EndVertical();

                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.EndScrollView();
            #endregion

            #region Deploy
            EditorGUILayout.LabelField("Deploy", EditorStyles.boldLabel);

            EditorGUILayout.Space();

            _deployWindowScrollPosition = EditorGUILayout.BeginScrollView(_deployWindowScrollPosition, GUILayout.MaxHeight(450), GUILayout.ExpandHeight(false));

            if (_currentVersions != null && _currentVersions.Length > 0)
            {
                if (_scenesInBuildSettings.Count > 0)
                {
                    EditorGUILayout.BeginHorizontal();

                    EditorGUILayout.BeginVertical();
                    EditorGUILayout.LabelField("Compression type:", GUILayout.MaxWidth(150));
                    EditorGUILayout.EndVertical();

                    EditorGUILayout.BeginVertical();
                    _deployCompressionTypeDropdownIndex = EditorGUILayout.Popup(_deployCompressionTypeDropdownIndex, _compressionTypes, GUILayout.MinWidth(60));
                    EditorGUILayout.EndVertical();

                    EditorGUILayout.EndHorizontal();

                    EditorGUILayout.BeginHorizontal();

                    EditorGUILayout.BeginVertical();
                    EditorGUILayout.LabelField("Version to deploy:", GUILayout.MaxWidth(150));
                    EditorGUILayout.EndVertical();

                    EditorGUILayout.BeginVertical();
                    _versionToDeployDropdownIndex = EditorGUILayout.Popup(_versionToDeployDropdownIndex, _currentVersions, GUILayout.MinWidth(60));
                    EditorGUILayout.EndVertical();

                    EditorGUILayout.EndHorizontal();

                    EditorGUILayout.BeginHorizontal();

                    EditorGUILayout.BeginVertical();
                    EditorGUILayout.LabelField("Platform to deploy:", GUILayout.MaxWidth(150));
                    EditorGUILayout.EndVertical();

                    EditorGUILayout.BeginVertical();
                    _platformToDeployDropdownIndex = EditorGUILayout.Popup(_platformToDeployDropdownIndex, _platformsAvailable, GUILayout.MinWidth(60));
                    EditorGUILayout.EndVertical();

                    EditorGUILayout.EndHorizontal();

                    EditorGUILayout.BeginHorizontal();

                    EditorGUILayout.BeginVertical();
                    EditorGUILayout.LabelField("Launcher scene:", GUILayout.MaxWidth(150));
                    EditorGUILayout.EndVertical();

                    EditorGUILayout.BeginVertical();
                    _launcherSceneToDeployDropdownIndex = EditorGUILayout.Popup(_launcherSceneToDeployDropdownIndex, _scenesInBuildSettings.ToArray(), GUILayout.MinWidth(60));
                    EditorGUILayout.EndVertical();

                    EditorGUILayout.EndHorizontal();

                    EditorGUILayout.BeginHorizontal();

                    EditorGUILayout.BeginVertical();
                    EditorGUILayout.LabelField("Launcher custom name:", GUILayout.MaxWidth(150));
                    EditorGUILayout.EndVertical();

                    EditorGUILayout.BeginVertical();
                    _launcherCustomName = EditorGUILayout.TextField(_launcherCustomName, GUILayout.MinWidth(60));
                    EditorGUILayout.EndVertical();

                    EditorGUILayout.EndHorizontal();

                    EditorGUILayout.BeginHorizontal(GUI.skin.box);

                    GUI.enabled = _patchBuilderStatus == PatchBuilderStatus.IDLE;

                    EditorGUILayout.BeginVertical();
                    if (GUILayout.Button("Update scenes", GUILayout.MaxWidth(150)))
                    {
                        UpdateScenesToBuild();
                    }
                    EditorGUILayout.EndVertical();

                    EditorGUILayout.BeginVertical();
                    if (GUILayout.Button("Deploy", GUILayout.MaxWidth(300)))
                    {
                        Deploy();

                    }
                    EditorGUILayout.EndVertical();
                    GUI.enabled = true;

                    EditorGUILayout.EndHorizontal();
                }
                else
                {
                    EditorGUILayout.BeginHorizontal(GUI.skin.box);

                    EditorGUILayout.BeginVertical();
                    EditorGUILayout.LabelField("There are no available scenes to build your Launcher!");
                    EditorGUILayout.EndVertical();

                    EditorGUILayout.EndHorizontal();
                }
            }
            else
            {
                EditorGUILayout.BeginHorizontal();

                EditorGUILayout.BeginVertical();
                EditorGUILayout.LabelField("There are no available builds to deploy your game!");
                EditorGUILayout.EndVertical();

                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.EndScrollView();
            #endregion

            #region Upload to FTP/SFTP
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.BeginVertical();
            EditorGUILayout.LabelField("Upload to FTP", EditorStyles.boldLabel, GUILayout.MaxWidth(280));
            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginVertical();
            if (GUILayout.Button("Save prefs", GUILayout.MaxWidth(150)))
            {
                SaveEditorPrefs();
            }
            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginVertical();
            if (GUILayout.Button("Delete prefs", GUILayout.MaxWidth(150)))
            {
                DeleteEditorPrefs();
            }
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space();

            _uploadWindowScrollPosition = EditorGUILayout.BeginScrollView(_uploadWindowScrollPosition, GUILayout.MaxHeight(450), GUILayout.ExpandHeight(false));

            /*EditorGUILayout.BeginHorizontal();

            EditorGUILayout.BeginVertical();
            EditorGUILayout.LabelField("Use SFTP:", GUILayout.MaxWidth(150));
            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginVertical();
            _isSFTP = EditorGUILayout.Toggle(_isSFTP, GUILayout.MaxWidth(60));
            EditorGUILayout.EndVertical();
            
            EditorGUILayout.EndHorizontal();*/

            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.BeginVertical();
            EditorGUILayout.LabelField("Host:", GUILayout.MaxWidth(150));
            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginVertical();
            _uploadHost = EditorGUILayout.TextField(_uploadHost, GUILayout.MinWidth(60));
            EditorGUILayout.EndVertical();

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.BeginVertical();
            EditorGUILayout.LabelField("Port:", GUILayout.MaxWidth(150));
            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginVertical();
            _uploadHostPort = EditorGUILayout.IntField(_uploadHostPort, GUILayout.MinWidth(60));
            EditorGUILayout.EndVertical();

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.BeginVertical();
            EditorGUILayout.LabelField("Username:", GUILayout.MaxWidth(150));
            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginVertical();
            _uploadUsername = EditorGUILayout.TextField(_uploadUsername, GUILayout.MinWidth(60));
            EditorGUILayout.EndVertical();

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.BeginVertical();
            EditorGUILayout.LabelField("Password:", GUILayout.MaxWidth(150));
            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginVertical();
            _uploadPassword = EditorGUILayout.TextField(_uploadPassword, GUILayout.MinWidth(60));
            EditorGUILayout.EndVertical();

            EditorGUILayout.EndHorizontal();

            if (_currentVersions != null && _currentVersions.Length > 0)
            {
				#region Builds
                EditorGUILayout.BeginHorizontal(GUI.skin.box);

                EditorGUILayout.BeginVertical();
                EditorGUILayout.LabelField("Remote path:", GUILayout.MaxWidth(80));
                EditorGUILayout.EndVertical();

                EditorGUILayout.BeginVertical();
                _buildsRemotePath = EditorGUILayout.TextField(_buildsRemotePath, GUILayout.MinWidth(60));
                EditorGUILayout.EndVertical();

                EditorGUILayout.BeginVertical();
                EditorGUILayout.LabelField("Select build:", GUILayout.MaxWidth(80));
                EditorGUILayout.EndVertical();

                EditorGUILayout.BeginVertical();
                _versionToUploadDropdownIndex = EditorGUILayout.Popup(_versionToUploadDropdownIndex, _currentVersions, GUILayout.MinWidth(60));
                EditorGUILayout.EndVertical();

                EditorGUILayout.BeginVertical();
                if (GUILayout.Button("Upload", GUILayout.MaxWidth(150)))
                {
                    EditorUtility.DisplayProgressBar("P.A.T.C.H.", "Instantiating uploader...", 0);
                    FileUploader uploader = new FileUploader(
                        Protocol.FTP,
                        _uploadHost, _uploadHostPort, _uploadUsername, _uploadPassword
                    );
                    EditorUtility.DisplayProgressBar("P.A.T.C.H.", "Setting up credentials...", 0.1f);
                                        
                    EditorUtility.DisplayProgressBar("P.A.T.C.H.", "Retrieving files to upload...", 0.2f);
                    string version = Path.Combine(SettingsManager.BUILDS_PATH, _currentVersions[_versionToUploadDropdownIndex]);
                    List<string> files = FileManager.GetFiles(version).ToList();
                    float singleProgress = 0.7f / files.Count;
                    float progress = 0.2f;
                    foreach(string file in files)
                    {
                        string relativeFile = file.Replace(SettingsManager.BUILDS_PATH, "");
                        string remoteDirectory = Path.Combine(_buildsRemotePath, relativeFile.Replace(Path.GetFileName(file), "")).Replace("\\", "/");
                        uploader.UploadFile(file, remoteDirectory);
                        progress += singleProgress;
                        EditorUtility.DisplayProgressBar("P.A.T.C.H.", "Uploaded " + relativeFile + "!", progress);
                    }
                    
                    EditorUtility.DisplayProgressBar("P.A.T.C.H.", "Uploading indexes...", 0.95f);
                    string index = Path.Combine(SettingsManager.BUILDS_PATH, "index");
                    string patchIndex = Path.Combine(SettingsManager.BUILDS_PATH, "index_" + _currentVersions[_versionToUploadDropdownIndex] + ".bix");
                    uploader.UploadFile(index, _buildsRemotePath);
                    uploader.UploadFile(patchIndex, _buildsRemotePath);

                    EditorUtility.DisplayProgressBar("P.A.T.C.H.", "Done!", 1f);

                    EditorUtility.ClearProgressBar();
                }
                EditorGUILayout.EndVertical();

                EditorGUILayout.EndHorizontal();
				#endregion

				#region Patcher
				EditorGUILayout.BeginHorizontal(GUI.skin.box);

				EditorGUILayout.BeginVertical();
				EditorGUILayout.LabelField("Remote path:", GUILayout.MaxWidth(80));
				EditorGUILayout.EndVertical();

				EditorGUILayout.BeginVertical();
				_patcherRemotePath = EditorGUILayout.TextField(_patcherRemotePath, GUILayout.MinWidth(60));
				EditorGUILayout.EndVertical();

				EditorGUILayout.BeginVertical();
				EditorGUILayout.LabelField("Remote path for your launcher", GUILayout.MaxWidth(290), GUILayout.MinWidth(140));
				EditorGUILayout.EndVertical();

				EditorGUILayout.BeginVertical();
				if (GUILayout.Button("Upload", GUILayout.MaxWidth(150)))
				{
					EditorUtility.DisplayProgressBar("P.A.T.C.H.", "Instantiating uploader...", 0);
					FileUploader uploader = new FileUploader(
						Protocol.FTP,
						_uploadHost, _uploadHostPort, _uploadUsername, _uploadPassword
					);
					EditorUtility.DisplayProgressBar("P.A.T.C.H.", "Setting up credentials...", 0.1f);

					EditorUtility.DisplayProgressBar("P.A.T.C.H.", "Building launcher...", 0.2f);
					DeployLauncher();

					EditorUtility.DisplayProgressBar("P.A.T.C.H.", "Uploading launcher...", 0.95f);
					uploader.UploadFile(Path.Combine(SettingsManager.DEPLOY_PATH, "patcher.zip"), _patcherRemotePath);

					EditorUtility.DisplayProgressBar("P.A.T.C.H.", "Done!", 1f);

					EditorUtility.ClearProgressBar();
				}
				EditorGUILayout.EndVertical();

				EditorGUILayout.EndHorizontal();
				#endregion
            }
            else
            {
                EditorGUILayout.BeginHorizontal(GUI.skin.box);

                EditorGUILayout.BeginVertical();
                EditorGUILayout.LabelField("There are no available builds to upload!");
                EditorGUILayout.EndVertical();

                EditorGUILayout.EndHorizontal();
            }

            if (_currentPatches != null && _currentPatches.Length > 0)
            {
                EditorGUILayout.BeginHorizontal(GUI.skin.box);
                
                EditorGUILayout.BeginVertical();
                EditorGUILayout.LabelField("Remote path:", GUILayout.MaxWidth(80));
                EditorGUILayout.EndVertical();

                EditorGUILayout.BeginVertical();
                _patchesRemotePath = EditorGUILayout.TextField(_patchesRemotePath, GUILayout.MinWidth(60));
                EditorGUILayout.EndVertical();

                EditorGUILayout.BeginVertical();
                EditorGUILayout.LabelField("Select patch:", GUILayout.MaxWidth(80));
                EditorGUILayout.EndVertical();

                EditorGUILayout.BeginVertical();
                _patchToUploadDropdownIndex = EditorGUILayout.Popup(_patchToUploadDropdownIndex, _currentPatches, GUILayout.MinWidth(60));
                EditorGUILayout.EndVertical();

                EditorGUILayout.BeginVertical();
                if (GUILayout.Button("Upload", GUILayout.MaxWidth(150)))
                {
                    try
                    {
                        EditorUtility.DisplayProgressBar("P.A.T.C.H.", "Instantiating uploader...", 0);
                    
                        FileUploader uploader = new FileUploader(Protocol.FTP,
                            _uploadHost, _uploadHostPort, _uploadUsername, _uploadPassword
                        );
                    
                        EditorUtility.DisplayProgressBar("P.A.T.C.H.", "Setting up credentials...", 0.1f);
                        
                        EditorUtility.DisplayProgressBar("P.A.T.C.H.", "Retrieving files to upload...", 0.2f);
                        string archive = Path.Combine(SettingsManager.FINAL_PATCHES_PATH, _currentPatches[_patchToUploadDropdownIndex]) + ".archive";
                        string pix = Path.Combine(SettingsManager.FINAL_PATCHES_PATH, _currentPatches[_patchToUploadDropdownIndex]) + ".pix";
                        string versions = Path.Combine(SettingsManager.FINAL_PATCHES_PATH, "versions.txt");
                    
                        EditorUtility.DisplayProgressBar("P.A.T.C.H.", "Uploading patch index...", 0.3f);
                        uploader.UploadFile(pix, _patchesRemotePath);
                        EditorUtility.DisplayProgressBar("P.A.T.C.H.", "Uploading patch archive...", 0.6f);
                        uploader.UploadFile(archive, _patchesRemotePath);
                        EditorUtility.DisplayProgressBar("P.A.T.C.H.", "Uploading versions indexer...", 0.9f);
                        uploader.UploadFile(versions, _patchesRemotePath);
                        EditorUtility.DisplayProgressBar("P.A.T.C.H.", "Done!", 1f);

                        EditorUtility.ClearProgressBar();
                    }
                    catch(Exception e)
                    {
                        Debug.Log(e.Message);
                        EditorUtility.ClearProgressBar();
                    }
                }
                EditorGUILayout.EndVertical();

                EditorGUILayout.EndHorizontal();
            }
            else
            {
                EditorGUILayout.BeginHorizontal(GUI.skin.box);

                EditorGUILayout.BeginVertical();
                EditorGUILayout.LabelField("There are no available patches to upload!");
                EditorGUILayout.EndVertical();

                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.EndScrollView();
            #endregion
        }
        catch(Exception e)
        {
            Debug.Log(e.Message);
        }
    }

    private void Deploy()
    {
		try
		{Debug.Log(SettingsManager.DEPLOY_PATH); Debug.Log(SettingsManager.BUILDS_PATH); Debug.Log(SettingsManager.APP_PATH); 
	        // Cache current build target
	        EditorUtility.DisplayProgressBar("P.A.T.C.H.", "Caching current build target...", 0);
	        BuildTarget cachedTarget = EditorUserBuildSettings.activeBuildTarget;

	        // Copy selected build in deploy environment
	        EditorUtility.DisplayProgressBar("P.A.T.C.H.", "Checking deploy directory...", 0.1f);
	        if (FileManager.DirectoryExists(SettingsManager.DEPLOY_PATH + _currentVersions[_versionToDeployDropdownIndex]))
	            FileManager.DeleteDirectory(SettingsManager.DEPLOY_PATH + _currentVersions[_versionToDeployDropdownIndex]);
	        EditorUtility.DisplayProgressBar("P.A.T.C.H.", "Copying selected build...", 0.2f);
	        FileManager.CopyDirectory(SettingsManager.BUILDS_PATH + Path.DirectorySeparatorChar + _currentVersions[_versionToDeployDropdownIndex], SettingsManager.DEPLOY_PATH + _currentVersions[_versionToDeployDropdownIndex]);

	        // Build selected Launcher scene
	        BuildTarget target = (BuildTarget)Enum.Parse(typeof(BuildTarget), _platformsAvailable[_platformToDeployDropdownIndex]);
	        EditorUserBuildSettings.SwitchActiveBuildTarget(target);
	        BuildPipeline.BuildPlayer(new string[] { _scenesInBuildSettings[_launcherSceneToDeployDropdownIndex] },
				Path.Combine(SettingsManager.DEPLOY_PATH, _currentVersions[_versionToDeployDropdownIndex] + Path.DirectorySeparatorChar + _launcherCustomName + GetAppExtension(target)), 
	            target, BuildOptions.None);

			FileManager.DeleteFiles (Path.Combine(SettingsManager.DEPLOY_PATH, _currentVersions[_versionToDeployDropdownIndex]), "*.pdb");

	        // ZIP generated application
	        EditorUtility.DisplayProgressBar("P.A.T.C.H.", "Compressing deployed build...", 0.5f);
	        CompressionType ctype;
	        string sCType = _compressionTypes[_deployCompressionTypeDropdownIndex];
	        switch(sCType)
	        {
	            case "ZIP": ctype = CompressionType.ZIP; break;
	            case "TAR": ctype = CompressionType.TAR; break;
	            case "TARGZ": ctype = CompressionType.TARGZ; break;
	            default: ctype = CompressionType.TAR; break;
	        }
	        m_patchManager.DeployCompress(
				Path.Combine(SettingsManager.DEPLOY_PATH, _currentVersions[_versionToDeployDropdownIndex]),
				Path.Combine(SettingsManager.DEPLOY_PATH, _launcherCustomName + "_" + _currentVersions[_versionToDeployDropdownIndex] + "_" + _platformsAvailable[_platformToDeployDropdownIndex] + ".zip"),
	            ctype
	        );

	        // Clean up useless files
	        EditorUtility.DisplayProgressBar("P.A.T.C.H.", "Cleaning up...", 0.9f);
			FileManager.DeleteDirectory(Path.Combine(SettingsManager.DEPLOY_PATH, _currentVersions[_versionToDeployDropdownIndex]));

	        // Restore original build target
	        EditorUtility.DisplayProgressBar("P.A.T.C.H.", "Finalization...", 1f);
	        EditorUserBuildSettings.SwitchActiveBuildTarget(cachedTarget);
	        EditorUtility.ClearProgressBar();
		}
		catch(Exception e)
		{
			Debug.Log(e.Message);
		}
    }

	private void DeployLauncher()
	{
		try
		{
			// Cache current build target
			EditorUtility.DisplayProgressBar("P.A.T.C.H.", "Caching current build target...", 0);
			BuildTarget cachedTarget = EditorUserBuildSettings.activeBuildTarget;

			// Delete previous generated archive
			if(FileManager.DirectoryExists(Path.Combine(SettingsManager.DEPLOY_PATH, "Launcher")))
				FileManager.DeleteDirectory (Path.Combine (SettingsManager.DEPLOY_PATH, "Launcher"));

			// Build selected Launcher scene
			BuildTarget target = (BuildTarget)Enum.Parse(typeof(BuildTarget), _platformsAvailable[_platformToDeployDropdownIndex]);
			EditorUserBuildSettings.SwitchActiveBuildTarget(target);
			BuildPipeline.BuildPlayer(new string[] { _scenesInBuildSettings[_launcherSceneToDeployDropdownIndex] },
				SettingsManager.DEPLOY_PATH + "Launcher" + Path.DirectorySeparatorChar + _launcherCustomName + GetAppExtension(target), 
				target, BuildOptions.None);

			FileManager.DeleteFiles (Path.Combine(SettingsManager.DEPLOY_PATH, "Launcher"), "*.pdb");

			// ZIP generated application
			EditorUtility.DisplayProgressBar("P.A.T.C.H.", "Compressing deployed build...", 0.5f);
			CompressionType ctype = CompressionType.ZIP;

			// Delete previous generated archive
			if(FileManager.FileExists(Path.Combine(SettingsManager.DEPLOY_PATH, "patcher.zip")))
				FileManager.DeleteFile (Path.Combine (SettingsManager.DEPLOY_PATH, "patcher.zip"));

			m_patchManager.DeployCompress(
				Path.Combine(SettingsManager.DEPLOY_PATH, "Launcher"),
				Path.Combine(SettingsManager.DEPLOY_PATH, "patcher.zip"),
				ctype
			);

			// Clean up useless files
			EditorUtility.DisplayProgressBar("P.A.T.C.H.", "Cleaning up...", 0.9f);
			Path.Combine(SettingsManager.DEPLOY_PATH, "Launcher");

			// Restore original build target
			EditorUtility.DisplayProgressBar("P.A.T.C.H.", "Finalization...", 1f);
			EditorUserBuildSettings.SwitchActiveBuildTarget(cachedTarget);
			EditorUtility.ClearProgressBar();
		}
		catch(Exception e)
		{
			Debug.Log(e.Message);
		}
	}

    private string GetAppExtension(BuildTarget target)
    {
        string extension = "";

        switch (target)
        {
            case BuildTarget.Android:
                extension = ".apk";
                break;
            /*case BuildTarget.BlackBerry:
                // Not implemented.
                break;
            case BuildTarget.WSAPlayer:
                // Not implemented.
                break;

            case BuildTarget.PS3:
                // Not implemented.
                break;
            case BuildTarget.PS4:
                // Not implemented.
                break;
            case BuildTarget.PSM:
                // Not implemented.
                break;
            case BuildTarget.PSP2:
                // Not implemented.
                break;
            case BuildTarget.SamsungTV:
                // Not implemented. apk?
                break;
            case BuildTarget.StandaloneGLESEmu:
                // Not implemented.
                break;*/
            case BuildTarget.StandaloneLinux:
                extension = ".x86";
                break;
            case BuildTarget.StandaloneLinux64:
                extension = ".x86_64";
                break;
            case BuildTarget.StandaloneLinuxUniversal:
                extension = ".x86";
                break;
            case BuildTarget.StandaloneOSXIntel:
                extension = ".app";
                break;
            case BuildTarget.StandaloneOSXIntel64:
                extension = ".app";
                break;
            case BuildTarget.StandaloneOSX:
                extension = ".app";
                break;
            case BuildTarget.StandaloneWindows:
                extension = ".exe";
                break;
            case BuildTarget.StandaloneWindows64:
                extension = ".exe";
                break;
            case BuildTarget.Tizen:
                extension = ".tpk";
                break;
                /*case BuildTarget.WP8Player:
                    //Not implemented. xap?
                    break;*/
// case BuildTarget.WebPlayer:
              //  extension = "";
             //   break;
            /*case BuildTarget.WebPlayerStreamed:
                //Not implemented. ""?
                break;
            case BuildTarget.XBOX360:
                //Not implemented.
                break;
            case BuildTarget.XboxOne:
                //Not implemented.
                break;
            case BuildTarget.iOS:
                //Not implemented.
                break;*/
            default: 
                break;
        }

        return extension;
    }
}