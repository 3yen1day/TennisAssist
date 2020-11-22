using UnityEngine;
using System.Collections;

public class NeuronOVRAdapter : MonoBehaviour
{
    public Transform            bindTransform = null;
    public Vector3 Offset = new Vector3(0, 0, 0);
    void Update( )
    {        
		// Re-Position the camera to our head bind Target 
        transform.position = bindTransform.position + Offset;
    }
}
