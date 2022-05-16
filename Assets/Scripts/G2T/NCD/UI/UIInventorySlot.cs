using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace G2T.NCD.UI {
    public class UIInventorySlot : UISlot, IBeginDragHandler, IDragHandler, IEndDragHandler, IScrollHandler, IPointerEnterHandler, IPointerExitHandler {
        private ScrollRect scrollRect;
        private Action onPointerEnter;
        private Action onPointerExit;

        public void OnBeginDrag(PointerEventData e) {
            scrollRect.OnBeginDrag(e);
        }

        public void OnDrag(PointerEventData e) {
            scrollRect.OnDrag(e);
        }

        public void OnEndDrag(PointerEventData e) {
            scrollRect.OnEndDrag(e);
        }

        public void OnScroll(PointerEventData e) {
            scrollRect.OnScroll(e);
        }

        public void OnPointerEnter(PointerEventData eventData) {
            this.onPointerEnter?.Invoke();
        }

        public void OnPointerExit(PointerEventData eventData) {
            this.onPointerExit?.Invoke();
        }

        public void SetUI(Sprite sprite, string text, Action onPointerEnter, Action onPointerExit, ScrollRect scrollRect) {
            this.scrollRect = scrollRect;

            this.onPointerEnter = onPointerEnter;
            this.onPointerExit = onPointerExit;

            this.image.sprite = sprite;
            this.text.text = text;
        }
    }
}