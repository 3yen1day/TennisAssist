using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            if (SceneManager.GetActiveScene().name == "StartMenu") SceneManager.LoadScene("Practice");
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            if (SceneManager.GetActiveScene().name == "PracticeToProduction") SceneManager.LoadScene("Production");
        }
    }
}
