using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//ボールprefabにアタッチするスクリプト
//ターゲットへの当たり判定
//HIT・MISSの処理

public class BallprefabScript : MonoBehaviour {

    private bool hit = false;
    private UIScript uiscript;

    private void Start()
    {
        //ボールの破壊
        Invoke("DestroyBall", Static_Paramater.GetCreateBallSpan()*2.0f);
        uiscript = GameObject.Find("Script").GetComponent<UIScript>();
    }

    void OnCollisionEnter(Collision collision)
    {
        if (this.gameObject.tag == "Jumping")
        {
            Static_WriteCSV.WriteFallPoint(this.gameObject.transform.position);
            this.gameObject.tag = "Untagged";
        }
        if(this.gameObject.tag != "Ball")
        {
            if(collision.gameObject.tag == "Target")
            {
                //hitをtrueにしたらすぐ書き込む
                hit = true;
                if (Static_Paramater.IsPractice)
                {
                    Static_Paramater.AddPracticeHitTargetNum();
                }
                else
                {
                    Static_Paramater.AddHitTargetNum();
                }
                //スコアをUIに書き込む
                uiscript.UIText();
            }
        }
    }

    //ボール破壊時の処理
    void DestroyBall()
    {
        if (hit == false)
        {
            if (Static_Paramater.IsPractice)
            {
                Static_Paramater.AddPracticeMissTargetNum();
            }
            else
            {
                Static_Paramater.AddMissTargetNum();
            }
            //スコアをUIに書き込む
            uiscript.UIText();
        }
        else
        {
        }
        Destroy(this.gameObject);
    }
}
