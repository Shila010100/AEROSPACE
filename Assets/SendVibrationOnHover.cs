using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class SendVibrationOnHover : MonoBehaviour
{
    private Dictionary<string, bool> fingerActive = new Dictionary<string, bool>();
    private Dictionary<string, byte[]> fingerIntensities = new Dictionary<string, byte[]>();
    private bool interactionInProgress = false;
    public bool isRightHandMode = true; // Default to right hand, toggle through UI
    private float interactionDuration = 0.1f; // Duration to wait for additional fingers

    void Start()
    {
        XRBaseInteractable interactable = GetComponent<XRBaseInteractable>();
        if (interactable != null)
        {
            interactable.hoverEntered.AddListener(HandleHoverEntered);
            interactable.hoverExited.AddListener(HandleHoverExited);
        }
    }

    private void HandleHoverEntered(HoverEnterEventArgs arg)
    {
        if (arg.interactorObject is XRBaseInteractor interactor && IsInteractorFromActiveHand(interactor.name))
        {
            fingerActive[interactor.name] = true;
            fingerIntensities[interactor.name] = DetermineVibrationIntensities(interactor.name);
            //Debug.Log($"SVOH: Finger entered: {interactor.name}");
            if (!interactionInProgress)
            {
                StartCoroutine(InteractionTimer());
            }
        }
    }

    private void HandleHoverExited(HoverExitEventArgs arg)
    {
        if (arg.interactorObject is XRBaseInteractor interactor && IsInteractorFromActiveHand(interactor.name))
        {
            // Only mark as inactive after processing
            //Debug.Log($"SVOH: Finger exited: {interactor.name}");
        }
    }

    IEnumerator InteractionTimer()
    {
        interactionInProgress = true;
        yield return new WaitForSeconds(interactionDuration);
        ProcessFingerInteractions();
        // Reset interactions after processing
        fingerActive.Clear();
        fingerIntensities.Clear();
        interactionInProgress = false;
    }

    private void ProcessFingerInteractions()
    {
        byte[] intensities = new byte[3];

        // Consolidate intensities from all active fingers
        foreach (var intensity in fingerIntensities.Values)
        {
            for (int i = 0; i < 3; i++)
            {
                intensities[i] = (byte)Mathf.Max(intensities[i], intensity[i]);
            }
        }

        if (intensities.Any(v => v > 0))
        {
            CommandManager.Instance.EnqueueCommand(intensities);
            //Debug.Log($"SVOH: Enqueued combined command at {Time.realtimeSinceStartup * 1000f} ms: {string.Join(", ", intensities)}");
        }
        else
        {
            //Debug.Log("SVOH: No active fingers to process for vibration.");
        }
    }

    private byte[] DetermineVibrationIntensities(string interactorName)
    {
        byte[] intensities = new byte[3];
        if (interactorName.Contains("Index")) intensities[1] = 255;
        if (interactorName.Contains("Middle")) intensities[isRightHandMode ? 0 : 2] = 255;
        if (interactorName.Contains("Thumb")) intensities[isRightHandMode ? 2 : 0] = 255;

        //Debug.Log($"SVOH: Intensities determined for {interactorName}: {string.Join(", ", intensities)}");
        return intensities;
    }

    public void SetHandMode(bool isRightHand)
    {
        isRightHandMode = isRightHand;
        //Debug.Log($"SVOH: Hand mode set to: {(isRightHand ? "Right" : "Left")}");
    }

    private bool IsInteractorFromActiveHand(string interactorName)
    {
        return isRightHandMode ? interactorName.Contains("Right") : interactorName.Contains("Left");
    }

    private void OnDestroy()
    {
        XRBaseInteractable interactable = GetComponent<XRBaseInteractable>();
        if (interactable != null)
        {
            interactable.hoverEntered.RemoveListener(HandleHoverEntered);
            interactable.hoverExited.RemoveListener(HandleHoverExited);
        }
        Debug.Log("SVOH: SendVibrationOnHover cleanup completed.");
    }
}
