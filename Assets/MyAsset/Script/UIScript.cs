using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIScript : MonoBehaviour
{
    Text text;
    float t = 0;

    void Start()
    {
        text = GameObject.Find("UIText").GetComponent<Text>();

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
        if (Static_Paramater.IsPractice)
        {
            score = Static_Paramater.GetPracticeTargetNum();
        }
        else
        {
            score = Static_Paramater.GetTargetNum();
        }
        text.text = "HIT : " + score.x + "\n" + "MISS : " + score.y + "\n" + "残り : " + score.z;
    }
}
