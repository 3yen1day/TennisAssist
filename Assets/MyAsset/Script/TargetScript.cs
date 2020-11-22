using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

//ターゲットPrefabに付ける
public class TargetScript : MonoBehaviour
{
    float t = 0;
    bool IsPractice = false;

    void Start()
    {
        if (SceneManager.GetActiveScene().name == "Practice_PoseAssistControllerTennis") IsPractice = true;
    }

    void Update()
    {
        t += Time.deltaTime;
        if(t > Static_Paramater.GetCreateBallSpan() && !(Static_Paramater.IsStop))
        {
            if (IsPractice)
            {
                Static_PracticeParamater.AddMissTargetNum();
                Static_PracticeParamater.MinusRemainingBallNum();
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
            if (IsPractice)
            {
                Static_PracticeParamater.AddHitTargetNum();
                Static_PracticeParamater.MinusRemainingBallNum();
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
