using System;
using System.Collections;
using MHLab.PATCH.Settings;
using System.IO;
using UnityEngine.UI;
using UnityEngine;
#if UNITY_5_3_OR_NEWER
using UnityEngine.SceneManagement;
#endif
using MHLab.PATCH;
using MHLab.PATCH.Debugging;
using MHLab.PATCH.Utilities;
using MHLab.PATCH.Install;
using UnityThreading;

public class Launcher : MonoBehaviour
{
    enum LauncherStatus
    {
        IDLE,
        IS_BUSY
    }

    #region Members
    LauncherManager m_launcher;
    InstallManager m_installer;
    //LauncherStatus m_launcherStatus;

	[Header("Patcher & Patches")]
	[Tooltip("If enabled your launcher will provide to check and apply patches to your current build.")]
	public bool ActivatePatcher = true;
    [Tooltip("Your versions.txt file remote URL")]
    public string VersionsFileDownloadURL = "http://your/url/to/versions.txt";
    [Tooltip("Your patches directory remote URL")]
    public string PatchesDirectoryURL = "http://your/url/to/patches/directory/";
    [Tooltip("Your game name! This string will be attached to app root path to launch your game when patching process will end!")]
    public string AppToLaunch = "Build.exe";
    [Tooltip("Determines if your launcher will be closed after your game starts!")]
    public bool CloseLauncherOnStart = true;
    [Tooltip("This argument will be attached to your game running command!")]
    public string Argument = "default";
    [Tooltip("If enabled your argument will be sent as raw text, if not your argument will be sent as \"YourGame.exe --LaunchArgs=YourArgument\"")]
    public bool UseRawArgument = false;
	[Tooltip("If enabled your patcher can be included in your Unity game build")]
	public bool IsIntegrated = false;
	[Tooltip("If IsIntegrated is true, your patcher will load this scene after patch process")]
	public int SceneToLoad = 1;
    [Space(10)]

    [Header("Installer & Repairer")]
	[Tooltip("If enabled your launcher will install your build files before patches checking.")]
	public bool ActivateInstaller = true;
	[Tooltip("If enabled your launcher will start to check files integrity before patches checking. It is useful to fix files corruption of your users' builds!")]
	public bool ActivateRepairer = true;
    [Tooltip("Your builds directory remote URL")]
    public string BuildsDirectoryURL = "http://your/url/to/builds/directory/";
    /*[Tooltip("Your launcher archive remote URL")]
    public string LauncherArchiveURL = "http://your/url/to/launcher/archive.zip";*/
    [Tooltip("Your launcher name!")]
    public string LauncherName = "PATCH.exe";
    [Tooltip("If enabled your installer will install locally your game, if not your installer will install your game in Program Files/ProgramFilesDirectoryToInstall directory")]
    public bool InstallInLocalPath = false;
    [Tooltip("If your installer have to install your game under Program Files folder, this will be the name of your game directory!")]
    public string ProgramFilesDirectoryToInstall = "MHLab";
	/*[Tooltip("If enabled your installer will install also a patcher. It is useful for standalone installers.")]
	public bool InstallPatcher = false;*/
	[Tooltip("If enabled your installer will create a shortcut to your patcher on desktop")]
	public bool CreateDesktopShortcut = true;
    [Space(10)]

    [Header("Common settings")]
    [Tooltip("How many times downloader can retry to download a file, if an error occurs?")]
    public ushort DownloadRetryAttempts = 0;
    [Tooltip("Enables WebRequests or FTPRequests with credentials. Generally, you need this when your remote directories are proteted by login or your remote URLs are FTP ones!")]
    public bool EnableCredentials;
    [Tooltip("Username for your requests")]
    public string Username = "YourUsernameHere";
    [Tooltip("Password for your requests")]
    public string Password = "YourPasswordHere";
    [Space(10)]

    [Header("GUI Components")]
    public ProgressBar MainBar;
    public ProgressBar DetailBar;
    public Text MainLog;
    public Text DetailLog;
    public Button LaunchButton;
	public RectTransform Overlay;
	public RectTransform MainMenu;
	public RectTransform RestartMenu;
	public RectTransform OptionsMenu;

    ActionThread m_updateCheckingThread;
    #endregion

