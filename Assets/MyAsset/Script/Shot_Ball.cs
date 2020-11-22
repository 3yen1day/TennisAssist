using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shot_Ball : MonoBehaviour
{

    GameObject Racket;
    Vector3 BallBeforeFramePos;
    Rigidbody rigit;
    bool Hit = false;

    void Start()
    {
        Racket = GameObject.Find("pCylinder4");
        rigit = this.gameObject.GetComponent<Rigidbody>();

    }

    void Update()
    {
        //ラケットのトランスフォーム
        Transform RacketTransform = Racket.transform;
        // Racketの中心からボールの中心までのベクトル
        Vector3 d = transform.position - RacketTransform.position;
        //ボールとRacketの距離
        float h = Vector3.Dot(d, RacketTransform.up);

        // 当たり判定
        if (h < transform.localScale.x && Hit == false)
        {
            Collision();
        }

        BallBeforeFramePos = this.transform.position;
    }

    private void Collision()
    {
        Hit = true;

        //ラケットスピードの取得

        //与える力
        Vector3 Force = Refrection() *10.0f ;
        rigit.velocity = new Vector3(0, 0, 0);
        rigit.AddForce(Force, ForceMode.Impulse);
    }

    // 反射ベクトルを計算する
    private Vector3 Refrection()
    {
        Vector3 MoveDir = rigit.GetPointVelocity(this.gameObject.transform.position);
        //面の法線
        Vector3 n = Racket.transform.up;
        //面とボールの距離
        float h = Mathf.Abs(Vector3.Dot(MoveDir, n));
        //反射ベクトル
        Vector3 r = MoveDir + 2 * n * h;
        return r;
    }
}
