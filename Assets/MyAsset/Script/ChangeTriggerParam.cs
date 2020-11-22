using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//メインモデルにアタッチする
//コントローラのスクイーズを代入
public class ChangeTriggerParam : MonoBehaviour
{
    private NeuronAnimatorInstance sc;
    // Start is called before the first frame update
    void Start()
    {
        sc = this.gameObject.GetComponent<NeuronAnimatorInstance>();
    }

    // Update is called once per frame
    void Update()
    {
        OVRInput.Update();
        sc.pers = Mathf.Max(OVRInput.Get(OVRInput.RawAxis1D.RIndexTrigger), 0);
    }
}
