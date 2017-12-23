/*
 * Copyright (C) 2015 Exit Games GmbH
 * by The Knights of Unity
 */

using UnityEngine;
using UnityEngine.UI;

namespace PhotonChatUI
{
    public class ChatEmoticonsSelectorUI : MonoBehaviour
    {
        public ChatDockUI Dock;

        private void Spawn()
        {
            // Clear
            while (transform.childCount > 0)
            {
                DestroyImmediate(transform.GetChild(0).gameObject);
            }

            foreach (var emoticon in ChatUI.Instance.ChatChannelPrefab.MessagePrefab.Emoticons)
            {
                var cacheEmoticon = emoticon;
                var go = new GameObject(emoticon.Tag);
                go.AddComponent<RectTransform>();
                go.AddComponent<Image>().sprite = emoticon.Sprite;
                go.AddComponent<Button>()
                    .onClick.AddListener(
                        () => { if (Chat.Instance.CanChat) Dock.MessageInputField.text += cacheEmoticon.Tag; });
                go.transform.SetParent(transform, false);
            }
        }

        protected void Start()
        {
            Spawn();
        }
    }
}
