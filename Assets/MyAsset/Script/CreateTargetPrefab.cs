using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

//ターゲットPrefabを作る
public class CreateTargetPrefab : MonoBehaviour
{
    [SerializeField]
    private GameObject TargetPrefab;
    GameObject TargetObject;
    Vector3 RangeMin;
    Vector3 RangeMax;
    Vector3 NotRandamPos;
    float t = 0;
    //Targetにアタッチするテキストファイル
    private StreamWriter sw;

    void Start()
    {
        //TargetRangeの大きさに入るように範囲調整
        GameObject TargetRangeObject = GameObject.Find("TargetRange");
        RangeMax = TargetRangeObject.transform.position + TargetRangeObject.transform.localScale * 0.5f;
        RangeMin = TargetRangeObject.transform.position - TargetRangeObject.transform.localScale * 0.5f;
        //RangeMax.y = TargetRangeObject.transform.position.y + TargetRangeObject.transform;

        //Zは中心から長さ1/4分うしろ
        NotRandamPos.x = (RangeMax.x + RangeMin.x) * 0.5f;
        //NotRandamPos.y = TargetRangeObject.transform.position.y;
        NotRandamPos.y = RangeMax.y;
        NotRandamPos.z = RangeMax.z;
        
    }

    void Update()
    {
        t += Time.deltaTime;
        if (t > Static_Paramater.GetCreateBallSpan() && !(Static_Paramater.IsStop))
        {
            CreateTarget();
            t = 0;
        }
    }

    void CreateTarget()
    {
        Vector3 pos = new Vector3(0, 0, 0);
        if (Static_Paramater.TargetRand)
        {
            pos.x = Random.Range(RangeMax.x, RangeMin.x);
            pos.y = RangeMax.y;
            pos.z = Random.Range(RangeMax.z, RangeMin.z);
        }
        else
        {
            pos = NotRandamPos;
        }
        TargetObject = Instantiate(TargetPrefab, pos, Quaternion.identity);
    }
}
