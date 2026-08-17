using TMPro;
using UnityEngine;

public class LevelTime : MonoBehaviour
{

    public TextMeshProUGUI txtField;
    public TextMeshProUGUI tooltipField;
    public float timer;
    public static bool timeStarted = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        timeStarted = true;
        GameObject txt = GameObject.Find("Timer");
        txtField = txt.GetComponent<TextMeshProUGUI>();
        GameObject tooltip = GameObject.Find("Tooltip");
        tooltipField = tooltip.GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        if (timeStarted == true)
        {
            timer += Time.deltaTime;
            PlayerData.timeToCompleteLevel = timer;
        }
        UpdateTimer();
        if (timer >= 5) //Make tooltip disappear after 5 seconds
        {
            tooltipField.enabled = false; 
        }
    }

    public void UpdateTimer()
    {
        int mins = Mathf.FloorToInt(timer / 60F);
        int secs = Mathf.FloorToInt(timer - mins * 60);

        string timeToDisplay = string.Format("{0:00}:{1:00}", mins, secs);
        txtField.text = timeToDisplay;
    }   
}