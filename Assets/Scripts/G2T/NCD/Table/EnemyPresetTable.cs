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
    using Game;

    public class EnemyPresetTable : TableConfig<EnemyPresetTable> {
#if UNITY_EDITOR
        [TitleGroup("엑셀 데이터 메뉴")]
        [SerializeField]
        [LabelText("파일 경로")]
        [FilePath(AbsolutePath = true)]
        private string filePath;

        [LabelText("시트")]
        [ShowIfGroup("엑셀 데이터 메뉴/@sheet!=null")]
        [ValueDropdown("sheetNames")]
        [OnValueChanged("LoadSheet")]
        [SerializeField]
        private string sheetName;

        [ShowIfGroup("엑셀 데이터 메뉴/@sheet!=null")]
        [SerializeField]
        [ValueDropdown("cols")]
        [LabelText("아이디 칼럼")]
        private string idCol;
        [ShowIfGroup("엑셀 데이터 메뉴/@sheet!=null")]
        [SerializeField]
        [ValueDropdown("cols")]
        [LabelText("적군 정보 아이디 칼럼")]
        private string enemyIdCol;

        [ShowIfGroup("엑셀 데이터 메뉴/@sheet!=null")]
        [SerializeField]
        [ValueDropdown("cols")]
        [LabelText("드랍 아이템 아이디 칼럼")]
        private string dropItemIdCol;
        [ShowIfGroup("엑셀 데이터 메뉴/@sheet!=null")]
        [SerializeField]
        [ValueDropdown("cols")]
        [LabelText("드랍 아이템 개수 칼럼")]
        private string dropItemAmountCol;

        [ShowIfGroup("엑셀 데이터 메뉴/@sheet!=null")]
        [SerializeField]
        [ValueDropdown("cols")]
        [LabelText("체력 칼럼")]
        private string hpCol;
        [ShowIfGroup("엑셀 데이터 메뉴/@sheet!=null")]
        [SerializeField]
        [ValueDropdown("cols")]
        [LabelText("공격력 칼럼")]
        private string atkCol;
        [ShowIfGroup("엑셀 데이터 메뉴/@sheet!=null")]
        [SerializeField]
        [ValueDropdown("cols")]
        [LabelText("방어력 칼럼")]
        private string defCol;
        [ShowIfGroup("엑셀 데이터 메뉴/@sheet!=null")]
        [SerializeField]
        [ValueDropdown("cols")]
        [LabelText("크리티컬 확률 칼럼")]
        private string criProbCol;
        [ShowIfGroup("엑셀 데이터 메뉴/@sheet!=null")]
        [SerializeField]
        [ValueDropdown("cols")]
        [LabelText("크리티컬 데미지 칼럼")]
        private string criDamageCol;
        [ShowIfGroup("엑셀 데이터 메뉴/@sheet!=null")]
        [SerializeField]
        [ValueDropdown("cols")]
        [LabelText("이동속도 칼럼")]
        private string moveSpeedCol;
        [ShowIfGroup("엑셀 데이터 메뉴/@sheet!=null")]
        [SerializeField]
        [ValueDropdown("cols")]
        [LabelText("공격속도 칼럼")]
        private string attackSpeedCol;

        private List<string> cols = new List<string>();
        private List<string> sheetNames = new List<string>();

        private IWorkbook workbook;
        private ISheet sheet;

        [TitleGroup("엑셀 데이터 메뉴")]
        [Button("엑셀 파일 로드하기")]
        private void LoadExcelFile() {
            var version = Path.GetExtension(filePath);
            Debug.Log(version);
            workbook = GetWorkbook(filePath, Path.GetExtension(filePath));

            sheetNames.Clear();
            for(int i = 0; i < workbook.NumberOfSheets; i++) {
                sheetNames.Add(workbook.GetSheetAt(i).SheetName);
            }
            if(sheetNames.Count > 0) {
                sheetName = sheetNames[0];
                LoadSheet();
            } else
                sheetName = "시트가 없습니다";
        }

        private void LoadSheet() {
            sheet = workbook.GetSheet(sheetName);
            var colsRow = GetRow(sheet, sheet.FirstRowNum);
            cols.Clear();
            foreach(var cell in colsRow.Cells) {
                string col = cell.StringCellValue;
                if(col != "HEAD" && col != "END") {
                    cols.Add(col);
                }
            }
        }

        [ShowIfGroup("엑셀 데이터 메뉴/@sheet!=null")]
        [Button("데이터 로드하기")]
        private void LoadData() {
            EnemyPresets.Clear();
            var colsRow = GetRow(sheet, sheet.FirstRowNum);

            int idIdx = 0;
            int enemyIdIdx = 0;

            int dropItemIdIdx = 0;
            int dropItemAmountIdx = 0;

            int hpIdx = 0;
            int atkIdx = 0;
            int defIdx = 0;
            int criProbIdx = 0;
            int criDamageIdx = 0;
            int moveSpeedIdx = 0;
            int attackSpeedIdx = 0;

            for(int i = 0; i < colsRow.LastCellNum; i++) {
                var cell = GetCell(colsRow, i);
                if(cell.StringCellValue == idCol) idIdx = i;
                if(cell.StringCellValue == enemyIdCol) enemyIdIdx = i;


                if(cell.StringCellValue == dropItemIdCol) dropItemIdIdx = i;
                if(cell.StringCellValue == dropItemAmountCol) dropItemAmountIdx = i;

                if(cell.StringCellValue == hpCol) hpIdx = i;
                if(cell.StringCellValue == atkCol) atkIdx = i;
                if(cell.StringCellValue == defCol) defIdx = i;
                if(cell.StringCellValue == criProbCol) criProbIdx = i;
                if(cell.StringCellValue == criDamageCol) criDamageIdx = i;
                if(cell.StringCellValue == moveSpeedCol) moveSpeedIdx = i;
                if(cell.StringCellValue == attackSpeedCol) attackSpeedIdx = i;
            }

            for(int i = sheet.FirstRowNum + 1; i < sheet.LastRowNum; i++) {
                var row = GetRow(sheet, i);
                var header = GetCell(row, 0);
                if(header.StringCellValue == "END")
                    break;

                var id = Convert.ToInt32(GetCell(row, idIdx).NumericCellValue);
                var enemyId = Convert.ToInt32(GetCell(row, enemyIdIdx).NumericCellValue);

                var dropItems = new List<EnemyPresetInfo.DropItem>();

                float atk = Convert.ToSingle(GetCell(row, atkIdx).NumericCellValue);
                float hp = Convert.ToSingle(GetCell(row, hpIdx).NumericCellValue);
                float def = Convert.ToSingle(GetCell(row, defIdx).NumericCellValue);
                float criProb = Convert.ToSingle(GetCell(row, criProbIdx).NumericCellValue);
                float criDamage = Convert.ToSingle(GetCell(row, criDamageIdx).NumericCellValue);
                float moveSpeed = Convert.ToSingle(GetCell(row, moveSpeedIdx).NumericCellValue);
                float attackSpeed = Convert.ToSingle(GetCell(row, attackSpeedIdx).NumericCellValue);

                var status = new Status(hp, atk, def, criProb, criDamage, moveSpeed, attackSpeed);

                var catchIdCell = GetCell(row, dropItemIdIdx);
                if(catchIdCell.CellType != CellType.Blank) {
                    var dropItemIds = new List<int>();
                    var dropItemAmounts = new List<int>();

                    if(catchIdCell.CellType == CellType.Numeric) {
                        dropItemIds.Add(Convert.ToInt32(catchIdCell.NumericCellValue));
                    } else {
                        dropItemIds = catchIdCell.StringCellValue.Split(new char[] { ',' }).Select(e => Convert.ToInt32(e)).ToList();
                    }

                    var catchAmountCell = GetCell(row, dropItemAmountIdx);
                    if(catchAmountCell.CellType == CellType.Numeric) {
                        dropItemAmounts.Add(Convert.ToInt32(catchAmountCell.NumericCellValue));
                    } else {
                        dropItemAmounts = catchAmountCell.StringCellValue.Split(new char[] { ',' }).Select(e => Convert.ToInt32(e)).ToList();
                    }

                    for(int j = 0; j < Mathf.Min(dropItemIds.Count, dropItemAmounts.Count); j++) {
                        dropItems.Add(new EnemyPresetInfo.DropItem(dropItemIds[j], dropItemAmounts[j]));
                    }
                }

                EnemyPresets.Add(new EnemyPresetInfo(id, enemyId, status, dropItems));
            }
        }

        public IWorkbook GetWorkbook(string filename, string version) {
            using(var stream = new FileStream(filename, FileMode.Open, FileAccess.Read)) {
                //표준 xls 버젼
                if(".xls".Equals(version)) {
                    return new HSSFWorkbook(stream);
                }
                //확장 xlsx 버젼
                else if(".xlsx".Equals(version)) {
                    return new XSSFWorkbook(stream);
                }
                throw new NotSupportedException();
            }
        }

        // Sheet로 부터 Row를 취득, 생성하기
        public IRow GetRow(ISheet sheet, int rownum) {
            var row = sheet.GetRow(rownum);
            if(row == null) {
                row = sheet.CreateRow(rownum);
            }
            return row;
        }

        // Row로 부터 Cell를 취득, 생성하기
        public ICell GetCell(IRow row, int cellnum) {
            var cell = row.GetCell(cellnum);
            if(cell == null) {
                cell = row.CreateCell(cellnum);
            }
            return cell;
        }
        public ICell GetCell(ISheet sheet, int rownum, int cellnum) {
            var row = GetRow(sheet, rownum);
            return GetCell(row, cellnum);
        }
        public void WriteExcel(IWorkbook workbook, string filepath) {
            using(var file = new FileStream(filepath, FileMode.Create, FileAccess.Write)) {
                workbook.Write(file);
            }
        }
#endif
        [SerializeField]
        [TitleGroup("데이터")]
        [LabelText("데이터")]
        [ListDrawerSettings]
        public List<EnemyPresetInfo> EnemyPresets;
    }
}
