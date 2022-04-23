// System
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
// UnityEngine
using UnityEngine;
//공통 NPOI
using NPOI;
using NPOI.SS.UserModel;
//표준 xls 버젼 excel시트
using NPOI.HSSF;
using NPOI.HSSF.UserModel;
//확장 xlsx 버젼 excel 시트
using NPOI.XSSF;
using NPOI.XSSF.UserModel;
// Editor
using Sirenix.OdinInspector;

namespace G2T.NCD.Table {
    public class MonsterTable : ExcelTable<MonsterTable, MonsterInfo> {
        [Button]
        private void Temp(string f1, string f2, string f3) {
            foreach(var data in Datas) data.Temp(f1, f2, f3);
        }
    }
}
