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

namespace G2T.NCD.Table.Editor {
    using Game;

    public class StageTimelineInfoLoader : TableConfig<StageTimelineInfoLoader> {
        [TitleGroup("테이블 경로")]
        [SerializeField]
        [FolderPath(AbsolutePath = false)]
        public string FolderPath;

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
        [LabelText("시간대 칼럼")]
        private string timepartCol;
        [ShowIfGroup("엑셀 데이터 메뉴/@sheet!=null")]
        [SerializeField]
        [ValueDropdown("cols")]
        [LabelText("기간 칼럼")]
        private string periodCol;

        [ShowIfGroup("엑셀 데이터 메뉴/@sheet!=null")]
        [SerializeField]
        [ValueDropdown("cols")]
        [LabelText("왼쪽 웨이브 아이디 칼럼")]
        private string waveLeftIdCol;
        [ShowIfGroup("엑셀 데이터 메뉴/@sheet!=null")]
        [SerializeField]
        [ValueDropdown("cols")]
        [LabelText("왼쪽 웨이브 시간 칼럼")]
        private string waveLeftTimeCol;
        [ShowIfGroup("엑셀 데이터 메뉴/@sheet!=null")]
        [SerializeField]
        [ValueDropdown("cols")]
        [LabelText("오른쪽 웨이브 아이디 칼럼")]
        private string waveRightIdCol;
        [ShowIfGroup("엑셀 데이터 메뉴/@sheet!=null")]
        [SerializeField]
        [ValueDropdown("cols")]
        [LabelText("오른쪽 웨이브 시간 칼럼")]
        private string waveRightTimeCol;
        [ShowIfGroup("엑셀 데이터 메뉴/@sheet!=null")]
        [SerializeField]
        [ValueDropdown("cols")]
        [LabelText("몬스터 아이디 칼럼")]
        private string monsterIdCol;
        [ShowIfGroup("엑셀 데이터 메뉴/@sheet!=null")]
        [SerializeField]
        [ValueDropdown("cols")]
        [LabelText("몬스터 시간 칼럼")]
        private string monsterTimeCol;

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
            var colsRow = GetRow(sheet, sheet.FirstRowNum);

            int timepartIdx = 0;
            int periodIdx = 0;

            int waveLeftIdIdx    = 0;
            int waveLeftTimeIdx  = 0;
            int waveRightIdIdx   = 0;
            int waveRightTimeIdx = 0;
            int monsterIdIdx     = 0;
            int monsterTimeIdx   = 0;

            for(int i = 0; i < colsRow.LastCellNum; i++) {
                var cell = GetCell(colsRow, i);
                if(cell.StringCellValue == timepartCol) timepartIdx = i;
                if(cell.StringCellValue == periodCol) periodIdx = i;

                if(cell.StringCellValue == waveLeftIdCol) waveLeftIdIdx    = i;
                if(cell.StringCellValue == waveLeftTimeCol) waveLeftTimeIdx  = i;
                if(cell.StringCellValue == waveRightIdCol) waveRightIdIdx   = i;
                if(cell.StringCellValue == waveRightTimeCol) waveRightTimeIdx = i;
                if(cell.StringCellValue == monsterIdCol) monsterIdIdx     = i;
                if(cell.StringCellValue == monsterTimeCol) monsterTimeIdx   = i;
            }

            var dayTimes = new List<StageTimelineInfo.DayTimeInfo>();

