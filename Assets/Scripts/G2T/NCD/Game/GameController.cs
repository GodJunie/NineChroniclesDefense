// System
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.IO;
// UnityEngine
using UnityEngine;
using UnityEngine.UI;
// Other
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;

namespace G2T.NCD.Game {
    using Data;
    using UI;
    using Table;
    using Management;

    public class GameController : SingletonBehaviour<GameController> {
        // Serialized Members
        #region Serialized Members
        [TabGroup("group", "오브젝트")]
        [SerializeField]
        private List<BackgroundGroup> backgrounds;
        [TabGroup("group", "오브젝트")]
        [SerializeField]
        private PlayerController player;
        [TabGroup("group", "오브젝트")]
        [SerializeField]
        private MonsterHouse house;

        #region UI
        // 패널
        [TabGroup("group", "UI")]
        [LabelText("게임 오버 패널")]
        [SerializeField]
        private GameObject panelGameOver;
        [TabGroup("group", "UI")]
        [LabelText("스테이지 클리어 패널")]
        [SerializeField]
        private GameObject panelGameClear;
        // 텍스트
        [TabGroup("group", "UI")]
        [LabelText("몬스터 인구수 텍스트")]
        [SerializeField]
        private Text textMonstersCount;
        [TabGroup("group", "UI")]
        [LabelText("적군 수 텍스트")]
        [SerializeField]
        private Text textEnemiesCount;

        [TabGroup("group", "UI")]
        [LabelText("몬스터 패널")]
        [SerializeField]
        private UIMonsterPanel uiMonsterPanel;

        [TabGroup("group", "UI")]
        [LabelText("아이템 카운트")]
        [SerializeField]
        private UIItemCount uiItemCount;
        #endregion

        [TabGroup("group", "게임 설정")]
        [BoxGroup("group/게임 설정/맵 크기")]
        [HorizontalGroup("group/게임 설정/맵 크기/group", .5f)]
        [LabelWidth(50f)]
        [LabelText("왼쪽")]
        [SerializeField]
        private float minRangeLeft;
        [HorizontalGroup("group/게임 설정/맵 크기/group", .5f)]
        [LabelWidth(50f)]
        [LabelText("오른쪽")]
        [SerializeField]
        private float minRangeRight;

        [TabGroup("group", "게임 설정")]
        [SerializeField]
        [LabelText("제이드 하우스 아이디")]
        private int houseId;


        [TabGroup("group", "임시")]
        [SerializeField]
        private bool isTestMode;
        [SerializeField]
        private StageTimelineTable timelineTable;
        #endregion

        public List<ItemData> Items { get; private set; }
        public List<Enemy> Enemies { get; private set; }
        public List<Monster> Monsters { get; private set; }
        public List<BuildingBase> Buildings { get; private set; }
        public float RangeLeft { get; private set; }
        public float RangeRight { get; private set; }

        public float BuildingRangeLeft { get; private set; }
        public float BuildingRangeRight { get; private set; }

        public PlayerController Player { get => player; }
        public MonsterHouse House { get => house; }

        private StageInfo stage;

        private bool constructing;
        private GameObject pendingBuilding;

        private int curEnemyCount;
        private int enemyCount;

        public int MaxMonsterAmount {
            get {
                return this.Buildings.Where(e => e is MonsterHouse).Select(e => e as MonsterHouse).Sum(e => e.MonsterAmount);
            }
        }

        protected override async void Awake() {
            this.Items = new List<ItemData>();
            this.Enemies = new List<Enemy>();
            this.Monsters = new List<Monster>();
            this.Buildings = new List<BuildingBase>();

            await this.house.Init(TableLoader.Instance.BuildingTable.Datas.Find(e => e.Id == houseId));
            this.house.ConfirmBuild();
        }

        private void Start() {
            this.textMonstersCount.text = string.Format("{0}/{1}", 0, this.house.MonsterAmount);

            if(isTestMode) {
                this.stage = TableLoader.Instance.StageTable.Datas[0];
            } else {
                this.stage = GameManager.Instance.CurrentStage;
            }

            // Cheat
            var itemTable = TableLoader.Instance.ItemTable;
            foreach(var item in itemTable.Datas) {
                this.Items.Add(new ItemData(item.Id, 999));
            }

            GameStart();
        }

        // Update is called once per frame
        private void Update() {
            
        }

        public void Retry() {

        }

