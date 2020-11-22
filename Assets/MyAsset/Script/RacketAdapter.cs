using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RacketAdapter : MonoBehaviour {
    public Transform PositionBindTransform = null;
    //public Transform RotationBindTransform = null;
    public Vector3 OffsetRot = new Vector3(0, 0, 0);
    private OVRInput.Controller controller;
    private void Start()
    {
        OVRInput.Update();
        if (OVRInput.IsControllerConnected(OVRInput.Controller.RTouch) == true) controller = OVRInput.Controller.RTouch;
        else if (OVRInput.IsControllerConnected(OVRInput.Controller.LTouch) == true) controller = OVRInput.Controller.LTouch;

    }

    void Update()
    {
        OVRInput.Update();
        //Debug.Log(OVRInput.Get(OVRInput.RawAxis1D.RIndexTrigger));
        transform.position = PositionBindTransform.position;
        //transform.rotation = Quaternion.Euler(RotationBindTransform.localEulerAngles) * Quaternion.Euler(OffsetRot);
        transform.rotation = OVRInput.GetLocalControllerRotation(controller);

    }
}
