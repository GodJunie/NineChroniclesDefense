using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace G2T.NCD.Data {
    [System.Serializable]
    public class ItemData {
        public int Id { get; set; }
        public int Count { get; set; }

        public DateTime CreatedAt { get; private set; }

        public ItemData(int id, int count) {
            this.Id = id;
            this.Count = count;
            this.CreatedAt = DateTime.Now;
        }

        public ItemData(int id) {
            this.Id = id;
            this.Count = 1;
            this.CreatedAt = DateTime.Now;
        }
    }
}