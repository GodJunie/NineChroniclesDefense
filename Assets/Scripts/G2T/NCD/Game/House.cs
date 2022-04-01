// System
using System;
using System.Collections;
using System.Collections.Generic;
// UnityEngine
using UnityEngine;
using UnityEngine.UI;
// Spine
using Spine.Unity;
// Other
using Sirenix.OdinInspector;
using Cysharp.Threading.Tasks;

namespace G2T.NCD.Game {
    public class House : MonoBehaviour {
        [SerializeField]
        private HpBar hpBar;

        [SerializeField]
        private float hp;
        private float curHp;

        // Start is called before the first frame update
        void Start() {
            this.curHp = this.hp;
            this.hpBar.Init(this.hp);
        }

        // Update is called once per frame
        void Update() {

        }

        public void OnDamaged(float damage) {
            this.curHp -= damage;
            this.hpBar.SetHp(curHp);
            if(this.curHp <= 0f) {
                GameController.Instance.OnGameOver();
            }
        }
    }
}