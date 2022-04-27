using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;

namespace G2T.NCD.UI {
    using Game;

    public class UIStatus : MonoBehaviour {
        [SerializeField]
        private Text textHp;
        [SerializeField]
        private Text textAtk;
        [SerializeField]
        private Text textDef;
        [SerializeField]
        private Text textAtkSpeed;
        [SerializeField]
        private Text textMoveSpeed;
        [SerializeField]
        private Text textCriRate;
        [SerializeField]
        private Text textCriDmg;

        public void SetUI(Status status) {
            if(this.textAtk)
                this.textAtk.text = status.Atk.ToString("0");
            if(this.textHp)
                this.textHp.text = status.Hp.ToString("0");
            if(this.textDef)
                this.textDef.text = status.Def.ToString("0");
            if(this.textAtkSpeed)
                this.textAtkSpeed.text = status.AttackSpeed.ToString("0.0");
            if(this.textMoveSpeed)
                this.textMoveSpeed.text = status.MoveSpeed.ToString("0.0");
            if(this.textCriRate)
                this.textCriRate.text = status.CriRate.ToString("0.0%");
            if(this.textCriDmg)
                this.textCriDmg.text = status.CriDamage.ToString("0.0%");
        }
    }
}
