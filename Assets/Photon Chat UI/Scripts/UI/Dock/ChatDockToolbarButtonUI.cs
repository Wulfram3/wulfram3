/*
 * Copyright (C) 2015 Exit Games GmbH
 * by The Knights of Unity
 */

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace PhotonChatUI
{
    public class ChatDockToolbarButtonUI : ChatBehaviourUI, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        private ScrollRect _scrollRect;

        public ScrollRect scrollRect
        {
            get { return _scrollRect ?? (_scrollRect = GetComponentInParent<ScrollRect>()); }
        }

        [HideInInspector] public ChatChannelUI ChannelUI;

        public Text Text;

        public Button Button;

        public Button CloseButton;

        public Color EnabledColor = Color.gray;

        public Color DisabledColor = Color.white;

        public GameObject UnreadBadge;

        public Text UnreadBadgeAmountText;

        public string PublicChannelDisplayFormat = "Public: {0}";

        public string PrivateChannelDisplayFormat = "Private: {0}";

        public bool Draggable = true;

        private bool _isDragged;

        private Vector2 _beginDragMousePosition;

        protected virtual void Start()
        {
            if (Button != null)
            {
                Button.onClick.AddListener(Activate);
            }
            if (CloseButton != null)
                CloseButton.onClick.AddListener(Close);
        }

        protected virtual void Update()
        {
            if (ChannelUI == null)
            {
                Destroy(gameObject);
            }
            else
            {
                if (Text != null)
                {
                    Text.text = ChannelUI.ChatChannel is ChatPrivateChannel
                        ? string.Format(PrivateChannelDisplayFormat, ChannelUI.ChatChannel.DisplayName)
                        : string.Format(PublicChannelDisplayFormat, ChannelUI.ChatChannel.DisplayName);
                    Text.color = ChannelUI.gameObject.activeSelf ? EnabledColor : DisabledColor;
                }

                if (CloseButton)
                {
                    CloseButton.targetGraphic.color = ChannelUI.gameObject.activeSelf ? EnabledColor : DisabledColor;
                }

                if (Button != null)
                {
                    Button.interactable = !ChannelUI.gameObject.activeSelf;
                }

                if (_isDragged)
                {
                    transform.localPosition = GetUIMousePosition();
                }

                if (UnreadBadge != null)
                {
                    if (ChannelUI.UnreadMessages > 0)
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
                    UnreadBadgeAmountText.text = ChannelUI.UnreadMessages > 9
                        ? "9+"
                        : ChannelUI.UnreadMessages.ToString();
            }
        }

        public void Activate()
        {
            ChannelUI.ChatDockUI.Activate(ChannelUI);
        }

        public void Close()
        {
            Destroy(ChannelUI.gameObject);
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (Draggable)
            {
                scrollRect.OnBeginDrag(eventData);
                _beginDragMousePosition = GetUIMousePosition();
            }
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (Draggable)
            {
                if (!_isDragged)
                {
                    scrollRect.OnDrag(eventData);
                    if (Mathf.Abs(GetUIMousePosition().y - _beginDragMousePosition.y) > rectTransform.rect.height*2.0f)
                    {
                        _isDragged = true;
                        transform.SetParent(canvas.transform, true);
                        scrollRect.OnEndDrag(eventData);
                    }
                }
            }
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (_isDragged)
            {
                _isDragged = false;
                var r = new List<RaycastResult>();
                EventSystem.current.RaycastAll(eventData, r);

                ChatDockUI newDock = null;

                for (int i = 0; i < r.Count; i++)
                {
                    var dock = r[i].gameObject.GetComponent<ChatDockUI>();

                    if (dock != null)
                    {
                        newDock = dock;
                        break;
                    }
                }

                if (newDock == null && ChatUI.Instance.FloatingDockPrefab != null)
                {
                    newDock = Instantiate(ChatUI.Instance.FloatingDockPrefab);
                    newDock.transform.SetParent(canvas.transform, false);
                    newDock.rectTransform.localPosition = GetUIMousePosition();
                }

                if (newDock != null)
                {
                    ChannelUI.ChatDockUI.Undock(ChannelUI);
                    newDock.Dock(ChannelUI);
                }
                else
                {
                    transform.SetParent(ChannelUI.ChatDockUI.DockToolbar.transform, true);
                }
            }
            else
            {
                scrollRect.OnEndDrag(eventData);
            }
        }
    }
}