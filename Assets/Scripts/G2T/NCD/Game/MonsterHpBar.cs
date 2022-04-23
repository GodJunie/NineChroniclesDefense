using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace G2T.NCD.Game {
    public class MonsterHpBar : HpBar {
        [SerializeField]
        private Sprite spriteNeutralityFill;
        [SerializeField]
        private Sprite spriteFriendlyFill;

        public void SetType (MonsterType type) {
            if(type == MonsterType.Wild) {
                this.hpFill.sprite = spriteNeutralityFill;
            } else {
                this.hpFill.sprite = spriteFriendlyFill;
            }
        }
    }
}