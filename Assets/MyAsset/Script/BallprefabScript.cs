using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//ボールprefabにアタッチするスクリプト

public class BallprefabScript : MonoBehaviour {
    
    void OnCollisionEnter(Collision collision)
    {
        if (this.gameObject.tag == "Jumping")
        {
            Static_WriteCSV.WriteFallPoint(this.gameObject.transform.position);
            this.gameObject.tag = "Untagged";
        }
    }
}
