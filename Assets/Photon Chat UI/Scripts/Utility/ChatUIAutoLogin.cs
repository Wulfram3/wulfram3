/*
 * Copyright (C) 2015 Exit Games GmbH
 * by The Knights of Unity
 */

using Photon;
using UnityEngine;

namespace PhotonChatUI
{
    public class ChatUIAutoLogin : PunBehaviour
    {
        bool ischatConnected = false;
        private ChatUI _chatUI;

        public ChatUI chatUI
        {
            get { return _chatUI ?? (_chatUI = GetComponent<ChatUI>()); }
        }

        void Update()
        {
      
            if (ischatConnected == false)
            {
                base.OnJoinedRoom();
                Debug.Log("Joined Room CHAT");
                chatUI.Connect(PhotonNetwork.playerName);
                    Debug.Log("Chat Player Connected");
                ischatConnected = true;
            }
        }
    }
}
