using UnityEngine;
using System.Collections.Generic;

public class MapData : MonoBehaviour
{
    [Header("地圖設定")]
    public List<Transform> tileList = new List<Transform>();

    [Header("自動檢查設定")]
    public float maxStepDistance = 2.0f;

    [ContextMenu("1. 自動抓取所有格子")]
    public void GetAllTiles()
    {
        tileList.Clear();
        foreach (Transform child in transform)
        {
            tileList.Add(child);
        }
        Debug.Log($"<color=cyan>地圖掃描完成！目前共有 {tileList.Count} 個格子。</color>");
    }

    private void OnDrawGizmos()
    {
        if (tileList.Count < 2) return;

        for (int i = 0; i < tileList.Count; i++)
        {
            Transform current = tileList[i];
            // 下一格 (i+1)
            int nextIndex = (i + 1) % tileList.Count;
            Transform next = tileList[nextIndex];

            // 下下格 (i+2) - 用來檢查順序是否打結
            int afterNextIndex = (i + 2) % tileList.Count;
            Transform afterNext = tileList[afterNextIndex];

            if (current == null || next == null) continue;

            float distToNext = Vector3.Distance(current.position, next.position);

            // --- 順序衝突檢查 (Sequence Error) ---
            // 如果「下下格」比「下一格」還要靠近目前這格，代表順序可能反了
            float distToAfterNext = Vector3.Distance(current.position, afterNext.position);
            bool isOrderWrong = distToAfterNext < distToNext * 0.7f; // 閾值可調整

            // --- 繪製連線邏輯 ---
            if (isOrderWrong)
            {
                // 順序打結：畫紫色粗線，表示這條線「繞路」了
                Gizmos.color = Color.magenta;
                DrawThickLine(current.position, next.position, 3);
                Gizmos.DrawSphere((current.position + next.position) / 2, 0.2f);
            }
            else if (distToNext > maxStepDistance)
            {
                // 距離太遠：畫紅色
                Gizmos.color = Color.red;
                Gizmos.DrawLine(current.position, next.position);
            }
            else
            {
                // 正常：畫黃色或綠色
                Gizmos.color = (i == tileList.Count - 1) ? Color.green : Color.yellow;
                Gizmos.DrawLine(current.position, next.position);
            }

            // 繪製格子編號（輔助教學）
#if UNITY_EDITOR
            UnityEditor.Handles.Label(current.position + Vector3.up * 0.3f, i.ToString());
#endif

            Gizmos.color = (i == 0) ? Color.red : Color.white;
            Gizmos.DrawSphere(current.position, 0.1f);
        }
    }

    // 輔助畫粗線的小工具
    void DrawThickLine(Vector3 start, Vector3 end, int thickness)
    {
        for (int i = 0; i < thickness; i++)
        {
            Gizmos.DrawLine(start + new Vector3(i * 0.01f, 0, 0), end + new Vector3(i * 0.01f, 0, 0));
        }
    }
}