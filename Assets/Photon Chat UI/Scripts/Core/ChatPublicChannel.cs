/*
 * Copyright (C) 2015 Exit Games GmbH
 * by The Knights of Unity
 */

using UnityEngine;

namespace PhotonChatUI
{
    /// <summary>
    /// Public chat channel.
    /// </summary>
    [AddComponentMenu(null)]
    public class ChatPublicChannel : ChatChannel
    {
        private string _channelName;

        public override string ChannelName
        {
            get { return _channelName; }
        }

        public override string DisplayName
        {
            get { return _channelName; }
        }

        /// <summary>
        /// Creates public channel and attaches it to given game object.
        /// </summary>
        public static ChatPublicChannel Create(string channelName, GameObject gameObject)
        {
            if (!Chat.Instance.ExistsPublicChannel(channelName))
            {
                var chatPublicChannel = gameObject.AddComponent<ChatPublicChannel>();
                chatPublicChannel._channelName = channelName;

                return chatPublicChannel;
            }
            return null;
        }
    }
}