// System
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.IO;
// UnityEngine
using UnityEngine;
using UnityEngine.UI;
// Spine
using Spine.Unity;
// Other
using Sirenix.OdinInspector;
using Cysharp.Threading.Tasks;
using DG.Tweening;

namespace G2T.NCD.Game {
    using Table;
    using Data;
    using UI;
    using Management;

    public enum MonsterType : int { None = 0, Wild, Friendly }

    [RequireComponent(typeof(Rigidbody2D))]
    public class Monster : MonoBehaviour, IInteractable {
        [Serializable]
        public class MonsterLevelInfo {
            [FoldoutGroup("레벨 정보")]
            [FoldoutGroup("레벨 정보/능력치")]
            [HideLabel]
            [SerializeField]
            private Status status;
            [FoldoutGroup("레벨 정보")]
            [LabelText("필요 아이템")]
            [SerializeField]
            private List<ItemData> needs;

            public Status Status { get => status; }
            public List<ItemData> Needs { get => needs; }
        }

        // Serialized Members
        // Components
        [TabGroup("group", "오브젝트")]
        [SerializeField]
        private SkeletonAnimation anim;
        [TabGroup("group", "오브젝트")]
        [SerializeField]
        private Transform skeletonTransform;

        // UI
        [TabGroup("group", "UI")]
        [SerializeField]
        private MonsterHpBar hpBar;
        [TabGroup("group", "UI")]
        [SerializeField]
        private PressSpacebar pressSpacebar;
        [TabGroup("group", "UI")]
        [SerializeField]
        private RectTransform uiRoot;
        [TabGroup("group", "UI")]
        [SerializeField]
        private Vector2 rootCanvasPosition;

        [TabGroup("group", "UI")]
        [SerializeField]
        private GameObject panelMove;
        [TabGroup("group", "UI")]
        [SerializeField]
        private GameObject buttonLeft;
        [TabGroup("group", "UI")]
        [SerializeField]
        private GameObject buttonRight;

        [TabGroup("group", "설정")]
        [LabelText("기본 방향 (스파인)")]
        [SerializeField]
        private Direction defaultDirection;
        [TabGroup("group", "설정")]
        [LabelText("아이디")]
        [SerializeField]
        private int id;
        public int Id { get => id; }


        private Direction curDirection;
       
        // Private Members
        // Components
        private Rigidbody2D rigid;
        private new Transform transform;
       
        // Game
        public MonsterType MonsterType { get; private set; }
        private State curState;
        private int level;
        public float CurHp { get; private set; }

        // objects
        private List<Enemy> enemies = new List<Enemy>();
        private PlayerController player = null;

        // Events
        public Action<Monster> OnDead;
        
        public string Name { get => this.name; }
        public int Level { get => this.level; }
        public Status CurStatus { get => StatusTable.Datas[level].Status; }

        public float PosX => this.transform.position.x;

        public bool Interacting { get; private set; }

        public MonsterInfo Info { get; private set; }
        public MonsterStatusTable StatusTable { get; private set; }

        private Vector3 startPos;
        public BuildingBase TargetBuilding { get; private set; }

        public async void Init(MonsterInfo info, MonsterType monsterType) {
            this.Info = info;
            this.level = 0;
            this.MonsterType = monsterType;
            this.startPos = this.transform.position;

            this.SetDirection(Direction.Right);

            this.curState = State.Move;

            this.StatusTable = await ResourcesManager.Instance.LoadAsync<ScriptableObject>(info.StatusPath) as MonsterStatusTable;

            this.CurHp = this.CurStatus.Hp;
            this.hpBar.SetType(this.MonsterType);
            this.hpBar.Init(CurStatus.Hp);
            this.pressSpacebar.SetType(this.MonsterType);
            this.pressSpacebar.SetActive(false);

            GameController.Instance.SetMonsterAmountsUI();

            if(monsterType == MonsterType.Friendly) {
                this.TargetBuilding = GameController.Instance.Buildings.OrderBy(e => Mathf.Abs(PosX - e.PosX)).First();
            }

            this.StartFSM();
        }

        private void Awake() {
            this.rigid = GetComponent<Rigidbody2D>();
            this.transform = GetComponent<Transform>();
        }

        // Start is called before the first frame update
        void Start() {

        }

        // Update is called once per frame
        void Update() {
            
        }

        private void StartFSM() {
            StartCoroutine(FSM());
        }

        public void OnInteract() {
            if(this.Interacting) {
                UIManager.Instance.CloseUI("monster-info");
                this.Interacting = false;

                if(TargetBuilding != null)
                    TargetBuilding.HideRange();

                this.curState = State.Move;

                this.panelMove.SetActive(false);
                this.TargetBuilding?.HideRange();
            } else {
                if(this.curState == State.Attack || this.curState == State.Dead)
                    return;

                UIManager.Instance.OpenUI("monster-info", this.uiRoot).GetComponent<UIMonsterInfo>().Open(this);

                var diff = this.player.transform.position - this.transform.position;
                if(diff.x > 0)
                    SetDirection(Direction.Right);
                else if(diff.x < 0)
                    SetDirection(Direction.Left);

                this.Interacting = true;

                this.curState = State.Idle;

                OpenMovePanel();
            }
        }

