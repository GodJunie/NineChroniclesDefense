using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace G2T.NCD.Game.UI {
    public class UIMinimapMarker : MonoBehaviour {
        [SerializeField]
        private Image image;

        private RectTransform rectTransform;

        public Transform Target { get; private set; }

        private void Awake() {
            this.rectTransform = GetComponent<RectTransform>();
        }

        public void SetColor(Color color) {
            image.color = color;
        }

        public void SetTarget(Transform target) {
            this.Target = target;
        }

        public void SetPosition(float x) {
            this.rectTransform.anchoredPosition = new Vector2(x, 0);
        }
    }
}