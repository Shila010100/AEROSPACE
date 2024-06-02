using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    [System.Serializable]
    class CanvasStep
    {
        public GameObject canvasObject;
        public string buttonText;
        public bool includeSkipButton;
    }

    [SerializeField]
    List<CanvasStep> canvasSteps = new List<CanvasStep>();
    [SerializeField]
    TextMeshProUGUI stepButtonTextField;
    [SerializeField]
    GameObject skipButton;
    [SerializeField]
    Button leftHandButton, rightHandButton;

    private int currentStepIndex = 0;

    void Start()
    {
        InitializeCanvasSteps();
    }

    private void InitializeCanvasSteps()
    {
        // Setup initial canvas visibility
        foreach (var step in canvasSteps)
        {
            step.canvasObject.SetActive(false);
        }
        if (canvasSteps.Count > 0)
        {
            canvasSteps[0].canvasObject.SetActive(true);
            UpdateUIElements();
        }
    }

    public void NextStep()
    {
        if (currentStepIndex + 1 < canvasSteps.Count)
        {
            canvasSteps[currentStepIndex].canvasObject.SetActive(false);
            currentStepIndex++;
            canvasSteps[currentStepIndex].canvasObject.SetActive(true);
            UpdateUIElements();
        }
    }

    public void PreviousStep()
    {
        if (currentStepIndex > 0)
        {
            canvasSteps[currentStepIndex].canvasObject.SetActive(false);
            currentStepIndex--;
            canvasSteps[currentStepIndex].canvasObject.SetActive(true);
            UpdateUIElements();
        }
    }

    private void UpdateUIElements()
    {
        stepButtonTextField.text = canvasSteps[currentStepIndex].buttonText;
        skipButton.SetActive(canvasSteps[currentStepIndex].includeSkipButton);
    }

    public void SelectHand(string handType)
    {
        Debug.Log("Hand selected: " + handType);
        // Additional logic to handle hand selection can be added here
    }
}