    #region Constructor
    void Start()
    {
        Localizatron.Instance.SetLanguage("en_EN");
        LocalizeGUI();

        OverrideSettings();

        m_launcher = new LauncherManager();
        m_launcher.SetOnSetMainProgressBarAction(OnSetMainProgressBar);
        m_launcher.SetOnSetDetailProgressBarAction(OnSetDetailProgressBar);
        m_launcher.SetOnIncreaseMainProgressBarAction(OnIncreaseMainProgressBar);
        m_launcher.SetOnIncreaseDetailProgressBarAction(OnIncreaseDetailProgressBar);
        m_launcher.SetOnLogAction(OnLog);
        m_launcher.SetOnErrorAction(OnError);
        m_launcher.SetOnFatalErrorAction(OnFatalError);
        m_launcher.SetOnTaskStartedAction(OnTaskStarted);
        m_launcher.SetOnTaskCompletedAction(OnTaskCompleted);
        m_launcher.SetOnDownloadProgressAction(OnDownloadProgress);
        m_launcher.SetOnDownloadCompletedAction(OnDownloadCompleted);

        m_installer = new InstallManager();
        m_installer.SetOnSetMainProgressBarAction(OnSetMainProgressBar);
        m_installer.SetOnSetDetailProgressBarAction(OnSetDetailProgressBar);
        m_installer.SetOnIncreaseMainProgressBarAction(OnIncreaseMainProgressBar);
        m_installer.SetOnIncreaseDetailProgressBarAction(OnIncreaseDetailProgressBar);
        m_installer.SetOnLogAction(OnLog);
        m_installer.SetOnErrorAction(OnError);
        m_installer.SetOnFatalErrorAction(OnFatalError);
        m_installer.SetOnTaskStartedAction(OnTaskStarted);
        m_installer.SetOnTaskCompletedAction(OnTaskCompleted);
        m_installer.SetOnDownloadProgressAction(OnDownloadProgress);
        m_installer.SetOnDownloadCompletedAction(OnDownloadCompleted);

        // Edit and uncomment this value to change size of download buffer, in byte
        // SettingsManager.DOWNLOAD_BUFFER_SIZE = 8192;
        
        m_updateCheckingThread = UnityThreadHelper.CreateThread(() => CheckForUpdates());
    }

    #endregion

    #region Callbacks for New version
    void OnSetMainProgressBar(int min, int max)
    {
        UnityThreadHelper.Dispatcher.Dispatch(() =>
        {
            MainBar.Clear();
            MainBar.Maximum = max;
            MainBar.Minimum = min;
            MainBar.Step = 1;
        });
    }

    void OnSetDetailProgressBar(int min, int max)
    {
        UnityThreadHelper.Dispatcher.Dispatch(() =>
        {
            DetailBar.Clear();
            DetailBar.Maximum = max;
            DetailBar.Minimum = min;
            DetailBar.Step = 1;
        });
    }

    void OnIncreaseMainProgressBar()
    {
        UnityThreadHelper.Dispatcher.Dispatch(() =>
        {
            MainBar.PerformStep();
        });
    }

    void OnIncreaseDetailProgressBar()
    {
        UnityThreadHelper.Dispatcher.Dispatch(() =>
        {
            DetailBar.PerformStep();
        });
    }

    void OnLog(string main, string detail)
    {
        UnityThreadHelper.Dispatcher.Dispatch(() =>
        {
            MainLog.text = Localizatron.Instance.Translate(main);
            DetailLog.text = Localizatron.Instance.Translate(detail);
        });
        Debugger.Log(main + " - " + detail);
    }

    void OnError(string main, string detail, Exception e)
    {
        UnityThreadHelper.Dispatcher.Dispatch(() =>
        {
            MainLog.text = Localizatron.Instance.Translate(main);
            DetailLog.text = Localizatron.Instance.Translate(detail);
        });
        Debugger.Log(e.Message);
        m_updateCheckingThread.Abort();
    }

    void OnFatalError(string main, string detail, Exception e)
    {
        UnityThreadHelper.Dispatcher.Dispatch(() =>
        {
            MainLog.text = Localizatron.Instance.Translate(main);
            DetailLog.text = Localizatron.Instance.Translate(detail);
        });
        Debugger.Log(e.Message);
        m_updateCheckingThread.Abort();
    }

    void OnTaskStarted(string message)
    {
        UnityThreadHelper.Dispatcher.Dispatch(() =>
        {
            LaunchButton.interactable = false;
            MainLog.text = Localizatron.Instance.Translate(message);
            DetailLog.text = "";
        });
        Debugger.Log(message);
    }

