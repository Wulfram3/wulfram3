/*
 * Copyright (C) 2015 Exit Games GmbH
 * by The Knights of Unity
 */

using UnityEngine;
using UnityEngine.EventSystems;

namespace PhotonChatUI
{
    [AddComponentMenu(null)]
    public class ChatPanelResizeHandleUI : ChatBehaviourUI, IBeginDragHandler, IDragHandler
    {
        [HideInInspector] public ChatPanelUI.ResizeAnchor Anchor;

        [HideInInspector] public ChatPanelUI ChatPanelUI;

        private Vector2 ResizeOffset
        {
            get
            {
                Vector2 s = Vector2.zero;

                switch (Anchor)
                {
                    case ChatPanelUI.ResizeAnchor.TopLeft:
                        return new Vector2(ChatPanelUI.rectTransform.offsetMin.x, ChatPanelUI.rectTransform.offsetMax.y);
                    case ChatPanelUI.ResizeAnchor.BottomLeft:
                        return ChatPanelUI.rectTransform.offsetMin;
                    case ChatPanelUI.ResizeAnchor.TopRight:
                        return ChatPanelUI.rectTransform.offsetMax;
                    case ChatPanelUI.ResizeAnchor.BottomRight:
                        return new Vector2(ChatPanelUI.rectTransform.offsetMax.x, ChatPanelUI.rectTransform.offsetMin.y);
                }

                return s;
            }
            set
            {
                var min = ChatPanelUI.rectTransform.offsetMin;
                var max = ChatPanelUI.rectTransform.offsetMax;
                Vector2 v;
                switch (Anchor)
                {
                    case ChatPanelUI.ResizeAnchor.TopLeft:
                        v = ChatPanelUI.ClampRectSizeDelta(new Vector2(min.x - value.x, value.y - max.y), Anchor);
                        min.x -= v.x;
                        max.y += v.y;
                        break;
                    case ChatPanelUI.ResizeAnchor.BottomRight:
                        v = ChatPanelUI.ClampRectSizeDelta(new Vector2(value.x - max.x, min.y - value.y), Anchor);
                        max.x += v.x;
                        min.y -= v.y;
                        break;
                    case ChatPanelUI.ResizeAnchor.TopRight:
                        v = ChatPanelUI.ClampRectSizeDelta(value - max, Anchor);
                        max.x += v.x;
                        max.y += v.y;
                        break;
                    case ChatPanelUI.ResizeAnchor.BottomLeft:
                        v = ChatPanelUI.ClampRectSizeDelta(new Vector2(min.x - value.x, min.y - value.y), Anchor);
                        min.x -= v.x;
                        min.y -= v.y;
                        break;
                }

                ChatPanelUI.rectTransform.offsetMin = min;
                ChatPanelUI.rectTransform.offsetMax = max;
                ChatPanelUI.UpdatePanelRect();
            }
        }

        private Vector2 _dragStartOffset;

        private Vector2 _dragStartPosition;

        public void OnBeginDrag(PointerEventData eventData)
        {
            _dragStartOffset = ResizeOffset;
            _dragStartPosition = GetUIMousePosition();
        }

        public void OnDrag(PointerEventData eventData)
        {
            ResizeOffset = _dragStartOffset - (_dragStartPosition - GetUIMousePosition());
        }

        public bool IsRaycastLocationValid(Vector2 screenPoint, Camera eventCamera)
        {
            return true;
        }
    }
}