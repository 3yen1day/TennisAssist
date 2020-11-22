using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateBall : MonoBehaviour {

    [SerializeField]
    private GameObject BallPrefab;
    [SerializeField]
    private Vector3 StartPos = new Vector3(0,0,10);
    [SerializeField]
    private Vector3 ForcePow = new Vector3(0, 0, -10);
    private Rigidbody BallRigitBody;

    private float InstantiateTime; 
    private float t = 0;

	void Start () {
        InstantiateTime = Static_Paramater.GetCreateBallSpan();
    }
	
	void Update () {
        t += Time.deltaTime;
        if (t > InstantiateTime && !(Static_Paramater.IsStop))
        {
            t = 0;
            GameObject Ball = Instantiate(BallPrefab, StartPos, Quaternion.identity);
            BallRigitBody = Ball.GetComponent<Rigidbody>();
            BallRigitBody.AddForce(ForcePow, ForceMode.Impulse);
            Destroy(Ball, InstantiateTime*2.0f);
        }
        
	}
}
