using UnityEngine;

// 新增 ChangeMoney 項目
public enum TileEffectType { None, MoveSteps, Teleport, ChangeMoney }

[CreateAssetMenu(fileName = "NewTileData", menuName = "BoardGame/TileData")]
public class TileData : ScriptableObject
{
    public string tileName;
    public Material tileMaterial;
    public TileEffectType effectType;

    // --- 移動屬性 ---
    public int steps;
    public bool isRandomSteps;

    // --- 傳送屬性 ---
    public int targetTileIndex;
    public bool isRandomTeleport;

    // --- 經濟系統屬性 (新增) ---
    [Header("經濟設定 (正數加錢，負數扣錢)")]
    public int moneyAmount;
}