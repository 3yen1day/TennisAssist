using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//ボールの生成
public class CreateBall : MonoBehaviour {

    [SerializeField]
    private GameObject BallPrefab;
    [SerializeField]
    private Vector3 StartPos = new Vector3(0,0,10);
    [SerializeField]
    private Vector3 ForcePow = new Vector3(0, 0, -10);
    private Rigidbody BallRigitBody;
    
    private float t = 0;

    private UIScript uiscript;

    void Start () {
        //InstantiateTime = Static_Paramater.GetCreateBallSpan();
        uiscript = GameObject.Find("Script").GetComponent<UIScript>();
    }
	
	void Update () {
        t += Time.deltaTime;
        if (t > Static_Paramater.GetCreateBallSpan() && !(Static_Paramater.IsStop))
        {
            t = 0;
            GameObject Ball = Instantiate(BallPrefab, StartPos, Quaternion.identity);
            BallRigitBody = Ball.GetComponent<Rigidbody>();
            BallRigitBody.AddForce(ForcePow, ForceMode.Impulse);

            if (Static_Paramater.IsPractice)
            {
                Static_Paramater.MinusPracticeRemainingBallNum();
            }
            else
            {
                Static_Paramater.MinusRemainingBallNum();
            }
            //スコアをUIに書き込む
            uiscript.UIText();

        }
        
	}
}
