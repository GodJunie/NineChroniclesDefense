using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace G2T.NCD.Game {
    using Table;
    using UI;

    public class Restaurant : BuildingBase {
        #region Structs
        public class CookingInfo {
            public RecipeInfo Info { get; private set; }
            public float Cooltime { get; private set; }
            public float Timer { get; private set; }

            public CookingInfo(RecipeInfo info, float coolTime) {
                this.Info = info;
                this.Cooltime = coolTime;
                this.Timer = coolTime;
            }

            public void Tick() {
                this.Timer -= Time.deltaTime;
            }
        }
        #endregion

        public List<CookingInfo> Cookings { get; private set; }

        protected override void Start() {
            base.Start();

            this.Cookings = new List<CookingInfo>();
        }

        protected override void Update() {
            base.Update();

            foreach(var cooking in Cookings) {
                cooking.Tick();
            }
        }

        public override List<BuildingStatusInfo> Statuses {
            get {
                return (StatusTable as RestaurantStatusTable).Datas.Select(e => e as BuildingStatusInfo).ToList();
            }
        }

        public void OnCooking(RecipeInfo info) {
            this.Cookings.Add(new CookingInfo(info, info.CoolTime));
        }

        public void OnCookingComplete(CookingInfo info) {
            GameController.Instance.AddItem(info.Info.ResultId, info.Info.ResultAmount);

            this.Cookings.Remove(info);
        }

        protected override void ClosePanel() {
            UIManager.Instance.CloseUI("restaurant-info");
        }

        protected override void OpenPanel() {
            UIManager.Instance.OpenUI("restaurant-info", this.uiRoot).GetComponent<UIRestaurantInfo>().Open(this);
        }
    }
}