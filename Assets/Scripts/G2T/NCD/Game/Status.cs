using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace G2T.NCD.Game {
    [System.Serializable]
    public struct Status {
        [FoldoutGroup("스탯")]
        [LabelText("체력")]
        [SerializeField]
        [LabelWidth(150f)]
        private float hp;
        [FoldoutGroup("스탯")]
        [LabelText("공격력")]
        [SerializeField]
        [LabelWidth(150f)]
        private float atk;
        [FoldoutGroup("스탯")]
        [LabelText("방어력")]
        [SerializeField]
        [LabelWidth(150f)]
        private float def;
        [FoldoutGroup("스탯")]
        [LabelText("크리티컬 확률")]
        [SerializeField]
        [LabelWidth(150f)]
        private float criProb;
        [FoldoutGroup("스탯")]
        [LabelText("크리티컬 데미지")]
        [SerializeField]
        [LabelWidth(150f)]
        private float criDamage;
        [FoldoutGroup("스탯")]
        [LabelText("이동 속도")]
        [SerializeField]
        [LabelWidth(150f)]
        private float moveSpeed;
        [FoldoutGroup("스탯")]
        [LabelText("공격 속도")]
        [SerializeField]
        [LabelWidth(150f)]
        private float attackSpeed;

        public Status(float hp, float atk, float def, float criProb, float criDamage, float moveSpeed, float attackSpeed) {
            this.hp = hp;
            this.atk = atk;
            this.def = def;
            this.criProb = criProb;
            this.criDamage = criDamage;
            this.moveSpeed = moveSpeed;
            this.attackSpeed = attackSpeed;
        }

        public float Hp { get => hp; }
        public float Atk { get => atk; }
        public float Def { get => def; }
        public float CriProb { get => criProb; }
        public float CriDamage { get => criDamage; }
        public float MoveSpeed { get => moveSpeed; }
        public float AttackSpeed { get => attackSpeed; }
    }
}