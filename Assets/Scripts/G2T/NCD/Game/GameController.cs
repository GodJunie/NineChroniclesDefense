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
        // Serialized Members
        #region Serialized Members
        [TabGroup("group", "오브젝트")]
        [SerializeField]
        private List<BackgroundGroup> backgrounds;
        [TabGroup("group", "오브젝트")]
        [SerializeField]
        private PlayerController player;

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
        #endregion

        public List<ItemData> Items { get; private set; }
        public List<Enemy> Enemies { get; private set; }
        public List<Monster> Monsters { get; private set; }
        public List<BuildingBase> Buildings { get; private set; }
        public float RangeLeft { get; private set; }
        public float RangeRight { get; private set; }

        public PlayerController Player { get => player; }
       

        protected override void Awake() {
        
        }

        private void Start() {

        }

        // Update is called once per frame
        private void Update() {
            
        }

        public void Retry() {

        }

        #region Item
        public void AddItem(int id, int count) {
            var info = TableLoader.Instance.ItemTable.Datas.Find(e => e.Id == id);
            if(info == null) {
                throw new Exception(string.Format("{0} 에 해당하는 아이템이 없음", id));
            }
        }
        #endregion

        #region Building
        public void OnConstructBuilding(int id) {

        }

        public void AddBuilding() {

        }
        #endregion

        #region Data
        public void LoadGame() {

        }

        public void GameStart() {


            RangeLeft = minRangeLeft;
            RangeRight = minRangeRight;
        }
        #endregion

    }
}
