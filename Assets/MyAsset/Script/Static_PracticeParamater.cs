using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class Static_PracticeParamater
{
    //軌道を作るかどうか
    public static bool CreateOrbit = false;
    //ターゲットの配置をランダムにするかどうか
    public static bool TargetRand = true;
    //ボール生成のスパン
    public static float CreateBallSpan = 3.0f;
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



    public static Vector3 GetTargetNum()
    {
        return new Vector3(HitTargetNum, MissTargetNum, RemainingBallNum);
    }

    public static void AddHitTargetNum()
    {
        HitTargetNum++;
    }

    public static void AddMissTargetNum()
    {
        MissTargetNum++;
    }

    public static void MinusRemainingBallNum()
    {
        RemainingBallNum--;
        if (RemainingBallNum < 0)
        {
            IsStop = true;
            SceneManager.LoadScene("PoseAssistControllerTennis");
        }
    }

}
