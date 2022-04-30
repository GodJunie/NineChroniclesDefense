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
        public override List<BuildingStatusInfo> Statuses {
            get {
                return (StatusTable as RestaurantStatusTable).Datas.Select(e => e as BuildingStatusInfo).ToList();
            }
        }



        public override async Task Init(BuildingInfo info) {
            await base.Init(info);
        }

        protected override void ClosePanel() {
            UIManager.Instance.CloseUI("restaurant-info");
        }

        protected override void OpenPanel() {
            UIManager.Instance.OpenUI("restaurant-info", this.uiRoot).GetComponent<UIRestaurantInfo>().Open(this);
        }
    }
}