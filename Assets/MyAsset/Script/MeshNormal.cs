using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;

//ボールのとびを制御するクラス
//法線取得＆RigitBodyにAddForce
public class MeshNormal : MonoBehaviour {

    public float distValue = 0.5f;
    public float InstantiateTime = 2;
    public float speed = 10;
    
    public GameObject BallPrefab;
    public Vector3 BallPos;
    private Vector3 beforeFramePos;
    private GameObject Ball;

    public ScoreUI ScoreUIScript;
    void Start () {
        CreateBall();
    }
	

	void Update () {
        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log("Space");
            CreateBall();
        }

        #region 
        //collision使わない当たり判定
        //ラケットのcolliderとrigitbody切る
        Vector3 Nowpos = this.gameObject.transform.position;
        //とりあえず当たり判定は球
        float dist = Vector3.Distance(BallPos, Nowpos);
        if (dist< distValue)
        {
            Rigidbody r = Ball.GetComponent<Rigidbody>();
            if (r.useGravity == false)
            {
                Debug.Log(dist);
                float power = Vector3.Distance(Nowpos, beforeFramePos) * 100;
                r.useGravity = true;
                r.AddForce(GetNormal() * Mathf.Pow(power, speed));
                Debug.Log(GetNormal());
                Destroy(Ball, InstantiateTime);
                Invoke("CreateBall", InstantiateTime);

                //UI表示
                var builder = new StringBuilder();
                builder.Append("ベクトル ");
                builder.Append(GetNormal().ToString());
                builder.Append("\nスピード ");
                builder.Append((int)power*100);
                ScoreUIScript.ChangeText(builder.ToString());
            }
        }
        beforeFramePos = Nowpos;
        #endregion

    }

    /*
    //collision使うときの当たり判定
    private void OnCollisionEnter(Collision collision)
    {
        //collision.gameObject.GetComponent<Rigidbody>().useGravity = true;
        Rigidbody r = collision.gameObject.GetComponent<Rigidbody>();
        if (r.useGravity == false)
        {
            r.useGravity = true;


            Destroy(collision.gameObject, InstantiateTime-0.3f);
            Invoke("CreateBall", InstantiateTime);
        }
    }
    */

    //ボールをつくる
    private void CreateBall()
    {
        Ball = Instantiate(BallPrefab, BallPos, Quaternion.identity);
        Ball.GetComponent<Rigidbody>().useGravity = false;
    }

    
    //メッシュの法線取得
    private Vector3 GetNormal()
    {
        //メッシュ法線だとローカル座標になってしまう
        //var meshFilt = GetComponent<MeshFilter>();
        //var mesh = meshFilt.mesh;
        //Vector3[] normals = mesh.normals;
        Quaternion parentRot = this.gameObject.transform.parent.rotation;
        var dir = parentRot * new Vector3(1,1,1);
        return dir;
    }


    //メッシュの法線デバッグ
    private void DebugNormal()
    {
        //メッシュ法線だとローカル座標になってしまう
        var meshFilt = GetComponent<MeshFilter>();
        var mesh = meshFilt.mesh;
        Vector3[] normals = mesh.normals;
        Quaternion parentRot = this.gameObject.transform.parent.rotation;
        var dir = parentRot * normals[0];
        Debug.Log("dir "+ dir);
        Debug.Log("rotation "+ parentRot);
    }
}
