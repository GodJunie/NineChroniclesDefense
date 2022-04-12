// System
using System;
using System.Collections;
using System.Collections.Generic;
// UnityEngine
using UnityEngine;
// Editor
using Sirenix.OdinInspector;

namespace G2T.NCD.Table {
    [System.Serializable]
    public class ItemInfo : ExcelData {
        [HorizontalGroup("group", 80f)]
        [BoxGroup("group/아이콘")]
        [HideLabel]
        [PreviewField(Alignment = ObjectFieldAlignment.Center, Height = 80f)]
        [SerializeField]
        private Sprite icon;

        [SerializeField]
        [HorizontalGroup("group")]
        [BoxGroup("group/정보")]
        [HorizontalGroup("group/정보/group")]
        [BoxGroup("group/정보/group/아이디")]
        [HideLabel]
        private int id;

        [SerializeField]
        [BoxGroup("group/정보/group/이름")]
        [HideLabel]
        private string name;

        [SerializeField]
        [BoxGroup("group/정보/group/설명")]
        [HideLabel]
        private string description;

        public ItemInfo(int id, string name, string description) {
            this.id = id;
            this.name = name;
            this.description = description;
        }

        public Sprite Icon { get => icon; }
        public int Id { get => id; }
        public string Name { get => name; }
        public string Description { get => description; }
    }
}
