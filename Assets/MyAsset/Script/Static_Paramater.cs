using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class Static_Paramater
{    
    //ゴーストを出すか
    public static bool CreateGhost = true;
    //遅延させるか
    public static bool IsDelay = false;
    //遅延のスパン
    public static float DelaySpan = 1.0f;
	//ボール生成のスパン
	public static float CreateBallSpan = 3.0f;
	//利き手は右手か
	public static bool IsRight = true;


	//ラケットの軌道を作るかどうか
	public static bool CreateOrbit = false;
    //ターゲットの配置をランダムにするかどうか
    public static bool TargetRand = false;
    //球数
    public static int RemainingBallNum = 30;
    //スピード
    public static Vector3 Speed;
    //止める
    public static bool IsStop = false;


    //ターゲットに当たった数
    public static int HitTargetNum = 0;
    //ターゲットを外した数
    public static int MissTargetNum = 0;

    //ディレイがtrueの時は、CreateBallSpanにDelaySpanを加算して返す
    public static float GetCreateBallSpan()
    {
        if (IsDelay) return CreateBallSpan + DelaySpan;
        else return CreateBallSpan;
    }

    //HIT数、MISS数、残り球数の取得
    public static Vector3 GetTargetNum()
    {
        return new Vector3(HitTargetNum, MissTargetNum, RemainingBallNum);
    }

    public static void AddHitTargetNum()
    {
        HitTargetNum++;
        Static_WriteCSV.WriteHitTargetNum();
    }

    public static void AddMissTargetNum()
    {
        MissTargetNum++;
        Static_WriteCSV.WriteMissTargetNum();
    }

    public static void MinusRemainingBallNum()
    {
        RemainingBallNum--;
        if (RemainingBallNum < 0)
        {
            IsStop = true;
            SceneManager.LoadScene("EndMenu");
        }
    }
    public static int GetRemainingBallNum()
    {
        return RemainingBallNum;
    }
    
}