            for(int i = sheet.FirstRowNum + 1; i < sheet.LastRowNum; i++) {
                var row = GetRow(sheet, i);
                var header = GetCell(row, 0);
                if(header.StringCellValue == "END")
                    break;

                DayTimePart timepart = (DayTimePart)Enum.Parse(typeof(DayTimePart), GetCell(row, timepartIdx).StringCellValue);
                float period = Convert.ToSingle(GetCell(row, periodIdx).NumericCellValue);

                var leftEnemies = new List<StageTimelineInfo.EnemySpawnInfo>();
                var rightEnemies = new List<StageTimelineInfo.EnemySpawnInfo>();
                var monsters = new List<StageTimelineInfo.MonsterSpawnInfo>();


                var waveLeftIdCell = GetCell(row, waveLeftIdIdx);
                if(waveLeftIdCell.CellType != CellType.Blank) {
                    var waveLeftIds = new List<int>();
                    var waveLeftTimes = new List<float>();

                    if(waveLeftIdCell.CellType == CellType.Numeric) {
                        waveLeftIds.Add(Convert.ToInt32(waveLeftIdCell.NumericCellValue));
                    } else {
                        waveLeftIds = waveLeftIdCell.StringCellValue.Split(new char[] { ',' }).Select(e => Convert.ToInt32(e)).ToList();
                    }

                    var waveLeftTimeCell = GetCell(row, waveLeftTimeIdx);
                    if(waveLeftTimeCell.CellType == CellType.Numeric) {
                        waveLeftTimes.Add(Convert.ToSingle(waveLeftTimeCell.NumericCellValue));
                    } else {
                        waveLeftTimes = waveLeftTimeCell.StringCellValue.Split(new char[] { ',' }).Select(e => Convert.ToSingle(e)).ToList();
                    }

                    for(int j = 0; j < Mathf.Min(waveLeftIds.Count, waveLeftTimes.Count); j++) {
                        leftEnemies.Add(new StageTimelineInfo.EnemySpawnInfo(waveLeftIds[j], waveLeftTimes[j]));
                    }
                }

                var waveRightIdCell = GetCell(row, waveRightIdIdx);
                if(waveRightIdCell.CellType != CellType.Blank) {
                    var waveRightIds = new List<int>();
                    var waveRightTimes = new List<float>();

                    if(waveRightIdCell.CellType == CellType.Numeric) {
                        waveRightIds.Add(Convert.ToInt32(waveRightIdCell.NumericCellValue));
                    } else {
                        waveRightIds = waveRightIdCell.StringCellValue.Split(new char[] { ',' }).Select(e => Convert.ToInt32(e)).ToList();
                    }

                    var waveRightTimeCell = GetCell(row, waveRightTimeIdx);
                    if(waveRightTimeCell.CellType == CellType.Numeric) {
                        waveRightTimes.Add(Convert.ToSingle(waveRightTimeCell.NumericCellValue));
                    } else {
                        waveRightTimes = waveRightTimeCell.StringCellValue.Split(new char[] { ',' }).Select(e => Convert.ToSingle(e)).ToList();
                    }

                    for(int j = 0; j < Mathf.Min(waveRightIds.Count, waveRightTimes.Count); j++) {
                        rightEnemies.Add(new StageTimelineInfo.EnemySpawnInfo(waveRightIds[j], waveRightTimes[j]));
                    }
                }

                var monsterIdCell = GetCell(row, monsterIdIdx);
                if(monsterIdCell.CellType != CellType.Blank) {
                    var monsterIds = new List<int>();
                    var monsterTimes = new List<float>();

                    if(monsterIdCell.CellType == CellType.Numeric) {
                        monsterIds.Add(Convert.ToInt32(monsterIdCell.NumericCellValue));
                    } else {
                        monsterIds = monsterIdCell.StringCellValue.Split(new char[] { ',' }).Select(e => Convert.ToInt32(e)).ToList();
                    }

                    var monsterTimeCell = GetCell(row, monsterTimeIdx);
                    if(monsterTimeCell.CellType == CellType.Numeric) {
                        monsterTimes.Add(Convert.ToSingle(monsterTimeCell.NumericCellValue));
                    } else {
                        monsterTimes = monsterTimeCell.StringCellValue.Split(new char[] { ',' }).Select(e => Convert.ToSingle(e)).ToList();
                    }

                    for(int j = 0; j < Mathf.Min(monsterIds.Count, monsterTimes.Count); j++) {
                        monsters.Add(new StageTimelineInfo.MonsterSpawnInfo(monsterIds[j], monsterTimes[j]));
                    }
                }

                var wave = new StageTimelineInfo.WaveInfo(leftEnemies, rightEnemies);

                var dayTime = new StageTimelineInfo.DayTimeInfo(timepart, period, wave, monsters);

                dayTimes.Add(dayTime);
            }

            var timelineInfo = CreateInstance<StageTimelineInfo>();
            timelineInfo.Init(dayTimes);

            string path = Path.Combine(FolderPath, sheetName);
            path = Path.ChangeExtension(path, "asset");

            Debug.Log(path);

            try {
                UnityEditor.AssetDatabase.CreateAsset(timelineInfo, path);
            }
            catch(Exception e) {
                Debug.LogError(e.Message);
                return;
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
    }
}
