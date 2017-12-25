using PhotonChatUI;
using UnityEngine;


/// <summary>
/// Keeps it's GameObject active until the Chat App Id is configured correctly.
/// </summary>
/// <remarks>
/// In other words: This script disables it's GameObject, when the Chat "App Id" is properly set.
/// </remarks>
[ExecuteInEditMode]
public class ChatConfigNote : MonoBehaviour
{
    private Chat chat;

    // Use this for initialization
    public void Start()
    {
        this.chat = FindObjectOfType<Chat>();
    }

    // Update is called once per frame
    public void Update()
    {
        if (this.chat == null)
        {
            this.chat = FindObjectOfType<Chat>();
            return;
        }

        // check if the chat component has a GUID as value
        if (Chat.IsGuid(this.chat.AppId))
        {
            gameObject.SetActive(false);
            Debug.Log("Your Chat App Id is correctly configured. Going to disable the UI hint for configuration in hierarchy. Remove: '"+ this.gameObject.name+"' from the hierarchy. (Click to highlight)", this.gameObject);
        }
    }
}