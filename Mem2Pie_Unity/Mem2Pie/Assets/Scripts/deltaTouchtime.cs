using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class deltaTouchtime : MonoBehaviour {
    Text txt;
    float currentTime = 0;
    bool delay_sw=false;
	void Start () {
        txt = gameObject.GetComponent<Text>();
	}
	void Update () {
        if (Input.touchCount == 1)
        {
            if (Input.GetTouch(0).phase == TouchPhase.Stationary)
            {
                delay_sw = true;
            }
            else
            {
                delay_sw = false;
                currentTime = 0;
            }
            DelayTime(delay_sw);
        }
        txt.text = currentTime.ToString();
    }
    void DelayTime(bool delay_sw)
    {
        if(delay_sw)
        {
            currentTime += Time.deltaTime;
        }
    }
}
