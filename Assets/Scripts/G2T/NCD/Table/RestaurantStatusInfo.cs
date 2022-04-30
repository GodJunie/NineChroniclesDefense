using Newtonsoft.Json.Linq;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


namespace G2T.NCD.Table {
    [Serializable]
    public class RestaurantStatusInfo : BuildingStatusInfo {
        [LabelText("합성 아이디")]
        [SerializeField]
        private List<int> recipeIds;

        public override string[] GetProperties() {
            return base.GetProperties().Concat(new string[] {
                "recipeIds"
            }).ToArray();
        }

        public override void InitFromJObject(JObject jObject) {
            base.InitFromJObject(jObject);

            recipeIds = jObject["recipeIds"].Values<int>().ToList();
        }

        public List<int> RecipeIds { get => recipeIds; }
    }
}
