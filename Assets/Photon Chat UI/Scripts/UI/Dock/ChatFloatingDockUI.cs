/*
 * Copyright (C) 2015 Exit Games GmbH
 * by The Knights of Unity
 */

using UnityEngine;

namespace PhotonChatUI
{
    [RequireComponent(typeof (ChatPanelUI))]
    public class ChatFloatingDockUI : ChatDockUI
    {
        protected override void Update()
        {
            base.Update();

            chatPanelUI.RectConstrain = parentRectTransform;

            if (Channels.Count <= 0)
            {
                Destroy(gameObject);
            }
        }

        public void Close()
        {
            Destroy(gameObject);
        }
    }
}