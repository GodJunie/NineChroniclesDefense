// System
using System;
using System.Collections;
using System.Collections.Generic;
// UnityEngine
using UnityEngine;
// Editor
using Sirenix.OdinInspector;

namespace G2T.NCD.Table {
    using Game;

    [Serializable]
    public class StageTimelineInfo : ScriptableObject {
        #region Struces
        [Serializable]
        public struct EnemySpawnInfo {
            [HorizontalGroup("group")]
            [BoxGroup("group/아이디")]
            [HideLabel]
            [SerializeField]
            private int id;
            [HorizontalGroup("group")]
            [BoxGroup("group/시간")]
            [HideLabel]
            [SerializeField]
            private float time;
          
            public EnemySpawnInfo(int id, float time) {
                this.id = id;
                this.time = time;
            }

            public float Time { get => time; }
        }

        [Serializable]
        public struct MonsterSpawnInfo {
            [HorizontalGroup("group")]
            [BoxGroup("group/프리팹")]
            [HideLabel]
            [SerializeField]
            private int id;
            [HorizontalGroup("group")]
            [BoxGroup("group/시간")]
            [HideLabel]
            [SerializeField]
            private float time;
            //[HorizontalGroup("group")]
            //[BoxGroup("group/위치")]
            //[HideLabel]
            //[SerializeField]
            //private float x;
           
            public MonsterSpawnInfo(int id, float time) {
                this.id = id;
                this.time = time;
            }

            public float Time { get => time; }
            //public float X { get => x; }
        }


        [Serializable]
        public struct WaveInfo {
            [FoldoutGroup("웨이브")]
            [HorizontalGroup("웨이브/group", .5f)]
            [BoxGroup("웨이브/group/왼쪽")]
            [ShowIf("@LeftEnemiesExists")]
            [LabelText("왼쪽 적군 리스트")]
            [SerializeField]
            private List<EnemySpawnInfo> leftEnemies;
            [BoxGroup("웨이브/group/오른쪽")]
            [ShowIf("@RightEnemiesExists")]
            [LabelText("오른쪽 적군 리스트")]
            [SerializeField]
            private List<EnemySpawnInfo> rightEnemies;

            public WaveInfo(List<EnemySpawnInfo> leftEnemies, List<EnemySpawnInfo> rightEnemies) {
                this.leftEnemies = leftEnemies;
                this.rightEnemies = rightEnemies;
            }

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
                this.leftEnemies = new List<EnemySpawnInfo>();
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
                this.rightEnemies = new List<EnemySpawnInfo>();
            }
#endif
            public bool LeftEnemiesExists { get => this.leftEnemies != null && this.leftEnemies.Count > 0; }
            public bool RightEnemiesExists { get => this.rightEnemies != null && this.rightEnemies.Count > 0; }

            public List<EnemySpawnInfo> LeftEnemies { get => this.leftEnemies; }
            public List<EnemySpawnInfo> RightEnemies { get => this.rightEnemies; }
        }

        [Serializable]
        public struct DayTimeInfo {
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
            [FoldoutGroup("@GroupName")]
            [SerializeField]
            [LabelText("몬스터 스폰 정보")]
            private List<MonsterSpawnInfo> monsters;

            public DayTimeInfo(DayTimePart timePart, float time, WaveInfo wave, List<MonsterSpawnInfo> monsters) {
                this.timePart = timePart;
                this.time = time;
                this.wave = wave;
                this.monsters = monsters;
            }

            public float Time { get => time; }
            public DayTimePart TimePart { get => timePart; }
            public WaveInfo Wave { get => wave; }

#if UNITY_EDITOR
            private string GroupName { get => string.Format("{0}/{1}초{2}{3}{4}", this.timePart, this.time, this.wave.LeftEnemiesExists ? string.Format("/왼쪽 {0}명", this.wave.LeftEnemies.Count) : "", this.wave.RightEnemiesExists ? string.Format("/오른쪽 {0}명", this.wave.RightEnemies.Count) : "", this.monsters.Count > 0 ? string.Format("/몬스터 {0}마리", this.monsters.Count) : ""); }
#endif
        }
        #endregion

        [SerializeField]
        [LabelText("시간")]
        private List<DayTimeInfo> dayTimes;

        public void Init(List<DayTimeInfo> dayTimes) {
            this.dayTimes = dayTimes;
        }
    }
}