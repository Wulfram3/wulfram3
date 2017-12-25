using PhotonChatUI;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof (Chat))]
public class ChatEditor : Editor
{
    private const string ChatDashboardUrl = "https://www.photonengine.com/en/dashboard/chat";

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        string appId = serializedObject.FindProperty("AppId").stringValue;
        appId = appId.Trim();

        bool validAppId = Chat.IsGuid(appId);

        if (!validAppId)
        {
            EditorGUILayout.HelpBox("App Id value is missing!\nGet your App Id from the Chat Dashboard.", MessageType.Error);
        }

        if (GUILayout.Button("Open Chat Dashboard for App Id"))
        {
            Application.OpenURL(ChatDashboardUrl);
        }
    }
}