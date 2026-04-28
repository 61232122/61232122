using UnityEngine;

public enum TileEffectType { None, MoveSteps, Teleport }

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
}