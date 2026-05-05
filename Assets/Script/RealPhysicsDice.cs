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
    public float throwForce = 5f;
    public float torqueForce = 500f;
    public Vector3 spawnHeight = new Vector3(0, 10, 0);

    [Serializable]
    public struct DiceFace
    {
        public int value;
        public Vector3 localDirection;
    }
    
    // 初始化六個面的標準方向
    public DiceFace[] diceFaces = new DiceFace[] {
        new DiceFace { value = 1, localDirection = Vector3.up },
        new DiceFace { value = 6, localDirection = Vector3.down },
        new DiceFace { value = 2, localDirection = Vector3.back },
        new DiceFace { value = 5, localDirection = Vector3.forward },
        new DiceFace { value = 3, localDirection = Vector3.left },
        new DiceFace { value = 4, localDirection = Vector3.right }
    };

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        startPos = transform.position;
        rb.useGravity = false;
        rb.isKinematic = true;
    }

    public void RollDice()
    {
        if (!isRolling && gameObject.activeInHierarchy) 
            StartCoroutine(ThrowRoutine());
    }

    IEnumerator ThrowRoutine()
    {
        isRolling = true;

        transform.position = startPos + spawnHeight;
        transform.rotation = UnityEngine.Random.rotation;

        rb.isKinematic = false;
        rb.useGravity = true;

        rb.AddForce(Vector3.down * throwForce, ForceMode.Impulse);
        rb.AddTorque(UnityEngine.Random.insideUnitSphere * torqueForce, ForceMode.Impulse);

        yield return new WaitForSeconds(1f);

        // 修正：增加一個安全計時器，防止無限迴圈
        float timeout = 5f; 
        // 這裡統一使用 velocity 以相容多數版本
        while ((rb.linearVelocity.magnitude > 0.05f || rb.angularVelocity.magnitude > 0.05f) && timeout > 0)
        {
            timeout -= 0.2f;
            yield return new WaitForSeconds(0.2f);
        }

        int result = DetectSide();
        yield return StartCoroutine(SnapRotation());

        isRolling = false;
        Debug.Log($"骰子落定！結果：{result}");
    }

    int DetectSide()
    {
        if (diceFaces == null || diceFaces.Length == 0) return -1;

        float maxDot = -2f;
        int result = 0;
        foreach (var face in diceFaces)
        {
            Vector3 worldDir = transform.TransformDirection(face.localDirection);
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
        rb.isKinematic = true;
        Quaternion startRot = transform.rotation;

        Vector3 euler = transform.eulerAngles;
        Vector3 snappedEuler = new Vector3(
            Mathf.Round(euler.x / 90f) * 90f,
            Mathf.Round(euler.y / 90f) * 90f, // 修改：Y軸也吸附會更穩定
            Mathf.Round(euler.z / 90f) * 90f
        );

        Quaternion endRot = Quaternion.Euler(snappedEuler);
        float t = 0;
        while (t < 1)
        {
            t += Time.deltaTime * 5f;
            transform.rotation = Quaternion.Slerp(startRot, endRot, t);
            yield return null;
        }
        transform.rotation = endRot;
    }
    private void Update()
    {
        // 偵測是否按下空白鍵 (Space)
        if (Input.GetKeyDown(KeyCode.Space))
        {
            // 呼叫原本的丟骰子邏輯
            RollDice();
        }
    }

}
