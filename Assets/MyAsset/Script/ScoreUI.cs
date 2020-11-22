using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreUI : MonoBehaviour {

    private Text targetText;

    void Start () {
        targetText = this.gameObject.GetComponent<Text>();
	}
	
	
	public void ChangeText (string ScoreText) {
        targetText.text = ScoreText;
    }
}
