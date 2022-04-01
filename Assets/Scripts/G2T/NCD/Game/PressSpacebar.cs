// System
using System.Collections;
using System.Collections.Generic;
// UnityEngine
using UnityEngine;
using UnityEngine.UI;

namespace G2T.NCD.Game {
    public class PressSpacebar : MonoBehaviour {
        [SerializeField]
        private Image imageSpacebarBtn;
        [SerializeField]
        private Sprite spriteNeutralityBtn;
        [SerializeField]
        private Sprite spriteFriendlyBtn;
        [SerializeField]
        private Text textSpacebar;
        [SerializeField]
        private Color colorTextNeutrality;
        [SerializeField]
        private Color colorTextFriendly;


        public void SetType(MonsterType type) {
            if(type == MonsterType.Neutrality) {
                this.imageSpacebarBtn.sprite = spriteNeutralityBtn;
                this.textSpacebar.color = colorTextNeutrality;
            } else if(type == MonsterType.Friendly) {
                this.imageSpacebarBtn.sprite = spriteFriendlyBtn;
                this.textSpacebar.color = colorTextFriendly;
            }
        }

        public void SetActive(bool value) {
            this.gameObject.SetActive(value);
        }
    }
}