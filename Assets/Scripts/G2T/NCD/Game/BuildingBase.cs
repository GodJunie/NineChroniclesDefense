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


        // Getter
        public int Level { get => level; }
        public float Range { get => range; }
        public int Count { get => count; }
        public int CurrentCount { get => monsters.Count; }


        // Start is called before the first frame update
        void Start() {

        }

        // Update is called once per frame
        void Update() {

        }

        public void OnInteraction() {
            // 제이드 상호작용 하는 부분
            
        }

        public void AddMonster(Monster monster) {
            this.monsters.Add(monster);

            monster.OnDead += (monster) => {
                this.monsters.Remove(monster);
            };
        }
    }

}