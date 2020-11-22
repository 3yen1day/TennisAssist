using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//ターゲットPrefabに付ける
public class TargetScript : MonoBehaviour
{
    float t = 0;

    void Start()
    {
    }

    void Update()
    {
        t += Time.deltaTime;
        if(t > Static_Paramater.GetCreateBallSpan() && !(Static_Paramater.IsStop))
        {
            if (Static_Paramater.IsPractice)
            {
                Static_Paramater.AddPracticeMissTargetNum();
                Static_Paramater.MinusPracticeRemainingBallNum();
            }
            else
            {
                Static_Paramater.AddMissTargetNum();
                Static_Paramater.MinusRemainingBallNum();
            }
            Destroy(this.gameObject);
            t = 0;
        }
    }

    private void OnTriggerEnter(Collider col)
    {
        if(!(col.gameObject.tag == "Ball") && !(Static_Paramater.IsStop))
        {
            if (Static_Paramater.IsPractice)
            {
                Static_Paramater.AddPracticeHitTargetNum();
                Static_Paramater.MinusPracticeRemainingBallNum();
            }
            else
            {
                Static_Paramater.AddHitTargetNum();
                Static_Paramater.MinusRemainingBallNum();
            }
            Destroy(this.gameObject);
        }
    }
}