        private void GameOver() {
            Time.timeScale = .01f;
            panelGameOver.SetActive(true);
        }

        #region Item
        public void AddItem(int id, int count) {
            var info = TableLoader.Instance.ItemTable.Datas.Find(e => e.Id == id);
            if(info == null) {
                throw new Exception(string.Format("{0} 에 해당하는 아이템이 없음", id));
            }
            var item = this.Items.Find(e => e.Id == id);
            if(item == null) {
                this.Items.Add(new ItemData(id, count));
            } else {
                item.Count += count;
            }

            uiItemCount.EnqueueItem(id, count);
        }

        public void UseItem(int id, int count) {
            var item = this.Items.Find(e => e.Id == id);
            item.Count -= count;
            if(item.Count == 0) {
                Items.Remove(item);
            }

            uiItemCount.EnqueueItem(id, -count);
        }
        #endregion

        #region Building
        public async void OnConstructBuilding(int id) {
            var data = TableLoader.Instance.BuildingTable.Datas.Find(e => e.Id == id);

            var prefab = await ResourcesManager.Instance.LoadAsync<GameObject>(data.PrefabPath);

            pendingBuilding = Instantiate(prefab, this.player.BuildingHoder);
            pendingBuilding.transform.localPosition = Vector3.zero;
            pendingBuilding.SetActive(true);

            pendingBuilding.GetComponent<BuildingBase>().Init(data);

            foreach(var building in this.Buildings) {
                building.ShowRange();
            }
        }

        public void AddBuilding(BuildingBase building) {
            this.Buildings.Add(building);
            foreach(var b in this.Buildings) {
                b.HideRange();
            }

            BuildingRangeLeft = this.Buildings.Min(b => b.RangeLeft);
            BuildingRangeRight = this.Buildings.Max(b => b.RangeRight);

            this.RangeLeft = Mathf.Min(BuildingRangeLeft - 5f, minRangeLeft);
            this.RangeRight = Mathf.Max(BuildingRangeRight + 5f, minRangeRight);

            this.SetMonsterAmountsUI();

            building.OnDead += (e) => {
                if(e == this.house) {
                    GameOver();
                    return;
                }

                this.Buildings.Remove(e);
                Destroy(e.gameObject);
                this.SetMonsterAmountsUI();

                foreach(var monster in this.Monsters.Where(e => e.TargetBuilding == e)) {
                    monster.OnTargetBuildingDestroyed();
                }
            };
        }

        public void OnCancelConstruct() {
            Destroy(pendingBuilding);
            pendingBuilding = null;

            foreach(var building in this.Buildings) {
                building.HideRange();
            }
        }

        public void SetMonsterAmountsUI() {
            this.textMonstersCount.text = string.Format("{0}/{1}", this.Monsters.Count(e => e.MonsterType == MonsterType.Friendly), MaxMonsterAmount);
        }

        public void SetEnemyCountUI() {
            this.textEnemiesCount.text = string.Format("{0}/{1}", curEnemyCount, enemyCount);
        }
        #endregion

        #region Data
        public void LoadGame() {

        }

        public void GameStart() {
            RangeLeft = minRangeLeft;
            RangeRight = minRangeRight;

            BuildingRangeLeft = house.transform.position.x - house.Range;
            BuildingRangeRight = house.transform.position.x + house.Range;

            PlayTimeline();
        }

        #endregion

        #region Timeline
        private async void PlayTimeline() {
            foreach(var data in timelineTable.Datas) {
                foreach(var background in backgrounds) {
                    background.SwitchBakcground(data.TimePart);
                }

                GenerateMonster(data);
                GenerateEnemies(data.LeftEnemies, true);
                GenerateEnemies(data.RightEnemies, false);
                GenerateFarmingItems(data);

                curEnemyCount += data.LeftEnemies.Count;
                curEnemyCount += data.RightEnemies.Count;

                enemyCount = data.LeftEnemies.Count;
                enemyCount += data.RightEnemies.Count;

                SetEnemyCountUI();

                await UniTask.Delay(TimeSpan.FromSeconds(data.Time));
            }
        }