        public void OnCatch() {
            this.MonsterType = MonsterType.Friendly;

            this.hpBar.SetType(this.MonsterType);
            this.pressSpacebar.SetType(this.MonsterType);

            GameController.Instance.SetMonsterAmountsUI();

            this.TargetBuilding = GameController.Instance.Buildings.OrderBy(e => Mathf.Abs(PosX - e.PosX)).First();

            OnInteract();
        }

        public void OnTargetBuildingDestroyed() {
            this.TargetBuilding = GameController.Instance.Buildings.OrderBy(e => Mathf.Abs(PosX - e.PosX)).First();
        }

        public void OnLevelUp() {
            this.level++;
            this.CurHp = CurStatus.Hp;
            this.hpBar.Init(CurHp);

            OnInteract();
        }

        public void OnEvolution() {
            GameController.Instance.GenerateMonster(Info.EvoutionResult, this.PosX, Game.MonsterType.Friendly);

            if(TargetBuilding != null) TargetBuilding.HideRange();

            GameController.Instance.Player.ResetInteractableTarget();

            this.OnDead?.Invoke(this);

            OnInteract();

            Destroy(this.gameObject);
        }

        private void OpenMovePanel() {
            this.panelMove.SetActive(true);
            this.TargetBuilding?.ShowRange();

            var orderedBuildings = GameController.Instance.Buildings.OrderBy(e => e.PosX).ToList();
            int index = orderedBuildings.IndexOf(this.TargetBuilding);

            if(index == 0) {
                buttonLeft.SetActive(false);
            } else {
                buttonLeft.SetActive(true);
            }

            if(index == orderedBuildings.Count - 1) {
                buttonRight.SetActive(false);
            } else {
                buttonRight.SetActive(true);
            }
        }

        public void OnMove(string direction) {
            if(direction == "Left") this.OnMove(Direction.Left);
            else this.OnMove(Direction.Right);
        }

        public void OnMove(Direction direction) {
            var orderedBuildings = GameController.Instance.Buildings.OrderBy(e => e.PosX).ToList();
            int index = orderedBuildings.IndexOf(this.TargetBuilding);

            TargetBuilding.HideRange();

            if(direction == Direction.Left) {
                TargetBuilding = orderedBuildings[index - 1];
            } else {
                TargetBuilding = orderedBuildings[index + 1];
            }

            OnInteract();
        }

        private IEnumerator FSM() {
            while(true) {
                switch(this.curState) {
                case State.Attack:
                    yield return Attack();
                    break;
                case State.Move:
                    yield return Move();
                    break;
                case State.Idle:
                    yield return Idle();
                    break;
                case State.Dead:
                    yield return Dead();
                    break;
                default:
                    yield return null;
                    break;
                }
            }
        }

        private IEnumerator Idle() {
            this.rigid.velocity = Vector2.zero;
            this.anim.AnimationState.SetAnimation(0, "Idle", true);
            while(this.curState == State.Idle) {
                yield return null;
            }
            Debug.Log("End Idle");
        }

        private IEnumerator Attack() {
            this.rigid.velocity = Vector2.zero;

            var first = this.enemies.OrderBy(e => Mathf.Abs((this.transform.position.x) - e.transform.position.x)).First();
            if(first.transform.position.x - this.transform.position.x < 0) {
                this.SetDirection(Direction.Left);
            } else {
                this.SetDirection(Direction.Right);
            }
            this.anim.AnimationState.ClearTracks();
            this.anim.AnimationState.SetAnimation(0, "Attack", false);

            float timer = this.CurStatus.AttackSpeed;
            while(timer > 0) {
                timer -= Time.fixedDeltaTime;
                yield return new WaitForFixedUpdate();
                if(this.curState != State.Attack)
                    break;
            }
            if(this.curState == State.Attack) {
                try {
                    if(this.enemies.Count > 0) {
                        var enemy = this.enemies.OrderBy(e => Mathf.Abs(e.transform.position.x - PosX)).First();
                        enemy.OnDamaged(this.CurStatus.Atk);
                    }
                }
                catch(Exception e) {
                    Debug.LogError(e.Message);
                }
            }
        }

