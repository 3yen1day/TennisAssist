using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Target : MonoBehaviour
{
    void Start()
    {
        
    }
    
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider col)
    {
        if(col.tag == "Jumping" || col.tag == "Untagged")
        {
            //HIT++
            //色を一瞬赤に変更
            //正解音
        }
    }
}
