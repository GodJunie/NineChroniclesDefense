using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;

namespace G2T.NCD.UI {
    public class UIItemSlot : UISlot {
        [BoxGroup("Colors")]
        [LabelText("White")]
        [SerializeField]
        private Color colorWhite;
        [BoxGroup("Colors")]
        [LabelText("Green")]
        [SerializeField]
        private Color colorGreen;
        [BoxGroup("Colors")]
        [LabelText("Red")]
        [SerializeField]
        private Color colorRed;

        public void SetUI(Sprite icon, int count, int maxCount, Action onClick = null) {
            string text = "";
            if(count < maxCount) {
                text = string.Format("<color=#{2}>{0}</color>/{1}", count, maxCount, colorRed.GetHexString());
            } else if(count == maxCount) {
                text = string.Format("<color=#{2}>{0}</color>/{1}", count, maxCount, colorWhite.GetHexString());
            } else {
                text = string.Format("<color=#{2}>{0}</color>/{1}", count, maxCount, colorGreen.GetHexString());
            }
            SetUI(icon, text, onClick);
        }
    }
}