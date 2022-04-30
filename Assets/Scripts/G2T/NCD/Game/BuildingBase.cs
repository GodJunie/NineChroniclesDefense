// System
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
// UnityEngine
using UnityEngine;
using UnityEngine.UI;
// Editor
using Sirenix.OdinInspector;

namespace G2T.NCD.Game {
    using Data;
    using Table;
    using Management;

    public abstract class BuildingBase : MonoBehaviour, IInteractable {
        // 정보
        [TabGroup("group", "Settings")]
        [SerializeField]
        private int id;
        [TabGroup("group", "Settings")]
        [SerializeField]
        private float range;

        [TabGroup("group", "UI")]
        [SerializeField]
        protected HpBar hpBar;
        [TabGroup("group", "UI")]
        [SerializeField]
        private GameObject panelSpacebar;

        [TabGroup("group", "UI")]
        [SerializeField]
        protected RectTransform uiRoot;
        [TabGroup("group", "UI")]
        [SerializeField]
        private Vector2 rootCanvasPosition;

        [SerializeField]
        private new SpriteRenderer renderer;

        [SerializeField]
        private GameObject panelConfirmBuild;

        [SerializeField]
        private Transform rangeTransform;

        protected Status curStatus;
        protected float curHp;
        protected List<Monster> monsters;

        // Event
        public Action<BuildingBase> OnDead;

        private bool constructing;

        public BuildingInfo Info { get; private set; }
        public ScriptableObject StatusTable { get; private set; }

        #region Getter
        public int Id { get => id; }
        public int Level { get; private set; }
        public float Range { get => range; }
        public int CurrentCount { get => monsters.Count; }
        public float PosX { 
            get {
                return this.transform.position.x;
            }
        }

        public bool Interacting { get; private set; }

        public float RangeLeft {
            get {
                return PosX - range;
            }
        }
        public float RangeRight {
            get {
                return PosX + range;
            }
        }
        #endregion

        public abstract List<BuildingStatusInfo> Statuses { get; }

        public virtual async Task Init(BuildingInfo info) {
            this.constructing = true;
            this.panelConfirmBuild.SetActive(true);

            this.Info = info;
            this.StatusTable = await ResourcesManager.Instance.LoadAsync<ScriptableObject>(info.StatusPath);

            this.Level = 0;

            this.curStatus = Statuses[Level].Status;
            this.curHp = curStatus.Hp;
            this.hpBar.Init(curHp);
        }

        // Start is called before the first frame update
        void Start() {
            this.Level = 0;
            this.curHp = curStatus.Hp;
            this.hpBar.Init(curStatus.Hp);

            rangeTransform.localScale = new Vector3(this.range * 2, 1, 1);
            this.Interacting = false;
        }

        // Update is called once per frame
        void Update() {
            if(constructing) {
                var house = GameController.Instance.House;
                var x = (this.transform.position - house.transform.position).x;
                this.renderer.flipX = x < 0;

                var posX = this.transform.position.x;
                if(posX > GameController.Instance.BuildingRangeLeft && posX < GameController.Instance.BuildingRangeRight) {
                    this.renderer.color = Color.green;
                } else {
                    this.renderer.color = Color.red;
                }
            }

            if(Interacting) {
                if((this.transform.position - GameController.Instance.Player.transform.position).x < 0) {
                    this.uiRoot.anchoredPosition = new Vector2(this.rootCanvasPosition.x, this.rootCanvasPosition.y);
                } else {
                    this.uiRoot.anchoredPosition = new Vector2(-this.rootCanvasPosition.x, this.rootCanvasPosition.y);
                }
            }
        }

        public void ShowSpacebar() {
            this.panelSpacebar.SetActive(true);
        }

        public void HideSpacebar() {
            this.panelSpacebar.SetActive(false);
        }


        public void OnDamaged(float damage) {
            this.curHp -= damage;
            this.hpBar.SetHp(curHp);
            if(this.curHp <= 0f) {
                this.OnDead?.Invoke(this);
            }
        }

        public virtual void OnLevelUp() {
            this.Level++;
            this.curStatus = Statuses[this.Level].Status;
            this.curHp = curStatus.Hp;
            this.hpBar.Init(curHp);

            this.OnInteract();
        }

        public void ConfirmBuild() {
            this.constructing = false;
            this.transform.SetParent(null);
            GameController.Instance.AddBuilding(this);
            this.panelConfirmBuild.SetActive(false);
            this.hpBar.gameObject.SetActive(true);
            renderer.color = Color.white;
        }

        public void CancelBuild() {
            GameController.Instance.OnCancelConstruct();
        }

        public void ShowRange() {
            this.rangeTransform.gameObject.SetActive(true);
        }

        public void HideRange() {
            this.rangeTransform.gameObject.SetActive(false);
        }

        public void OnInteract() {
            if(this.Interacting) {
                this.Interacting = false;
                ClosePanel();
            } else {
                this.Interacting = true;
                OpenPanel();
            }
        }

        protected abstract void OpenPanel();
        protected abstract void ClosePanel();
    }
}