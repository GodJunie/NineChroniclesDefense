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
                { "몬스터 정보 테이블", new MonsterTableLoader() },
                { "몬스터 스탯 테이블", MonsterStatusInfoLoader.Instance },
                //{ "적군 정보 테이블", EnemyTable.Instance },
                //{ "적군 프리셋 테이블",  },
                { "스테이지 정보 테이블", StageTable.Instance },
                { "스테이지 타임라인 테이블", StageTimelineInfoLoader.Instance },
            };

            tree.Config.DrawSearchToolbar = true;
            // 메뉴 설정
            tree.DefaultMenuStyle = customMenuStyle;

            tree.AddAllAssetsAtPath("몬스터 스탯 테이블", MonsterStatusInfoLoader.Instance.FolderPath, typeof(MonsterStatusInfo));
            tree.AddAllAssetsAtPath("스테이지 타임라인 테이블", StageTimelineInfoLoader.Instance.FolderPath, typeof(StageTimelineInfo));

            tree.AddObjectAtPath("에디터 스타일 편집", customMenuStyle);

            return tree;
        }
    }
}