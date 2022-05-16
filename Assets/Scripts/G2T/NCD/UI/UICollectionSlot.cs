using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace G2T.NCD.UI {
    public class UICollectionSlot : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IScrollHandler {
        [SerializeField]
        private Sprite spriteOn;
        [SerializeField]
        private Sprite spriteOff;
        //[SerializeField]
        //private GameObject selectedEffect;

        [SerializeField]
        private Image imageBackground;
        [SerializeField]
        private Image imageIcon;
        [SerializeField]
        private Toggle toggle;

        private ScrollRect scrollRect;

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

        public void SetUI(Sprite icon, Action<bool> onValueChanged, ScrollRect scrollRect, ToggleGroup toggleGroup) {
            this.scrollRect = scrollRect;
            this.imageIcon.sprite = icon;
            this.toggle.onValueChanged.AddListener(isOn => {
                //this.selectedEffect.SetActive(isOn);
                this.imageBackground.sprite = isOn ? spriteOn : spriteOff;
                onValueChanged?.Invoke(isOn);
            });
            this.toggle.group = toggleGroup;
            this.toggle.isOn = false;
        }
    }
}