using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//ヒットした瞬間ゴーストを表示
//ゴースト軌道とゴーストのデータ書き出し
//ラケットにアタッチ

public class CreateGhost : MonoBehaviour
{

    //Prefab
    [SerializeField]
    private GameObject GhostPrefab;
    //軌道ラケットのインターバル
    [SerializeField]
    private float OrbitInterbal = 0.1f;
    //軌道ラケットの数
    [SerializeField]
    public int GhostRacketNum = 5;
    //サイズ
    Vector3 Size = new Vector3(0.03f, 0.03f, 0.03f);
    //ラケット位置
    private Vector3[] GhostRacketPos;
    //ラケット回転
    private Quaternion[] GhostRacketRot;

    private float t = 0.0f;
    private int n = 0;

    //Delay時の一時保管用
    Vector3 tempP;
    Quaternion tempQ;
    Vector3[] tempGhostRacketPos;
    Quaternion[] tempGhostRacketRot;

    private void Start()
    {
        GhostRacketPos = new Vector3[GhostRacketNum];
        GhostRacketRot = new Quaternion[GhostRacketNum];

        tempGhostRacketPos = new Vector3[GhostRacketNum];
        tempGhostRacketRot = new Quaternion[GhostRacketNum];

        for (int i=0; i< GhostRacketNum; i++)
        {
            GhostRacketPos[i] = new Vector3(0, 0, 0);
            GhostRacketRot[i] = new Quaternion(0, 0, 0, 0);

            tempGhostRacketPos[i] = new Vector3(0, 0, 0);
            tempGhostRacketRot[i] = new Quaternion(0, 0, 0, 0);
        }
    }

    private void FixedUpdate()
    {
        for (int i = 0; i < GhostRacketNum - 1; i++)
        {
            GhostRacketPos[GhostRacketNum - i - 1] = GhostRacketPos[GhostRacketNum - i - 2];
            GhostRacketRot[GhostRacketNum - i - 1] = GhostRacketRot[GhostRacketNum - i - 2];
        }
        GhostRacketPos[0] = this.gameObject.transform.position;
        GhostRacketRot[0] = this.gameObject.transform.rotation;
    }

    //Triggerに入ったらShotから呼ばれる
    public void OnTriggerBall()
    {
        //Delayにも対応できるように一時保管
        tempP = this.gameObject.transform.position;
        tempQ = this.gameObject.transform.rotation;
        for (int i = 0; i < GhostRacketNum; i++)
        {
            tempGhostRacketPos[i] = GhostRacketPos[i];
            tempGhostRacketRot[i] = GhostRacketRot[i];
        }

            if (Static_Paramater.IsDelay)
        {
            Invoke("GhostArray", Static_Paramater.DelaySpan);
            Invoke("Ghost", Static_Paramater.DelaySpan);
        }
        else
        {
            GhostArray();
            Ghost();
        }
    }

    //ゴースト
    private void Ghost()
    {
        //ゴーストの生成、破棄
        if (Static_Paramater.CreateGhost)
        {
            GameObject Ghost = Instantiate(GhostPrefab, tempP, tempQ);
            //Ghost.transform.localScale = Size;
            ChangeMaterialParam(Ghost, new Vector4(1, 0, 0, 1), 0.5f);
            //破壊する時間はGetCreateBallSpanじゃない
            //この関数を呼び出すのに、Delayspan分の時間を使っているので
            Destroy(Ghost, Static_Paramater.GetCreateBallSpan()*0.5f);
        }
        
        //FixedUpdateにしたら誤差がなくなったので必要なし
        /*
        //書き出し
        //物理演算部で、Updateよりも早く呼び出されるので正確な比較にはならない
        Static_WriteCSV.WriteRacketQuat(tempQ);
        Static_WriteCSV.WriteRacketSpeed(Vector3.Distance(tempGhostRacketPos[0], tempP));
        */
    }

    //ゴースト軌道
    private void GhostArray()
    {
        for(int i=0; i< GhostRacketNum; i++)
        {
            //ゴースト軌道の生成、破棄
            if (Static_Paramater.CreateOrbit)
            {
                GameObject GhostArray = Instantiate(GhostPrefab, tempGhostRacketPos[i], tempGhostRacketRot[i]);
                GhostArray.transform.localScale = Size;
                ChangeMaterialParam(GhostArray, new Vector4(0, 1, 1, 1), 0.5f * (GhostRacketNum - i) / GhostRacketNum);
                //破壊する時間はGetCreateBallSpanじゃない
                //この関数を呼び出すのに、Delayspan分の時間を使っているので
                Destroy(GhostArray, Static_Paramater.CreateBallSpan - 2.0f);
            }

            //回転の書き出し
            Static_WriteCSV.WriteRacketQuat(tempGhostRacketRot[i]);
            if (i > 0)
            {
                //スピードを書き出し
                Static_WriteCSV.WriteRacketSpeed(Vector3.Distance(tempGhostRacketPos[i - 1], tempGhostRacketPos[i]));
            }
        }
        Static_WriteCSV.NextRow();
    }


    //マテリアルの変更
    private void ChangeMaterialParam(GameObject gameogj, Vector4 col, float alpha)
    {
        if (gameogj.GetComponent<Renderer>().material)
        {
            gameogj.GetComponent<Renderer>().material.SetColor("_MainColor", col);
            gameogj.GetComponent<Renderer>().material.SetFloat("_Alpha", alpha);
        }
        else
        {
            for(int i=0; i<gameogj.transform.childCount; i++)
            {
                GameObject Panel = gameogj.transform.GetChild(i).gameObject;
                Panel.GetComponent<Renderer>().material.SetColor("_MainColor", col);
                Panel.GetComponent<Renderer>().material.SetFloat("_Alpha", alpha);
            }
        }
    }
}
