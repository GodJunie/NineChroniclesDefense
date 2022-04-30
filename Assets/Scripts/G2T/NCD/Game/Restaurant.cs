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

        private List<int> synthesisIds;

        public override async Task Init(BuildingInfo info) {
            await base.Init(info);

            this.synthesisIds = (StatusTable as RestaurantStatusTable).Datas[this.Level].RecipeIds;
        }

        protected override void ClosePanel() {

        }

        protected override void OpenPanel() {

        }

        
    }
}