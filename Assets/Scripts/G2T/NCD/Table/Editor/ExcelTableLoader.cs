// System
using System;
using System.Linq;
using System.IO;
using System.Collections;
using System.Collections.Generic;
// UnityEngine
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
    [Serializable]
    public class ExcelTableLoader<Loader, Table, Data> : ScriptableObject 
        where Loader : ExcelTableLoader<Loader, Table, Data>, new() 
        where Table : ExcelTable<Table, Data>, new() 
        where Data : ExcelData, new() {
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

            //public Column(List<string> cols, string[] properties, string ColumnName, string PropertyName, int index) {
            //    this.cols = cols;
            //    this.properties = properties;
            //    this.ColumnName = ColumnName;
            //    this.PropertyName = PropertyName;
            //    this.index = index;
            //}

            [HideInInspector]
            public string[] cols;
            [HideInInspector]
            public string[] properties;

            public override string ToString() {
                return string.Format("{0},{1},{2},{3}", this.ColumnName, this.PropertyName, (int)this.Type, this.IsArray);
            }

            public static Column FromString(string str) {
                var values = str.Split(new char[] { ',' });
                var col = values[0];
                var property = values[1];
                var type = (ValueType)Convert.ToInt32(values[2]);
                var isArray = Convert.ToBoolean(values[3]);
                return new Column() {
                    ColumnName = col,
                    PropertyName = property,
                    Type = type,
                    IsArray = isArray
                };
            }
        }

        [TitleGroup("���̺� ���")]
        [LabelText("���̺� ���")]
        [FolderPath(AbsolutePath = false)]
        [SerializeField]
        public string FolderPath;

        [TitleGroup("���� ���� �ε�")]
        [LabelText("���� ���")]
        [FilePath(AbsolutePath = true)]
        [SerializeField]
        private string filePath;

        [TitleGroup("��Ʈ ����")]
        [LabelText("��Ʈ")]
        [ShowIf("@sheet!=null")]
        [ValueDropdown("sheetNames")]
        [OnValueChanged("LoadSheet")]
        [SerializeField]
        private string sheetName;

        [TitleGroup("�� ����")]
        [ShowIf("@sheet!=null")]
        [LabelText("�� ����")]
        [ListDrawerSettings(Expanded = true)]
        [SerializeField]
        private List<Column> Columns = new List<Column>();

        private List<string> sheetNames = new List<string>();

        private IWorkbook workbook;
        private ISheet sheet;

        [TitleGroup("���� ���� �ε�")]
        [Button("���� �ҷ�����", Style = ButtonStyle.Box, ButtonHeight = 50)]
        private void LoadExcelFile() {
            var version = Path.GetExtension(filePath);
            Debug.Log(version);
            workbook = GetWorkbook(filePath, Path.GetExtension(filePath));

            sheetNames.Clear();
            for(int i = 0; i < workbook.NumberOfSheets; i++) {
                sheetNames.Add(workbook.GetSheetAt(i).SheetName);
            }

            if(sheetName != "" && sheetNames.Contains(sheetName)) {
                LoadSheet();
            } else {
                if(sheetNames.Count > 0) {
                    sheetName = sheetNames[0];
                    LoadSheet();
                } else {
                    sheetName = "��Ʈ�� �����ϴ�";
                }
            }
        }

        private void LoadSheet() {
            sheet = workbook.GetSheet(sheetName);
            var colsRow = GetRow(sheet, sheet.FirstRowNum);

            var cols = new List<string>();
            foreach(var cell in colsRow.Cells) {
                string col = cell.StringCellValue;
                if(col != "HEAD" && col != "END") {
                    cols.Add(col);
                }
            }

            var properties = new Data().GetProperties();

            if(Columns == null || Columns.Count < properties.Length) {
                Columns = new List<Column>();
                foreach(var property in properties) {
                    string adjustCol = cols.Find(e => e.ToLower() == property.ToLower());
                    if(adjustCol == null) adjustCol = "";
                    Columns.Add(new Column() {
                        cols = cols.ToArray(),
                        ColumnName = adjustCol,
                        properties = properties,
                        PropertyName = property,
                        index = 0
                    });
                }
            } else {
                foreach(var col in Columns) {
                    col.cols = cols.ToArray();
                }
            }
        }

        [TitleGroup("�� ����")]
        [ShowIf("@sheet!=null")]
        [Button("������ �����ϱ�", Style = ButtonStyle.Box, ButtonHeight = 50)]
        private void LoadData() {
            // �ε� �� �� 
            var table = new Table();
            table.Datas = new List<Data>();

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
                    object value = null;

                    switch(cell.CellType) {
                    case CellType.Numeric:
                        value = cell.NumericCellValue;
                        break;
                    case CellType.String:
                        value = cell.StringCellValue;
                        break;
                    default:
                        Debug.LogError(string.Format("Undefined Cell Type {0}", cell.CellType));
                        break;
                    }

                    if(col.IsArray) {
                        var values = Convert.ToString(value).Split(new char[] { ',' });
                        switch(col.Type) {
                        case ValueType.Boolean:
                            jObj.Add(col.PropertyName, new JArray(values.Select(val => Convert.ToBoolean(val)).ToArray()));
                            break;
                        case ValueType.Float:
                            jObj.Add(col.PropertyName, new JArray(values.Select(val => Convert.ToSingle(val)).ToArray()));
                            break;
                        case ValueType.Int:
                            jObj.Add(col.PropertyName, new JArray(values.Select(val => Convert.ToInt32(val)).ToArray()));
                            break;
                        case ValueType.String:
                            jObj.Add(col.PropertyName, new JArray(values.Select(val => Convert.ToString(val)).ToArray()));
                            break;
                        }
                    } else {
                        switch(col.Type) {
                        case ValueType.Boolean:
                            jObj.Add(col.PropertyName, Convert.ToBoolean(value));
                            break;
                        case ValueType.Float:
                            jObj.Add(col.PropertyName, Convert.ToSingle(value));
                            break;
                        case ValueType.Int:
                            jObj.Add(col.PropertyName, Convert.ToInt32(value));
                            break;
                        case ValueType.String:
                            jObj.Add(col.PropertyName, Convert.ToString(value));
                            break;
                        }
                    }
                }
                string json = jObj.ToString();
                Debug.Log(json);

                var data = new Data();
                data.InitFromJObject(jObj);
                table.Datas.Add(data);
            }

            foreach(var data in table.Datas) {
                Debug.Log(data.ToString());
            }

            string path = UnityEditor.EditorUtility.SaveFilePanel("�����ϱ�", FolderPath, typeof(Table).Name, "asset");
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

        #region Instance
        private bool isLoadedFromAsset = true;
        public bool IsLoadedFromAsset { get { return isLoadedFromAsset; } }

        private static Loader _instance = null;
        public static Loader Instance {
            get {
                if(_instance == null) {
                    var guid = UnityEditor.AssetDatabase.FindAssets(string.Format("t:{0}", typeof(Loader).Name)).FirstOrDefault();
                    Debug.Log(guid);
                    if(guid == default) {
                        _instance = new Loader();
                        _instance.isLoadedFromAsset = false;
                    } else {
                        string path = UnityEditor.AssetDatabase.GUIDToAssetPath(guid);
                        Debug.Log(path);
                        _instance = UnityEditor.AssetDatabase.LoadAssetAtPath<Loader>(path);
                    }
                }
                return _instance;
            }
        }

        [InfoBox("���� �� ���� �����մϴ�. ���� ���Ŀ� �ڵ����� ������ ���̺��� �ҷ��ɴϴ�.")]
        [TitleGroup("�δ� ����")]
        [Button("�����ϱ�", ButtonHeight = 50), HideIf("isLoadedFromAsset")]
        private void CreateTable() {
            string path = UnityEditor.EditorUtility.SaveFilePanel("�����ϱ�", "Assets", typeof(Loader).Name, "asset");
            if(!path.StartsWith(Application.dataPath))
                return;
            path = path.Replace(Application.dataPath, "Assets");
            Debug.Log(path);
            try {
                _instance.isLoadedFromAsset = true;
                UnityEditor.AssetDatabase.CreateAsset(_instance, path);
            }
            catch(Exception e) {
                Debug.LogError(e.Message);
                return;
            }
        }
        #endregion
    }
}