using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json.Linq;

namespace G2T.NCD.Table {
    public abstract class ExcelData {
        public abstract string[] GetProperties();
        public abstract void InitFromJObject(JObject jObject);
    }
}
