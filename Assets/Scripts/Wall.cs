using System.Collections;
using UnityEngine;

public class Wall : MonoBehaviour
{
    [Header("生成プレハブオブジェクト")]
    public GameObject effectPrefab; //生成プレハブ

    [Header("耐久力")]
    public float life = 5.0f; //耐久力

    [Header("ダメージ時間・振動対象・振動スピード・振動量")]
    public float damageTime = 0.25f; //ダメージ中時間
    public GameObject damageBody; //振動対象オブジェクト
    public float speed = 75.0f; //振動スピード
    public float amplitude = 1.5f;  //振動量

    Vector3 startPosition; //振動対象の初期位置
    float x; //振動による移動座標

    Coroutine currentDamage; //ダメージコルーチン

    [Header("スコア点数")]
    public int point = 100;

    void Start()
    {
        startPosition = damageBody.transform.localPosition;
    }

    void Update()
    {
        //ダメージコルーチンが発動中だったら振動する
        if (currentDamage != null)
        {
            //Mathf.Sinで一定間隔の±の値に移動する
            x = (amplitude * 0.01f) * Mathf.Sin(Time.time * speed);
            damageBody.transform.localPosition = startPosition + new Vector3(x, 0, 0); // += StarPositionでもOK
        }
    }

    //衝突
    void OnTriggerEnter(Collider other)
    {
        if (currentDamage != null) return;

        //コルーチン型のcurrentDamageではなくBool型でも成立はする

        //衝突相手が「Bullet」タグまたは「ソード」を持っていたら
        if (other.gameObject.tag == "Bullet" || other.gameObject.tag == "Sword")
        {
            //相手のタグを取得
            string tag = other.gameObject.tag;

            currentDamage = StartCoroutine(DamageCol(tag));
            if (life <= 0) //lifeが残っていなければ
            {
                ScoreManager.ScoreUp(point); //敵撃破でスコアを加算
                CreateEffect();
            }
        }
    }

    //ダメージコルーチン
    IEnumerator DamageCol(string tag)
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (tag == "Bullet")
        {
            life -= player.GetComponent<NormalShooter>().GetShootPower();
        }
        else if (tag == "Sword")
        {
            life -= player.GetComponent<NormalSword>().GetSwordPower();
        }

        yield return new WaitForSeconds(damageTime);
        //コルーチンを発動していたという情報の解除
        currentDamage = null;
        //振動していたボディを元の位置に戻す
        damageBody.transform.localPosition = new Vector3(0, 0, 0);

    }

    public void CreateEffect()
    {
        if (effectPrefab != null)
        {
            //エフェクトプレハブを生成
            Instantiate(effectPrefab,
                transform.position,
                Quaternion.identity);
        }

        //Wall自身を消去
        Destroy(gameObject);
    }
}
