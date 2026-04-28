using UnityEngine;
using UnityEditor; // 必須引用編輯器命名空間

[CustomEditor(typeof(TileData))]
public class TileDataEditor : Editor
{
    public override void OnInspectorGUI()
    {
        // 取得目標物件
        TileData data = (TileData)target;

        // 1. 繪製基本資訊
        data.tileName = EditorGUILayout.TextField("格子名稱", data.tileName);
        data.tileMaterial = (Material)EditorGUILayout.ObjectField("格子材質", data.tileMaterial, typeof(Material), false);

        EditorGUILayout.Space();
        data.effectType = (TileEffectType)EditorGUILayout.EnumPopup("屬性類型", data.effectType);
        EditorGUILayout.Space();

        // 2. 根據下拉選單顯示不同介面
        switch (data.effectType)
        {
            case TileEffectType.MoveSteps:
                data.isRandomSteps = EditorGUILayout.Toggle("是否隨機步數", data.isRandomSteps);
                if (!data.isRandomSteps)
                    data.steps = EditorGUILayout.IntField("移動步數 (負數為後退)", data.steps);
                else
                    EditorGUILayout.HelpBox("系統將自動產生隨機步數", MessageType.Info);
                break;

            case TileEffectType.Teleport:
                data.isRandomTeleport = EditorGUILayout.Toggle("是否隨機傳送", data.isRandomTeleport);
                if (!data.isRandomTeleport)
                    data.targetTileIndex = EditorGUILayout.IntField("傳送目標索引", data.targetTileIndex);
                else
                    EditorGUILayout.HelpBox("將隨機傳送至地圖任一處", MessageType.Info);
                break;
        }

        // 儲存修改 (這行很重要，否則重開專案資料會消失)
        if (GUI.changed) EditorUtility.SetDirty(data);
    }
}