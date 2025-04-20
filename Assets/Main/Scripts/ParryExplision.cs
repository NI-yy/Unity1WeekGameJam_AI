using UnityEngine;

public class ParryExplision : MonoBehaviour
{
    [Header("爆散のパラメータ")]
    public float explosionForce = 10f;
    public float explosionRadius = 5f;
    public float upwardWeight = 0.8f;
    public float drag = 1.0f;
    public float angularDrag = 1.0f;
    public ForceMode forceMode = ForceMode.Impulse;

    [Header("回転のパラメータ")]
    public float torqueStrength = 5f;  // 回転の強さ

    [Header("爆発を起こすタイミング")]
    public bool explodeOnStart = true;

    void Start()
    {
        if (explodeOnStart)
        {
            Explode();
        }
    }

    public void Explode()
    {
        foreach (Transform child in transform)
        {
            Rigidbody rb = child.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.linearDamping = drag;
                rb.angularDamping = angularDrag;
                rb.useGravity = true;

                // ランダムな飛ぶ方向
                Vector3 direction = (child.position - transform.position).normalized;
                Vector3 randomOffset = Random.insideUnitSphere * 0.3f;
                Vector3 upward = Vector3.up * upwardWeight;
                Vector3 forceDirection = (direction + randomOffset + upward).normalized;

                // ランダムな回転軸
                Vector3 randomTorque = Random.onUnitSphere * torqueStrength;

                // 力と回転を加える
                rb.AddForce(forceDirection * explosionForce, forceMode);
                rb.AddTorque(randomTorque, forceMode);
            }
        }
    }
}
