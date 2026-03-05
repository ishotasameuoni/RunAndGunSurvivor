using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NejikoController : MonoBehaviour
{
    CharacterController controller;
    //Animator animator;

    Vector3 moveDirection = Vector3.zero;

    public float gravity = 20; //重力加速度
    public float speedZ = 5; //前進する力
    public float speedJump = 8; //ジャンプ力

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //必要なコンポーネントの自動取得
        controller = GetComponent<CharacterController>();
        //animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (controller.isGrounded)
        {
            //垂直方向のボタン入力をチェック(Vertical ↑↓　WS)
            if (Input.GetAxis("Vertical") > 0.0f)
            {
                //このフレームにおける前進/後退の移動量が決まる
                moveDirection.z = Input.GetAxis("Vertical") * speedZ;
            }
            else
            {
                moveDirection.z = 0;
            }

            //左右キーを押した時の回転
            transform.Rotate(0, Input.GetAxis("Horizontal") * 3, 0);

            if (Input.GetButton("Jump")) //スペースキー
            {
                moveDirection.y = speedJump;
                //animator.SetTrigger("jump");
            }
        }
        //ここまででそのフレームの移動するべき量が決まる(moveDirectionのXとY）
        //重力分の力を毎フレーム追加
        moveDirection.y -= gravity * Time.deltaTime;

        //移動の実行
        //引数に与えたVector3の値をそのオブジェクトの向きに合わせてグローバルな値として何が正しいか判断している
        Vector3 globalDirection = transform.TransformDirection(moveDirection);

        //Moveメソッドに与えたVector3値の分だけ実際にPlayerが動く
        controller.Move(globalDirection * Time.deltaTime);

        //移動後着地していたらY方向の速度はリセットする
        //｛｝は一つの処理だけなら必要ない
        if (controller.isGrounded) moveDirection.y = 0;

        //速度が0以上なら走っているフラグをTrueにする
        //animator.SetBool("run", moveDirection.z > 0.0f);
    }
}
