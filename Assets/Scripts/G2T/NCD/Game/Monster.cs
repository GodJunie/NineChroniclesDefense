// System
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
// UnityEngine
using UnityEngine;
// Editor
using Sirenix.OdinInspector;

namespace G2T.NCD.Game {
    using Table;
    using Management;
    using UI;

    public class Monster : MonoBehaviour, IInteractable {
        #region Members

        #region Objects
        [SerializeField]
        private Transform monsterHolder;

        [SerializeField]
        private Detector attackDetector;
        [SerializeField]
        private Detector aggroDetector;
        #endregion

        #region UI
        // Game UI
        [SerializeField]
        private MonsterHpBar hpBar;
        [SerializeField]
        private PressSpacebar pressSpacebar;
        // UI Root
        [SerializeField]
        private RectTransform uiRoot;
        [SerializeField]
        private Vector2 rootCanvasPosition;
        #endregion


        // 게임 동작에 필요한 멤버들
        #region Game
        // 현재 상태, 몬스터 타입
        public State State { get; private set; }
        public MonsterType MonsterType { get; private set; }
        public BuildingBase TargetBuilding { get; private set; }
        // 스탯 수치 (레벨, 체력, 현재 스탯)
        public int Level { get; private set; }
        public float CurHp { get; private set; }
        public Status CurStatus { get; private set; }
        // 상호작용 등
        public bool Interacting { get; private set; }
        public float PosX { get => transform.position.x; }

        private List<Enemy> attackTargets = new List<Enemy>();
        private List<Enemy> aggroTargets = new List<Enemy>();
        private MonsterAnimation monsterAnimation;
        #endregion

        #region Events
        public Action<Monster> OnDead;
        public Action<Monster> OnLevelUp;
        public Action<Monster> OnEvolution;
        #endregion

        #endregion

        // 몬스터 스탯 테이블 등의 데이터
        #region Data
        public int Id { get; private set; }
        public MonsterInfo Info { get; private set; }
        public MonsterStatusTable StatusTable { get; private set; }
        #endregion

        #region Mono
        private void Start() {
            this.aggroDetector.OnEnter = (collider) => this.OnAggroTriggerEnter(collider);
            this.aggroDetector.OnExit = (collider) => this.OnAggroTriggerExit(collider);

            this.attackDetector.OnEnter = (collider) => this.OnAttackTriggerEnter(collider);
            this.attackDetector.OnExit = (collider) => this.OnAttackTriggerExit(collider);

        }

        private void Update() {
            switch(this.State) {
            case State.Idle:
                Idle();
                break;
            case State.Attack:
                Attack();
                break;
            case State.Dead:
                Dead();
                break;
            case State.Interact:
                Interact();
                break;
            case State.Aggro:
                Aggro();
                break;
            default:
                break;
            }
        }
        #endregion

        #region Management
        // 초기화 (데이터, 몬스터 타입, 스탯 등)
        #region Init
        public void Init(int id, MonsterType monsterType = MonsterType.Wild) {
            var info = TableLoader.Instance.MonsterTable.Datas.Find(e => e.Id == id);
            this.Init(id, info, monsterType);
        }

        public void Init(int id, MonsterInfo info, MonsterType monsterType = MonsterType.Wild) {
            this.Id = id;
            this.Info = info;

            // Init level
            this.Level = 0;

            // Get Table
            this.StatusTable = ResourcesManager.Instance.Load<ScriptableObject>(info.StatusPath) as MonsterStatusTable;

            // Instantiate monste prefab
            var monsterPrefab = ResourcesManager.Instance.Load<GameObject>(info.PrefabPath);
            this.monsterAnimation = Instantiate(monsterPrefab, Vector3.zero, Quaternion.identity, monsterHolder).GetComponent<MonsterAnimation>();
            this.monsterAnimation.transform.localPosition = Vector3.zero;
            this.monsterAnimation.transform.localScale = Vector3.one;

            // Set MonsterType
            this.InitMonsterType(monsterType);

            // Set Status
            InitStatus();

            ChangeState(State.Idle);
        }

        private void InitMonsterType(MonsterType monsterType) {
            this.MonsterType = monsterType;
            this.hpBar.SetType(monsterType);
            this.pressSpacebar.SetType(monsterType);
        }

        private void InitStatus() {
            this.CurStatus = StatusTable.Datas[this.Level].Status;

            this.CurHp = CurStatus.Hp;
            this.hpBar.Init(CurStatus.Hp);
            this.attackDetector.SetRange(CurStatus.AttackRange);
            this.aggroDetector.SetRange(2f);
        }
        #endregion

        // 몬스터 레벨업 진화 등
        #region Interact
        public async void Catch() {
            this.InitMonsterType(MonsterType.Friendly);
            this.InitStatus();

            this.TargetBuilding = GameController.Instance.Buildings.OrderBy(e => Math.Abs(e.PosX - this.PosX)).First();
            this.TargetBuilding.ShowRange();

            await this.monsterAnimation.OnCatch();

            GameController.Instance.SetMonsterAmountsUI();
        }

