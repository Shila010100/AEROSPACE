using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour
{
    public Button startButton; // Reference to the UI button that starts the timer
    public float timeValue = 60 * 5; // Duration in seconds, here set to 5 minutes
    public TextMeshProUGUI timerText; // Reference to the TextMeshProUGUI component to display the timer
    private bool timerIsActive = false; // Controls whether the timer is active

    void Start()
    {
        startButton.onClick.AddListener(StartTimer); // Add an event listener to the start button
        DisplayTime(timeValue); // Initial display of the time
    }

    // Update is called once per frame
    void Update()
    {
        if (timerIsActive && timeValue > 0)
        {
            timeValue -= Time.deltaTime;
            DisplayTime(timeValue);
        }
        else if (timeValue <= 0)
        {
            timerIsActive = false; // Stop the timer when it reaches 0
            timeValue = 0;
            DisplayTime(timeValue); // Ensure the display is updated to show 00:00
        }
    }

    void DisplayTime(float timeToDisplay)
    {
        float minutes = Mathf.FloorToInt(timeToDisplay / 60);
        float seconds = Mathf.FloorToInt(timeToDisplay % 60);
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    public void StartTimer()
    {
        timerIsActive = true; // Set the timer active
    }
}
