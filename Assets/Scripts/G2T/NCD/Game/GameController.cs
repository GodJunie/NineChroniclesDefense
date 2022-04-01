// System
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
// UnityEngine
using UnityEngine;
using UnityEngine.UI;
// Other
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;

namespace G2T.NCD.Game {
    using Data;
    using Table;

    public class GameController : SingletonBehaviour<GameController> {
        // Class
        #region Classes
        [Serializable]
        public class EnemySpawnInfo {
            [HorizontalGroup("group", 50f)]
            [BoxGroup("group/시간")]
            [HideLabel]
            [SerializeField]
            private float time;
            [HorizontalGroup("group")]
            [BoxGroup("group/프리팹")]
            [HideLabel]
            [SerializeField]
            private GameObject prefab;

            public float Time { get => time; }
            public GameObject Prefab { get => prefab; }
        }

        [Serializable]
        public class MonsterSpawnInfo {
            [HorizontalGroup("group", 50f)]
            [BoxGroup("group/시간")]
            [HideLabel]
            [SerializeField]
            private float time;
            [HorizontalGroup("group", 50f)]
            [BoxGroup("group/위치")]
            [HideLabel]
            [SerializeField]
            private float x;
            [HorizontalGroup("group")]
            [BoxGroup("group/프리팹")]
            [HideLabel]
            [SerializeField]
            private GameObject prefab;

            public float Time { get => time; }
            public float X { get => x; }
            public GameObject Prefab { get => prefab; }
        }


        [Serializable]
        public class WaveInfo {
            [FoldoutGroup("웨이브")]
            [HorizontalGroup("웨이브/group", .5f)]
            [BoxGroup("웨이브/group/왼쪽")]
            [ShowIf("@LeftEnemiesExists")]
            [LabelText("왼쪽 적군 리스트")]
            [SerializeField]
            private List<EnemySpawnInfo> leftEnemies = null;
            [BoxGroup("웨이브/group/오른쪽")]
            [ShowIf("@RightEnemiesExists")]
            [LabelText("오른쪽 적군 리스트")]
            [SerializeField]
            private List<EnemySpawnInfo> rightEnemies = null;

#if UNITY_EDITOR
            [BoxGroup("웨이브/group/왼쪽")]
            [ShowIf("@!LeftEnemiesExists")]
            [Button("생성하기")]
            private void AddLeft() {
                this.leftEnemies = new List<EnemySpawnInfo>();
                this.leftEnemies.Add(new EnemySpawnInfo());
            }
            [BoxGroup("웨이브/group/왼쪽")]
            [ShowIf("@LeftEnemiesExists")]
            [Button("삭제하기")]
            private void RemoveLeft() {
                this.leftEnemies = null;
            }

            [BoxGroup("웨이브/group/오른쪽")]
            [ShowIf("@!RightEnemiesExists")]
            [Button]
            private void AddRight() {
                this.rightEnemies = new List<EnemySpawnInfo>();
                this.rightEnemies.Add(new EnemySpawnInfo());
            }
            [BoxGroup("웨이브/group/오른쪽")]
            [ShowIf("@RightEnemiesExists")]
            [Button("삭제하기")]
            private void RemoveRight() {
                this.rightEnemies = null;
            }
#endif
            public bool LeftEnemiesExists { get => this.leftEnemies != null && this.leftEnemies.Count > 0; }
            public bool RightEnemiesExists { get => this.rightEnemies != null && this.rightEnemies.Count > 0; }

            public List<EnemySpawnInfo> LeftEnemies { get => this.leftEnemies; }
            public List<EnemySpawnInfo> RightEnemies { get => this.rightEnemies; }
        }

        [Serializable]
        public class DayTimeInfo {
            [FoldoutGroup("@GroupName")]
            [BoxGroup("@GroupName/시간 정보")]
            [HorizontalGroup("@GroupName/시간 정보/group", .5f)]
            [BoxGroup("@GroupName/시간 정보/group/시간대")]
            [HideLabel]
            [SerializeField]
            private DayTimePart timePart;
            [BoxGroup("@GroupName/시간 정보/group/시간 (초)")]
            [HideLabel]
            [SerializeField]
            private float time;
            [FoldoutGroup("@GroupName")]
            [SerializeField]
            [HideLabel]
            private WaveInfo wave;

            public float Time { get => time; }
            public DayTimePart TimePart { get => timePart; }
            public WaveInfo Wave { get => wave; }

#if UNITY_EDITOR
            private string GroupName { get => string.Format("{0}/{1}초{2}{3}", this.timePart, this.time, this.wave.LeftEnemiesExists ? string.Format("/왼쪽 {0}명", this.wave.LeftEnemies.Count) : "", this.wave.RightEnemiesExists ? string.Format("/오른쪽 {0}명", this.wave.RightEnemies.Count) : ""); }
#endif
        }
        #endregion

