using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace G2T.NCD.UI {
    public class UIManager : SingletonBehaviour<UIManager> {
        [SerializeField]
        private List<UIInfo> uis;

        [SerializeField]
        private RectTransform worldRootCanvas;
        [SerializeField]
        private RectTransform overlayRootCanvas;
        [SerializeField]
        private RectTransform screenRootCanvas;

        // Start is called before the first frame update
        void Start() {

        }

        // Update is called once per frame
        void Update() {

        }

        public GameObject OpenUI(string id, RectTransform parent = null) {
            var ui = this.uis.Find(e => e.Id == id);

            if(parent != null) {
                ui.Panel.transform.SetParent(parent);
            } else {
                switch(ui.Type) {
                case UIPanelType.World:
                    ui.Panel.transform.SetParent(worldRootCanvas);
                    break;
                case UIPanelType.Overlay:
                    ui.Panel.transform.SetParent(overlayRootCanvas);
                    break;
                case UIPanelType.Screen:
                    ui.Panel.transform.SetParent(screenRootCanvas);
                    break;
                }
            }

            var rectTransform = ui.Panel.GetComponent<RectTransform>();
            rectTransform.localScale = Vector3.one;
            rectTransform.anchoredPosition = Vector2.zero;

            ui.Panel.SetActive(true);

            return ui.Panel;
        }

        public void CloseUI(string id) {
            var ui = this.uis.Find(e => e.Id == id);

            if(ui == null) return;

            switch(ui.Type) {
            case UIPanelType.World:
                ui.Panel.transform.SetParent(worldRootCanvas);
                break;
            case UIPanelType.Overlay:
                ui.Panel.transform.SetParent(overlayRootCanvas);
                break;
            case UIPanelType.Screen:
                ui.Panel.transform.SetParent(screenRootCanvas);
                break;
            }

            ui.Panel.SetActive(false);
        }

        [System.Serializable]
        public class UIInfo {
            public string Id;
            public GameObject Panel;
            public UIPanelType Type;
        }

        public enum UIPanelType : int { World, Overlay, Screen }
    }
}