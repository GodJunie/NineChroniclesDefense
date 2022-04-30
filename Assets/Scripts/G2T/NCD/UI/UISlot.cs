using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;

namespace G2T.NCD.UI {
    public class UISlot : MonoBehaviour {
        [SerializeField]
        protected Text text;
        [SerializeField]
        protected Image image;
        [SerializeField]
        protected Button button;

        [Button]
        private void Init() {
            this.button = this.GetComponent<Button>();
            this.text = this.GetComponentInChildren<Text>();
            this.image = this.GetComponentInChildren<Image>();
        }

        public void SetUI(Sprite icon, string text = "", Action onClick = null) {
            this.image.sprite = icon;
            this.text.text = text;
            this.button.onClick.RemoveAllListeners();
            this.button.onClick.AddListener(() => onClick?.Invoke());
        }
    }
}