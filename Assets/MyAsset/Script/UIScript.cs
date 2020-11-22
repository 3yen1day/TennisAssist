using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIScript : MonoBehaviour
{
    Text text;
    float t = 0;
    bool IsPractice = false;

    void Start()
    {
        text = GameObject.Find("UIText").GetComponent<Text>();
        //シーンが練習か否か
        if (SceneManager.GetActiveScene().name == "Practice_PoseAssistControllerTennis") IsPractice = true;

        UIText();
    }
    
    void Update()
    {
        t += Time.deltaTime;
        if (t> Static_Paramater.CreateBallSpan + 0.05f)
        {
            UIText();
        }
        
    }

    void UIText()
    {
        Vector3 score;
        if (IsPractice)
        {
            score = Static_PracticeParamater.GetTargetNum();
        }
        else
        {
            score = Static_Paramater.GetTargetNum();
        }
        text.text = "HIT : " + score.x + "\n" + "MISS : " + score.y + "\n" + "残り : " + score.z;
    }
}
