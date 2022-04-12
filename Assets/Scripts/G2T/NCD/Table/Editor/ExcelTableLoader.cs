// System
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//���� NPOI
using NPOI;
using NPOI.SS.UserModel;
//ǥ�� xls ���� excel��Ʈ
using NPOI.HSSF;
using NPOI.HSSF.UserModel;
//Ȯ�� xlsx ���� excel ��Ʈ
using NPOI.XSSF;
using NPOI.XSSF.UserModel;
// Editor
using Sirenix.OdinInspector;
// Json
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace G2T.NCD.Table.Editor {
    public class ExcelTableLoader<T, U> where T : ExcelTable<T, U>, new() where U : ExcelData, new() {
        public enum ValueType { Int, Float, String, Boolean };

        [Serializable]
        public class Column {
            [HorizontalGroup("group", .25f)]
            [BoxGroup("group/���� �� �̸�")]
            [HideLabel]
            [ValueDropdown("cols")]
            public string ColumnName;
            [HorizontalGroup("group", .25f)]
            [BoxGroup("group/������ ���� �̸�")]
            [HideLabel]
            [ValueDropdown("properties")]
            public string PropertyName;
            [HorizontalGroup("group", .25f)]
            [BoxGroup("group/Ÿ��")]
            [HideLabel]
            public ValueType Type;
            [HorizontalGroup("group", .25f)]
            [BoxGroup("group/�迭")]
            [HideLabel]
            public bool IsArray;

            [HideInInspector]
            public int index;

            public Column(List<string> cols, List<string> properties, string ColumnName, string PropertyName, int index) {
                this.cols = cols;
                this.properties = properties;
                this.ColumnName = ColumnName;
                this.PropertyName = PropertyName;
                this.index = index;
            }

            private List<string> cols;
            private List<string> properties;
        }

        [TitleGroup("����")]
        [LabelText("���� ���")]
        [FilePath(AbsolutePath = true)]
        [SerializeField]
        private string filePath;

        [TitleGroup("����")]
        [LabelText("��Ʈ")]
        [ShowIf("@sheet!=null")]
        [ValueDropdown("sheetNames")]
        [OnValueChanged("LoadSheet")]
        [SerializeField]
        private string sheetName;

        [TitleGroup("����")]
        [ShowIf("@sheet!=null")]
        [LabelText("�� ����")]
        [ListDrawerSettings(Expanded = true)]
        [SerializeField]
        public List<Column> Columns = new List<Column>();

        private List<string> cols = new List<string>();
        private List<string> properties = new List<string>();
        private List<string> sheetNames = new List<string>();

        private IWorkbook workbook;
        private ISheet sheet;

        [TitleGroup("����")]
        [Button("���� �ҷ�����", Style = ButtonStyle.Box, ButtonHeight = 50)]
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
                sheetName = "��Ʈ�� �����ϴ�";
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

            var data = JsonConvert.SerializeObject(new U());
            var jObj = JsonConvert.DeserializeObject<Dictionary<string, object>>(data);
            properties.Clear();
            foreach(var key in jObj.Keys) {
                properties.Add(key);
            }

            Columns = new List<Column>();
            foreach(var col in cols) {
                Columns.Add(new Column(cols, properties, col, "", 0));
            }
        }

        [TitleGroup("����")]
        [ShowIf("@sheet!=null")]
        [Button("������ �����ϱ�", Style = ButtonStyle.Box, ButtonHeight = 50)]
        private void LoadData() {
            // �ε� �� �� 
            var table = new T();
            table.Datas = new List<U>();

            var colsRow = GetRow(sheet, sheet.FirstRowNum);

            for(int i = 0; i < colsRow.LastCellNum; i++) {
                var cell = GetCell(colsRow, i);

                var colName = cell.StringCellValue;
                var col = Columns.Find(c => c.ColumnName == colName);
                if(col != null) {
                    col.index = i;
                }
            }


            for(int i = sheet.FirstRowNum + 1; i < sheet.LastRowNum; i++) {
                var row = GetRow(sheet, i);
                var header = GetCell(row, 0);
                if(header.StringCellValue == "END")
                    break;

                var jObj = new JObject();
                foreach(var col in Columns) {
                    var cell = GetCell(row, col.index);
                    switch(col.Type) {
                    case CellType.Numeric:
                        jObj.Add(col.PropertyName, cell.NumericCellValue);
                        break;
                    case CellType.String:
                        jObj.Add(col.PropertyName, cell.StringCellValue);
                        break;
                    }
                }
                string json = jObj.ToString();
                table.Datas.Add(JsonConvert.DeserializeObject<U>(json));
            }

            foreach(var data in table.Datas) {
                Debug.Log(data.ToString());
            }

            string path = UnityEditor.EditorUtility.SaveFilePanel("�����ϱ�", "Assets", "", "asset");
            if(!path.StartsWith(Application.dataPath))
                return;
            path = path.Replace(Application.dataPath, "Assets");

            Debug.Log(path);

            try {
                UnityEditor.AssetDatabase.CreateAsset(table, path);
            }
            catch(Exception e) {
                Debug.LogError(e.Message);
                return;
            }
        }

        #region Excel
        public IWorkbook GetWorkbook(string filename, string version) {
            using(var stream = new FileStream(filename, FileMode.Open, FileAccess.Read)) {
                //ǥ�� xls ����
                if(".xls".Equals(version)) {
                    return new HSSFWorkbook(stream);
                }
                //Ȯ�� xlsx ����
                else if(".xlsx".Equals(version)) {
                    return new XSSFWorkbook(stream);
                }
                throw new NotSupportedException();
            }
        }

        // Sheet�� ���� Row�� ���, �����ϱ�
        public IRow GetRow(ISheet sheet, int rownum) {
            var row = sheet.GetRow(rownum);
            if(row == null) {
                row = sheet.CreateRow(rownum);
            }
            return row;
        }

        // Row�� ���� Cell�� ���, �����ϱ�
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
        #endregion
    }
}