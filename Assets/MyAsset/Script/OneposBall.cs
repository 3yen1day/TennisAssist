using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OneposBall : MonoBehaviour
{

    [SerializeField]
    private GameObject BallPrefab;
    [SerializeField]
    private Vector3 StartPos = new Vector3(0, 0, 10);
    [SerializeField]
    private float InstantiateTime = 3;

    void Start()
    {
        CreateBall();
    }

    private void Update()
    {
       // Debug.Log(Time.deltaTime);
    }

    public void Collide(GameObject obj)
    {
        Rigidbody r = obj.GetComponent<Rigidbody>();
        if (r.useGravity == false)
        {
            r.useGravity = true;
            Destroy(obj, InstantiateTime);
            Invoke("CreateBall", InstantiateTime);
        }
    }

    void CreateBall()
    {
        GameObject Ball = Instantiate(BallPrefab, StartPos, Quaternion.identity);
        Ball.GetComponent<Rigidbody>().useGravity = false;
    }
}
