/*
 * Copyright (C) 2015 Exit Games GmbH
 * by The Knights of Unity
 */

using UnityEngine;
using UnityEngine.UI;

namespace PhotonChatUI
{
    public class ChatBehaviourUI : MonoBehaviour
    {
        private RectTransform _rectTransform;

        public RectTransform rectTransform
        {
            get
            {
                if (_rectTransform == null)
                {
                    try
                    {
                        _rectTransform = GetComponent<RectTransform>();
                    }
                    catch (MissingComponentException)
                    {
                        _rectTransform = gameObject.AddComponent<RectTransform>();
                    }
                }
                return _rectTransform;
            }
        }

        private RectTransform _parentRectTransform;

        public RectTransform parentRectTransform
        {
            get
            {
                if (transform.parent != null)
                {
                    return _parentRectTransform == null || transform.parent != _parentRectTransform
                        ? (_parentRectTransform = transform.parent.GetComponent<RectTransform>())
                        : _parentRectTransform;
                }
                return null;
            }
        }

        private ChatPanelUI _chatPanelUI;

        public ChatPanelUI chatPanelUI
        {
            get { return _chatPanelUI ?? (_chatPanelUI = GetComponent<ChatPanelUI>()); }
        }

        private CanvasGroup _canvasGroup;

        public CanvasGroup canvasGroup
        {
            get { return _canvasGroup ?? (_canvasGroup = GetComponent<CanvasGroup>()); }
        }

        private Canvas _canvas;

        public Canvas canvas
        {
            get { return _canvas ?? (_canvas = GetComponentInParent<Canvas>()); }
        }

        /// <summary>
        /// Returns UI position according to parent.
        /// </summary>
        /// <param name="parent">If <c>null</c> then returns position according to Canvas.</param>
        public Vector2 GetUIMousePosition(Transform parent = null)
        {
            //return RectTransformUtility.PixelAdjustPoint(Input.mousePosition, (parent ?? canvas.transform), canvas);
            return
                (parent != null ? parent : canvas.transform).InverseTransformPoint(canvas.renderMode ==
                                                                                   RenderMode.ScreenSpaceOverlay
                    ? Input.mousePosition
                    : canvas.worldCamera.ScreenToWorldPoint(Input.mousePosition));
        }

        public void SetCanvasGroupAlpha(Slider alpha)
        {
            SetCanvasGroupAlpha(alpha.value/alpha.maxValue);
        }

        public void SetCanvasGroupAlpha(float alpha)
        {
            if (canvasGroup != null)
                canvasGroup.alpha = alpha;
            else
                Debug.LogWarning("Canvas Group is missing - unable to set alpha.", gameObject);
        }
    }
}