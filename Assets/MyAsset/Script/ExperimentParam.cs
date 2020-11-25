using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

//実験毎にStaticのパラメーターを変える
public class ExperimentParam : MonoBehaviour
{
    //実験群
    private enum ExperimentType
    {
        Ghost,
        Delay,
        Nothing
    }

    private enum HandType
    {
        Right,
        Left
    }

    ///////////////////実験群選択///////////////////////
    private ExperimentType experiment = ExperimentType.Delay;
    ///////////////////利き手選択///////////////////////
    private HandType hand = HandType.Right;

    void Start()
    {

        //練習か否か
        if (SceneManager.GetActiveScene().name == "Practice") Static_Paramater.IsPractice = true;
        else Static_Paramater.IsPractice = false;

        Static_Paramater.IsStop = false;

        if (experiment == ExperimentType.Ghost)
        {
            Static_Paramater.CreateGhost = true;
            Static_Paramater.IsDelay = false;
        }
        else if(experiment == ExperimentType.Delay)
        {
            Static_Paramater.CreateGhost = true;
            Static_Paramater.IsDelay = true;
        }
        else if(experiment == ExperimentType.Nothing)
        {
            Static_Paramater.CreateGhost = false;
            Static_Paramater.IsDelay = false;
        }

        if (hand == HandType.Right)
        {
            Static_Paramater.IsRight = true;
        }else if(hand == HandType.Left)
        {
            Static_Paramater.IsRight = false;
        }
    }
}
