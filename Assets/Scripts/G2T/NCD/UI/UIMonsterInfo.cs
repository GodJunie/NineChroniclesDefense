// System
using System.Collections;
using System.Collections.Generic;
// UnityEngine
using UnityEngine;
using UnityEngine.UI;
// Editor
using Sirenix.OdinInspector;

namespace G2T.NCD.UI {
    using Game;
    using Table;
    using Management;

    public class UIMonsterInfo : MonoBehaviour {
        #region Settings
        [TabGroup("group", "Settings")]
        [SerializeField]
        private UIItemSlot itemSlotPrefab;
        [TabGroup("group", "Settings")]
        [SerializeField]
        private UIMonsterSlot monsterSlotPrefab;
        #endregion
        
        #region Catch
        [TabGroup("group", "Catch")]
        [LabelText("Panel")]
        [SerializeField]
        private GameObject panelCatch;
        [TabGroup("group", "Catch")]
        [LabelText("Slot Container")]
        [SerializeField]
        private Transform catchSlotContainer;
        [TabGroup("group", "Catch")]
        [LabelText("Catch Button")]
        [SerializeField]
        private Button buttonCatch;
        #endregion

        #region Info
        [TabGroup("group", "Info")]
        [LabelText("Panel")]
        [SerializeField]
        private GameObject panelInfo;
        [TabGroup("group", "Info")]
        [SerializeField]
        private Text textInfoName;
        [TabGroup("group", "Info")]
        [SerializeField]
        private Text textInfoLevel;
        [TabGroup("group", "Info")]
        [SerializeField]
        private UIStatus uiStatusInfo;
        #endregion

        #region Level Up
        [TabGroup("group", "LevelUp")]
        [LabelText("Panel")]
        [SerializeField]
        private GameObject panelLevelUp;
        [TabGroup("group", "LevelUp")]
        [SerializeField]
        private Text textLevelUpBefore;
        [TabGroup("group", "LevelUp")]
        [SerializeField]
        private Text textLevelUpAfter;
        [TabGroup("group", "LevelUp")]
        [SerializeField]
        private Button buttonLevelUp;
        [TabGroup("group", "LevelUp")]
        [SerializeField]
        private Text textButtonLevelUp;
        #endregion

        #region Evolution
        [TabGroup("group", "Evolution")]
        [LabelText("Panel")]
        [SerializeField]
        private GameObject panelEvolution;
        [TabGroup("group", "Evolution")]
        [SerializeField]
        private Transform evolutionSlotContainer;
        [TabGroup("group", "Evolution")]
        [SerializeField]
        private Button buttonEvolution;
        [TabGroup("group", "Evolution")]
        [SerializeField]
        private UIItemSlot evolutionResultSlot;
        #endregion


        public void Open(Monster monster) {
            panelCatch.SetActive(false);
            panelInfo.SetActive(false);
            panelEvolution.SetActive(false);
            panelLevelUp.SetActive(false);

            if(monster.CurrentMonsterType == MonsterType.Wild) {
                OpenCatch();
            } else {
                OpenInfo();
            }
        }

        private void OpenCatch() {

        }

        private void OpenInfo() {

        }

        public void OpenLevelUp() {

        }

        public void OpenEvolution() {

        }

        public void Close() {

        }
    }
}