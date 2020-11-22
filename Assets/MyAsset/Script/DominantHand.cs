using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//利き手の設定
//racketにアタッチ
public class DominantHand : MonoBehaviour
{

    void Start()
    {
		if (Static_Paramater.IsRight == false)
		{
			GameObject parent = GameObject.Find("LeftHandAnchor");
			GameObject racket = this.transform.parent.gameObject;
			racket.transform.parent = parent.transform;
			this.gameObject.transform.localPosition = new Vector3(0.0f, 9.5f, 1.5f);
		}
	}
}
