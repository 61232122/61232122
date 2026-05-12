/*
using UnityEngine;
using UnityEngine.UI; // 使用傳統 Text 必須引用此項

public class LobbyManager : MonoBehaviour
{
    [Header("UI 面板設定")]
    public GameObject homePanel; // 放置你的 Home 空物件
    public GameObject gamePanel; // 放置你的 Game 空物件

    [Header("遊戲核心引用")]
    public GameManager gameManager; // 引用場景中的 GameManager

    void Start()
    {
        // 遊戲啟動時：顯示選單，隱藏遊戲 UI
        homePanel.SetActive(true);
        gamePanel.SetActive(false);
    }

    // 此函式由按鈕呼叫
    public void SelectPlayers(int count)
    {
        // 1. 切換 UI 狀態
        homePanel.SetActive(false); // 關閉首頁
        gamePanel.SetActive(true);  // 開啟遊戲 UI

        // 2. 啟動遊戲邏輯 (通知 GameManager 生成人數)
        if (gameManager != null)
        {
            gameManager.StartGame(count);
        }
        else
        {
            Debug.LogError("找不到 GameManager！請在 Inspector 中拖入。");
        }
    }
}
*/