        private IEnumerator Move() {
            Direction direction;
            float timer = 0f;

            if(MonsterType == MonsterType.Friendly && this.TargetBuilding != null) {
                var enemy = GameController.Instance.Enemies.Where(e => e.transform.position.x > TargetBuilding.RangeLeft && e.transform.position.x < TargetBuilding.RangeRight).OrderBy(e => Mathf.Abs(e.transform.position.x - this.transform.position.x)).FirstOrDefault();

                if(enemy == default(Enemy)) {
                    direction = (Direction)UnityEngine.Random.Range(0, 2);
                    timer = UnityEngine.Random.Range(1f, 3f);
                } else {
                    var diff = enemy.transform.position.x - this.transform.position.x;
                    if(diff < 0)
                        direction = Direction.Left;
                    else
                        direction = Direction.Right;
                    timer = 10f;
                }
            } else {
                direction = (Direction)UnityEngine.Random.Range(0, 2);
                timer = UnityEngine.Random.Range(1f, 3f);
            }

            SetDirection(direction);

            this.rigid.velocity = (this.curDirection == Direction.Left ? Vector2.left : Vector2.right) * this.CurStatus.MoveSpeed;
            this.anim.AnimationState.SetAnimation(0, "Run", true);

            var rangeRight = this.MonsterType == MonsterType.Wild ? startPos.x + 1f : TargetBuilding.RangeRight;
            var rangeLeft = this.MonsterType == MonsterType.Wild ? startPos.x - 1f : TargetBuilding.RangeLeft;

            while(true) {
                if(this.curState != State.Move) {
                    break;
                }

                if(this.transform.position.x > rangeRight && direction == Direction.Right)
                    break;
                if(this.transform.position.x < rangeLeft && direction == Direction.Left)
                    break;
                timer -= Time.fixedDeltaTime;

                if(timer < 0)
                    break;
                yield return new WaitForFixedUpdate();
            }
        }

        private IEnumerator Dead() {
            var entry = this.anim.AnimationState.SetAnimation(0, "Die", false);
            yield return new WaitForSpineAnimationComplete(entry);
            Destroy(this.gameObject);
            yield break;
        }

        public void OnDamaged(float damage) {
            this.CurHp -= damage;
            this.hpBar.SetHp(this.CurHp);
            if(this.CurHp <= 0f) {
                this.curState = State.Dead;
                this.OnDead?.Invoke(this);
                Debug.Log("Dead");
            }
        }

        private void OnTriggerEnter2D(Collider2D collision) {
            switch(collision.tag) {
            case "Player":
                var player = collision.GetComponent<PlayerController>();
                if(player != null) {
                    this.player = player;
                    this.pressSpacebar.SetActive(true);
                    this.uiRoot.gameObject.SetActive(true);
                }
                break;
            case "Enemy":
                this.curState = State.Attack;
                var enemy = collision.GetComponent<Enemy>();
                if(this.enemies.Contains(enemy)) return;
                this.enemies.Add(enemy);
                enemy.OnDead += (enemy) => {
                    if(this.enemies.Contains(enemy)) {
                        this.enemies.Remove(enemy);
                        if(this.enemies.Count == 0) {
                            if(this.MonsterType == MonsterType.Wild) {
                                this.curState = State.Idle;
                            } else if(this.MonsterType == MonsterType.Friendly) {
                                this.curState = State.Move;   
                            }
                        }
                    }
                };
                break;
            }

        }

        private void OnTriggerExit2D(Collider2D collision) {
            switch(collision.tag) {
            case "Player":
                var player = collision.GetComponent<PlayerController>();
                if(this.player == player) {
                    this.player = null;
                    this.uiRoot.gameObject.SetActive(false);

                    //if(this.catchPanel != null && this.catchPanel.transform.parent == this.uiRoot) {
                    //    UIManager.Instance.CloseUI("catch");
                    //}

                    //if(this.monsterInfoPanel != null && this.monsterInfoPanel.transform.parent == this.uiRoot) {
                    //    UIManager.Instance.CloseUI("monsterInfo");
                    //}

                    this.curState = State.Move;
                }
                break;
            case "Enemy":
                var enemy = collision.GetComponent<Enemy>();
                if(this.enemies.Contains(enemy)) {
                    this.enemies.Remove(enemy);
                    if(this.enemies.Count == 0) {
                        if(this.MonsterType == MonsterType.Wild) {
                            this.curState = State.Idle;
                        } else if(this.MonsterType == MonsterType.Friendly) {
                            this.curState = State.Move;
                        }
                    }
                }
                break;
            }
        }

        private void SetDirection(Direction direction) {
            this.curDirection = direction;
            this.skeletonTransform.rotation = Quaternion.Euler(0f, this.defaultDirection == direction ? 0f : 180f, 0f);

            this.uiRoot.anchoredPosition = new Vector2(this.rootCanvasPosition.x * (direction == Direction.Right ? 1 : -1), this.rootCanvasPosition.y);
        }

        public void ShowSpacebar() {
            this.pressSpacebar.SetActive(true);
        }

        public void HideSpacebar() {
            this.pressSpacebar.SetActive(false);
        }
    }
}