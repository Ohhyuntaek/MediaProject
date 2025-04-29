using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

public class DetectionPatternEditor : EditorWindow
{
    // 선택한 패턴 에셋
    private DetectionPatternSO _pattern;
    // 그리드 절반 크기 (예: 5 x 5 그리드면 gridSize = 2)
    private int _gridSize = 4;

    [MenuItem("Window/Detection Pattern Editor")]
    public static void OpenWindow()
    {
        GetWindow<DetectionPatternEditor>("Detection Pattern");
    }

    private void OnGUI()
    {
        GUILayout.Label("DetectionPattern 에디터", EditorStyles.boldLabel);
        EditorGUI.BeginChangeCheck();
        _pattern = (DetectionPatternSO)EditorGUILayout.ObjectField("패턴 SO", _pattern, typeof(DetectionPatternSO), false);
        _gridSize = EditorGUILayout.IntSlider("그리드 범위", _gridSize, 1, 8);

        if (_pattern == null)
        {
            EditorGUILayout.HelpBox("먼저 DetectionPattern 에셋을 드래그하세요.", MessageType.Info);
            return;
        }

        if (EditorGUI.EndChangeCheck())
            Repaint();

        DrawGridEditor();
    }

    private void DrawGridEditor()
    {
        var size = _gridSize;
        var boxSize = 20;
        var center = size;

        // 그리드 토글 버튼 배열 생성
        bool[,] toggles = new bool[size * 2 + 1, size * 2 + 1];
        // 현재 오프셋을 바탕으로 토글 초기화
        foreach (var ofs in _pattern.cellOffsets)
        {
            int x = ofs.x + center;
            int y = -ofs.y + center; // Y는 위로 증가하므로 반전
            if (x >= 0 && x < toggles.GetLength(0)
             && y >= 0 && y < toggles.GetLength(1))
                toggles[x, y] = true;
        }

        // 토글 그리기
        GUILayout.Space(10);
        for (int y = 0; y < toggles.GetLength(1); y++)
        {
            GUILayout.BeginHorizontal();
            for (int x = 0; x < toggles.GetLength(0); x++)
            {
                // (center,center)는 기준(0,0)
                var style = (x == center && y == center)
                    ? GUI.skin.button
                    : GUI.skin.toggle;
                toggles[x, y] = GUILayout.Toggle(toggles[x, y], "", style, GUILayout.Width(boxSize), GUILayout.Height(boxSize));
            }
            GUILayout.EndHorizontal();
        }

        // 토글 변경사항을 SO에 반영
        if (GUILayout.Button("패턴 저장"))
        {
            Undo.RecordObject(_pattern, "Modify DetectionPattern");
            var newOffsets = new List<Vector2Int>();
            for (int x = 0; x < toggles.GetLength(0); x++)
            for (int y = 0; y < toggles.GetLength(1); y++)
            {
                if (!toggles[x, y]) continue;
                int ox = x - center;
                int oy = -(y - center);
                // 기준 셀(0,0)은 제외하거나 포함시킬 수 있습니다
                if (ox == 0 && oy == 0) continue;
                newOffsets.Add(new Vector2Int(ox, oy));
            }
            _pattern.cellOffsets = newOffsets;
            EditorUtility.SetDirty(_pattern);
        }
    }
}
