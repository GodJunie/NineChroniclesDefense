using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.Utilities;
using Sirenix.Utilities.Editor;
using Sirenix.OdinInspector.Editor;
using UnityEditor;

namespace G2T.NCD.Table.Editor {
    using Table;

    public class DataWindow : OdinMenuEditorWindow {
        [MenuItem("게임2팀/데이터")]
        private static void OpenWindow() {
            var window = GetWindow<DataWindow>("데이터");
            window.position = GUIHelper.GetEditorWindowRect().AlignCenter(1200, 700);
        }

        private static OdinMenuStyle customMenuStyle = new OdinMenuStyle {
            BorderPadding = 0f,
            AlignTriangleLeft = true,
            TriangleSize = 16f,
            TrianglePadding = 0f,
            Offset = 20f,
            Height = 23,
            IconPadding = 0f,
            BorderAlpha = 0.323f
        };

        protected override OdinMenuTree BuildMenuTree() {
            OdinMenuTree tree = new OdinMenuTree() {
                //{ "아이템 정보 테이블", ItemTable.Instance },
                { "몬스터 정보 테이블", MonsterTableLoader.Instance },
                { "적군 정보 테이블", EnemyTableLoader.Instance },
                { "적군 프리셋 테이블", EnemyPresetTableLoader.Instance },
                { "아이템 테이블", ItemTableLoader.Instance },
                { "스테이지 정보 테이블", StageTableLoader.Instance },

                { "건물 테이블", BuildingTableLoader.Instance },
                { "건물 스탯 테이블", BuildingStatusTableLoader.Instance },
                { "건물 스탯 테이블/몬스터 하우스 스탯 테이블", MonsterHouseStatusTableLoader.Instance },
                { "건물 스탯 테이블/합성대 스탯 테이블", RestaurantStatusTableLoader.Instance },

                { "몬스터 스탯 테이블", MonsterStatusTableLoader.Instance },
                { "스테이지 타임라인 테이블", StageTimelineTableLoader.Instance },
            };

            tree.Config.DrawSearchToolbar = true;
            // 메뉴 설정
            tree.DefaultMenuStyle = customMenuStyle;

            tree.AddAllAssetsAtPath("몬스터 정보 테이블", MonsterTableLoader.Instance.FolderPath, typeof(MonsterTable));
            tree.AddAllAssetsAtPath("적군 정보 테이블", EnemyTableLoader.Instance.FolderPath, typeof(EnemyTable));
            tree.AddAllAssetsAtPath("적군 프리셋 테이블", EnemyPresetTableLoader.Instance.FolderPath, typeof(EnemyPresetTable));
            tree.AddAllAssetsAtPath("아이템 테이블", ItemTableLoader.Instance.FolderPath, typeof(ItemTable));
            tree.AddAllAssetsAtPath("스테이지 정보 테이블", StageTableLoader.Instance.FolderPath, typeof(StageTable));

            tree.AddAllAssetsAtPath("건물 테이블", BuildingTableLoader.Instance.FolderPath, typeof(BuildingTable));
            tree.AddAllAssetsAtPath("건물 스탯 테이블", BuildingStatusTableLoader.Instance.FolderPath, typeof(BuildingStatusTable));

            tree.AddAllAssetsAtPath("몬스터 스탯 테이블", MonsterStatusTableLoader.Instance.FolderPath, typeof(MonsterStatusTable));
            tree.AddAllAssetsAtPath("스테이지 타임라인 테이블", StageTimelineTableLoader.Instance.FolderPath, typeof(StageTimelineTable));

            tree.AddObjectAtPath("에디터 스타일 편집", customMenuStyle);

            return tree;
        }
    }
}