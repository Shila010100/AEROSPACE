using UnityEngine;
using System.IO;
using UnityEngine.UI;

public class GameLogger : MonoBehaviour
{
    public StoneInteractionManager stoneInteractionManager;
    public HandSelectionManager handSelectionManager; // Reference to the hand selection manager
    public Button startButton;
    private string playerTag;
    private string filePath;

    void Awake()
    {
        startButton.onClick.AddListener(StartLogging);
    }

    private void StartLogging()
    {
        playerTag = System.DateTime.Now.ToString("yyyyMMdd_HHmmss") + "_" + Random.Range(1000, 9999);
        filePath = Path.Combine(Application.persistentDataPath, $"Project_{playerTag}.txt");
        LogGameStart();
    }

    private void LogGameStart()
    {
        string logText = "Game started at: " + System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "\n";
        WriteToFile(logText);
    }

    private Timer GetCurrentTimer()
    {
        // Fetch the timer component from the active UI text element
        Timer timer = FindObjectOfType<Timer>(); // Assuming only one active at a time
        return timer;
    }


    public void LogGameEnd(string endCondition)
    {
        string elapsedTime = GetCurrentTimer().GetElapsedTime().ToString("F2");
        string logText = $"Game End: {System.DateTime.Now}\n";
        logText += $"End Condition: {endCondition}\n";
        logText += $"Player ID: {playerTag}\n";
        logText += $"Used Hand: {handSelectionManager.CurrentHandMode}\n";
        logText += $"Elapsed Time: {elapsedTime} seconds\n";
        logText += $"Stone Counts: {GetStoneCountLog()}\n";
        WriteToFile(logText);
    }

    private string GetStoneCountLog()
    {
        string log = "";
        foreach (var stoneType in stoneInteractionManager.stoneCounts.Keys)
        {
            int interactedCount = stoneInteractionManager.uniqueInteractions[stoneType].Count;
            int totalCount = stoneInteractionManager.stoneCounts[stoneType];
            log += $"{stoneType}: {interactedCount} / {totalCount}\n";
        }
        return log;
    }

    private void WriteToFile(string text)
    {
        File.AppendAllText(filePath, text);
        Debug.Log($"Log written to {filePath}");
    }
}
