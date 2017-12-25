/*
 * Copyright (C) 2015 Exit Games GmbH
 * by The Knights of Unity
 */

using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace PhotonChatUI
{
    public class ChatDockUI : ChatBehaviourUI
    {
        public Text TitleText;

        public InputField MessageInputField;

        public Transform ChannelsContainer;

        public Transform DockToolbar;

        public ChatDockToolbarButtonUI DockToolbarButtonPrefab;

        public GameObject UnreadBadge;

        public Text UnreadBadgeAmountText;

        public ChatChannelUI ActiveChannel { get; private set; }

        public string DisplayName { get; private set; }

        private bool IsToolbarAvailable
        {
            get { return DockToolbar != null && DockToolbarButtonPrefab != null; }
        }

        protected readonly List<ChatChannelUI> Channels = new List<ChatChannelUI>();

        private readonly List<ChatDockToolbarButtonUI> _channelToolbarButtons = new List<ChatDockToolbarButtonUI>();

        private void RefreshDisplayName()
        {
            StringBuilder b = new StringBuilder();
            for (int i = 0; i < Channels.Count; i++)
            {
                if (i > 0)
                    b.Append(", ");
                b.Append(Channels[i].ChatChannel.DisplayName);
            }
            DisplayName = b.ToString();
        }

        private void CleanupMissing()
        {
            for (var i = 0; i < Channels.Count; i++)
            {
                if (!IsDocked(Channels[i]))
                {
                    Channels.RemoveAt(i);
                    i--;
                }
            }

            for (var i = 0; i < _channelToolbarButtons.Count; i++)
            {
                bool remove = false;

                if (_channelToolbarButtons[i] == null || _channelToolbarButtons[i].gameObject == null)
                {
                    remove = true;
                }
                else if (!IsDocked(_channelToolbarButtons[i].ChannelUI))
                {
                    DestroyImmediate(_channelToolbarButtons[i].gameObject);
                    remove = true;
                }

                if (remove)
                {
                    _channelToolbarButtons.RemoveAt(i);
                    i--;
                }
            }

            RefreshDisplayName();
        }

        private void ActivateFirstAvailable()
        {
            CleanupMissing();

            if (Channels.Count > 0)
                Activate(Channels[0]);
        }

        private void ActivateIndexDiff(int n)
        {
            if (Channels.Count > 0)
            {
                if (ActiveChannel == null || !IsDocked(ActiveChannel))
                {
                    ActivateFirstAvailable();
                }
                else
                {
                    for (int i = 0; i < Channels.Count; i++)
                    {
                        if (Channels[i] == ActiveChannel)
                        {
                            Activate(Channels[(i + n)%Channels.Count]);
                            return;
                        }
                    }
                    ActivateFirstAvailable();
                }
            }
        }

        public void Activate(ChatChannelUI channelUI)
        {
            ActiveChannel = channelUI;
        }

        public void ActivateNext()
        {
            ActivateIndexDiff(1);
        }

        public void ActivatePrevious()
        {
            ActivateIndexDiff(-1);
        }

        public void Dock(ChatChannelUI channelUI, bool focus = true)
        {
            if (!Channels.Contains(channelUI))
            {
                Channels.Add(channelUI);
            }

            if (IsToolbarAvailable)
            {
                if (!_channelToolbarButtons.Exists(x => x.ChannelUI == channelUI))
                {
                    var toolbarButton = Instantiate(DockToolbarButtonPrefab);
                    toolbarButton.ChannelUI = channelUI;
                    toolbarButton.transform.SetParent(DockToolbar.transform, false);
                    toolbarButton.transform.SetAsFirstSibling();
                    _channelToolbarButtons.Add(toolbarButton);

                }
            }

            channelUI.transform.SetParent(ChannelsContainer, false);

            if (focus)
                Activate(channelUI);

            RefreshDisplayName();
        }

        public void Undock(ChatChannelUI channelUI)
        {
            Channels.Remove(channelUI);
            channelUI.transform.SetParent(null, false);

            CleanupMissing();
        }

        public bool IsDocked(ChatChannelUI channelUI)
        {
            return channelUI != null && channelUI.gameObject != null && Channels.Contains(channelUI) &&
                   channelUI.transform.parent == ChannelsContainer;
        }

        public void PublishMessage()
        {
            PublishMessage(MessageInputField);
        }

        public void PublishMessage(InputField message)
        {
            PublishMessage(message.text);
            message.text = string.Empty;
        }

        public void PublishMessage(string message)
        {
            if (Chat.Instance.CanChat && ActiveChannel != null && !string.IsNullOrEmpty(message))
            {
                Chat.Instance.PublishMessage(ActiveChannel.ChatChannel, message);
            }
        }

        public void ClearMessages()
        {
            if (ActiveChannel != null)
                ActiveChannel.ClearMessages();
        }

        protected virtual void Update()
        {
            CleanupMissing();

            if (ActiveChannel == null || !Channels.Contains(ActiveChannel))
            {
                ActivateFirstAvailable();
            }

            int unreadTotal = 0;

            for (var i = 0; i < Channels.Count; i++)
            {
                if (ActiveChannel != Channels[i])
                {
                    if (Channels[i].gameObject.activeSelf)
                    {
                        Channels[i].gameObject.SetActive(false);
                    }
                }
                else if (!Channels[i].gameObject.activeSelf)
                {
                    Channels[i].gameObject.SetActive(true);
                }

                unreadTotal += Channels[i].UnreadMessages;
            }

            if (Chat.Instance.CanChat && ActiveChannel != null)
            {
                MessageInputField.interactable = true;

                if (Input.GetKeyDown(KeyCode.Return))
                {
                    if (EventSystem.current.currentSelectedGameObject == MessageInputField.gameObject)
                    {
                        PublishMessage();
                        MessageInputField.ActivateInputField();
                    }
                }

                MessageInputField.text = MessageInputField.text.Replace("\n", "");
            }
            else
            {
                MessageInputField.interactable = false;
            }

            if (UnreadBadge != null)
            {
                if (unreadTotal > 0)
                {
                    if (!UnreadBadge.activeSelf)
                        UnreadBadge.SetActive(true);
                }
                else
                {
                    if (UnreadBadge.activeSelf)
                        UnreadBadge.SetActive(false);
                }
            }

            if (UnreadBadgeAmountText != null)
                UnreadBadgeAmountText.text = unreadTotal > 9 ? "9+" : unreadTotal.ToString();

            if (TitleText != null)
            {
                TitleText.text = DisplayName;
            }
        }

        protected virtual void OnDisable()
        {
            if (this != ChatUI.Instance.MainDock)
            {
                for (int i = 0; i < Channels.Count; i++)
                {
                    ChatUI.Instance.MainDock.Dock(Channels[i]);
                }
            }
        }
    }
}