        // Serialized Members
        #region Serialized Members
        [TabGroup("group", "오브젝트")]
        [SerializeField]
        private List<BackgroundGroup> backgrounds;
        [TabGroup("group", "오브젝트")]
        [SerializeField]
        private House house;

        [TabGroup("group", "UI")]
        [LabelText("게임 오버 패널")]
        [SerializeField]
        private GameObject panelGameOver;
        [TabGroup("group", "UI")]
        [LabelText("스테이지 클리어 패널")]
        [SerializeField]
        private GameObject panelGameClear;
        [TabGroup("group", "UI")]
        [LabelText("웨이브 텍스트")]
        [SerializeField]
        private Text textWave;

        [TabGroup("group", "런타임 데이터")]
        [ListDrawerSettings]
        [SerializeField]
        private List<ItemData> items = new List<ItemData>();

        [TabGroup("group", "게임 설정")]
        [BoxGroup("group/게임 설정/맵 크기")]
        [HorizontalGroup("group/게임 설정/맵 크기/group", .5f)]
        [LabelWidth(50f)]
        [LabelText("왼쪽")]
        [SerializeField]
        private float rangeLeft;
        [HorizontalGroup("group/게임 설정/맵 크기/group", .5f)]
        [LabelWidth(50f)]
        [LabelText("오른쪽")]
        [SerializeField]
        private float rangeRight;

        [BoxGroup("group/게임 설정/시간")]
        [LabelText("시간")]
        [SerializeField]
        private List<DayTimeInfo> days;

        [BoxGroup("group/게임 설정/몬스터")]
        [LabelText("몬스터 스폰 리스트")]
        [ListDrawerSettings(AddCopiesLastElement = true)]
        [SerializeField]
        private List<MonsterSpawnInfo> monsterSpawnList;
        #endregion

        // Private Members
        #region Private Members
        private DayTimePart curTimePart;
        private int waveIndex = 0;

        private List<Monster> monsters = new List<Monster>();
        private List<Enemy> enemies = new List<Enemy>();

        // Getter
        public float RangeLeft { get => rangeLeft; }
        public float RangeRight { get => rangeRight; }
        public List<ItemData> Items { get => items; }
        public House House { get => house; }
        public List<Enemy> Enemies { get => enemies; }
        #endregion

        protected override void Awake() {
        
        }

        private void Start() {
            this.textWave.text = string.Format("WAVE  {0} / {1}", waveIndex, this.days.Count(e => e.Wave.LeftEnemiesExists || e.Wave.RightEnemiesExists));
            StartGame();
        }

        // Update is called once per frame
        private void Update() {
            
        }

        public void CatchMonster(Monster monster) {
            this.monsters.Add(monster);
        }

        public void OnGameOver() {
            Debug.Log("Game Over!");
            this.panelGameOver.SetActive(true);
            Time.timeScale = 0f;
        }

        public void Retry() {
            UnityEngine.SceneManagement.SceneManager.LoadScene(0);
        }

        public void AddItem(int id, int count) {
            var info = TableLoader.Instance.ItemTable.Items.Find(e => e.Id == id);
            if(info == null) {
                throw new Exception(string.Format("{0} 에 해당하는 아이템이 없음", id));
            }
        }

        private async void StartGame() {
            GenerateMonster();

            foreach(var day in this.days) {
                this.curTimePart = day.TimePart;
                foreach(var bg in this.backgrounds) {
                    bg.SwitchBakcground(this.curTimePart);
                }
               
                if(day.Wave.LeftEnemiesExists) 
                    StartWave(day.Wave.LeftEnemies, Direction.Left);
                if(day.Wave.RightEnemiesExists) 
                    StartWave(day.Wave.RightEnemies, Direction.Right);

                if(day.Wave.LeftEnemiesExists || day.Wave.RightEnemiesExists)
                    waveIndex++;

                this.textWave.text = string.Format("WAVE  {0} / {1}", waveIndex, this.days.Count(e => e.Wave.LeftEnemiesExists || e.Wave.RightEnemiesExists));

                await UniTask.Delay(TimeSpan.FromSeconds(day.Time));
            }
        }

        private async void StartWave(List<EnemySpawnInfo> enemyInfoList, Direction dir) {
            foreach(var info in enemyInfoList) {
                await UniTask.Delay(TimeSpan.FromSeconds(info.Time));
                var enemy = Instantiate(info.Prefab, new Vector3(dir == Direction.Right ? rangeRight + 3f : rangeLeft - 3f, 0f, 0f) , Quaternion.identity).GetComponent<Enemy>();
                this.enemies.Add(enemy);
                enemy.OnDead += (enemy) => {
                    this.enemies.Remove(enemy);
                };
            }
        }

        private async void GenerateMonster() {
            foreach(var info in this.monsterSpawnList) {
                await UniTask.Delay(TimeSpan.FromSeconds(info.Time));
                var monster = Instantiate(info.Prefab, new Vector3(info.X, 0f, 0f), Quaternion.identity);
            }
        }
    }
}
