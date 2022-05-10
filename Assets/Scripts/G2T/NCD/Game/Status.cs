using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Newtonsoft.Json.Linq;

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
        private float criRate;
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
        [FoldoutGroup("스탯")]
        [LabelText("공격 범위")]
        [SerializeField]
        [LabelWidth(150f)]
        private float attackRange;

        public Status(float hp, float atk, float def, float criRate, float criDamage, float moveSpeed, float attackSpeed, float attackRange) {
            this.hp = hp;
            this.atk = atk;
            this.def = def;
            this.criRate = criRate;
            this.criDamage = criDamage;
            this.moveSpeed = moveSpeed;
            this.attackSpeed = attackSpeed;
            this.attackRange = attackRange;
        }

        public float Hp { get => hp; }
        public float Atk { get => atk; }
        public float Def { get => def; }
        public float CriRate { get => criRate; }
        public float CriDamage { get => criDamage; }
        public float MoveSpeed { get => moveSpeed; }
        public float AttackSpeed { get => attackSpeed; }
        public float AttackRange { get => attackRange; }

        public static string[] Properties = new string[] {
           "hp",
           "atk",
           "def",
           "criRate",
           "criDamage",
           "moveSpeed",
           "attackSpeed",
           "attackRange",
        };

        public static Status FromJObject(JObject jObject) {
            var status = new Status();
            status.hp          = jObject.Value<float>("hp");
            status.atk         = jObject.Value<float>("atk");
            status.def         = jObject.Value<float>("def");
            status.criRate     = jObject.Value<float>("criRate");
            status.criDamage   = jObject.Value<float>("criDamage");
            status.moveSpeed   = jObject.Value<float>("moveSpeed");
            status.attackSpeed = jObject.Value<float>("attackSpeed");
            status.attackRange = jObject.Value<float>("attackRange");
            return status;
        }
    }
}