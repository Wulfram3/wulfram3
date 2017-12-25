/*
 * Copyright (C) 2015 Exit Games GmbH
 * by The Knights of Unity
 */

using System;
using System.Collections.Generic;
using System.Linq;
using ExitGames.Client.Photon;
using ExitGames.Client.Photon.Chat;
using UnityEngine;

namespace PhotonChatUI
{
    /// <summary>
    /// Chat component. It must be attached to chat root game object.
    /// </summary>
    [AddComponentMenu(null)]
    public class Chat : MonoBehaviour, IChatClientListener
    {
        private static Chat _instance;

        public static Chat Instance
        {
            get
            {
                return _instance == null || _instance.gameObject == null
                    ? (_instance = FindObjectOfType<Chat>())
                    : _instance;
            }
        }

        /// <summary>
        /// User's friend.
        /// </summary>
        public class Friend
        {
            public Friend(string name)
            {
                Name = name;
            }

            /// <summary>
            /// Friend's name.
            /// </summary>
            public readonly string Name;

            /// <summary>
            /// Friend's status.
            /// </summary>
            public int Status;

            /// <summary>
            /// Friend's status message.
            /// </summary>
            public object Message;
        }

        private ChatUI _chatUI;

        public ChatUI ChatUI
        {
            get { return _chatUI ?? (_chatUI = GetComponent<ChatUI>()); }
        }

        [Tooltip("Used for initialization in Start.")] public ConnectionProtocol ConnectionProtocol;

        [Tooltip(
            "Used for initialization in Start. The AppID as assigned from the Photon Cloud. If you host yourself, this is the \"regular\" Photon Server Application Name (most likely: \"LoadBalancing\")."
            )] public string AppId = "xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx";

        [Tooltip(
            "Used for initialization in Start. The version of your client. A new version also creates a new \"virtual app\" to separate players from older client versions."
            )] public string AppVersion = "1.0";

        [Tooltip(
            "Used for initialization in Start. Region used to connect to. Currently all chat is done in EU. It can make sense to use only one region for the whole game."
            )] public string ChatRegion = "EU";

        public DebugLevel DebugOut
        {
            get { return _chatClient.DebugOut; }
            set { _chatClient.DebugOut = value; }
        }

        public ChatDisconnectCause DisconnectedCause
        {
            get { return _chatClient.DisconnectedCause; }
        }

        public string FrontendAddress
        {
            get { return _chatClient.FrontendAddress; }
        }

        public string NameServerAddress
        {
            get { return _chatClient.NameServerAddress; }
        }

        public ChatState State
        {
            get { return _chatClient.State; }
        }

        public string UserId
        {
            get { return _chatClient.UserId; }
        }

        public bool CanChat
        {
            get { return _chatClient.CanChat; }
        }

        /// <summary>
        /// Current user status.
        /// </summary>
        public int UserStatus
        {
            get { return _userStatus; }
            set
            {
                _userStatus = value;
                _userStatusDirty = true;
            }
        }

        /// <summary>
        /// Current user status message.
        /// </summary>
        public object UserStatusMessage
        {
            get { return _userStatusMessage; }
            set
            {
                _userStatusMessage = value;
                _userStatusDirty = true;
            }
        }

        private int _userStatus;

        private object _userStatusMessage;

        private bool _userStatusDirty;

        private ChatClient _chatClient;

        private readonly List<ChatChannel> _chatChannels = new List<ChatChannel>();

        private readonly List<Friend> _friends = new List<Friend>();

        /// <summary>
        /// Connects chat client to the Photon Chat Cloud service, which will also authenticate the user (and set a UserId).
        /// </summary>
        /// <param name="authenticationValues">Values for authentication. You can leave this null, if you set a UserId before. If you set authValues, they will override any UserId set before.</param>
        /// <returns><c>true</c> if operations has succeed, otherwise <c>false</c>.</returns>
        public bool Connect(ExitGames.Client.Photon.Chat.AuthenticationValues authenticationValues = null)
        {
            if (string.IsNullOrEmpty(AppId))
            {
                Debug.LogError("Missing App Id! Please insert your App Id in Chat component to configure it properly.",
                    this);

                return false;
            }

            return _chatClient.Connect(AppId, AppVersion, authenticationValues);
        }

        /// <summary>
        /// Disconnects chat client from server.
        /// </summary>
        public void Disconnect()
        {
            _chatClient.Disconnect();
        }

        /// <summary>
        /// Is client connecting.
        /// </summary>
        /// <returns><c>true</c> if client is connecting, otherwise <c>false</c>.</returns>
        public bool IsConnecting()
        {
            return State == ChatState.Authenticating ||
                   State == ChatState.ConnectingToFrontEnd ||
                   State == ChatState.ConnectedToNameServer ||
                   State == ChatState.ConnectingToNameServer;
        }

