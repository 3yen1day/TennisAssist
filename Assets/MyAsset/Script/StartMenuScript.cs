using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartMenuScript : MonoBehaviour
{
    void Start()
    {
    }
    
    void Update()
    {
        if (OVRInput.Get(OVRInput.RawButton.A) || OVRInput.Get(OVRInput.RawButton.B) || OVRInput.Get(OVRInput.RawButton.X) || OVRInput.Get(OVRInput.RawButton.Y))
        {
            SceneManager.LoadScene("Practice_PoseAssistControllerTennis");
        }
    }
}
