using UnityEngine;
using System.Collections;

public class PlayerMovement : MonoBehaviour
{
    [Header("角色狀態")]
    public string playerName;
    public bool isAI; // 由 GameManager 生成時自動設定
    private bool isMoving = false;
    private int currentTileIndex = 0; // 紀錄目前在哪一格

    [Header("地圖資料")]
    public MapData mapData; // 現在會由 Awake 自動抓取

    [Header("移動設定")]
    public float moveSpeed = 5f;    // 移動速度
    public float pauseTime = 0.2f; // 每到一格停頓的時間

    [Header("排隊設定")]
    public Vector3 myOffset; // 這隻棋子專屬的偏移位置

    void Awake()
    {
        // --- 自動抓取 MapData 邏輯 ---
        // 尋找場景中的 GameManager，並取得其身上引用的 MapData
        if (mapData == null)
        {
            GameManager gm = Object.FindObjectOfType<GameManager>();
            if (gm != null)
            {
                // 注意：GameManager 必須先將 MapData 變數設為 public
                mapData = gm.mapData;
            }
        }
    }

    public void StartMove(int steps)
    {
        if (isMoving) return;
        if (mapData == null)
        {
            Debug.LogError($"{playerName} 找不到 MapData！請確認 GameManager 物件上有拖入 MapData。");
            return;
        }
        StartCoroutine(MoveRoutine(steps));
    }

    IEnumerator MoveRoutine(int steps)
    {
        isMoving = true;

        // --- 核心修正 1：處理方向與絕對步數 ---
        int direction = (steps >= 0) ? 1 : -1;
        int absSteps = Mathf.Abs(steps);

        for (int i = 0; i < absSteps; i++)
        {
            // --- 核心修正 2：安全的陣列輪迴計算 ---
            if (direction == 1)
            {
                currentTileIndex = (currentTileIndex + 1) % mapData.tileList.Count;
            }
            else
            {
                // 加上 mapData.tileList.Count 避免產生負數索引報錯
                currentTileIndex = (currentTileIndex - 1 + mapData.tileList.Count) % mapData.tileList.Count;
            }

            // 取得目標格座標並加上偏移量
            Vector3 targetPos = mapData.tileList[currentTileIndex].position + myOffset;
            targetPos.y = transform.position.y;

            // 平滑移動
            while (Vector3.Distance(transform.position, targetPos) > 0.01f)
            {
                transform.position = Vector3.MoveTowards(
                    transform.position,
                    targetPos,
                    moveSpeed * Time.deltaTime
                );
                yield return null;
            }
            yield return new WaitForSeconds(pauseTime);
        }

        isMoving = false;
        Debug.Log($"{playerName} 抵達第 {currentTileIndex} 格");

        // --- 核心修正 3：檢查格子效果並分流協程 ---
        bool triggeredNewMove = CheckTileEffect();

        // 只有在「沒有觸發連續移動」時，才呼叫 GameManager 換下一個人
        if (!triggeredNewMove)
        {
            yield return new WaitForSeconds(1.0f); // 加一點小延遲讓玩家看清楚
            GameManager gm = Object.FindObjectOfType<GameManager>();
            if (gm != null)
            {
                gm.NextTurn(); // 呼叫切換回合
            }
        }
    }

    // 將回傳值改為 bool，讓 MoveRoutine 知道是否開啟了新的移動
    private bool CheckTileEffect()
    {
        TileProperty currentTile = mapData.tileList[currentTileIndex].GetComponent<TileProperty>();

        if (currentTile != null && currentTile.data != null)
        {
            TileData data = currentTile.data;

            switch (data.effectType)
            {
                case TileEffectType.MoveSteps:
                    int stepsToMove = data.isRandomSteps ? UnityEngine.Random.Range(1, 4) : data.steps;
                    if (stepsToMove != 0) // 防呆機制
                    {
                        Debug.Log($"{playerName} 觸發移動效果：{stepsToMove} 步");
                        StartCoroutine(MoveRoutine(stepsToMove));
                        return true; // 回傳 true：代表正在執行新的移動協程，請先不要換玩家
                    }
                    break;

                case TileEffectType.Teleport:
                    int targetIndex = data.isRandomTeleport ? UnityEngine.Random.Range(0, mapData.tileList.Count) : data.targetTileIndex;
                    Debug.Log($"{playerName} 觸發傳送，目標索引：{targetIndex}");
                    TeleportTo(targetIndex);
                    return false; // 傳送是瞬間完成的，可以直接回傳 false 讓原本的程式繼續換人
            }
        }
        return false;
    }

    public void TeleportTo(int targetIndex)
    {
        if (targetIndex < 0 || targetIndex >= mapData.tileList.Count) return;

        currentTileIndex = targetIndex;
        Vector3 targetPos = mapData.tileList[currentTileIndex].position + myOffset;
        targetPos.y = transform.position.y;
        transform.position = targetPos;
    }
}