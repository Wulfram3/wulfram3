/*
 * Copyright (C) 2015 Exit Games GmbH
 * by The Knights of Unity
 */

using UnityEngine;

namespace PhotonChatUI
{
    /// <summary>
    /// Base chat channel.
    /// </summary>
    [AddComponentMenu(null)]
    public abstract class ChatChannel : MonoBehaviour
    {
        private ChatChannelUI _chatChannelUI;

        public ChatChannelUI ChatChannelUI
        {
            get { return _chatChannelUI ?? (_chatChannelUI = GetComponent<ChatChannelUI>()); }
        }

        /// <summary>
        /// Channel name.
        /// </summary>
        public abstract string ChannelName { get; }

        /// <summary>
        /// Display channel name.
        /// </summary>
        public abstract string DisplayName { get; }

        protected virtual void Update()
        {
            name = DisplayName;
        }

        protected virtual void OnDestroy()
        {
            Chat.Instance.UnsubscribeChannel(this);
        }

        public void OnUpdateMessages(string[] senders, object[] messages)
        {
            ChatChannelUI.OnUpdateMessages(messages, senders);
        }

        public void OnSubscribed()
        {
            ChatChannelUI.OnSubscribed();
        }

        public void OnUnsubscribed()
        {
            ChatChannelUI.OnUnsubscribed();
        }
    }
}