using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;


namespace G2T.NCD.Game.UI {
    public class UICatchPanelSlot : MonoBehaviour {
        public Image IconImage;
        public Text CountText;

        [Button]
        public void Init() {
            this.IconImage = this.transform.GetChild(0).GetComponent<Image>();
            this.CountText = this.transform.GetChild(1).GetComponent<Text>();
        }
    }
}