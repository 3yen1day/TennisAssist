using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//当たり判定とするオブジェクトにアタッチ
public class Shot : MonoBehaviour
{
    //スピード調整
    float SpeedSize = 10.0f;
    Rigidbody rigid;
    Vector3 BeforeFramePos = new Vector3(0, 0, 0);
    //法線用ノーマルメッシュ
    [SerializeField]
    GameObject NormalMeshObject;

    private Vector3 NMPos = new Vector3(0, 0, 0);
    private Vector3 MeshN = new Vector3(0, 0, 0);

    private CreateGhost CreateGhostScript;

    void Start()
    {
        rigid = this.gameObject.GetComponent<Rigidbody>();
        CreateGhostScript = this.gameObject.GetComponent<CreateGhost>();

    }

    void Update()
    {
        BeforeFramePos = this.gameObject.transform.position;
    }


    //衝突したら
    private void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.tag == "Ball")
        {
            //ラケットに当たった後はJumpingに変更
            //その後、床にバウンドしたら、Untaggedに変更
            col.gameObject.tag = "Jumping";

            //ゴーストの生成
            CreateGhostScript.OnTriggerBall();

            //コントローラの振動
            ControllerVibration();

            //BallのRigitBody取得
            Rigidbody BallRigit = col.gameObject.GetComponent<Rigidbody>();
            
            //反射ベクトル計算
            Vector3 Reflection = GetReflection(GetTravel(col.transform.position, BallRigit), GetNormal());

            //ラケットの移動ベクトル計算
            Vector3 Difference = this.gameObject.transform.position - BeforeFramePos;

            //速度ベクトル計算
            float Speed = Mathf.Max(0.09f, Difference.magnitude * SpeedSize);

            Vector3 Force = Reflection * Speed;
            
            //跳ね返す
            BallRigit.velocity = new Vector3(0, 0, 0);
            BallRigit.AddForce(-Force, ForceMode.Impulse);
        }
    }

    //反射ベクトル
    private Vector3 GetReflection(Vector3 Travel, Vector3 Normal)
    {
        return Travel + 2 * (Vector3.Dot(Travel, Normal)) * Normal;
    }

    //ボールの進行方向ベクトル
    private Vector3 GetTravel(Vector3 BallPos, Rigidbody r)
    {
        return r.GetPointVelocity(BallPos);
    }

    //法線ベクトル
    private Vector3 GetNormal()
    {

        Vector3 MeshN = NormalMeshObject.transform.up;
        return MeshN;

    }

    private void ControllerVibration()
    {
        OVRInput.SetControllerVibration(0.5f, 0.3f);
        Invoke("EndControllerVibration", 0.2f);
    }

    //振動終わり
    private void EndControllerVibration()
    {
        OVRInput.SetControllerVibration(0, 0);
    }

    private void DebugLine()
    {
        Debug.DrawLine
    (
        NMPos,
        NMPos + MeshN, Color.green);
    }
}
