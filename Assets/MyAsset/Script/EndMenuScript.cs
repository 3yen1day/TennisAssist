using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EndMenuScript : MonoBehaviour
{
    Text text;
    void Start()
    {
        text = GameObject.Find("UIText").GetComponent<Text>();
        Vector3 score = Static_Paramater.GetTargetNum();
        text.text = "SCORE\n"+"HIT : " + score.x + "\n" + "MISS : " + score.y + "\n" + "HIT率 : " + ToPercent(score.x/(score.x + score.y))+" %";
        Static_WriteCSV.EndTextFile();
        Static_WriteCSV.CloseTextfile();
    }

    float ToPercent (float f)
    {
       return Mathf.Floor(f * 10000) / 100.0f;
    }

}