        /// <summary>
        /// Sends a request to subscribe a chat channel.
        /// </summary>
        /// <param name="channel">Channel to subscribe.</param>
        /// <param name="messagesFromHistory">Amount of previous messages to fetch while subscribing.</param>
        /// <returns><c>true</c> if request has been sent, otherwise <c>false</c>.</returns>
        public bool SubscribeChannel(ChatChannel channel, int messagesFromHistory)
        {
            if (_chatClient.CanChat && !IsChannelSubscribed(channel) &&
                _chatClient.Subscribe(new[] {channel.ChannelName}, messagesFromHistory))
            {
                _chatChannels.Add(channel);

                return true;
            }

            return false;
        }

        /// <summary>
        /// Sends a request to unsubscribe a chat channel.
        /// </summary>
        /// <param name="channel">Channel to unsubscribe.</param>
        /// <returns><c>true</c> if request has been sent, otherwise <c>false</c>.</returns>
        public bool UnsubscribeChannel(ChatChannel channel)
        {
            if (_chatClient.CanChat && _chatClient.Unsubscribe(new[] {channel.ChannelName}))
            {
                return true;
            }

            return false;
        }

        public ChatChannel FindChannel(Predicate<ChatChannel> predicate)
        {
            for (int i = 0; i < _chatChannels.Count; i++)
            {
                if (_chatChannels[i] != null && predicate(_chatChannels[i]))
                {
                    return _chatChannels[i];
                }
            }

            return null;
        }

        public ChatPublicChannel FindPublicChannel(string channelName)
        {
            return (ChatPublicChannel) FindChannel(x => x is ChatPublicChannel && x.ChannelName == channelName);
        }

        public ChatPrivateChannel FindPrivateChannel(string username)
        {
            return
                (ChatPrivateChannel)
                    FindChannel(x => x is ChatPrivateChannel && ((ChatPrivateChannel) x).Username == username);
        }

        public bool ExistsPublicChannel(string channelName)
        {
            return FindPublicChannel(channelName) != null;
        }

        public bool ExistsPrivateChannel(string username)
        {
            return FindPrivateChannel(username) != null;
        }

        /// <summary>
        /// Checks if channel is subscribed.
        /// </summary>
        /// <param name="channel"></param>
        /// <returns><c>true</c> if channel is subscribed, otherwise <c>false</c>.</returns>
        public bool IsChannelSubscribed(ChatChannel channel)
        {
            if (channel == null)
                return false;

            if (!CanChat)
                return false;

            if (!_chatChannels.Contains(channel))
                return false;

            return true;
        }

