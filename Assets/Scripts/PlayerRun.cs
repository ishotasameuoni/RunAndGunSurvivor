using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerRun : MonoBehaviour
{
    //横移動のX軸の限界
    const int MinLane = -2;
    const int MaxLane = 2;
    const float LaneWidth = 1.0f;

    //体力の最大値
    const int DefaultLife = 3;
    //ダメージ食らった時の硬直時間
    const float StunDuration = 0.5f;

    CharacterController controller;
    Animator animator;

    Vector3 moveDirection = Vector3.zero; //移動すべき量
    int targetLane; //向かうべきX座標
    int life = DefaultLife; //現在体力
    float recoverTime = 0.0f; //復帰までのカウントダウン

    float currentMoveInputX; //InputSystemの入力値の格納

    Coroutine resetIntervalCol; //Inputを連続受付のインターバルを担当するコルーチン

    public float gravity = 20.0f; //重力加速値
    public float speedZ = 5.0f; //前進スピード
    public float speedX = 3.0f; //横移動スピード
    public float speedJump = 8.0f; //ジャンプ力
    public float accelerationZ = 10.0f; //前進加速力

    void OnMove(InputValue value)
    {
        //入力検知前がインターバル中なら何もしない
        if (resetIntervalCol == null)
        {
            //検知した値(value)をVector2で表現して変数inputVectorに格納
            Vector2 inputVector = value.Get<Vector2>();
            //変数inputVectorのうち、x座標にまつわる値を変数currentMoveInputに格納
            currentMoveInputX = inputVector.x;
        }
    }

    void OnJump(InputValue value)
    {
        //ジャンプボタンを検知したら発動するメソッド
        Jump();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        //InputManagerシステム採用の場合
        //if (Input.GetKeyDown("left")) MoveToLeft();
        //if (Input.GetKeyDown("right")) MoveToRight();
        //if (Input.GetKeyDown("Space")) Jump();

        //左入力時
        if (currentMoveInputX < 0) MoveToLeft();
        //右入力時
        if (currentMoveInputX > 0) MoveToRight();

        if (IsStun()) //硬直がtrueならば
        {
            //x.yを0に固定
            moveDirection.x = 0;
            moveDirection.y = 0;

            //recoverTimeをカウントダウン
            recoverTime -= Time.deltaTime;
        }
        else
        {
            //その時のmoveDirection.zにaccelerationZの加速度を足していく
            float acceleratedZ = moveDirection.z + (accelerationZ * Time.deltaTime);
            //導き出した値に上限を設けて、それをmoveDirection.zとする
            moveDirection.z = Mathf.Clamp(acceleratedZ, 0, speedZ);

            //横移動のアルゴリズム
            //目的地と自分の位置の差を取り、1レーンあたりの幅に対して割合をみる
            float ratioX = (targetLane * LaneWidth - transform.position.x) / LaneWidth;

            //割合に変数speedXを係数としてかけた値がmoveDirectioin.xになる
            moveDirection.x = ratioX * speedX;
        }

        //重力の加速度をｍoveDirection.y
        moveDirection.y -= gravity * Time.deltaTime;

        //回転時、自分のローカル座標をグローバル座標に変換する
        Vector3 globalDirection = transform.TransformDirection(moveDirection);
        //CharacterControllerコンポーネントのMoveメソッドに授けてPlayerを動かす
        controller.Move(globalDirection * Time.deltaTime);

        //地面についたら重力をリセットする
        if (controller.isGrounded) moveDirection.y = 0;
    }

    public void MoveToLeft()
    {
        //硬直フラグがtrueなら何もしない
        if (IsStun()) return;

        //地面にいるかつtargetが最小ではないなら
        if (controller.isGrounded && targetLane > MinLane)
        {
            targetLane--;
            currentMoveInputX = 0; //何も入力していない状況にリセット
                                   //次の入力検知が有効になるまでのインターバルを行う
            resetIntervalCol = StartCoroutine(ResetIntervalCol());
        }
    }

    public void MoveToRight()
    {
        //硬直フラグがtrueなら何もしない
        if (IsStun()) return;

        //地面にいるかつtargetが最大ではないなら
        if (controller.isGrounded && targetLane < MaxLane)
        {
            targetLane++;
            currentMoveInputX = 0; //何も入力していない状況にリセット
                                   //次の入力検知が有効になるまでのインターバルを行う
            resetIntervalCol = StartCoroutine(ResetIntervalCol());
        }
    }

    IEnumerator ResetIntervalCol()
    {
        yield return new WaitForSeconds(0.1f); //0.1秒待つ
        resetIntervalCol = null; //リセットコルーチンを解除
    }

    //現在の体力を返す
    public int Life()
    {
        return life;
    }

    //体力を1回復する
    public void LifeUp()
    {
        life++;
        if (life > DefaultLife) life = DefaultLife;
    }

    //Playerを硬直させるべきかチェックするメソッド
    private bool IsStun()
    {
        return recoverTime > 0.0f || life <= 0;
    }

    public void Jump()
    {
        if (IsStun()) return;

        if (controller.isGrounded)
        {
            moveDirection.y = speedJump;
        }
    }

    //CharacterControllerComponentが何かとぶつかった時
    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (IsStun()) return;

        //相手がEnemyなら
        if (hit.gameObject.tag == "Enemy")
        {
            life--; //体力減少
            recoverTime = StunDuration; //recoverTimeに定数の値をセッティング

            Destroy(hit.gameObject); //敵を消滅
        }
    }
}