        private async void GenerateMonster(StageTimelineInfo data) {
            float time = data.Time;
            var times = new List<float>();
            for(int i = 0; i < data.MonsterAmount; i++) {
                times.Add(UnityEngine.Random.Range(0f, time));
            }
            times.Sort();
            
            foreach(var t in times) {
                await UniTask.Delay(TimeSpan.FromSeconds(t));

                var sum = data.Monsters.Sum(e => e.Prob);
                var rand = UnityEngine.Random.Range(0f, sum);

                foreach(var info in data.Monsters) {
                    if(rand <= info.Prob) {
                        var posX = UnityEngine.Random.Range(0, 2) == 0 ? UnityEngine.Random.Range(RangeLeft, BuildingRangeLeft) : UnityEngine.Random.Range(BuildingRangeRight, RangeRight);

                        GenerateMonster(info.Id, posX, MonsterType.Wild);
                        break;
                    }
                    rand -= info.Prob;
                }
            }
        }

        public async void GenerateMonster(int id, float posX, MonsterType monsterType) {
            var monsterData = TableLoader.Instance.MonsterTable.Datas.Find(e => e.Id == id);

            var monsterPrefab = await ResourcesManager.Instance.LoadAsync<GameObject>(monsterData.PrefabPath);

            var monster = Instantiate(monsterPrefab).GetComponent<Monster>();

            monster.transform.position = new Vector3(posX, 0f, 0f);
            monster.gameObject.SetActive(true);

            monster.Init(monsterData, monsterType);

            this.Monsters.Add(monster);

            monster.OnDead += (e) => {
                this.Monsters.Remove(e);
                this.SetMonsterAmountsUI();
            };
        }

        private async void GenerateEnemies(List<int> ids, bool isLeft) {
            foreach(var presetId in ids) {
                await UniTask.Delay(TimeSpan.FromSeconds(0.5));

                var enemyPreset = TableLoader.Instance.EnemyPresetTable.Datas.Find(e => e.Id == presetId);

                int enemyId = enemyPreset.EnemyId;

                var enemyInfo = TableLoader.Instance.EnemyTable.Datas.Find(e => e.Id == enemyId);
           
                var enemyPrefab = await ResourcesManager.Instance.LoadAsync<GameObject>(enemyInfo.PrefabPath);

                var enemy = Instantiate(enemyPrefab).GetComponent<Enemy>();

                enemy.gameObject.SetActive(true);

                enemy.Init(enemyPreset);

                enemy.transform.position = new Vector3(isLeft ? RangeLeft - 3f : RangeRight + 3f, 0f, 0f);

                this.Enemies.Add(enemy);

                enemy.OnDead += (e) => {
                    this.Enemies.Remove(e);
                    curEnemyCount--;
                };
            }
        }

        private async void GenerateFarmingItems(StageTimelineInfo data) {
            float time = data.Time;
            var times = new List<float>();
            for(int i = 0; i < data.FarmingItemAmount; i++) {
                times.Add(UnityEngine.Random.Range(0f, time));
            }
            times.Sort();

            foreach(var t in times) {
                await UniTask.Delay(TimeSpan.FromSeconds(t));

                var sum = data.FarmingItems.Sum(e => e.Prob);
                var rand = UnityEngine.Random.Range(0f, sum);

                foreach(var info in data.FarmingItems) {
                    if(rand <= info.Prob) {
                        var posX = UnityEngine.Random.Range(0, 2) == 0 ? UnityEngine.Random.Range(RangeLeft, BuildingRangeLeft) : UnityEngine.Random.Range(BuildingRangeRight, RangeRight);

                        GenerateFarmingItem(info.Id, posX);
                        break;
                    }
                    rand -= info.Prob;
                }
            }
        }

        private async void GenerateFarmingItem(int id, float posX) {
            Debug.Log(id);

            var farmingItemData = TableLoader.Instance.FarmingItemTable.Datas.Find(e => e.Id == id);

            var itemPrefab = await ResourcesManager.Instance.LoadAsync<GameObject>(farmingItemData.PrefabPath);

            var farmingItem = Instantiate(itemPrefab, new Vector3(posX, 0f, 0f), Quaternion.identity).GetComponent<FarmingItem>();

            farmingItem.Init(farmingItemData);
        }

        private void ChangeBackground(DayTimePart timePart) {
            foreach(var background in backgrounds) {
                background.SwitchBakcground(timePart);
            }
        }
        #endregion

        public void OpenMonsterPanel() {
            this.uiMonsterPanel.Open();
        }

        public void OpenMonsterPanel(List<Monster> monsters, Action<Monster> onSelect) {
            this.uiMonsterPanel.Open(monsters, onSelect);
        }
    }
}
