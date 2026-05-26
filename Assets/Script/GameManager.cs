using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    [Header("不同造型的棋子設定")]
    public GameObject[] playerPrefabs; // 3D 棋子模型 Prefab

    [Header("UI 列表設定")]
    public GameObject playerItemPrefab;   // 左側玩家資訊列的 Prefab
    public Transform playerListContainer; // PlayerListPanel 的 Transform (掛載 Vertical Layout Group 的那個)
    public Sprite[] playerSprites;        // 2D 角色頭像圖片 (順序需與 Prefabs 一致)
    public GameObject rollButton;         // 擲骰子按鈕

    [Header("生成位置設定")]
    public Transform[] startPositions;

    [Header("遊戲狀態")]
    public List<GameObject> activePlayers = new List<GameObject>();
    private List<PlayerUIItem> uiItems = new List<PlayerUIItem>(); // 儲存產生的 UI 項目
    private int currentPlayerIndex = 0;

    public MapData mapData;

    // 定義四個角落的偏移數值
    private Vector3[] offsets = new Vector3[]
    {
        new Vector3(-0.25f, 0, 0.25f),  // 左上
        new Vector3(0.25f, 0, 0.25f),   // 右上
        new Vector3(-0.25f, 0, -0.25f), // 左下
        new Vector3(0.25f, 0, -0.25f)   // 右下
    };

    public void StartGame(int totalCount)
    {
        // 1. 清除舊棋子與舊 UI
        foreach (GameObject p in activePlayers) { Destroy(p); }
        activePlayers.Clear();

        foreach (var item in uiItems) { if (item != null) Destroy(item.gameObject); }
        uiItems.Clear();

        // 2. 生成 3D 棋子與對應 UI
        for (int i = 0; i < totalCount; i++)
        {
            // --- A. 生成 3D 棋子 ---
            Vector3 spawnPos = startPositions[0].position + offsets[i];
            GameObject newPlayer = Instantiate(playerPrefabs[i], spawnPos, Quaternion.identity);
            PlayerMovement moveScript = newPlayer.GetComponent<PlayerMovement>();

            if (moveScript != null)
            {
                moveScript.isAI = (i != 0);
                moveScript.playerName = (i == 0) ? "玩家 1" : "電腦 " + i;
                moveScript.myOffset = offsets[i];
            }
            activePlayers.Add(newPlayer);

            // --- B. 生成左側 UI 項目 ---
            GameObject itemGo = Instantiate(playerItemPrefab, playerListContainer);
            PlayerUIItem uiScript = itemGo.GetComponent<PlayerUIItem>();

            if (uiScript != null)
            {
                uiScript.nameText.text = (i == 0) ? "玩家 1" : "電腦 " + i;
                // 分配頭像圖片
                if (i < playerSprites.Length) uiScript.playerIcon.sprite = playerSprites[i];
                // 預設關閉箭頭
                uiScript.arrow.SetActive(false);
                uiItems.Add(uiScript);
            }
        }

        // 3. 設定初始回合 (從玩家 1 開始)
        SetTurn(0);
        Debug.Log($"遊戲開始：已生成 {totalCount} 名玩家與 UI 列表。");
    }

    public RealPhysicsDice diceScript; // 拖入場景中的骰子

    // 由 PlayerMovement 移動結束後呼叫
    public void NextTurn()
    {
        // 計算下一個人的 index
        int nextIndex = (currentPlayerIndex + 1) % activePlayers.Count;
        SetTurn(nextIndex);
    }

    public void SetTurn(int index)
    {
        currentPlayerIndex = index;

        // 1. 更新 UI 箭頭與亮度
        for (int i = 0; i < uiItems.Count; i++)
        {
            uiItems[i].arrow.SetActive(i == index);
            uiItems[i].playerIcon.color = (i == index) ? Color.white : new Color(0.5f, 0.5f, 0.5f);
        }

        // 2. 指派目前的棋子給骰子腳本
        diceScript.player = activePlayers[index].GetComponent<PlayerMovement>();

        // 3. 判斷玩家類型
        if (index == 0)
        {
            rollButton.SetActive(true); // 真人回合，顯示按鈕
        }
        else
        {
            rollButton.SetActive(false); // 電腦回合，隱藏按鈕
            StartCoroutine(AICustomRoutine()); // 執行電腦自動動作
        }
    }

    System.Collections.IEnumerator AICustomRoutine()
    {
        Debug.Log($"電腦 {currentPlayerIndex} 思考中...");
        yield return new WaitForSeconds(1.5f); // 模擬電腦思考時間

        // 電腦自動執行擲骰
        diceScript.RollDice();
    }
}