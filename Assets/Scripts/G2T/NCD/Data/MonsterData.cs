// System
using System;
using System.Collections;
using System.Collections.Generic;
// UnityEngine
using UnityEngine;
// Etc
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace G2T.NCD.Data {
    [Serializable]
    public class MonsterData {
        public string Id { get; set; }
        public int Level { get; set; }
        public DateTime CreatedAt { get; set; }

        #region Constructor
        public MonsterData() {
            this.Id = "";
            this.Level = 0;
            this.CreatedAt = DateTime.Now;
        }
        #endregion
    }
}