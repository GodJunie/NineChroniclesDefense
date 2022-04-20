// System
using System;
using System.Collections;
using System.Collections.Generic;
// UnityEngine
using UnityEngine;

namespace G2T.NCD.Game {
    using Data;
    using Table;

    public class BuildingBase : MonoBehaviour {
        // 정보
        [SerializeField]
        private int id;


        [SerializeField]
        private HpBar hpBar;

        [SerializeField]
        private float hp;
        private float curHp;

        [SerializeField]
        private float range;
        [SerializeField]
        private int count;

        [SerializeField]
        private bool canAttack;
        [SerializeField]
        private Status curStatus;

        [SerializeField]
        private RectTransform uiRoot;

        private int level;
        private List<Monster> monsters;

        // Event
        private Action OnBuild;
        private Action OnDestroy;
        private Action OnRepair;


        #region Getter
        public int Id { get => id; }
        public int Level { get => level; }
        public float Range { get => range; }
        public int Count { get => count; }
        public int CurrentCount { get => monsters.Count; }
        #endregion


        // Start is called before the first frame update
        void Start() {
            this.curHp = this.hp;
            this.hpBar.Init(this.hp);
        }

        // Update is called once per frame
        void Update() {

        }

        public void OnInteraction() {
            // 제이드 상호작용 하는 부분
            
        }

        public void OnDamaged(float damage) {
            this.curHp -= damage;
            this.hpBar.SetHp(curHp);
            if(this.curHp <= 0f) {
                //GameController.Instance.OnGameOver();
            }
        }
    }
}