    void OnTaskCompleted(string message)
    {
        UnityThreadHelper.Dispatcher.Dispatch(() =>
        {
            MainLog.text = Localizatron.Instance.Translate(message);
            DetailLog.text = "";
            LaunchButton.interactable = true;
			if(IsIntegrated)
			{
				if(Overlay != null)
					Overlay.gameObject.SetActive(true);
				if(m_launcher.IsDirty())
					RestartMenu.gameObject.SetActive(true);
				else
					MainMenu.gameObject.SetActive(true);
			}
			#if !UNITY_STANDALONE_WIN
			EnsureExecutePrivileges();
			#endif
        });
        Debugger.Log(message);
    }

    DateTime _lastTime = DateTime.UtcNow;
    long _lastSize = 0;
    int _downloadSpeed = 0;

    void OnDownloadProgress(long currentFileSize, long totalFileSize, int percentageCompleted)
    {
        if (_lastTime.AddSeconds(1) <= DateTime.UtcNow)
        {
            _downloadSpeed = (int)((currentFileSize - _lastSize) / (DateTime.UtcNow - _lastTime).TotalSeconds);
            _lastSize = currentFileSize;
            _lastTime = DateTime.UtcNow;
        }

        UnityThreadHelper.Dispatcher.Dispatch(() =>
        {
            DetailBar.Progress = percentageCompleted;
            DetailBar.SetProgressText(percentageCompleted + "% - (" + Utility.FormatSizeBinary(currentFileSize, 2) + "/" + Utility.FormatSizeBinary(totalFileSize, 2) + ") @ " + Utility.FormatSizeBinary(_downloadSpeed, 2) + "/s");
        });
    }

    void OnDownloadCompleted()
    {
        UnityThreadHelper.Dispatcher.Dispatch(() =>
        {
            DetailBar.SetProgressText("");
        });
    }
    #endregion

    void CheckForUpdates()
    {
		if (!InstallInLocalPath) 
		{
			SettingsManager.APP_PATH = m_installer.GetInstallationPath();
			SettingsManager.RegeneratePaths();
			OverrideSettings (false);
		}

		bool isToInstall = m_installer.IsToInstall();
		InstallationState installStatus;
		//InstallationState installPatcherStatus;
		//InstallationState createShortcutStatus;

		if (ActivateInstaller) 
		{
			if (isToInstall) 
			{
				installStatus = m_installer.Install();

				/*if (InstallPatcher && installStatus == InstallationState.SUCCESS && !InstallInLocalPath)
					installPatcherStatus = m_installer.InstallPatcher();
				else
					installPatcherStatus = InstallationState.SUCCESS;*/
				
				if (CreateDesktopShortcut && installStatus == InstallationState.SUCCESS/* && installPatcherStatus == InstallationState.SUCCESS && InstallPatcher == true*/) 
				{
					//Debugger.Log ("Shortcut creation linked to launcher");
					m_installer.CreateShortcut ();
				}
				else //if (CreateDesktopShortcut && (installPatcherStatus == InstallationState.FAILED || InstallPatcher == false)) 
				{
                    if (CreateDesktopShortcut)
                    {
                        if (ActivatePatcher)
                            m_installer.CreateShortcut();
                        else
                            m_installer.CreateShortcut(false);
                    }
				}
			}
		}

		if(ActivateRepairer)
		{
			if (!isToInstall) 
			{
				m_installer.Repair();
			}
		}

		if(ActivatePatcher)
        	CheckForPatches();

		if (!InstallInLocalPath) 
		{
			SettingsManager.APP_PATH = Directory.GetParent(Application.dataPath).FullName;
			SettingsManager.RegeneratePaths();
		}
    }

    void CheckForPatches()
    {
        m_launcher.CheckForUpdates();
    }

    public void CloseButton_click()
    {
        Application.Quit();
    }

	public void StartGame_click()
	{
        #if UNITY_5_3_OR_NEWER
		SceneManager.LoadScene(SceneToLoad);
		#else
		Application.LoadLevel(SceneToLoad);
		#endif
        //SceneManager.LoadScene(SceneToLoad);
	}

	public void OptionButton_click()
	{
		if (MainMenu != null)
			MainMenu.gameObject.SetActive(false);
		if (OptionsMenu != null)
			OptionsMenu.gameObject.SetActive(true);
	}

	public void BackButton_click()
	{
		if (OptionsMenu != null)
			OptionsMenu.gameObject.SetActive(false);
		if (MainMenu != null)
			MainMenu.gameObject.SetActive(true);
	}

