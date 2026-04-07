using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class CubeGameUI : MonoBehaviour
{

    public TextMeshProUGUI TimerText;
    public float Timer;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Timer += Time.deltaTime;
        TimerText.text = "생존 시간 : " + Timer.ToString("0.00");

    }
    
}
