using UnityEngine;
using UnityEngine.InputSystem;

public class NormalShooter : MonoBehaviour
{
    [Header("Bullet管理スクリプトと連携")]
    public BulletManager bulletManager;

    [Header("生成オブジェクトと位置")]
    public GameObject bulletPrefabs;//生成対象プレハブ
    public GameObject gate; //生成位置

    [Header("弾速")]
    public float shootSpeed = 10.0f; //弾速

    GameObject bullets; //生成した弾をまとめるオブジェクト

    const int maxShootPower = 3; //最大威力
    int shootPower = 1; //現在の威力

    [Header("ソードのスクリプト")]
    public NormalSword normalSword;

    //InputAction(Playerマップ)のAttackアクションがおされたら
    void OnAttack(InputValue value)
    {
        if (normalSword.GetIsSword()) return;

        //クリアまたはゲームオーバーならアクションボタンで次に進める
        if (GameManager.gameState == GameState.retry)
        {
            //staticメソッドなので簡単に呼び出し
            GameManager.RetryScene();
        }
        else if (GameManager.gameState == GameState.result)
        {   
            //行き先を自由に編集できるようpublic変数を使っているのでアクションボタンで先に進める
            GameManager gm = GameObject.FindGameObjectWithTag("GM").GetComponent<GameManager>();
            gm.NextScene(gm.nextScene);
        }
        else //ゲームがプレイ中なら
        {
            Shoot();
        }
    }

    void Shoot()
    {
        if (bulletManager.GetBulletRemaining() > 0)
        {
            //プレハブの生成と情報の取得
            GameObject obj = Instantiate(bulletPrefabs,
                gate.transform.position,
                Quaternion.Euler(90, 0, 0)
                );

            //生成したBulletをBulletsオブジェクトの子供にしてまとめる
            //Transformはコンポーネントでもよくつかう物なのでmonobehaviorクラスに省略
            obj.transform.parent = bullets.transform;

            //bulletを消費
            bulletManager.ConsumeBullet();

            //生成したbullet自身のRigidbodyの力で飛ばす
            Rigidbody bulletRbody = obj.GetComponent<Rigidbody>();
            bulletRbody.AddForce(new Vector3(0, 0, shootSpeed),
                ForceMode.Impulse);
        }
        else　//残弾がなければ補充
        {
            bulletManager.RecoverBullet();
        }
    }

    void Start()
    {
        //タグをつける際は完全に名前を一致させる必要がある（大文字小文字）
        bullets = GameObject.FindGameObjectWithTag("Bullets");
    }

    //威力を上げる
    public void ShootPowerUp()
    {
        shootPower++; //威力をあげる
        if (shootPower > maxShootPower) shootPower = maxShootPower; //最大威力まで抑える
        GameObject canvas = GameObject.FindGameObjectWithTag("UI");
        canvas.GetComponent <UIController>().UpdateGun();
    }

    //威力を下げる
    public void ShootPowerDown()
    {
        shootPower--; //威力を下げる
        if (shootPower <= 0) shootPower = 1; //最小を1にする
        GameObject canvas = GameObject.FindGameObjectWithTag("UI");
        canvas.GetComponent<UIController>().UpdateGun();
    }

    //現在威力の取得
    public int GetShootPower()
    {
        return shootPower;
    }
}
