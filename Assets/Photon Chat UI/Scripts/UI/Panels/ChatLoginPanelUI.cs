/*
 * Copyright (C) 2015 Exit Games GmbH
 * by The Knights of Unity
 */

using UnityEngine;

namespace PhotonChatUI
{
    public class ChatLoginPanelUI : ChatPanelUI
    {
        [Tooltip(
            "Animator's float parameter which will be set to 1.0 if login result is successful, otherwise 0.0. Insert empty text to disable this feature."
            )] public string LoginResultParameterName = "LoginResult";

        [Tooltip(
            "Animator's boolean parameter which is set to true if login is processing. Insert empty text to disable this feature."
            )] public string IsLoginProcessingParameterName = "IsLoginProcessing";

        public override void Update()
        {
            if (!Chat.Instance.CanChat)
            {
                Open();
            }
            else
            {
                Close();
            }

            if (!string.IsNullOrEmpty(LoginResultParameterName))
                animator.SetFloat(LoginResultParameterName, Chat.Instance.CanChat ? 1.0f : 0.0f);

            if (!string.IsNullOrEmpty(IsLoginProcessingParameterName))
                animator.SetBool(IsLoginProcessingParameterName, Chat.Instance.IsConnecting());
        }
    }
}