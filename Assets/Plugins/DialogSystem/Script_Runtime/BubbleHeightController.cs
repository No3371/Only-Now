using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class BubbleHeightController : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        FitHeight();
	}

    void FitHeight()
    {
        this.GetComponent<RectTransform>().sizeDelta = new Vector2(this.GetComponent<RectTransform>().sizeDelta.x, this.GetComponentInChildren<Text>().preferredHeight + 12);
    }
}
