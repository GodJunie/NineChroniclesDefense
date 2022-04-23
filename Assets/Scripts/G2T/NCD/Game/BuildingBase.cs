// System
using System;
using System.Collections;
using System.Collections.Generic;
// UnityEngine
using UnityEngine;

namespace G2T.NCD.Game {
    using Data;
    using Table;
    using UnityEngine.UI;

    public abstract class BuildingBase : MonoBehaviour, IInteractable {
        // 정보
        [SerializeField]
        private int id;

        [SerializeField]
        protected HpBar hpBar;
        [SerializeField]
        private GameObject panelSpacebar;

        [SerializeField]
        private float range;
        [SerializeField]
        private int count;

        [SerializeField]
        private new SpriteRenderer renderer;

        [SerializeField]
        private RectTransform uiRoot;
        [SerializeField]
        private Vector2 rootCanvasPosition;

        [SerializeField]
        private GameObject panelConfirmBuild;

        [SerializeField]
        private Transform rangeTransform;

        [SerializeField]
        private GameObject panel;
        [SerializeField]
        protected GameObject infoPanel;
        [SerializeField]
        protected GameObject levelUpPanel;

        [SerializeField]
        protected Text textLevelBefore;
        [SerializeField]
        protected Text textLevelAfter;
        [SerializeField]
        protected Text textCurrentLevel;
        [SerializeField]
        protected Button buttonLevelUp;
        [SerializeField]
        protected Transform levelUpItemSlotContainer;


        protected Status curStatus;
        protected float curHp;
        protected int level;
        protected List<Monster> monsters;

        // Event
        public Action<BuildingBase> OnDead;

        private bool constructing;

        #region Getter
        public int Id { get => id; }
        public int Level { get => level; }
        public float Range { get => range; }
        public int Count { get => count; }
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

        // Start is called before the first frame update
        void Start() {
            this.level = 0;
            this.curHp = curStatus.Hp;
            this.hpBar.Init(curStatus.Hp);

            //this.curStatus = statusTable.Datas[this.level].Status;

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

            if(panel.activeInHierarchy) {
                if((this.transform.position - GameController.Instance.Player.transform.position).x < 0) {
                    this.uiRoot.anchoredPosition = new Vector2(this.rootCanvasPosition.x, this.rootCanvasPosition.y);
                } else {
                    this.uiRoot.anchoredPosition = new Vector2(-this.rootCanvasPosition.x, this.rootCanvasPosition.y);
                }
            }
        }

        public abstract void Init();

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

        public void TryBuild() {
            this.constructing = true;
            this.panelConfirmBuild.SetActive(true);
        }

        public void ConfirmBuild() {
            Init();

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

        public void OnSpacebar() {
            if(this.Interacting) {
                ClosePanel();
            } else {
                OpenPanel();
            }
        }

        public virtual void OpenPanel() {
            this.panel.SetActive(true);
            this.infoPanel.SetActive(true);
            this.levelUpPanel.SetActive(false);
            this.textCurrentLevel.text = string.Format("Lv. {0}", this.level + 1);
            this.Interacting = true;
        }

        public void ClosePanel() {
            this.panel.SetActive(false);
            this.Interacting = false;
        }

        public virtual void OpenLevelUpUI() {
            this.infoPanel.SetActive(false);
            this.levelUpPanel.SetActive(true);
            InitLevelUpUI();
        }

        protected virtual void InitLevelUpUI() {
            textLevelBefore.text = string.Format("Lv. {0}", this.level + 1);
            textLevelAfter.text = string.Format("Lv. {0}", this.level + 2);
        }

        public abstract void OnLevelUp();
    }
}