        /// <summary>
        /// Adds friend.
        /// </summary>
        /// <param name="friend">Friend's name.</param>
        /// <returns><c>true</c> if user has been added, otherwise <c>false</c>.</returns>
        public bool AddFriend(string friend)
        {
            if (!IsFriend(friend))
            {
                if (_chatClient.AddFriends(new[] {friend}))
                {
                    _friends.Add(new Friend(friend));
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Removes friend.
        /// </summary>
        /// <param name="friend">Friend's name.</param>
        /// <returns><c>true</c> if user has been removed, otherwise <c>false</c>.</returns>
        public bool RemoveFriend(string friend)
        {
            if (IsFriend(friend))
            {
                if (_chatClient.RemoveFriends(new[] {friend}))
                {
                    _friends.RemoveAll(x => x.Name == friend);
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Check friend is added.
        /// </summary>
        /// <param name="friend">Friend's name.</param>
        /// <returns><c>true</c> if friend is added, otherwise <c>false</c>.</returns>
        public bool IsFriend(string friend)
        {
            return _friends.Any(x => x.Name == friend);
        }

        /// <summary>
        /// Get you the (locally used) channel name for the chat between this client and another user.
        /// </summary>
        /// <param name="userName">Remote user's name or UserId.</param>
        /// <returns>The (locally used) channel name for a private channel.</returns>
        public string GetPrivateChannelNameByUser(string userName)
        {
            return _chatClient.GetPrivateChannelNameByUser(userName);
        }

        /// <summary>
        /// Publishes message for <paramref name="channel"/>.
        /// </summary>
        /// <param name="channel">Channel.</param>
        /// <param name="message">Message to be published.</param>
        public void PublishMessage(ChatChannel channel, object message)
        {
            if (IsChannelSubscribed(channel))
            {
                if (channel is ChatPublicChannel)
                {
                    _chatClient.PublishMessage(channel.ChannelName, message);
                }
                else
                {
                    var privateChannel = channel as ChatPrivateChannel;

                    if (privateChannel != null)
                        _chatClient.SendPrivateMessage(privateChannel.Username, message, privateChannel.Encrypt);
                }
            }
        }

        private void Reset()
        {
            _userStatus = 0;
            _userStatusMessage = null;
            _userStatusDirty = false;
            for (int i = 0; i < _chatChannels.Count; i++)
            {
                if (_chatChannels[i] != null && _chatChannels[i].gameObject != null)
                {
                    Destroy(_chatChannels[i].gameObject);
                }
            }
            _chatChannels.Clear();
            _friends.Clear();
        }

        protected void Awake()
        {
            _instance = this;
        }

        protected void Start()
        {
            _chatClient = new ChatClient(this, ConnectionProtocol);
            DebugOut = DebugLevel.WARNING;
        }

        protected void Update()
        {
            if (_chatClient.State != ChatState.Uninitialized)
                _chatClient.Service();

            if (_chatClient.CanChat)
            {
                if (_userStatusDirty)
                {
                    _userStatusDirty = !_chatClient.SetOnlineStatus(_userStatus, _userStatusMessage);
                }
            }

            for (int i = 0; i < _chatChannels.Count; i++)
            {
                if (_chatChannels[i] == null || _chatChannels[i].gameObject == null)
                {
                    _chatChannels.RemoveAt(i);
                    i--;
                }
            }
        }

        public void OnApplicationQuit()
        {
            if (_chatClient != null)
            {
                _chatClient.Disconnect();
            }
        }

        public void OnDestroy()
        {
            if (_chatClient != null)
            {
                _chatClient.Disconnect();
            }
        }

        public void DebugReturn(DebugLevel level, string message)
        {
            switch (level)
            {
                case DebugLevel.WARNING:
                {
                    Debug.LogWarning(message);
                    break;
                }
                case DebugLevel.ERROR:
                {
                    Debug.LogError(message);
                    break;
                }
                default:
                {
                    Debug.Log(message);
                    break;
                }
            }
            ChatUI.DebugReturn(level, message);
        }

        public void OnDisconnected()
        {
            Reset();
            ChatUI.OnDisconnected();
        }

        public void OnConnected()
        {
            Reset();
            ChatUI.OnConnected();
        }

        public void OnChatStateChange(ChatState state)
        {
            ChatUI.OnChatStateChange(state);
        }

        public void OnGetMessages(string channelName, string[] senders, object[] messages)
        {
            for (var i = 0; i < _chatChannels.Count; i++)
            {
                if (_chatChannels[i] != null && _chatChannels[i].ChannelName == channelName)
                {
                    _chatChannels[i].OnUpdateMessages(senders, messages);

                    break;
                }
            }
            ChatUI.OnGetMessages(channelName, senders, messages);
        }

        public void OnPrivateMessage(string sender, object message, string channelName)
        {
            for (var i = 0; i < _chatChannels.Count; i++)
            {
                if (_chatChannels[i] != null && _chatChannels[i].ChannelName == channelName)
                {
                    _chatChannels[i].OnUpdateMessages(new[] {sender}, new[] {message});

                    break;
                }
            }

            ChatUI.OnPrivateMessage(sender, message, channelName);
        }

        public void OnSubscribed(string[] channels, bool[] results)
        {
            for (int i = 0; i < channels.Length; i++)
            {
                for (int j = 0; j < _chatChannels.Count; j++)
                {
                    if (_chatChannels[j] != null && _chatChannels[j].gameObject != null &&
                        _chatChannels[j].ChannelName == channels[i])
                    {
                        if (results[i])
                        {
                            _chatChannels[j].OnSubscribed();

                            if (_chatChannels[j] is ChatPrivateChannel)
                            {
                                ExitGames.Client.Photon.Chat.ChatChannel pChannel;

                                if (_chatClient.TryGetChannel(_chatChannels[j].ChannelName, true, out pChannel))
                                {
                                    _chatChannels[j].OnUpdateMessages(pChannel.Senders.ToArray(),
                                        pChannel.Messages.ToArray());
                                }
                            }
                        }
                        else
                        {
                            _chatChannels[j].OnUnsubscribed();
                            Destroy(_chatChannels[i].gameObject);
                            _chatChannels.RemoveAt(j);
                            j--;
                        }
                    }
                }
            }

            ChatUI.OnSubscribed(channels, results);
        }

        public void OnUnsubscribed(string[] channels)
        {
            for (int i = 0; i < channels.Length; i++)
            {
                for (int j = 0; j < _chatChannels.Count; j++)
                {
                    if (_chatChannels[j] != null && _chatChannels[j].gameObject != null &&
                        _chatChannels[j].ChannelName == channels[i])
                    {
                        _chatChannels[j].OnUnsubscribed();
                        Destroy(_chatChannels[i].gameObject);
                        _chatChannels.RemoveAt(j);
                        j--;
                    }
                }
            }

            ChatUI.OnUnsubscribed(channels);
        }

        public void OnStatusUpdate(string user, int status, bool gotMessage, object message)
        {
            for (int i = 0; i < _friends.Count; i++)
            {
                if (_friends[i].Name == user)
                {
                    _friends[i].Status = status;

                    if (gotMessage)
                        _friends[i].Message = message;
                }
            }
            ChatUI.OnStatusUpdate(user, status, gotMessage, message);
        }

        ///<summary>Checks if a string represents a System.GUID. Those are used as App Ids for Photon Cloud Apps.</summary>
        public static bool IsGuid(string possibleGuid)
        {
            if (string.IsNullOrEmpty(possibleGuid))
            {
                return false;
            }

            try
            {
                Guid gid = new Guid(possibleGuid);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
