/*
 * Copyright (C) 2015 Exit Games GmbH
 * by The Knights of Unity
 */

using UnityEngine;

namespace PhotonChatUI
{
    /// <summary>
    /// Private chat channel.
    /// </summary>
    [AddComponentMenu(null)]
    public class ChatPrivateChannel : ChatChannel
    {
        /// <summary>
        /// Is private channel encrypted.
        /// </summary>
        public bool Encrypt;

        /// <summary>
        /// Private channel username.
        /// </summary>
        public string Username { get; private set; }

        public override string ChannelName
        {
            get { return Chat.Instance.GetPrivateChannelNameByUser(Username); }
        }

        public override string DisplayName
        {
            get { return Username; }
        }

        /// <summary>
        /// Creates private channel and attaches it to given game object.
        /// </summary>
        public static ChatPrivateChannel Create(string username, GameObject gameObject)
        {
            if (username != Chat.Instance.UserId && !Chat.Instance.ExistsPrivateChannel(username))
            {
                var chatPrivateChannel = gameObject.AddComponent<ChatPrivateChannel>();
                chatPrivateChannel.Username = username;

                return chatPrivateChannel;
            }
            return null;
        }

        protected virtual void Awake()
        {
            Username = null;
        }
    }
}