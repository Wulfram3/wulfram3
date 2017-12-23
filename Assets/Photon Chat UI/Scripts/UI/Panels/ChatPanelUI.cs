/*
 * Copyright (C) 2015 Exit Games GmbH
 * by The Knights of Unity
 */

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace PhotonChatUI
{
    /// <summary>
    /// Panel that can be opened, closed or resized.
    /// </summary>
    [RequireComponent(typeof (Image))]
    public class ChatPanelUI : ChatBehaviourUI, IDragHandler, IBeginDragHandler, IPointerClickHandler
    {
        [Flags]
        public enum ResizeAnchor
        {
            TopLeft = (1 << 0),
            TopRight = (1 << 1),
            BottomLeft = (1 << 2),
            BottomRight = (1 << 3),
        }

        [Tooltip("Enables panel resizing.")] public bool IsResizable = false;

        [Tooltip("Anchors of resize handle.")] public ResizeAnchor ResizeHandleAnchors;

        [Tooltip("Resize handle size.")] public Vector2 ResizeHandleSize;

        [Tooltip("Enables panel dragging.")] public bool IsDraggable = false;

        [Tooltip("The maximum rect area which should be respected by resizing and dragging operations.")] public
            RectTransform RectConstrain;

        [Tooltip("The maximum size of rect. Set \"0\" to disable.")] public Vector2 RectMaxSize = Vector2.zero;

        [Tooltip("The minimum size of rect. Set \"0\" to disable.")] public Vector2 RectMinSize = Vector2.zero;

        [Tooltip("If specified panel will use this animator instead of the one attached to the panel game object.")] public Animator OverrideAnimator = null;

        [Tooltip(
            "By using animator you are able to create fade in and fade out animations for the panel. Otherwise panel will be simply enabled or disabled."
            )] public bool UseAnimator = false;

        [Tooltip(
            "Animator's boolean parameter which will be set to true if panel is opened. Insert empty text to disable this feature."
            )] public string IsOpenedParameterName = "IsOpened";

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

        private readonly Dictionary<ResizeAnchor, ChatPanelResizeHandleUI> _resizeHandles =
            new Dictionary<ResizeAnchor, ChatPanelResizeHandleUI>();

        private void UpdateResizeHandle(ResizeAnchor resizeAnchor)
        {
            ChatPanelResizeHandleUI resizeHandle = null;

            if (_resizeHandles.ContainsKey(resizeAnchor))
            {
                resizeHandle = _resizeHandles[resizeAnchor];
            }

            if ((ResizeHandleAnchors & resizeAnchor) == resizeAnchor)
            {
                if (resizeHandle == null && IsResizable)
                {
                    var go = new GameObject("Panel Resize Handle");
                    go.AddComponent<RectTransform>();
                    go.AddComponent<Image>().color = Color.clear;
                    resizeHandle = go.AddComponent<ChatPanelResizeHandleUI>();
                    resizeHandle.transform.SetParent(rectTransform.transform, false);
                    _resizeHandles.Add(resizeAnchor, resizeHandle);
                }

                if (resizeHandle != null)
                {
                    if (IsResizable)
                    {
                        resizeHandle.enabled = true;

                        Vector2 pivot = Vector2.zero;

                        if (ResizeAnchor.BottomLeft == resizeAnchor)
                            pivot = new Vector2(0.0f, 0.0f);
                        else if (ResizeAnchor.TopLeft == resizeAnchor)
                            pivot = new Vector2(0.0f, 1.0f);
                        else if (ResizeAnchor.BottomRight == resizeAnchor)
                            pivot = new Vector2(1.0f, 0.0f);
                        else if (ResizeAnchor.TopRight == resizeAnchor)
                            pivot = new Vector2(1.0f, 1.0f);

                        resizeHandle.rectTransform.anchorMin =
                            resizeHandle.rectTransform.anchorMax = resizeHandle.rectTransform.pivot = pivot;

                        resizeHandle.rectTransform.anchoredPosition = Vector2.zero;
                        resizeHandle.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal,
                            ResizeHandleSize.x);
                        resizeHandle.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical,
                            ResizeHandleSize.y);

                        resizeHandle.Anchor = resizeAnchor;

                        resizeHandle.transform.SetAsLastSibling();
                        resizeHandle.ChatPanelUI = this;
                    }
                    else
                    {
                        resizeHandle.enabled = false;
                    }
                }
            }
            else if (resizeHandle != null)
            {
                resizeHandle.enabled = false;
            }
        }

        private void UpdateResizeHandles()
        {
            UpdateResizeHandle(ResizeAnchor.BottomLeft);
            UpdateResizeHandle(ResizeAnchor.BottomRight);
            UpdateResizeHandle(ResizeAnchor.TopLeft);
            UpdateResizeHandle(ResizeAnchor.TopRight);
        }

        private Rect GetRectConstrainRect()
        {
            var rectConstrainRect = RectConstrain.rect;

            rectConstrainRect.min =
                rectTransform.InverseTransformPoint(RectConstrain.TransformPoint(rectConstrainRect.min));
            rectConstrainRect.max =
                rectTransform.InverseTransformPoint(RectConstrain.TransformPoint(rectConstrainRect.max));

            return rectConstrainRect;
        }

        private Vector2 ClampRectSize(Vector2 rectSize)
        {
            if (RectMinSize.x > 0.0f)
                rectSize.x = Mathf.Max(RectMinSize.x, rectSize.x);
            if (RectMinSize.y > 0.0f)
                rectSize.y = Mathf.Max(RectMinSize.y, rectSize.y);
            if (RectMaxSize.x > 0.0f)
                rectSize.x = Mathf.Min(RectMaxSize.x, rectSize.x);
            if (RectMaxSize.y > 0.0f)
                rectSize.y = Mathf.Min(RectMaxSize.y, rectSize.y);

            if (RectConstrain != null)
            {
                var rectConstrainRect = GetRectConstrainRect();

                rectSize.x = Mathf.Min(rectSize.x, rectConstrainRect.width);
                rectSize.y = Mathf.Min(rectSize.y, rectConstrainRect.height);
            }

            return rectSize;
        }

        public Vector2 ClampRectSizeDelta(Vector2 rectSizeDelta, ResizeAnchor anchor)
        {
            var rect = rectTransform.rect;

            if (RectConstrain != null)
            {
                var rectConstrainRect = GetRectConstrainRect();

                if (anchor == ResizeAnchor.BottomLeft || anchor == ResizeAnchor.TopLeft)
                {
                    rectSizeDelta.x = Mathf.Min(rectSizeDelta.x, rect.xMin - rectConstrainRect.xMin);
                }
                else if (anchor == ResizeAnchor.BottomRight || anchor == ResizeAnchor.TopRight)
                {
                    rectSizeDelta.x = Mathf.Min(rectSizeDelta.x, rectConstrainRect.xMax - rect.xMax);
                }

                if (anchor == ResizeAnchor.BottomLeft || anchor == ResizeAnchor.BottomRight)
                {
                    rectSizeDelta.y = Mathf.Min(rectSizeDelta.y, rect.yMin - rectConstrainRect.yMin);
                }
                else if (anchor == ResizeAnchor.TopLeft || anchor == ResizeAnchor.TopRight)
                {
                    rectSizeDelta.y = Mathf.Min(rectSizeDelta.y, rectConstrainRect.yMax - rect.yMax);
                }
            }

            return ClampRectSize(rectSizeDelta + rect.size) - rect.size;
        }

        public void UpdatePanelRect()
        {
            var rect = rectTransform.rect;

            var originalRect = rect;

            rect.size = ClampRectSize(rect.size);

            if (RectConstrain != null)
            {
                var rectConstrainRect = GetRectConstrainRect();

                Vector2 translation = Vector2.zero;

                if (rectConstrainRect.width > rect.width)
                {
                    if (rect.xMax > rectConstrainRect.xMax)
                        translation.x = rectConstrainRect.xMax - rect.xMax;
                    else if (rect.xMin < rectConstrainRect.xMin)
                        translation.x = rectConstrainRect.xMin - rect.xMin;
                }

                if (rectConstrainRect.height > rect.height)
                {
                    if (rect.yMax > rectConstrainRect.yMax)
                        translation.y = rectConstrainRect.yMax - rect.yMax;
                    else if (rect.yMin < rectConstrainRect.yMin)
                        translation.y = rectConstrainRect.yMin - rect.yMin;
                }

                rectTransform.anchoredPosition += translation;
            }

            rectTransform.offsetMax += rect.max - originalRect.max;
            rectTransform.offsetMin += rect.min - originalRect.min;
        }

        public bool IsOpened
        {
            get { return UseAnimator ? animator.GetBool(IsOpenedParameterName) : gameObject.activeSelf; }
            private set
            {
                if (IsOpened != value)
                {
                    if (UseAnimator && !string.IsNullOrEmpty(IsOpenedParameterName))
                        animator.SetBool(IsOpenedParameterName, value);
                    else
                        gameObject.SetActive(value);
                }
            }
        }

        public void Open()
        {
            IsOpened = true;
        }

        public void Close()
        {
            IsOpened = false;
        }

        public void Toggle()
        {
            if (IsOpened)
                Close();
            else
                Open();
        }

        public virtual void Update()
        {
            UpdatePanelRect();
            UpdateResizeHandles();
        }

        private Vector2 _dragStartMousePosition;
        private Vector2 _dragStartAnchoredPosition;

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (IsDraggable)
            {
                _dragStartMousePosition = GetUIMousePosition(transform.parent);
                _dragStartAnchoredPosition = rectTransform.anchoredPosition;
            }
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (IsDraggable)
            {
                rectTransform.anchoredPosition = _dragStartAnchoredPosition +
                                                 (GetUIMousePosition(transform.parent) - _dragStartMousePosition);

                transform.SetAsLastSibling();
            }
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (IsDraggable)
            {
                transform.SetAsLastSibling();
            }
        }
    }
}