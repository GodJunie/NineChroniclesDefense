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
    public class EnemyTable : TableConfig<EnemyTable> {
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
        [LabelText("이름 칼럼")]
        private string nameCol;
        [ShowIfGroup("엑셀 데이터 메뉴/@sheet!=null")]
        [SerializeField]
        [ValueDropdown("cols")]
        [LabelText("설명 칼럼")]
        private string descriptionCol;

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
            Enemys.Clear();
            var colsRow = GetRow(sheet, sheet.FirstRowNum);

            int idIdx = 0;
            int nameIdx = 0;
            int desIdx = 0;

            for(int i = 0; i < colsRow.LastCellNum; i++) {
                var cell = GetCell(colsRow, i);
                if(cell.StringCellValue == idCol) idIdx = i;
                if(cell.StringCellValue == nameCol) nameIdx = i;
                if(cell.StringCellValue == descriptionCol) desIdx = i;
            }

            for(int i = sheet.FirstRowNum + 1; i < sheet.LastRowNum; i++) {
                var row = GetRow(sheet, i);
                var header = GetCell(row, 0);
                if(header.StringCellValue == "END")
                    break;

                var id = Convert.ToInt32(GetCell(row, idIdx).NumericCellValue);
                var name = GetCell(row, nameIdx).StringCellValue;
                var des = GetCell(row, desIdx).StringCellValue;

                Enemys.Add(new EnemyInfo(id, name, des));
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
        public List<EnemyInfo> Enemys;
    }
}
