using UnityEngine;

[ExecuteInEditMode] // 讓腳本在編輯模式下也能執行
public class TileProperty : MonoBehaviour
{
    public TileData data;

    // 用來儲存格子原始的材質，以便清空時恢復
    [SerializeField] private Material defaultMaterial;

    // 當腳本第一次掛載或點擊 Reset 時執行
    private void Reset()
    {
        Renderer ren = GetComponent<Renderer>();
        if (ren != null)
        {
            // 紀錄當下（還沒放 SO 前）的材質作為預設值
            defaultMaterial = ren.sharedMaterial;
        }
    }

    // 關鍵：當 Inspector 數值發生改變（包括拖入或清空 SO）時，Unity 會自動呼叫此方法
    private void OnValidate()
    {
        UpdateAppearance();
    }

    private void UpdateAppearance()
    {
        Renderer ren = GetComponent<Renderer>();
        if (ren == null) return;

        if (data != null && data.tileMaterial != null)
        {
            // 1. 如果有資料，直接更換材質
            ren.sharedMaterial = data.tileMaterial;
            // 同時可以同步名稱 (選配)
            if (!string.IsNullOrEmpty(data.tileName)) gameObject.name = "Tile_" + data.tileName;
        }
        else
        {
            // 2. 如果 data 被清空 (null)，恢復成預設材質
            if (defaultMaterial != null)
            {
                ren.sharedMaterial = defaultMaterial;
                gameObject.name = "Tile_Normal";
            }
        }
    }
}