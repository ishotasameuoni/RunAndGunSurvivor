using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageGenerator : MonoBehaviour
{
    const int StageChipSize = 30; //Zのスケールが30であるステージ

    int currentChipIndex; //現在作成済みのステージ番号

    public Transform character; //プレイヤーの位置
    public GameObject[] stageChips; //生成されるステージのカタログ
    public int startChipIndex = 1; //最初のステージ番号
    public int preInstantiate = 5; //どこまで先のステージを用意しておくか
    //現ヒエラルキーに存在しているステージ情報を生成順に取得
    public List<GameObject> generatedStageList = new List<GameObject>();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //初期の現在番号を定めている（0番)
        currentChipIndex = startChipIndex - 1;
        //初期状態からいくつかのステージを生成
        UpdateStage(preInstantiate);
    }

    // Update is called once per frame
    void Update()
    {
        //キャラクターがどのステージのIndex番号にいるか常に把握
        int charaPositionIndex = (int)(character.position.z / StageChipSize);

        //キャラのいる位置*5番目の位置が
        if (charaPositionIndex + preInstantiate > currentChipIndex)
        {
            UpdateStage(charaPositionIndex + preInstantiate);
        }
    }

    //ステージ生成＆古いステージ廃棄
    void UpdateStage(int toChipIndex)
    {
        //作りたいステージ番号(引数)より現在番号の方が大きければ何もしない
        if (toChipIndex <= currentChipIndex) return;

        //指定のステージチップまでを作成
        for (int i = currentChipIndex + 1; i <= toChipIndex; i++)
        {
            //戻り値GameObjectがGenerateStageメソッドで変数stageObjectにに格納
            GameObject stageObject = GenerateStage(i);

            //確保したstageObject情報をリストに加える
            generatedStageList.Add(stageObject);
        }

        //ステージ8個以上になったら最も古いステージを削除
        while (generatedStageList.Count > preInstantiate + 2) DestroyOldestStage();

        //現在番号を更新
        currentChipIndex = toChipIndex;
    }

    //指定のインデックスの位置にStageオブジェクトをランダムに生成
    GameObject GenerateStage(int chipIndex)
    {
        int nextStageChip = Random.Range(0, stageChips.Length);

        GameObject stageObject = (GameObject)Instantiate(
            stageChips[nextStageChip], new Vector3(0, 0, chipIndex * StageChipSize),
            Quaternion.identity);

        return stageObject;
    }

    //古いステージを消すメソッド
    void DestroyOldestStage()
    {   
        //リストから先頭に掲載されているオブジェクト番号を取得
        GameObject oldStage = generatedStageList[0];
        //リストの先頭に情報(0番)をリスト上から抹消する
        generatedStageList.RemoveAt(0);
        //ヒエラルキーから対象ステージを消去
        Destroy(oldStage);
    }

}
