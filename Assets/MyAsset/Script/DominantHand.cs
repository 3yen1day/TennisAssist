using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//利き手の設定
//racketにアタッチ
//右手のコントローラー左手でも持てるので、必要なし
public class DominantHand : MonoBehaviour
{

    void Start()
    {
		if (Static_Paramater.IsRight == false)
		{
			GameObject parent = GameObject.Find("LeftHandAnchor");
			GameObject racket = this.transform.parent.gameObject;
			racket.transform.parent = parent.transform;
			this.gameObject.transform.localPosition = new Vector3(0.0f, -4.5f, -5.0f);
		}
	}
}