        public async void LevelUp() {
            this.Level++;
            await this.monsterAnimation.OnLevelUp();
            this.InitStatus();
        }

        public async void Evolution() {
            await this.monsterAnimation.OnEvolutionStart();
            Destroy(this.monsterAnimation.gameObject);
            this.Init(this.Info.EvolutionResult, MonsterType.Friendly);
            await this.monsterAnimation.OnEvolutionEnd();
        }

        public void MoveTargetBuilding(Direction direction) {
            // 타겟 빌딩 이동
            
        }
        #endregion

        #region Combat
        public void OnDamaged(float damage) {
            this.CurHp -= damage;
            this.hpBar.SetHp(CurHp);
            if(this.CurHp <= 0f) {
                this.ChangeState(State.Dead);
            }
        }
        #endregion
        #endregion

        #region FSM
        private void ChangeState(State state) {
            switch(this.State) {
            case State.Idle:
                IdleExit();
                break;
            case State.Aggro:
                AggroExit();
                break;
            case State.Attack:
                AttackExit();
                break;
            case State.Interact:
                InteractExit();
                break;
            case State.Dead:
                DeadExit();
                break;
            }

            this.State = state;

            switch(this.State) {
            case State.Idle:
                IdleEnter();
                break;
            case State.Aggro:
                AggroEnter();
                break;
            case State.Attack:
                AttackEnter();
                break;
            case State.Interact:
                InteractEnter();
                break;
            case State.Dead:
                DeadEnter();
                break;
            }
        }

        private void CheckState() {
            if(this.MonsterType == MonsterType.Friendly) {
                if(this.attackTargets.Count > 0) {
                    ChangeState(State.Attack);
                } else if(this.aggroTargets.Concat(TargetBuilding.Enemies).ToList().Count > 0) {
                    if(this.State == State.Aggro) return;
                    ChangeState(State.Aggro);
                } else {
                    ChangeState(State.Idle);
                }
            } else {
                if(this.attackTargets.Count > 0) {
                    ChangeState(State.Attack);
                } else {
                    ChangeState(State.Idle);
                }
            }
        }

        #region Idle
        private float idleRangeLeft;
        private float idleRangeRight;
        private Direction idleDirection;
        private bool idleMove;
        private float idleTimer = 0f;

        private void IdleEnter() {
            if(this.MonsterType == MonsterType.Wild) {
                idleRangeLeft = this.PosX - 1f;
                idleRangeRight = this.PosX + 1f;
            } else {
                idleRangeLeft = TargetBuilding.RangeLeft;
                idleRangeRight = TargetBuilding.RangeRight;
            }

            idleTimer = 0f;
        }

        private void Idle() {
            idleTimer -= Time.deltaTime;

            if(idleTimer < 0) {
                // 다음 행동 결정 (움직이기, 멈추기)
                if(this.PosX < idleRangeLeft) {
                    idleDirection = Direction.Right;
                    idleMove = true;
                } else if(this.PosX > idleRangeRight) {
                    idleDirection = Direction.Left;
                    idleMove = true;
                } else {
                    idleMove = UnityEngine.Random.Range(0f, 2f) > 1f;

                    idleDirection = UnityEngine.Random.Range(0f, 2f) > 1f ? Direction.Left : Direction.Right;
                }

                idleTimer = UnityEngine.Random.Range(1f, 2f);

                if(idleMove) {
                    SetDirection(idleDirection);
                    this.monsterAnimation.Move();
                } else {
                    this.monsterAnimation.Idle();
                }
            }

            if(idleMove) {
                this.transform.position += new Vector3(this.CurStatus.MoveSpeed * Time.deltaTime * (idleDirection == Direction.Right ? 1f : -1f), 0f, 0f);

                if(this.PosX < idleRangeLeft && idleDirection == Direction.Left) idleTimer = 0f;
                if(this.PosX > idleRangeRight && idleDirection == Direction.Right) idleTimer = 0f;
            }
        }

        private void IdleExit() {

        }
        #endregion

        #region Aggro
        private float aggroIdleTimer;

        public void AggroTrigger() {
            if(this.State == State.Idle) {
                ChangeState(State.Aggro);
            }
        }

        private void AggroEnter() {
            this.monsterAnimation.Idle();
            this.monsterAnimation.OnAggro();
            aggroIdleTimer = .5f;

            var target = aggroTargets.Concat(TargetBuilding == null ? new List<Enemy>() : TargetBuilding.Enemies).OrderBy(e => Mathf.Abs(e.transform.position.x - this.PosX)).First();

            var direction = this.PosX > target.transform.position.x ? Direction.Right : Direction.Left;

            SetDirection(direction);
        }

        private void Aggro() {
            aggroIdleTimer -= Time.deltaTime;

            if(aggroIdleTimer < 0) {
                this.monsterAnimation.Move();
                var target = aggroTargets.Concat(TargetBuilding == null ? new List<Enemy>() : TargetBuilding.Enemies).OrderBy(e => Mathf.Abs(e.transform.position.x - this.PosX)).First();

                var direction = this.PosX > target.transform.position.x ? Direction.Left : Direction.Right;

                SetDirection(direction);

                this.transform.position += new Vector3(this.CurStatus.MoveSpeed * Time.deltaTime * (direction == Direction.Right ? 1f : -1f), 0f, 0f);
            }
        }

