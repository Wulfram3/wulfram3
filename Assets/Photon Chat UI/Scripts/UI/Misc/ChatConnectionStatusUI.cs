/*
 * Copyright (C) 2015 Exit Games GmbH
 * by The Knights of Unity
 */

using UnityEngine;

namespace PhotonChatUI
{
    public class ChatConnectionStatusUI : MonoBehaviour
    {
        [Tooltip("Animator's boolean parameter which will be set to true if chat is online.")] public string
            IsOnlineParameterName = "IsOnline";

        [Tooltip("If specified component will use this animator instead of the one attached to the game object.")] public Animator OverrideAnimator = null;

        private Animator _animator;

        public Animator animator
        {
            get
            {
                return OverrideAnimator == null
                    ? (_animator ?? (_animator = GetComponent<Animator>()))
                    : OverrideAnimator;
            }
        }

        protected virtual void Update()
        {
            animator.SetBool(IsOnlineParameterName, Chat.Instance.CanChat);
        }
    }
}
