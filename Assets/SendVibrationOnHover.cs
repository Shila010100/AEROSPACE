using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class SendVibrationOnHover : MonoBehaviour
{
    private float lastSentTime = 0.0f;
    private float sendInterval = 0.017f; // Interval in seconds, adjust as needed
    public bool isRightHandMode = true; // Default to right hand, toggle through UI

    void Start()
    {
        XRBaseInteractable interactable = GetComponent<XRBaseInteractable>();
        if (interactable != null)
        {
            interactable.hoverEntered.RemoveListener(HandleHoverEntered);
            interactable.hoverEntered.AddListener(HandleHoverEntered);
        }
    }

    private void HandleHoverEntered(HoverEnterEventArgs arg)
    {
        if (arg.interactorObject is XRBaseInteractor interactor && IsInteractorFromActiveHand(interactor.name))
        {
            if (Time.realtimeSinceStartup - lastSentTime > sendInterval)
            {
                lastSentTime = Time.realtimeSinceStartup;
                byte[] intensities = DetermineVibrationIntensities(interactor.name);
                CommandManager.Instance.EnqueueCommand(intensities);
                Debug.Log($"Enqueued command for {interactor.name} at {lastSentTime * 1000f} ms.");
            }
        }
    }

    public void SetHandMode(bool isRightHand)
    {
        isRightHandMode = isRightHand;
        Debug.Log("Hand mode set to: " + (isRightHand ? "Right" : "Left"));
    }

    private bool IsInteractorFromActiveHand(string interactorName)
    {
        return isRightHandMode ? interactorName.Contains("Right") : interactorName.Contains("Left");
    }

    private byte[] DetermineVibrationIntensities(string interactorName)
    {
        byte[] intensities = new byte[3];
        if (interactorName.Contains("Index")) intensities[1] = 255;
        else if (interactorName.Contains("Middle")) intensities[isRightHandMode ? 0 : 2] = 255;
        else if (interactorName.Contains("Thumb")) intensities[isRightHandMode ? 2 : 0] = 255;

        Debug.Log($"Intensities determined for {interactorName}: {string.Join(", ", intensities)}");
        return intensities;
    }

    private void OnDestroy()
    {
        // Clean up event listener
        XRBaseInteractable interactable = GetComponent<XRBaseInteractable>();
        if (interactable != null)
        {
            interactable.hoverEntered.RemoveListener(HandleHoverEntered);
        }
        Debug.Log("SendVibrationOnHover cleanup completed.");
    }
}
