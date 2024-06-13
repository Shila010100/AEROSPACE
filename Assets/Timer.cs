using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour
{
    public Button startButton; // Reference to the UI button that starts the timer
    public static float timeValue; // Duration in seconds, here set to 5 minutes
    public float initialTime = 60 * 5;
    public TextMeshProUGUI timerText; // Reference to the TextMeshProUGUI component to display the timer
    private bool timerIsActive = false; // Controls whether the timer is active

    void Start()
    {
        timeValue = initialTime;
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
            EndTimer();
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
        timeValue = initialTime; // Reset the timer to the full initial time when restarted
        DisplayTime(timeValue); // Update the display when restarted
    }

    private void EndTimer()
    {
        timerIsActive = false; // Stop the timer when it reaches 0
        timeValue = 0;
        DisplayTime(timeValue); // Ensure the display is updated to show 00:00
    }
    public float GetElapsedTime()
    {
        return initialTime - timeValue; // Ensure 'initialTime' is set when the timer starts
    }
}
