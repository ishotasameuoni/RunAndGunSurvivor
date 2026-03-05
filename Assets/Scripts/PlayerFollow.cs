using UnityEngine;

public class PlayerFollow : MonoBehaviour
{
    Vector3 diff;

    //追従ターゲット
    public GameObject target;

    //追従スピード
    public float followSpeed = 5.0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        diff =
            target.transform.position - transform.position;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        //Lerpによる補完関数
        //第一引数→第二引数へ　第三引数は進捗率

        transform.position = Vector3.Lerp(
            transform.position,
            target.transform.position - diff,
            Time.deltaTime * followSpeed
            );

    }
}
