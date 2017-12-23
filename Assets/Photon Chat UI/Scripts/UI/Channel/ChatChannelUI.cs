/*
 * Copyright (C) 2015 Exit Games GmbH
 * by The Knights of Unity
 */

using UnityEngine;

namespace PhotonChatUI
{
    /// <summary>
    /// UI for channel.
    /// </summary>
    public class ChatChannelUI : ChatBehaviourUI
    {
        private ChatChannel _chatChannel;

        public ChatChannel ChatChannel
        {
            get { return _chatChannel ?? (_chatChannel = GetComponent<ChatChannel>()); }
        }

        private ChatDockUI _chatDockUI;

        public ChatDockUI ChatDockUI
        {
            get
            {
                if (_chatDockUI == null || _chatDockUI.gameObject == null ||
                    _chatDockUI.ChannelsContainer != transform.parent)
                {
                    var dockList = GetComponentsInParent<ChatDockUI>();
                    for (int i = 0; i < dockList.Length; i++)
                    {
                        if (dockList[i].ChannelsContainer == transform.parent)
                        {
                            _chatDockUI = dockList[i];
                            break;
                        }
                    }
                }
                return _chatDockUI;
            }
        }

        public ChatChannelMessageUI MessagePrefab;

        public Transform MessagesContainer;

        public int UnreadMessages { get; private set; }

        private string _lastSender;

        private void AddMessage(object message, string sender)
        {
            var messageInstance = Instantiate(MessagePrefab);

            if (messageInstance == null)
            {
                Debug.LogError(
                    "Failed to create message " + message + " from " + sender + " with prefab " + MessagePrefab, this);

                return;
            }

            messageInstance.transform.SetParent(MessagesContainer, false);
            messageInstance.SetData(sender, message, _lastSender == sender);
            _lastSender = sender;
        }

        public void ClearMessages()
        {
            _lastSender = null;
            while (MessagesContainer != null && MessagesContainer.childCount > 0)
                DestroyImmediate(MessagesContainer.GetChild(0).gameObject);
        }

        public void OnUpdateMessages(object[] messages, string[] senders)
        {
            for (var i = 0; i < messages.Length; i++)
            {
                AddMessage(messages[i], senders[i]);
            }
            if (!gameObject.activeSelf && ChatDockUI.chatPanelUI != null && !ChatDockUI.chatPanelUI.IsOpened)
                UnreadMessages += messages.Length;
        }

        public void OnSubscribed()
        {
            UnreadMessages = 0;
        }

        public void OnUnsubscribed()
        {
            ClearMessages();
        }

        public void Close()
        {
            Destroy(gameObject);
        }

        public virtual void Awake()
        {
            ClearMessages();
        }

        public virtual void Update()
        {
            if (ChatDockUI != ChatUI.Instance.MainDock || ChatDockUI.chatPanelUI == null ||
                ChatDockUI.chatPanelUI.IsOpened)
                UnreadMessages = 0;
        }
    }
}