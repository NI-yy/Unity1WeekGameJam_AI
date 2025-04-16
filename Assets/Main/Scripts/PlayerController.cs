using UnityEngine;
using UnityEngine.Scripting.APIUpdating;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float speed = 5f; // プレイヤーの移動速度 
    [SerializeField] private float warpDistance = 5f; // ワープ距離

    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private float bulletSpeed = 20f;     // 弾の速度
    [SerializeField] private Transform shootPoint;        // 発射位置（空の子オブジェクトを設定すると良い）

    private float hAxis; // 水平方向の入力
    private float vAxis; // 垂直方向の入力

    private Vector3 moveVector; // 移動ベクトル

    private bool dashButtonDown;

    private Rigidbody rigidbody;
    private bool isDodge = false;

    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
    }

    void Update()
    {
        GetInput();
        Move();
        Turn();
        Dash();
    }

    private void GetInput()
    {
        hAxis = Input.GetAxisRaw("Horizontal"); // 水平方向の入力を取得
        vAxis = Input.GetAxisRaw("Vertical"); // 垂直方向の入力を取得
        dashButtonDown = Input.GetMouseButtonDown(1);
    }

    private void Move()
    {
        // 入力に基づいて移動ベクトルを計算
        moveVector = new Vector3(hAxis, 0, vAxis).normalized;

        // 通常の移動処理
        transform.position += moveVector * speed * Time.deltaTime;
    }

    private void Turn(){
        transform.LookAt(transform.position + moveVector);
    }

    private void Dash(){
        if(dashButtonDown && moveVector != Vector3.zero && !isDodge){
            speed *= 2;
            isDodge = true;

            Invoke("DashOut", 0.4f); // 0.5秒後にDashOutメソッドを呼び出す
        }
    }

    private void DashOut(){
        speed *= 0.5f;
        isDodge = false;
    }

    private void Attack(){
        // 弾を生成
        GameObject bullet = Instantiate(bulletPrefab, shootPoint.position, Quaternion.identity);

        // Rigidbodyを取得し、プレイヤーの向いている方向に力を加える
        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.AddForce(this.transform.forward * bulletSpeed); //キャラクターが向いている方向に弾に力を加える
        }
    }
}
