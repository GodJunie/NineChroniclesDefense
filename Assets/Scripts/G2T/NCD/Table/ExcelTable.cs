// System
using System;
using System.Collections;
using System.Collections.Generic;
// UnityEngine
using UnityEngine;
// Editor
using Sirenix.OdinInspector;

namespace G2T.NCD.Table {
    public class ExcelTable<T, U> : ScriptableObject where T : ExcelTable<T, U> where U : ExcelData {
        [SerializeField]
        [ListDrawerSettings]
        public List<U> Datas;
    }
}