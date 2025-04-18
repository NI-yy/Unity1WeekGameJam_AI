using UnityEngine;
using R3;

public class PlayerController : MonoBehaviour
{
    public bool inputEnabled = false;

    [SerializeField] private float speed = 5f; // プレイヤーの移動速度 
    [SerializeField] private float parryActiveTime = 0.5f;
    [SerializeField] private int parryCoolFrame = 30;
    [SerializeField] private GameObject parrtyArea;

    private float hAxis; // 水平方向の入力
    private float vAxis; // 垂直方向の入力

    private Vector3 moveVector;

    private bool dashButtonDown;
    private bool parryButtonDown;

    private bool isDodge = false;
    private bool canParry = true;

    void Update()
    {
        if (inputEnabled)
        {
            GetInput();
            Move();
            Turn();
            TriggerParry();
            Dash();
        }
    }

    private void GetInput()
    {
        hAxis = Input.GetAxisRaw("Horizontal"); // 水平方向の入力を取得
        vAxis = Input.GetAxisRaw("Vertical"); // 垂直方向の入力を取得
        parryButtonDown = Input.GetMouseButtonDown(0);
        dashButtonDown = Input.GetMouseButtonDown(1);
    }

    private void Move()
    {
        Vector3 rawInput = new Vector3(hAxis, 0, vAxis);
        if(rawInput.sqrMagnitude < 0.01f)
        {
            moveVector = Vector3.zero;
        }
        else
        {
            moveVector = rawInput.normalized;
        }
        
        // 通常の移動処理
        transform.position += moveVector * speed * Time.deltaTime;
    }

    private void Turn(){
        transform.LookAt(transform.position + moveVector);
    }

    private void TriggerParry()
    {
        if (parryButtonDown && canParry)
        {
            parrtyArea.SetActive(true);
            canParry = false;

            // 0.5秒後に判定オブジェクトを非アクティブ化
            Observable.Timer(System.TimeSpan.FromSeconds(parryActiveTime))
                .Subscribe(_ => parrtyArea.SetActive(false))
                .AddTo(this);

            // 30フレーム後に再び入力を受け付ける
            Observable.TimerFrame(parryCoolFrame)
                .Subscribe(_ => canParry = true)
                .AddTo(this);
        }
    }

    private void Dash(){
        if(dashButtonDown && moveVector != Vector3.zero && !isDodge){
            speed *= 2;
            isDodge = true;

            Invoke("DashOut", 0.4f); // hoge秒後にDashOutメソッドを呼び出す
        }
    }

    private void DashOut(){
        speed *= 0.5f;
        isDodge = false;
    }
}
