using System;
using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public class ChatEditor : EditorWindow
{
    static ChatEditor()
    {
        EditorApplication.update += OnEditorUpdate;
    }

    private static void OnEditorUpdate()
    {
        EditorApplication.update -= OnEditorUpdate;

        ChatSettings settings = ChatSettings.Load();
        if (settings != null && !settings.WizardDone && string.IsNullOrEmpty(settings.AppId))
        {
            OpenWizard();
        }
    }


    [MenuItem("Window/Photon Chat/Setup")]
    public static void OpenWizard()
    {
        currentSettings = ChatSettings.Load();
        currentSettings.WizardDone = true;
        EditorUtility.SetDirty(currentSettings);

        ChatEditor editor = (ChatEditor)EditorWindow.GetWindow(typeof (ChatEditor), false, "Photon Chat");
        editor.minSize = editor.preferredSize;
        editor.mailOrAppId = currentSettings.AppId;
    }


    private static ChatSettings currentSettings;
    internal string mailOrAppId;
    internal bool showDashboardLink = false;
    internal bool showRegistrationDone = false;
    internal bool showRegistrationError = false;
    private readonly Vector2 preferredSize = new Vector2(350, 400);

    internal static string UrlCloudDashboard = "https://www.photonengine.com/Dashboard/Chat?email=";

    public string WelcomeText = "Thanks for importing Photon Chat.\nThis window should set you up.\n\n<b>*</b> To use an existing Photon Chat App, enter your AppId.\n<b>*</b> To register or access an existing account, enter the account’s mail address.";
    public string AlreadyRegisteredInfo = "The email is registered so we can't fetch your AppId (without password).\n\nPlease login online to get your AppId and paste it above.";
    public string RegisteredNewAccountInfo = "We created a (free) account and fetched you an AppId.\nWelcome. Your Photon Chat project is setup.";
    public string FailedToRegisterAccount = "This wizard failed to register an account right now. Please check your mail address or try via the Dashboard.";
    public string AppliedToSettingsInfo = "Your AppId is now applied to this project.";
    public string SetupCompleteInfo = "<b>Done!</b>\nYour Chat AppId is now stored in the <b>ChatSettingsFile</b> now.\nHave a look.";
    public string CloseWindowButton = "Close";
    public string OpenCloudDashboardText = "Cloud Dashboard Login";
    public string OpenCloudDashboardTooltip = "Review Cloud App information and statistics.";


    public void OnGUI()
    {
        if (currentSettings == null)
        {
            currentSettings = ChatSettings.Load();
        }

        GUI.skin.label.wordWrap = true;
        GUI.skin.label.richText = true;
        if (string.IsNullOrEmpty(mailOrAppId))
        {
            mailOrAppId = string.Empty;
        }

        GUILayout.Label("Chat Settings", EditorStyles.boldLabel);
        GUILayout.Label(this.WelcomeText);
        GUILayout.Space(15);


        GUILayout.Label("AppId or Email");
        string input = EditorGUILayout.TextField(this.mailOrAppId);


        if (GUI.changed)
        {
            this.mailOrAppId = input.Trim();
        }

        bool isMail = false;
        bool minimumInput = false;

        if (this.mailOrAppId.Contains("@"))
        {
            // this should be a mail address
            minimumInput = (this.mailOrAppId.Length >= 5 && this.mailOrAppId.Contains("."));
            isMail = true;
        }
        else
        {
            // this should be an appId
            minimumInput = IsAppId(this.mailOrAppId);
            isMail = false;
        }


        EditorGUI.BeginDisabledGroup(!minimumInput);


        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        bool setupBtn = GUILayout.Button("Setup", GUILayout.Width(205));
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

        if (setupBtn)
        {
            this.showDashboardLink = false;
            this.showRegistrationDone = false;
            this.showRegistrationError = false;

            if (isMail)
            {
                EditorUtility.DisplayProgressBar("Fetching Account", "Trying to register a Photon Cloud Account.", 0.5f);
                AccountService service = new AccountService();
                service.RegisterByEmail(this.mailOrAppId, AccountService.Origin.Pun);
                EditorUtility.ClearProgressBar();

                if (service.ReturnCode == 0)
                {
                    Debug.Log("Photon Account Service returned Chat AppId: " + service.AppId);

                    currentSettings.AppId = service.AppId;
                    EditorUtility.SetDirty(currentSettings);
                    this.showRegistrationDone = true;

                    Selection.objects = new UnityEngine.Object[] { currentSettings };
                }
                else
                {
                    Debug.LogWarning("Photon Account Service error message: " + service.Message);
                    if (service.Message.Contains("registered"))
                    {
                        this.showDashboardLink = true;
                    }
                    else
                    {
                        this.showRegistrationError = true;
                    }
                }
            }
            else
            {
                currentSettings.AppId = this.mailOrAppId;
                EditorUtility.SetDirty(currentSettings);
                showRegistrationDone = true;
            }

            EditorGUIUtility.PingObject(currentSettings);
        }
        EditorGUI.EndDisabledGroup();

        if (this.showDashboardLink)
        {
            // button to open dashboard and get the AppId
            GUILayout.Space(15);
            GUILayout.Label(AlreadyRegisteredInfo);


            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button(new GUIContent(OpenCloudDashboardText, OpenCloudDashboardTooltip), GUILayout.Width(205)))
            {
                EditorUtility.OpenWithDefaultApp(UrlCloudDashboard + Uri.EscapeUriString(this.mailOrAppId));
                this.mailOrAppId = "";
                this.showDashboardLink = false;
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }
        if (this.showRegistrationError)
        {
            GUILayout.Space(15);
            GUILayout.Label(FailedToRegisterAccount);

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button(new GUIContent(OpenCloudDashboardText, OpenCloudDashboardTooltip), GUILayout.Width(205)))
            {
                EditorUtility.OpenWithDefaultApp(UrlCloudDashboard + Uri.EscapeUriString(this.mailOrAppId));
                this.mailOrAppId = "";
                this.showDashboardLink = false;
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

        }
        if (this.showRegistrationDone)
        {
            GUILayout.Space(15);
            GUILayout.Label("Registration done");
            if (isMail)
            {
                GUILayout.Label(RegisteredNewAccountInfo);
            }
            else
            {
                GUILayout.Label(AppliedToSettingsInfo);
            }

            // setup-complete info
            GUILayout.Space(15);
            GUILayout.Label(SetupCompleteInfo);


            // close window (done)
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button(CloseWindowButton, GUILayout.Width(205)))
            {
                this.Close();
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }
    }

    public static bool IsAppId(string val)
    {
        if (string.IsNullOrEmpty(val) || val.Length < 16)
        {
            return false;
        }

        try
        {
            new Guid(val);
        }
        catch
        {
            return false;
        }
        return true;
    }
}