    public void EnglishButton_click()
    {
        Localizatron.Instance.SetLanguage("en_EN");
        LocalizeGUI();
    }

    public void ItalianButton_click()
    {
        Localizatron.Instance.SetLanguage("it_IT");
        LocalizeGUI();
    }

    public void LaunchButton_click()
    {
        try
        {
            if (!InstallInLocalPath)
            {
                SettingsManager.APP_PATH = m_installer.GetInstallationPath();
                SettingsManager.RegeneratePaths();
                OverrideSettings(false);
			}
			System.Diagnostics.Process process = new System.Diagnostics.Process();

#if UNITY_STANDALONE_WIN
			//process.Start(SettingsManager.LAUNCH_APP, (SettingsManager.USE_RAW_LAUNCH_ARG) ? SettingsManager.LAUNCH_ARG : SettingsManager.LAUNCH_COMMAND);
			process.StartInfo.FileName = SettingsManager.LAUNCH_APP;
			process.StartInfo.Arguments = (SettingsManager.USE_RAW_LAUNCH_ARG) ? SettingsManager.LAUNCH_ARG : SettingsManager.LAUNCH_COMMAND;
			process.StartInfo.UseShellExecute = false;
			process.Start();
#elif UNITY_STANDALONE_OSX
			/*process.Start( new System.Diagnostics.ProcessStartInfo(
	            "open",
	            "-a '" + SettingsManager.LAUNCH_APP + "' -n --args " + ((SettingsManager.USE_RAW_LAUNCH_ARG) ? SettingsManager.LAUNCH_ARG : SettingsManager.LAUNCH_COMMAND)
	            ){ UseShellExecute = false }
	        );*/
			process.StartInfo.FileName = "open";
			process.StartInfo.Arguments = "-a '" + SettingsManager.LAUNCH_APP + "' -n --args " + ((SettingsManager.USE_RAW_LAUNCH_ARG) ? SettingsManager.LAUNCH_ARG : SettingsManager.LAUNCH_COMMAND);
			process.StartInfo.UseShellExecute = false;
			process.Start();
#else
        	//process.Start(SettingsManager.LAUNCH_APP, (SettingsManager.USE_RAW_LAUNCH_ARG) ? SettingsManager.LAUNCH_ARG : SettingsManager.LAUNCH_COMMAND);
			//process.Start(new System.Diagnostics.ProcessStartInfo(SettingsManager.LAUNCH_APP, (SettingsManager.USE_RAW_LAUNCH_ARG) ? SettingsManager.LAUNCH_ARG : SettingsManager.LAUNCH_COMMAND));
			process.StartInfo.FileName = SettingsManager.LAUNCH_APP;
			process.StartInfo.Arguments = (SettingsManager.USE_RAW_LAUNCH_ARG) ? SettingsManager.LAUNCH_ARG : SettingsManager.LAUNCH_COMMAND;
			process.StartInfo.UseShellExecute = false;
			process.Start();
#endif

            if (this.CloseLauncherOnStart)
                Application.Quit();

            if (!InstallInLocalPath)
            {
                SettingsManager.APP_PATH = Directory.GetParent(Application.dataPath).FullName;
                SettingsManager.RegeneratePaths();
            }
        }
        catch
        {
            if (this.CloseLauncherOnStart)
                Application.Quit();
        }
    }

