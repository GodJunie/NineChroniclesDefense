// System
using System.Collections;
using System.Collections.Generic;
// UnityEngine
using UnityEngine;
using UnityEngine.SceneManagement;
// Editor
using Sirenix.OdinInspector;


namespace G2T.NCD {
    using Table;

    public class GameManager : SingletonBehaviour<GameManager> {
        #region Serialized Members
        // 나중에 이름 말고 Path 로 바꿔서 Addressable 에서 로드
        [TitleGroup("Scene")]
        [SerializeField]
        private string mainSceneName;
        [TitleGroup("Scene")]
        [SerializeField]
        private string gameSceneName;
        [TitleGroup("Scene")]
        [SerializeField]
        private string titleSceneName;
        #endregion

        #region Fields
        public StageInfo CurrentStage { get; private set; }
        #endregion

        public void GoToTitle() {
            SceneManager.LoadScene(titleSceneName);
        }

        public void GoToMain() {
            SceneManager.LoadScene(mainSceneName);
        }

        public void GameStart(StageInfo stage) {
            this.CurrentStage = stage;
            SceneManager.LoadScene(gameSceneName);
        }
    }
}