using UnityEngine;
using TMPro;
using UnityEngine.XR.Interaction.Toolkit;
using System.Collections.Generic;

public class StoneInteractionManager : MonoBehaviour
{
    public TextMeshProUGUI interactionText1; // First text display
    public TextMeshProUGUI interactionText2; // Second text display
    public Dictionary<string, int> stoneCounts = new Dictionary<string, int>();
    public Dictionary<string, HashSet<int>> uniqueInteractions = new Dictionary<string, HashSet<int>>();
    public Dictionary<string, int> interactedStones = new Dictionary<string, int>();

    void Start()
    {
        SetupInteractions();
    }

    private void SetupInteractions()
    {
        GameObject[] allObjects = FindObjectsOfType<GameObject>();
        foreach (GameObject obj in allObjects)
        {
            if (obj.name.Contains("Stone"))
            {
                XRBaseInteractable interactable = obj.GetComponent<XRBaseInteractable>();
                if (interactable != null)
                {
                    interactable.hoverExited.AddListener((arg) => RegisterInteraction(obj.name, obj.GetInstanceID()));

                    string stoneType = GetStoneType(obj.name);
                    if (!stoneCounts.ContainsKey(stoneType))
                    {
                        stoneCounts[stoneType] = 0;
                        interactedStones[stoneType] = 0;
                        uniqueInteractions[stoneType] = new HashSet<int>();
                    }
                    stoneCounts[stoneType]++;
                }
            }
        }

        UpdateUI();
    }

    private void RegisterInteraction(string stoneName, int uniqueId)
    {
        string stoneType = GetStoneType(stoneName);
        if (uniqueInteractions[stoneType].Add(uniqueId))
        {
            interactedStones[stoneType]++;
            UpdateUI();
        }
    }

    private void UpdateUI()
    {
        string displayText = "";
        foreach (var stoneType in stoneCounts.Keys)
        {
            displayText += $": {interactedStones[stoneType]} / {stoneCounts[stoneType]}\n";
        }
        if (interactionText1 != null)
            interactionText1.text = displayText;
        if (interactionText2 != null)
            interactionText2.text = displayText;
    }

    private string GetStoneType(string stoneName)
    {
        if (stoneName.Contains("T-Stone"))
            return "T";
        else if (stoneName.Contains("F-Stone"))
            return "F";
        else if (stoneName.Contains("W-Stone"))
            return "W";
        else
            return "Unknown";
    }
    public string GetStoneCountLog()
    {
        string result = "";
        foreach (var stoneType in stoneCounts.Keys)
        {
            result += $"{stoneType}: {interactedStones[stoneType]} / {stoneCounts[stoneType]}\n";
        }
        return result;
    }

}
