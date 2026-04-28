using UnityEngine;

// 讓這個腳本在編輯模式也能執行，不用按下 Play 就能看到線
[ExecuteInEditMode]
public class DiceGizmoVisualizer : MonoBehaviour
{
    [System.Serializable]
    public struct DiceFace
    {
        public int value;
        public Vector3 localDirection;
    }

    [Header("請根據骰子面設定方向")]
    public DiceFace[] diceFaces = new DiceFace[6] {
        new DiceFace { value = 1, localDirection = Vector3.up },
        new DiceFace { value = 2, localDirection = Vector3.down },
        new DiceFace { value = 3, localDirection = Vector3.right },
        new DiceFace { value = 4, localDirection = Vector3.left },
        new DiceFace { value = 5, localDirection = Vector3.forward },
        new DiceFace { value = 6, localDirection = Vector3.back }
    };

    private void OnDrawGizmos()
    {
        // 將繪圖矩陣設為物件本身的座標系
        Gizmos.matrix = transform.localToWorldMatrix;

        foreach (var face in diceFaces)
        {
            // 畫出箭頭
            Gizmos.color = Color.cyan;
            Gizmos.DrawRay(Vector3.zero, face.localDirection * 0.5f);

            // 畫出實心球代表末端
            Gizmos.DrawSphere(face.localDirection * 0.5f, 0.05f);

#if UNITY_EDITOR
            // 標記文字
            UnityEditor.Handles.matrix = transform.localToWorldMatrix;
            UnityEditor.Handles.Label(face.localDirection * 0.55f, face.value.ToString());
#endif
        }
    }
}