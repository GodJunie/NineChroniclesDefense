// System
using System;
using System.Collections;
using System.Collections.Generic;
// UnityEngine
using UnityEngine;
using UnityEngine.UI;
// Other
using Sirenix.OdinInspector;
using DG.Tweening;

namespace G2T.NCD.Game {
    public class HpBar : MonoBehaviour {
        [SerializeField]
        protected Image hpFill;
        [SerializeField]
        private Text hpText;

        //[FoldoutGroup("체력 바 UI")]
        //[SerializeField]
        //private Sprite hpBarYellowSprite;
        //[FoldoutGroup("체력 바 UI")]
        //[SerializeField]
        //private Sprite hpBarBlueSprite;

        private float maxHp;
        private float curHp;

        public void Init(float hp) {
            this.curHp = this.maxHp = hp;
            this.SetHp(hp);
        }

        public void SetHp(float hp) {
            this.curHp = Mathf.Clamp(hp, 0f, maxHp);
            //this.hpFill.DOKill();
            this.hpFill.fillAmount = curHp / maxHp;
            this.hpText.text = string.Format("{0:0}/{1:0}", this.curHp, this.maxHp);
        }

        [Button]
        private void Init() {
            this.hpFill = this.transform.GetChild(0).GetComponent<Image>();
            this.hpText = this.transform.GetChild(1).GetComponent<Text>();
        }
    }
}