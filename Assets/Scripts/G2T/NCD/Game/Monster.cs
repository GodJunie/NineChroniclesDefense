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

        //[TabGroup("group", "UI")]
        //[SerializeField]
        //private Image imageIcon;
        //[TabGroup("group", "UI")]
        //[SerializeField]
        //private GameObject panel;
        //[TabGroup("group", "UI")]
        //[SerializeField]
        //private GameObject panelInfo;
        //[TabGroup("group", "UI")]
        //[SerializeField]
        //private GameObject panelCatch;
        //[TabGroup("group", "UI")]
        //[SerializeField]
        //private GameObject panelLevelUp;
        //[TabGroup("group", "UI")]
        //[SerializeField]
        //private GameObject panelEvolution;

        [TabGroup("group", "UI")]
        [SerializeField]
        private GameObject panelMove;
        [TabGroup("group", "UI")]
        [SerializeField]
        private GameObject buttonLeft;
        [TabGroup("group", "UI")]
        [SerializeField]
        private GameObject buttonRight;

        // Settings
        //[TabGroup("group", "설정")]
        //[LabelText("이름")]
        //[SerializeField]
        //private new string name;
        //[TabGroup("group", "설정")]
        //[SerializeField]
        //private Sprite icon;
        [TabGroup("group", "설정")]
        [LabelText("기본 방향 (스파인)")]
        [SerializeField]
        private Direction defaultDirection;
        [TabGroup("group", "설정")]
        [LabelText("아이디")]
        [SerializeField]
        private int id;
        public int Id { get => id; }

        private MonsterStatusTable statusTable;

        //[BoxGroup("group/UI/Info")]
        //[SerializeField]
        //private Text textName;
        //[BoxGroup("group/UI/Info")]
        //[SerializeField]
        //private Text textLevel;
        //[BoxGroup("group/UI/Info")]
        //[SerializeField]
        //private StatusTextGroup statusTextGroup;

        //[BoxGroup("group/UI/Catch")]
        //[SerializeField]
        //private Transform catchSlotContainer;
        //[BoxGroup("group/UI/Catch")]
        //[SerializeField]
        //private UIItemSlot itemSlotPerfab;
        //[BoxGroup("group/UI/Catch")]
        //[SerializeField]
        //private Button catchButton;

        //[BoxGroup("group/UI/LevelUp")]
        //[SerializeField]
        //private Transform levelUpSlotContainer;
        //[BoxGroup("group/UI/LevelUp")]
        //[SerializeField]
        //private Text textLevelBefore;
        //[BoxGroup("group/UI/LevelUp")]
        //[SerializeField]
        //private Text textLevelAfter;
        //[BoxGroup("group/UI/LevelUp")]
        //[SerializeField]
        //private Button buttonLevelUp;
        //[BoxGroup("group/UI/LevelUp")]
        //[SerializeField]
        //private Text buttonTextLevelUp;

        //[BoxGroup("group/UI/Evolution")]
        //[SerializeField]
        //private Transform evolutionSlotContainer;
        //[BoxGroup("group/UI/Evolution")]
        //[SerializeField]
        //private Button buttonEvolution;
        //[BoxGroup("group/UI/Evolution")]
        //[SerializeField]
        //private UISlot evolutionResultSlot;

        private Direction curDirection;
       
        // Private Members
        // Components
        private Rigidbody2D rigid;
        private new Transform transform;
       
        // Game
        private State curState;
        private MonsterType monsterType;
        private int level;
        public float CurHp { get; private set; }

        // objects
        private List<Enemy> enemies = new List<Enemy>();
        private PlayerController player = null;

        // Events
        public Action<Monster> OnDead;
        
        public MonsterType CurrentMonsterType { get => this.monsterType; }
        public string Name { get => this.name; }
        public int Level { get => this.level; }
        public Status CurStatus { get => statusTable.Datas[level].Status; }

        public float PosX => this.transform.position.x;

        public bool Interacting { get; private set; }

        public MonsterInfo Info { get; private set; }

        private Vector3 startPos;
        public BuildingBase TargetBuilding { get; private set; }

        public void Init(MonsterInfo info, MonsterType monsterType) {
            this.Info = info;
            this.textName.text = info.Name;
            this.level = 0;
            this.monsterType = monsterType;
            this.startPos = this.transform.position;

            this.SetDirection(Direction.Right);

            this.curState = State.Move;

            var iconPath = info.IconPath;
            iconPath = iconPath.Replace("Assets/Resources/", "").Replace(Path.GetExtension(iconPath), "");

            var icon = Resources.Load<Sprite>(iconPath);

            imageIcon.sprite = icon;

            var statusPath = info.StatusPath;
            Debug.Log(statusPath);

            statusPath = statusPath.Replace("Assets/Resources/", "").Replace(Path.GetExtension(statusPath), "");

            Debug.Log(statusPath);

            this.statusTable = Resources.Load<ScriptableObject>(statusPath) as MonsterStatusTable;

            this.CurHp = this.CurStatus.Hp;
            this.hpBar.SetType(this.monsterType);
            this.hpBar.Init(CurStatus.Hp);
            this.pressSpacebar.SetType(this.monsterType);
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
            //if(this.player != null && this.curState == State.Idle) {
            //    var diff = this.player.transform.position - this.transform.position;
            //    if(diff.x > 0)
            //        SetDirection(Direction.Right);
            //    else if(diff.x < 0)
            //        SetDirection(Direction.Left);
            //}
        }

        private void StartFSM() {
            StartCoroutine(FSM());
        }

        public void OnSpacebar() {
            if(this.Interacting) {
                ClosePanel();
                return;
            }

            if(this.curState == State.Attack || this.curState == State.Dead)
                return;

            this.curState = State.Idle;
               
            //this.OpenPanel();
        }

        //private void OpenPanel() {
        //    this.panel.SetActive(true);
        //    if(this.monsterType == MonsterType.Wild) {
        //        this.OpenCatchPanel();
        //    } else {
        //        this.OpenInfoPanel();
        //        this.OpenMovePanel();
        //    }
               
        //    var diff = this.player.transform.position - this.transform.position;
        //    if(diff.x > 0)
        //        SetDirection(Direction.Right);
        //    else if(diff.x < 0)
        //        SetDirection(Direction.Left);

        //    this.Interacting = true;
        //}

        //private async void OpenCatchPanel() {
        //    this.panelCatch.SetActive(true);

        //    for(int i = 0; i < catchSlotContainer.childCount; i++) {
        //        Destroy(catchSlotContainer.GetChild(i).gameObject);
        //    }
        //    foreach(var item in Info.CatchMaterials) {
        //        var slot = Instantiate(itemSlotPerfab, this.catchSlotContainer);

        //        var itemData = TableLoader.Instance.ItemTable.Datas.Find(e => e.Id == item.Id);

        //        var icon = await ResourcesManager.Instance.LoadAsync<Sprite>(itemData.IconPath);

        //        //slot.IconImage.sprite = icon;

        //        var ownedItem = GameController.Instance.Items.Find(e => e.Id == itemData.Id);
        //        int count = ownedItem == null ? 0 : ownedItem.Count;

        //        //slot.CountText.text = string.Format("{0}/{1}", count, item.Amount);

        //        slot.SetUI(icon, count, item.Amount);
        //    }
        //}

        public void OnCatch() {
            foreach(var item in Info.CatchMaterials) {
                var ownedItem = GameController.Instance.Items.Find(e => e.Id == item.Id);
                int count = ownedItem == null ? 0 : ownedItem.Count;

                if(count < item.Amount) return;
            }

            foreach(var item in Info.CatchMaterials) {
                GameController.Instance.UseItem(item.Id, item.Amount);
            }

            this.monsterType = MonsterType.Friendly;

            this.hpBar.SetType(this.monsterType);
            this.pressSpacebar.SetType(this.monsterType);

            GameController.Instance.SetMonsterAmountsUI();

            this.TargetBuilding = GameController.Instance.Buildings.OrderBy(e => Mathf.Abs(PosX - e.PosX)).First();

            this.ClosePanel();
        }

        //private void OpenInfoPanel() {
        //    this.panelInfo.SetActive(true);

        //    this.statusTextGroup.InitUI(this.CurStatus);
        //    this.textLevel.text = string.Format("Lv. {0}", this.level + 1);
        //}

        public void OnTargetBuildingDestroyed() {
            this.TargetBuilding = GameController.Instance.Buildings.OrderBy(e => Mathf.Abs(PosX - e.PosX)).First();
        }

        //public void OnOpenLevelUpPanel() {
        //    if(this.level == statusTable.Datas.Count - 1) {
        //        this.OpenEvolutionPanel();
        //    } else {
        //        this.OpenLevelUpPanel();
        //    }
        //}

        //private async void OpenLevelUpPanel() {
        //    this.panelLevelUp.SetActive(true);
        //    this.panelInfo.SetActive(false);

        //    this.textLevelBefore.text = string.Format("Lv. {0}", level + 1);
        //    this.textLevelAfter.text = string.Format("Lv. {0}", level + 2);

        //    for(int i = 0; i < levelUpSlotContainer.childCount; i++) {
        //        Destroy(levelUpSlotContainer.GetChild(i).gameObject);
        //    }

        //    foreach(var item in statusTable.Datas[this.level].LevelUpItems) {
        //        var slot = Instantiate(itemSlotPerfab, this.levelUpSlotContainer);

        //        var itemData = TableLoader.Instance.ItemTable.Datas.Find(e => e.Id == item.Id);

        //        var icon = await ResourcesManager.Instance.LoadAsync<Sprite>(itemData.IconPath);

        //        //slot.IconImage.sprite = icon;

        //        var ownedItem = GameController.Instance.Items.Find(e => e.Id == itemData.Id);
        //        int count = ownedItem == null ? 0 : ownedItem.Count;

        //        //slot.CountText.text = string.Format("{0}/{1}", count, item.Amount);

        //        slot.SetUI(icon, count, item.Amount);
        //    }
        //}

        public void OnLevelUp() {
            var data = statusTable.Datas[this.level];
            foreach(var item in data.LevelUpItems) {
                var ownedItem = GameController.Instance.Items.Find(e => e.Id == item.Id);
                int count = ownedItem == null ? 0 : ownedItem.Count;

                if(count < item.Amount) return;
            }

            foreach(var item in data.LevelUpItems) {
                GameController.Instance.UseItem(item.Id, item.Amount);
            }

            this.level++;
            this.CurHp = CurStatus.Hp;
            this.hpBar.Init(CurHp);

            if(this.level == statusTable.Datas.Count - 1) {
                this.buttonTextLevelUp.text = "Evolution";
            }

            this.ClosePanel();
        }

        private List<Monster> evolMat = new List<Monster>();

        //private async void OpenEvolutionPanel() {
        //    this.panelInfo.SetActive(false);
        //    this.panelEvolution.SetActive(true);

        //    evolMat.Clear();

        //    for(int i = 0; i < evolutionSlotContainer.childCount; i++) {
        //        Destroy(evolutionSlotContainer.GetChild(i).gameObject);
        //    }

        //    var data = TableLoader.Instance.MonsterTable.Datas.Find(e => e.Id == id);

        //    var resultIconPath = TableLoader.Instance.MonsterTable.Datas.Find(e => e.Id == data.EvoutionResult).IconPath;
        //    resultIconPath = resultIconPath.Replace("Assets/Resources/", "").Replace(Path.GetExtension(resultIconPath), "");

        //    var resultIcon = Resources.Load<Sprite>(resultIconPath);

        //    //this.evolutionResultSlot.IconImage.sprite = resultIcon;
        //    //this.evolutionResultSlot.CountText.text = "";

        //    this.evolutionResultSlot.SetUI(resultIcon);

        //    foreach(var mat in Info.EvolutionMaterials) {
        //        var itemData = TableLoader.Instance.ItemTable.Datas.Find(e => e.Id == mat.Id);
        //        if(itemData != null) {
        //            var slot = Instantiate(itemSlotPerfab, this.evolutionSlotContainer);

        //            var icon = await ResourcesManager.Instance.LoadAsync
        //                <Sprite>(itemData.IconPath);

        //            //slot.IconImage.sprite = icon;

        //            var ownedItem = GameController.Instance.Items.Find(e => e.Id == itemData.Id);
        //            int count = ownedItem == null ? 0 : ownedItem.Count;

        //            //slot.CountText.text = string.Format("{0}/{1}", count, mat.Amount);

        //            slot.SetUI(icon, count, mat.Amount);

        //            continue;
        //        }

        //        var monsterData = TableLoader.Instance.MonsterTable.Datas.Find(e => e.Id == mat.Id);
        //        if(monsterData != null) {
        //            var slot = Instantiate(itemSlotPerfab, this.evolutionSlotContainer);

        //            var icon = await ResourcesManager.Instance.LoadAsync<Sprite>(monsterData.IconPath);

        //            //slot.IconImage.sprite = icon;

        //            //slot.CountText.text = string.Format("{0}/{1}", 0, mat.Amount);

        //            slot.SetUI(icon, 0, mat.Amount, () => {
        //                GameController.Instance.OpenMonsterPanel(GameController.Instance.Monsters.Where(e => e.monsterType == MonsterType.Friendly && e.Id == mat.Id && !evolMat.Contains(e)).ToList(), monster => {
        //                    this.evolMat.Add(monster);
        //                    //slot.CountText.text = string.Format("{0}/{1}", evolMat.Count(e => e.Id == mat.Id), mat.Amount);
        //                });
        //            });
        //        }
        //    }
        //}

        public void OnEvolution() {
            var data = TableLoader.Instance.MonsterTable.Datas.Find(e => e.Id == id);

            var items = data.EvolutionMaterials.Where(e => TableLoader.Instance.ItemTable.Datas.Find(i => i.Id == e.Id) != null);

            var monsters = data.EvolutionMaterials.Where(e => TableLoader.Instance.MonsterTable.Datas.Find(m => m.Id == e.Id) != null);

            foreach(var item in items) {
                var ownedItem = GameController.Instance.Items.Find(e => e.Id == item.Id);
                int count = ownedItem == null ? 0 : ownedItem.Count;

                if(count < item.Amount) return;
            }

            foreach(var monster in monsters) {
                if(evolMat.Count(e => e.Id == monster.Id) < monster.Amount)
                    return;
            }

            foreach(var item in items) {
                GameController.Instance.UseItem(item.Id, item.Amount);
            }

            foreach(var monster in evolMat) {
                monster.OnDead?.Invoke(monster);
                Destroy(monster.gameObject);
            }

            GameController.Instance.GenerateMonster(data.EvoutionResult, this.PosX, MonsterType.Friendly);

            if(TargetBuilding != null) TargetBuilding.HideRange();

            GameController.Instance.Player.ResetInteractableTarget();

            this.OnDead?.Invoke(this);
            Destroy(this.gameObject);
        }

        private void OpenMovePanel() {
            this.panelMove.SetActive(true);
            this.TargetBuilding.ShowRange();

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
            var orderedBuildings = GameController.Instance.Buildings.OrderBy(e => e.PosX).ToList();
            int index = orderedBuildings.IndexOf(this.TargetBuilding);

            TargetBuilding.HideRange();

            if(direction == "Left") {
                TargetBuilding = orderedBuildings[index - 1];
            } else {
                TargetBuilding = orderedBuildings[index + 1];
            }

            ClosePanel();
        }

        public void ClosePanel() {
            this.Interacting = false;

            this.panel.SetActive(false);
            this.panelInfo.SetActive(false);
            this.panelCatch.SetActive(false);
            this.panelLevelUp.SetActive(false);
            this.panelMove.SetActive(false);

            if(TargetBuilding != null)
                TargetBuilding.HideRange();

            this.curState = State.Move;
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

            if(monsterType == MonsterType.Friendly && this.TargetBuilding != null) {
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

            var rangeRight = this.monsterType == MonsterType.Wild ? startPos.x + 1f : TargetBuilding.RangeRight;
            var rangeLeft = this.monsterType == MonsterType.Wild ? startPos.x - 1f : TargetBuilding.RangeLeft;

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
                            if(this.monsterType == MonsterType.Wild) {
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
                        if(this.monsterType == MonsterType.Wild) {
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

        public void ShowSpacebar() {
            this.pressSpacebar.SetActive(true);
        }

        public void HideSpacebar() {
            this.pressSpacebar.SetActive(false);
        }
    }
}