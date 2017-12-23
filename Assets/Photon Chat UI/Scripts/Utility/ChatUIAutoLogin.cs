/*
 * Copyright (C) 2015 Exit Games GmbH
 * by The Knights of Unity
 */

using Photon;
using UnityEngine;
using System.Collections;
using Com.Wulfram3;
namespace PhotonChatUI
{
    public class ChatUIAutoLogin : PunBehaviour
    {
        private ChatUI _chatUI;

        public ChatUI chatUI
        {
            get { return _chatUI ?? (_chatUI = GetComponent<ChatUI>()); }
        }

        void Start()
        {
            base.OnJoinedRoom();
            chatUI.Connect(PhotonNetwork.playerName);
            
        }
    }
}