        private void AggroExit() {

        }
        #endregion

        #region Attack
        private void AttackEnter() {
            if(attackTargets.Count > 0) {
                var target = attackTargets.OrderBy(e => Mathf.Abs(this.PosX - e.transform.position.x)).First();

                SetDirection(target.transform.position.x < this.PosX ? Direction.Left : Direction.Right);

                this.monsterAnimation.Attack(() => {
                    if(target != null) {
                        target.OnDamaged(this.CurStatus.Atk);
                    }
                }, () => {
                    Debug.Log("attack end!");
                    CheckState();
                });
            } else {
                // 공격 어케하지 이건?
                // 공격을 해야하는데 타겟이 없음
                // 애초에 공격 상태로 어케 왔는지..
            }
        }

        private void Attack() {
            // 공격 모션 대기중임 (어차피 공격 시점에 콜백으로 처리할거라..)
        }

        private void AttackExit() {

        }
        #endregion

        #region Interact
        private void InteractEnter() {
            this.monsterAnimation.Idle();

            UIManager.Instance.OpenUI("monster-info", this.uiRoot).GetComponent<UIMonsterInfo>().Open(this);

            var diff = GameController.Instance.Player.transform.position - this.transform.position;

            if(diff.x > 0) {
                SetDirection(Direction.Right);
                uiRoot.anchoredPosition = this.rootCanvasPosition;
            } else {
                SetDirection(Direction.Left);
                uiRoot.anchoredPosition = this.rootCanvasPosition * -1f;
            }

            this.Interacting = true;

            this.aggroDetector.gameObject.SetActive(false);
            this.attackDetector.gameObject.SetActive(false);

            this.TargetBuilding?.ShowRange();
        }

        private void Interact() {
            // 현재 상호작용 중임
        }

        private void InteractExit() {
            this.TargetBuilding?.HideRange();

            UIManager.Instance.CloseUI("monster-info");

            this.aggroDetector.gameObject.SetActive(true);
            this.attackDetector.gameObject.SetActive(true);

            this.Interacting = false;
        }
        #endregion

        #region Dead
        private void DeadEnter() {
            this.OnDead?.Invoke(this);
            this.monsterAnimation.Dead(() => {
                Destroy(gameObject);
            });
        }

        private void Dead() {

        }

        private void DeadExit() {

        }
        #endregion


        #endregion

        #region Interact
        public void ShowSpacebar() {
            this.pressSpacebar.SetActive(true);
        }

        public void HideSpacebar() {
            this.pressSpacebar.SetActive(false);
        }

        public void OnInteract() {
            if(this.State == State.Idle) {
                ChangeState(State.Interact);
            } else if(this.State == State.Interact) {
                CheckState();
            }
        }
        #endregion

        #region Graphic
        private void SetDirection(Direction direction) {
            this.monsterHolder.rotation = Quaternion.Euler(0f, direction == Direction.Left ? -180f : 0f, 0f);
        }
        #endregion

        #region Trigger
        private void OnAttackTriggerEnter(Collider2D collider) {
            switch(collider.tag) {
            case "Enemy":
                var enemy = collider.GetComponent<Enemy>();
                if(!this.attackTargets.Contains(enemy)) {
                    this.attackTargets.Add(enemy);
                }
                if(this.State == State.Aggro || this.State == State.Idle)
                    ChangeState(State.Attack);
                break;
            }
        }

        private void OnAttackTriggerExit(Collider2D collider) {
            switch(collider.tag) {
            case "Enemy":
                var enemy = collider.GetComponent<Enemy>();
                if(this.attackTargets.Contains(enemy)) {
                    this.attackTargets.Remove(enemy);
                }
                if(this.attackTargets.Count == 0) {
                    ChangeState(State.Idle);
                }
                break;
            }
        }

        private void OnAggroTriggerEnter(Collider2D collider) {
            switch(collider.tag) {
            case "Enemy":
                var enemy = collider.GetComponent<Enemy>();
                if(!this.aggroTargets.Contains(enemy)) {
                    this.aggroTargets.Add(enemy);
                }

                if(this.State == State.Idle && this.MonsterType == MonsterType.Friendly) {
                    ChangeState(State.Aggro);
                }
                break;
            }
        }

        private void OnAggroTriggerExit(Collider2D collider) {
            switch(collider.tag) {
            case "Enemy":
                var enemy = collider.GetComponent<Enemy>();
                if(this.aggroTargets.Contains(enemy)) {
                    this.aggroTargets.Remove(enemy);
                }
                if(this.aggroTargets.Count == 0) {
                    if(this.State == State.Attack || this.State == State.Aggro) {
                        ChangeState(State.Idle);
                    }
                }
                break;
            }
        }
        #endregion

    }
}