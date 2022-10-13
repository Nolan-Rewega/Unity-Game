using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class fpsCounter : MonoBehaviour
{
    private TextMeshProUGUI fpsText;

    void Start(){
        fpsText = gameObject.GetComponent<TextMeshProUGUI>();
    }


    void Update(){
        int fps = (int)(1.0f / Time.unscaledDeltaTime);
        fpsText.text = "FPS: " + fps.ToString();
    }
}
