/*
 * Copyright (C) 2015 Exit Games GmbH
 * by The Knights of Unity
 */

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace PhotonChatUI
{
    /// <summary>
    /// Base chat message UI. Supports emoticons. Note that emoticons are not case sensitive.
    /// </summary>
    public class ChatChannelMessageUI : ChatBehaviourUI, IPointerClickHandler
    {
        /// <summary>
        /// Emoticon.
        /// </summary>
        [Serializable]
        public class Emoticon
        {
            [Tooltip("Please note that tag isn't case sensitive.")] public string Tag = "";

            public Sprite Sprite;

            public int CharWidth;
        }

        private class GeneratedEmoticon
        {
            public int Index;

            public int Length;

            public Sprite Sprite;

            private GameObject _gameObject;

            public GameObject GameObject
            {
                get { return _gameObject ?? (_gameObject = new GameObject("Emoticon")); }
            }

            private RectTransform _rectTransform;

            public RectTransform RectTransform
            {
                get
                {
                    return _rectTransform ??
                           ((_rectTransform = GameObject.GetComponent<RectTransform>()) ??
                            (_rectTransform = GameObject.AddComponent<RectTransform>()));
                }
            }

            private LayoutElement _layoutElement;

            public LayoutElement LayoutElement
            {
                get
                {
                    return _layoutElement ??
                           ((_layoutElement = GameObject.GetComponent<LayoutElement>()) ??
                            (_layoutElement = GameObject.AddComponent<LayoutElement>()));
                }
            }

            private Image _image;

            public Image Image
            {
                get
                {
                    return _image ??
                           ((_image = GameObject.GetComponent<Image>()) ??
                            (_image = GameObject.AddComponent<Image>()));
                }
            }
        }

        public Emoticon[] Emoticons;

        public Text Text;

        public string MessageFormat = "{0}: {1}";

        public Color ClientSenderColor = Color.white;

        public Color SenderColor = Color.white;

        public Color MessageColor = Color.white;

        public bool OpenPrivateChatWithSenderAfterClick = true;

        public string Sender { get; private set; }

        public object Message { get; private set; }

        private readonly List<GeneratedEmoticon> _generatedEmoticons = new List<GeneratedEmoticon>();

        private Vector2 _generatedSize;

        private readonly string _whiteCharacter = Convert.ToChar(160).ToString();

        private string ParseEmoticons(string str)
        {
            int i = 0;
            while (i < str.Length)
            {
                int c = str.Length;

                Emoticon foundEmoticon = null;

                foreach (var emoticon in Emoticons)
                {
                    int n = str.ToLower().IndexOf(emoticon.Tag.ToLower(), i, StringComparison.Ordinal);

                    if ((n < c || (n == c && foundEmoticon != null && foundEmoticon.Tag.Length < emoticon.Tag.Length)) &&
                        n >= 0)
                    {
                        c = n;
                        foundEmoticon = emoticon;
                    }
                }

                if (foundEmoticon == null)
                    break;

                i = c;

                str = str.Remove(i, foundEmoticon.Tag.Length);

                _generatedEmoticons.Add(new GeneratedEmoticon
                {
                    Index = i,
                    Length = foundEmoticon.CharWidth,
                    Sprite = foundEmoticon.Sprite
                });

                for (int p = 0; p < foundEmoticon.CharWidth; p++)
                {
                    if (p == 0 || p == foundEmoticon.CharWidth - 1)
                        str = str.Insert(i, _whiteCharacter);
                    else
                        str = str.Insert(i, " ");
                    i++;
                }
            }
            return str;
        }

        private void GenerateEmoticons()
        {
            var settings = Text.GetGenerationSettings(Text.rectTransform.rect.size);

            Text.cachedTextGeneratorForLayout.Populate(Text.text, settings);

            foreach (var emoticon in _generatedEmoticons)
            {
                emoticon.GameObject.transform.SetParent(Text.transform, false);
                emoticon.LayoutElement.ignoreLayout = true;
                emoticon.RectTransform.pivot = new Vector2(0.0f, 0.5f);
                emoticon.RectTransform.anchorMax = new Vector2(0.0f, 0.0f);
                emoticon.RectTransform.anchorMin = new Vector2(0.0f, 0.0f);

                var min = (Text.cachedTextGeneratorForLayout.characters[emoticon.Index].cursorPos/Text.pixelsPerUnit);

                float w = 0.0f;

                float y = Text.cachedTextGeneratorForLayout.rectExtents.height -
                          (Text.cachedTextGeneratorForLayout.lines[0].height/2.0f);

                int l = 0;

                for (int i = 0; i < emoticon.Index + 1 && l + 1 < Text.cachedTextGeneratorForLayout.lines.Count; i++)
                {
                    if (Text.cachedTextGeneratorForLayout.lines[l + 1].startCharIdx <= i)
                    {
                        y -= Text.cachedTextGeneratorForLayout.lines[l + 1].height;
                        l++;
                    }
                }

                y /= Text.pixelsPerUnit;

                for (int i = emoticon.Index; i < emoticon.Index + emoticon.Length; i++)
                {
                    w += Text.cachedTextGeneratorForLayout.characters[i].charWidth;
                }

                w /= Text.pixelsPerUnit;

                var pos = new Vector2(min.x, y);


                emoticon.RectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, w);
                emoticon.RectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, w);

                emoticon.RectTransform.anchoredPosition = pos;

                emoticon.Image.preserveAspect = true;
                emoticon.Image.sprite = emoticon.Sprite;
            }
        }

        public virtual void SetData(string sender, object message, bool hideSender)
        {
            Sender = sender;
            Message = message;

            string senderColorHex = string.Format("#{0:X2}{1:X2}{2:X2}{3:X2}",
                (int) (SenderColor.r*255),
                (int) (SenderColor.g*255),
                (int) (SenderColor.b*255),
                (int) (SenderColor.a*255));

            string messageColorHex = string.Format("#{0:X2}{1:X2}{2:X2}{3:X2}",
                (int) (MessageColor.r*255),
                (int) (MessageColor.g*255),
                (int) (MessageColor.b*255),
                (int) (MessageColor.a*255));

            string senderText = hideSender ? "" : "<color=" + senderColorHex + "><b>" + sender + "</b></color>";
            string messageText = "<color=" + messageColorHex + ">" + message + "</color>";

            Text.text = ParseEmoticons(hideSender ? messageText : string.Format(MessageFormat, senderText, messageText));
            Text.rectTransform.pivot = Vector2.zero;
        }

        protected virtual void Update()
        {
            if (_generatedSize != Text.rectTransform.rect.size)
            {
                _generatedSize = Text.rectTransform.rect.size;
                GenerateEmoticons();
            }
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (OpenPrivateChatWithSenderAfterClick)
                ChatUI.Instance.CreatePrivateChannel(Sender);
        }
    }
}