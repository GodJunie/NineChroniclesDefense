// System
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.IO;
// UnityEngine
using UnityEngine;
using UnityEngine.UI;
// Editor
using Sirenix.OdinInspector;

namespace G2T.NCD.Game {
    using Table;
    using UI;

    public class WorkShop : BuildingBase {
        public override List<BuildingStatusInfo> Statuses {
            get {
                return (StatusTable as RestaurantStatusTable).Datas.Select(e => e as BuildingStatusInfo).ToList();
            }
        }

        protected override void ClosePanel() {
            UIManager.Instance.CloseUI("workshop-info");
        }

        protected override void OpenPanel() {
            UIManager.Instance.OpenUI("workshop-info", this.uiRoot).GetComponent<UIWorkShopInfo>().Open(this);
        }
    }
}