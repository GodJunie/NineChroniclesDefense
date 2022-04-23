// System
using System.Collections;
using System.Collections.Generic;
// UnityEngine 
using UnityEngine;
using UnityEngine.UI;
// Editor
using Sirenix.OdinInspector;

namespace G2T.NCD.Screen {
    using Table;

    public class MainScreen : MonoBehaviour {
        [TitleGroup("UI")]
        // Stage Select
        [TabGroup("UI/group", "Stage Select")]
        [SerializeField]
        private List<Button> stageButtons;
        [TabGroup("UI/group", "Stage Select")]
        [SerializeField]
        private List<Image> stageBackgroundImages;
        // Stage Info
        [TabGroup("UI/group", "Stage Info")]
        [SerializeField]
        private GameObject panelStageInfo;
        [TabGroup("UI/group", "Stage Info")]
        [SerializeField]
        private Transform stageInfoMonstersContainer;
        [TabGroup("UI/group", "Stage Info")]
        [SerializeField]
        private Transform stageInfoEnemiesContainer;
        [TabGroup("UI/group", "Stage Info")]
        [SerializeField]
        private Transform stageInfoDeckContainer;
        //[TabGroup("UI/group", "Stage Info")]
        //[SerializeField]
        //private UIMonsterSlot monsterSlot;

        private StageTable stageTable;
        private StageInfo currentStage;

        private void Awake() {
            InitStageSelectUI();
        }

        // Start is called before the first frame update
        void Start() {
            
        }

        // Update is called once per frame
        void Update() {

        }

        private void InitStageSelectUI() {
            this.stageTable = TableLoader.Instance.StageTable;

            for(int i = 0; i < stageTable.Datas.Count; i++) {
                var data = stageTable.Datas[i];
                var backgroundImage = this.stageBackgroundImages[i];
                var button = this.stageButtons[i];

                // 버튼의 배경 이미지 로드
                var path = data.BackgroundPath.Replace("Assets/Resources/", "");
                path = path.Replace(".png", "");
                
                Debug.Log(path);
                var background = Resources.Load<Sprite>(path);
                backgroundImage.sprite = background;
                
                button.onClick.AddListener(() => {
                    SelectStage(data);
                });
            }
        }

        private void SelectStage(StageInfo data) {
            Debug.Log(string.Format("Select Stage id: {0}", data.Id));
            //panelStageInfo.SetActive(true);
            //this.currentStage = data;
        
            GameManager.Instance.GameStart(data);
        }

        public void GameStart() {
            GameManager.Instance.GameStart(this.currentStage);
        }
    }
}