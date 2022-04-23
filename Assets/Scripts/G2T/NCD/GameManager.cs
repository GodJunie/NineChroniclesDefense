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
        // ���߿� �̸� ���� Path �� �ٲ㼭 Addressable ���� �ε�
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

        public void GameStart(StageInfo stage) {
            this.CurrentStage = stage;
            SceneManager.LoadScene(gameSceneName);
        }
    }
}