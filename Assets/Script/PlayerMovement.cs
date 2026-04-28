using UnityEngine;
using System.Collections;

public class PlayerMovement : MonoBehaviour
{
    [Header("地圖資料")]
    public MapData mapData; // 連結先前做好的 MapData

    [Header("移動設定")]
    public float moveSpeed = 5f;    // 移動速度
    public float pauseTime = 0.2f; // 每到一格停頓的時間

    private int currentTileIndex = 0; // 紀錄目前在哪一格
    private bool isMoving = false;

    // --- 給骰子腳本呼叫的入口 ---
    public void StartMove(int steps)
    {
        if (isMoving) return;
        StartCoroutine(MoveRoutine(steps));
    }

    IEnumerator MoveRoutine(int steps)
    {
        isMoving = true;

        for (int i = 0; i < steps; i++)
        {
            // 1. 計算下一格的索引（使用 % 確保超過最後一格會回到 0）
            currentTileIndex = (currentTileIndex + 1) % mapData.tileList.Count;

            // 2. 取得目標格子的座標
            Vector3 targetPos = mapData.tileList[currentTileIndex].position;

            // 為了不讓棋子陷進地板，我們微調 Y 軸（高度）
            targetPos.y = transform.position.y;

            // 3. 平滑移動到該格子
            while (Vector3.Distance(transform.position, targetPos) > 0.01f)
            {
                transform.position = Vector3.MoveTowards(
                    transform.position,
                    targetPos,
                    moveSpeed * Time.deltaTime
                );
                yield return null; // 等待下一幀
            }

            // 4. 到達該格，稍微停頓一下增加節奏感
            yield return new WaitForSeconds(pauseTime);
        }

        isMoving = false;
        Debug.Log($"玩家抵達第 {currentTileIndex} 格");
    }
}