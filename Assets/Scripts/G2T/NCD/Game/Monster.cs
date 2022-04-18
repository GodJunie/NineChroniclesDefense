// System
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

    public enum MonsterType : int { None = 0, Neutrality, Friendly }

    [RequireComponent(typeof(Rigidbody2D))]
    public class Monster : MonoBehaviour {
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

        // Settings
        [TabGroup("group", "설정")]
        [LabelText("이름")]
        [SerializeField]
        private new string name;
        [TabGroup("group", "설정")]
        [SerializeField]
        private Sprite icon;
        [TabGroup("group", "설정")]
        [LabelText("기본 방향 (스파인)")]
        [SerializeField]
        private Direction defaultDirection;
        [TabGroup("group", "설정")]
        [LabelText("레벨 정보 (스탯, 필요 아이템)")]
        [SerializeField]
        private List<MonsterLevelInfo> levelInfos;

        private Direction curDirection;
       
        // Private Members
        // Components
        private Rigidbody2D rigid;
        private new Transform transform;
       
        // Game
        private State curState;
        private MonsterType monsterType;
        private int level;
        private float curHp;

        // objects
        private List<Enemy> enemies = new List<Enemy>();
        private PlayerController player = null;

        // Events
        public Action<Monster> OnDead;
        public Action<Monster> OnCatch;
        public Action<Monster> OnLevelUp;

        // 나중에 바꾸기
        private UICatchInfoPanel catchPanel;
        private UIMonsterInfoPanel monsterInfoPanel;

        // Getter
        public Sprite Icon { get => this.icon; }
        public MonsterType CurrentMonsterType { get => this.monsterType; }
        public List<MonsterLevelInfo> LevelInfos { get => this.levelInfos; }
        public string Name { get => this.name; }
        public int Level { get => this.level; }
        public Status CurStatus { get => this.levelInfos[Mathf.Clamp(this.level - 1, 0, this.levelInfos.Count - 1)].Status; }

        private void Awake() {
            this.rigid = GetComponent<Rigidbody2D>();
            this.transform = GetComponent<Transform>();
        }

        // Start is called before the first frame update
        void Start() {
            this.monsterType = MonsterType.Neutrality;
            
            this.curHp = this.CurStatus.Hp;
            this.hpBar.SetType(this.monsterType);
            this.hpBar.Init(CurStatus.Hp);
            this.pressSpacebar.SetType(this.monsterType);
            this.pressSpacebar.SetActive(false);

            this.SetDirection(Direction.Right);

            this.curState = State.Idle;

            this.level = 0;

            this.StartFSM();
        }

        // Update is called once per frame
        void Update() {
            if(this.player != null && this.curState == State.Idle) {
                var diff = this.player.transform.position - this.transform.position;
                if(diff.x > 0)
                    SetDirection(Direction.Right);
                else if(diff.x < 0)
                    SetDirection(Direction.Left);
            }
        }

        private void StartFSM() {
            StartCoroutine(FSM());
        }

        public void OnInteraction() {
            if(this.curState == State.Attack || this.curState == State.Dead)
                return;

            if(this.pressSpacebar.gameObject.activeInHierarchy) {
                this.curState = State.Idle;
                if(this.monsterType == MonsterType.Neutrality) {
                    this.OpenCatchPanel();
                } else {
                    this.OpenInfoPanel();
                }
            } else {
                // 이미 상호작용 중
                return;
            }
        }

        private void OpenCatchPanel() {
            this.catchPanel = UIManager.Instance.OpenUI("catch", this.uiRoot).GetComponent<UICatchInfoPanel>();
            this.catchPanel.Open(this.levelInfos[0].Needs, () => {
                this.level = 1;
                this.monsterType = MonsterType.Friendly;
                this.hpBar.SetType(this.monsterType);
                this.pressSpacebar.SetType(this.monsterType);
                this.curState = State.Move;
                this.OnCatch?.Invoke(this);
                UIManager.Instance.CloseUI("catch");
            });
        }

        private void OpenInfoPanel() {
            this.monsterInfoPanel = UIManager.Instance.OpenUI("monsterInfo", this.uiRoot).GetComponent<UIMonsterInfoPanel>();
            this.monsterInfoPanel.Open(this, () => {
                this.level++;
                this.hpBar.Init(this.CurStatus.Hp);
                this.curState = State.Move;
                this.OnLevelUp?.Invoke(this);
                UIManager.Instance.CloseUI("monsterInfo");
            });
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
                        foreach(var monster in this.enemies) {
                            if(monster != null)
                                monster.OnDamaged(this.CurStatus.Atk);
                        }
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

            var enemy = GameController.Instance.Enemies.OrderBy(e => Mathf.Abs(e.transform.position.x - this.transform.position.x)).FirstOrDefault();

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

            SetDirection(direction);

            this.rigid.velocity = (this.curDirection == Direction.Left ? Vector2.left : Vector2.right) * this.CurStatus.MoveSpeed;
            this.anim.AnimationState.SetAnimation(0, "Run", true);

            while(true) {
                if(this.curState != State.Move) {
                    break;
                }
                if(this.transform.position.x > GameController.Instance.RangeRight && direction == Direction.Right)
                    break;
                if(this.transform.position.x < GameController.Instance.RangeLeft && direction == Direction.Left)
                    break;
                timer -= Time.fixedDeltaTime;

                if(timer < 0)
                    break;
                yield return new WaitForFixedUpdate();
            }
        }

        private IEnumerator Dead() {
            var entry = this.anim.AnimationState.SetAnimation(0, "Die", false);
            Destroy(this.gameObject);
            yield break;
        }

        public void OnDamaged(float damage) {
            this.curHp -= damage;
            this.hpBar.SetHp(this.curHp);
            if(this.curHp <= 0f) {
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
                            if(this.monsterType == MonsterType.Neutrality) {
                                this.curState = State.Idle;

                            } else if(this.monsterType == MonsterType.Friendly) {
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
                    this.pressSpacebar.SetActive(false);
                    this.uiRoot.gameObject.SetActive(false);

                    if(this.catchPanel != null && this.catchPanel.transform.parent == this.uiRoot) {
                        UIManager.Instance.CloseUI("catch");
                    }
                    if(this.monsterInfoPanel != null && this.monsterInfoPanel.transform.parent == this.uiRoot) {
                        UIManager.Instance.CloseUI("monsterInfo");
                    }

                    if(this.monsterType == MonsterType.Neutrality) {
                        this.curState = State.Idle;

                    } else if(this.monsterType == MonsterType.Friendly) {
                        this.curState = State.Move;
                    }
                }
                break;
            case "Enemy":
                var enemy = collision.GetComponent<Enemy>();
                if(this.enemies.Contains(enemy)) {
                    this.enemies.Remove(enemy);
                    if(this.enemies.Count == 0) {
                        if(this.monsterType == MonsterType.Neutrality) {
                            this.curState = State.Idle;

                        } else if(this.monsterType == MonsterType.Friendly) {
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
    }
}