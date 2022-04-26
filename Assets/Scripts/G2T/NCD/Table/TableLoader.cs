using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace G2T.NCD.Table {
    public class TableLoader : SingletonBehaviour<TableLoader> {
        public ItemTable ItemTable;
        public MonsterTable MonsterTable;
        public EnemyTable EnemyTable;
        public EnemyPresetTable EnemyPresetTable;
        public StageTable StageTable;
        public BuildingTable BuildingTable;
        public FarmingItemTable FarmingItemTable;
    }
}