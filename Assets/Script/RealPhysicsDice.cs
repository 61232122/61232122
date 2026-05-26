using UnityEngine;
using System.Collections;
using System;

[RequireComponent(typeof(Rigidbody))]
public class RealPhysicsDice : MonoBehaviour
{
    private Rigidbody rb;
    private Vector3 startPos;
    private bool isRolling = false;

    [Header("物理力道")]
    public float throwForce = 5f;     // 往下/往前丟的力量
    public float torqueForce = 500f;  // 隨機亂轉的力量
    public Vector3 spawnHeight = new Vector3(0, 10, 0); // 初始高空位置

    public PlayerMovement player;

    // 定義六個面與點數的關係
    [Serializable]
    public struct DiceFace
    {
        public int value;
        public Vector3 localDirection;
    }
    public DiceFace[] diceFaces;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        startPos = transform.position;
        rb.useGravity = false;
        rb.isKinematic = true; // 初始固定
    }

    public void RollDice()
    {
        if (!isRolling) StartCoroutine(ThrowRoutine());
    }

    IEnumerator ThrowRoutine()
    {
        isRolling = true;

        // 1. 初始化：瞬移到高空並隨機旋轉
        transform.position = startPos + spawnHeight;
        transform.rotation = UnityEngine.Random.rotation;

        rb.isKinematic = false;
        rb.useGravity = true;

        // 2. 給予初始推力與扭力 (讓它滾得更漂亮)
        rb.AddForce(Vector3.down * throwForce, ForceMode.Impulse);
        rb.AddTorque(new Vector3(
            UnityEngine.Random.Range(-torqueForce, torqueForce),
            UnityEngine.Random.Range(-torqueForce, torqueForce),
            UnityEngine.Random.Range(-torqueForce, torqueForce)
        ), ForceMode.Impulse);

        // 3. 監控狀態：等待骰子完全靜止
        // 先等 1 秒讓它落地彈跳
        yield return new WaitForSeconds(1f);

        // 判斷靜止：當移動與旋轉速度都極小時
        while (rb.linearVelocity.magnitude > 0.01f || rb.angularVelocity.magnitude > 0.01f)
        {
            yield return new WaitForSeconds(0.2f);
        }

        // 4. 偵測點數並移動
        int result = DetectSide();
        yield return StartCoroutine(SnapRotation());

        isRolling = false;

        if (player != null)
        {
            // 執行移動，並等待移動完成
            // 我們修改 StartMove 讓它變成可以等待的協程，或者在 GameManager 處理切換
            player.StartMove(result);
        }
    }

    int DetectSide()
    {
        float maxDot = -2f;
        int result = 0;
        foreach (var face in diceFaces)
        {
            // 將本地向量轉為世界方向
            Vector3 worldDir = transform.TransformDirection(face.localDirection);
            // 利用點積比較與世界正上方的夾角
            float dot = Vector3.Dot(worldDir, Vector3.up);
            if (dot > maxDot)
            {
                maxDot = dot;
                result = face.value;
            }
        }
        return result;
    }

    IEnumerator SnapRotation()
    {
        rb.isKinematic = true; // 關閉物理，準備微調
        Quaternion startRot = transform.rotation;

        // 將旋轉角度四捨五入到最近的 90 度
        Vector3 euler = transform.eulerAngles;
        Vector3 snappedEuler = new Vector3(
            Mathf.Round(euler.x / 90) * 90,
            euler.y, // 保留 Y 軸旋轉，視覺最自然
            Mathf.Round(euler.z / 90) * 90
        );

        Quaternion endRot = Quaternion.Euler(snappedEuler);
        float t = 0;
        while (t < 1)
        {
            t += Time.deltaTime * 5f; // 快速修正
            transform.rotation = Quaternion.Slerp(startRot, endRot, t);
            yield return null;
        }
        transform.rotation = endRot;
    }
}

