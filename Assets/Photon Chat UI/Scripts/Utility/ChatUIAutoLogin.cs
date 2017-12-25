/*
 * Copyright (C) 2015 Exit Games GmbH
 * by The Knights of Unity
 */

using Photon;

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

                chatUI.Connect(PhotonNetwork.playerName);
                ischatConnected = true;
            }
        }
    }
}