	public void RestartButton_click()
	{
		System.Diagnostics.Process process = new System.Diagnostics.Process();
		#if UNITY_STANDALONE_WIN
		//process.Start(SettingsManager.LAUNCH_APP, (SettingsManager.USE_RAW_LAUNCH_ARG) ? SettingsManager.LAUNCH_ARG : SettingsManager.LAUNCH_COMMAND);
		process.StartInfo.FileName = SettingsManager.LAUNCH_APP;
		process.StartInfo.Arguments = (SettingsManager.USE_RAW_LAUNCH_ARG) ? SettingsManager.LAUNCH_ARG : SettingsManager.LAUNCH_COMMAND;
		process.StartInfo.UseShellExecute = false;
		process.StartInfo.Verb = "runas";
		process.Start();
		#elif UNITY_STANDALONE_OSX
		/*process.Start( new System.Diagnostics.ProcessStartInfo(
		"open",
		"-a '" + SettingsManager.LAUNCH_APP + "' -n --args " + ((SettingsManager.USE_RAW_LAUNCH_ARG) ? SettingsManager.LAUNCH_ARG : SettingsManager.LAUNCH_COMMAND)
		){ UseShellExecute = false }
		);*/
		process.StartInfo.FileName = "open";
		process.StartInfo.Arguments = "-a '" + SettingsManager.LAUNCH_APP + "' -n --args " + ((SettingsManager.USE_RAW_LAUNCH_ARG) ? SettingsManager.LAUNCH_ARG : SettingsManager.LAUNCH_COMMAND);
		process.StartInfo.UseShellExecute = false;
		process.Start();
		#else
		//process.Start(SettingsManager.LAUNCH_APP, (SettingsManager.USE_RAW_LAUNCH_ARG) ? SettingsManager.LAUNCH_ARG : SettingsManager.LAUNCH_COMMAND);
		process.StartInfo.FileName = SettingsManager.LAUNCH_APP;
		process.StartInfo.Arguments = (SettingsManager.USE_RAW_LAUNCH_ARG) ? SettingsManager.LAUNCH_ARG : SettingsManager.LAUNCH_COMMAND;
		process.StartInfo.UseShellExecute = false;
		process.Start();
		#endif
		Application.Quit();
	}

	protected void OverrideSettings(bool overrideAppPath = true)
    {
		if (overrideAppPath) 
		{
#if UNITY_EDITOR
            SettingsManager.APP_PATH = Directory.GetParent(Application.dataPath).FullName + Path.DirectorySeparatorChar + "PATCH";
#elif UNITY_STANDALONE_WIN
            SettingsManager.APP_PATH = Directory.GetParent(Application.dataPath).FullName;
#elif UNITY_STANDALONE_LINUX
            SettingsManager.APP_PATH = Directory.GetParent(Application.dataPath).FullName;
#elif UNITY_STANDALONE_OSX
            SettingsManager.APP_PATH = Directory.GetParent(Directory.GetParent(Application.dataPath).FullName).FullName;
#else
            SettingsManager.APP_PATH = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
#endif
            SettingsManager.RegeneratePaths ();
		}
        SettingsManager.VERSIONS_FILE_DOWNLOAD_URL = VersionsFileDownloadURL;
        SettingsManager.PATCHES_DOWNLOAD_URL = PatchesDirectoryURL;
        SettingsManager.BUILDS_DOWNLOAD_URL = BuildsDirectoryURL;
		//SettingsManager.PATCHER_DOWNLOAD_URL = LauncherArchiveURL;
        SettingsManager.PATCH_DOWNLOAD_RETRY_ATTEMPTS = DownloadRetryAttempts;
        SettingsManager.LAUNCH_APP = SettingsManager.APP_PATH + Path.DirectorySeparatorChar + AppToLaunch;
        SettingsManager.LAUNCHER_NAME = LauncherName;
        SettingsManager.LAUNCH_ARG = Argument;
        SettingsManager.USE_RAW_LAUNCH_ARG = UseRawArgument;
        SettingsManager.ENABLE_FTP = EnableCredentials;
        SettingsManager.FTP_USERNAME = Username;
        SettingsManager.FTP_PASSWORD = Password;
        SettingsManager.INSTALL_IN_LOCAL_PATH = InstallInLocalPath;
        SettingsManager.PROGRAM_FILES_DIRECTORY_TO_INSTALL = ProgramFilesDirectoryToInstall;
        SettingsManager.PROGRAM_FILES_DIRECTORY_TO_INSTALL_ABS_PATH = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), ProgramFilesDirectoryToInstall);
		SettingsManager.PATCH_VERSION_PATH = SettingsManager.APP_PATH + Path.DirectorySeparatorChar + "version";
		SettingsManager.VERSION_FILE_LOCAL_PATH = SettingsManager.APP_PATH + Path.DirectorySeparatorChar + "version";
    }

    protected void LocalizeGUI()
    {
        LaunchButton.GetComponentInChildren<Text>().text = Localizatron.Instance.Translate("LAUNCH");
    }

	protected void EnsureExecutePrivileges()
	{
		#if UNITY_STANDALONE_OSX
		System.Diagnostics.Process process = new System.Diagnostics.Process();
		string file = Path.GetFileName(SettingsManager.LAUNCH_APP);
		process.StartInfo.FileName = "chmod";
		process.StartInfo.Arguments = "+x \"" + file + "/Contents/MacOS/" + file.Replace(".app", "") + "\"";
		process.Start();
		#endif
	}
}
