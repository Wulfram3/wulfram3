/*
 * Copyright (C) 2015 Exit Games GmbH
 * by The Knights of Unity
 */

using ExitGames.Client.Photon;
using ExitGames.Client.Photon.Chat;
using UnityEngine;
using UnityEngine.UI;

namespace PhotonChatUI
{
    /// <summary>
    /// Base UI for Chat.
    /// </summary>
    [RequireComponent(typeof (Chat))]
    public class ChatUI : ChatBehaviourUI, IChatClientListener
    {
        private static ChatUI _instance;

        public static ChatUI Instance
        {
            get
            {
                return _instance == null || _instance.gameObject == null
                    ? (_instance = FindObjectOfType<ChatUI>())
                    : _instance;
            }
        }

        public ChatDockUI MainDock;

        [Tooltip("Setting it to null will disable creating floating docks. ")] public ChatFloatingDockUI
            FloatingDockPrefab;

        public ChatChannelUI ChatChannelPrefab;

        public string[] ChannelsToSubscribeAtStart;

        private void CreateChannel(string channelNameOrUsername, bool isPrivate, bool focus = true)
        {
            ChatChannel channel;

            if (isPrivate)
                channel = Chat.Instance.FindPrivateChannel(channelNameOrUsername);
            else
                channel = Chat.Instance.FindPublicChannel(channelNameOrUsername);

            if (channel == null)
            {
                var channelUI = Instantiate(ChatChannelPrefab);

                if (isPrivate)
                {
                    channel = ChatPrivateChannel.Create(channelNameOrUsername, channelUI.gameObject);
                }
                else
                {
                    channel = ChatPublicChannel.Create(channelNameOrUsername, channelUI.gameObject);
                }

                if (channel == null)
                {
                    DestroyImmediate(channelUI.gameObject);
                    return;
                }

                Chat.Instance.SubscribeChannel(channel, 10);

                MainDock.Dock(channelUI, focus);

                return;
            }

            if (focus)
            {
                channel.ChatChannelUI.ChatDockUI.Activate(channel.ChatChannelUI);
            }
        }

        public void Connect(InputField loginInputField)
        {
            Connect(loginInputField.text);
        }

        public void Connect(string login)
        {
            if (!string.IsNullOrEmpty(login))
                Chat.Instance.Connect(new ExitGames.Client.Photon.Chat.AuthenticationValues(login));
        }

        public void Connect(ExitGames.Client.Photon.Chat.AuthenticationValues authenticationValues)
        {
            Chat.Instance.Connect(authenticationValues);
        }

        public void CreatePrivateChannel(InputField usernameInputField)
        {
            CreatePrivateChannel(usernameInputField.text);
        }

        public void CreatePrivateChannel(string username)
        {
            if (!string.IsNullOrEmpty(username))
                CreateChannel(username, true);
        }

        public void CreatePublicChannel(InputField channelNameInputField)
        {
            CreatePublicChannel(channelNameInputField.text);
        }

        public void CreatePublicChannel(string channelName)
        {
            if (!string.IsNullOrEmpty(channelName))
                CreateChannel(channelName, false);
        }

        public virtual void Awake()
        {
            _instance = this;
        }

        public void OnConnected()
        {
            if (ChannelsToSubscribeAtStart != null && ChannelsToSubscribeAtStart.Length > 0)
            {
                for (int i = 0; i < ChannelsToSubscribeAtStart.Length; i++)
                {
                    CreateChannel(ChannelsToSubscribeAtStart[i], false);
                }
            }
        }

        public void OnDisconnected()
        {
        }

        public void DebugReturn(DebugLevel level, string message)
        {
        }

        public void OnChatStateChange(ChatState state)
        {
        }

        public void OnGetMessages(string channelName, string[] senders, object[] messages)
        {
        }

        public void OnPrivateMessage(string sender, object message, string channelName)
        {
            CreateChannel(sender, true, false);
        }

        public void OnSubscribed(string[] channels, bool[] results)
        {
        }

        public void OnUnsubscribed(string[] channels)
        {
        }

        public void OnStatusUpdate(string user, int status, bool gotMessage, object message)
        {
        }
    }
}