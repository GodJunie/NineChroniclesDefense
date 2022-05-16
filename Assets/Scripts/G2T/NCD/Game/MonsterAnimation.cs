// System
using System;
using System.Collections;
using System.Collections.Generic;
// UnityEngine
using UnityEngine;
// UniTask
using Cysharp.Threading.Tasks;
// Spine
using Spine.Unity;
// Odin Inspector
using Sirenix.OdinInspector;

namespace G2T.NCD.Game {
    // 몬스터 애니메이션 관리
    // 스파인 애니메이션 객체가 있어야 하고
    public class MonsterAnimation : MonoBehaviour {
        #region Fields
        #region Objects
        [SerializeField]
        private SkeletonAnimation anim;
        #endregion

        #region Effects
        [SerializeField]
        private GameObject effectCatch;
        [SerializeField]
        private float catchDuration;

        [SerializeField]
        private GameObject effectLevelUp;
        [SerializeField]
        private float levelUpDuration;
        
        [SerializeField]
        private GameObject effectEvolutionStart;
        [SerializeField]
        private float evolutionStartDuration;

        [SerializeField]
        private GameObject effectEvolutionEnd;
        [SerializeField]
        private float evolutionEndDuration;

        [SerializeField]
        private GameObject effectAggro;
        [SerializeField]
        private float aggroDuration;
        #endregion

        #region Events
        private Action onDead;
        private Action onAttack;
        private Action onAttackEnd;
        #endregion
        #endregion

        #region Mono
        private void Start() {
            this.effectCatch.SetActive(false);
            this.effectLevelUp.SetActive(false);
            this.effectEvolutionStart.SetActive(false);
            this.effectEvolutionEnd.SetActive(false);
            this.effectAggro.SetActive(false);

            anim.AnimationState.Start += (entry) => {

            };
            anim.AnimationState.Event += (entry, e) => {
                Debug.Log(e.Data.Name);
                if(e.Data.Name == "attackPoint")
                    onAttack?.Invoke();
            };
            anim.AnimationState.End += (entry) => {
                Debug.Log(entry.Animation.Name);
                if(entry.Animation.Name == "Die") {
                    onDead?.Invoke();
                }
                if(entry.Animation.Name == "Attack") {
                    onAttackEnd?.Invoke();
                }
            };
        }
        #endregion

        #region Effect
        public async UniTask OnCatch() {
            effectCatch?.SetActive(true);
            await UniTask.Delay(TimeSpan.FromSeconds(catchDuration));
            effectCatch?.SetActive(false);
        }

        public async UniTask OnAggro() {
            effectAggro?.SetActive(true);
            await UniTask.Delay(TimeSpan.FromSeconds(aggroDuration));
            effectAggro?.SetActive(false);
        }

        public async UniTask OnLevelUp() {
            effectLevelUp?.SetActive(true);
            await UniTask.Delay(TimeSpan.FromSeconds(levelUpDuration));
            effectLevelUp?.SetActive(false);
        }

        public async UniTask OnEvolutionStart() {
            effectEvolutionStart?.SetActive(true);
            await UniTask.Delay(TimeSpan.FromSeconds(evolutionStartDuration));
            effectEvolutionStart?.SetActive(false);
        }

        public async UniTask OnEvolutionEnd() {
            effectEvolutionEnd?.SetActive(true);
            await UniTask.Delay(TimeSpan.FromSeconds(evolutionEndDuration));
            effectEvolutionEnd?.SetActive(false);
        }
        #endregion

        public void Idle() {
            if(anim.AnimationName == "Idle")
                return;
            anim.AnimationState.ClearTracks();
            anim.AnimationState.SetAnimation(0, "Idle", true);
        }

        public void Move() {
            if(anim.AnimationName == "Run")
                return;
            anim.AnimationState.ClearTracks();
            anim.AnimationState.SetAnimation(0, "Run", true);
        }

        public void Attack(Action onAttack, Action onAttackEnd) {
            this.onAttack = onAttack;
            this.onAttackEnd = onAttackEnd;

            anim.AnimationState.ClearTracks();
            anim.AnimationState.SetAnimation(0, "Attack", true);
        }

        public void Dead(Action onDead) {
            this.onDead = onDead;
            anim.AnimationState.ClearTracks();
            anim.AnimationState.SetAnimation(0, "Die", false);
        }
        
        [Button]
        private void Init() {
            this.effectAggro = transform.Find("Aggro").gameObject;
            this.aggroDuration = .5f;
        }
    }
}