using UnityEngine;
using System;
using System.Net;
using System.IO;
using UnityEngine.XR.Interaction.Toolkit;

public class SendVibrationData : MonoBehaviour
{
    public string deviceIP = "192.168.178.42"; // The IP address of the M5StickC Plus
    public int port = 8080;
    private XRSimpleInteractable simpleInteractable;

    private void Awake()
    {
        Debug.Log("Awake called"); // Debug log for Awake
        simpleInteractable = GetComponent<XRSimpleInteractable>();
        if (simpleInteractable != null)
        {
            Debug.Log("XR Simple Interactable found");
            simpleInteractable.hoverEntered.AddListener(OnHoverEntered);
        }
        else
        {
            Debug.LogError("XRSimpleInteractable component not found!");
        }
    }

    private void OnEnable()
    {
        Debug.Log("OnEnable called"); // Debug log for OnEnable
        if (simpleInteractable != null)
        {
            simpleInteractable.hoverEntered.AddListener(OnHoverEntered);
        }
    }

    private void OnDisable()
    {
        Debug.Log("OnDisable called"); // Debug log for OnDisable
        if (simpleInteractable != null)
        {
            simpleInteractable.hoverEntered.RemoveListener(OnHoverEntered);
        }
    }

    public void OnHoverEntered(HoverEnterEventArgs args)
    {
        Debug.Log("Hover entered"); // Log when hover starts
        SendVibrationCommand(new int[] { 255, 0, 0 }); // Example: vibrate only the first motor
    }

    public void SendVibrationCommand(int[] intensities)
    {
        string url = $"http://{deviceIP}:{port}/vibrate?intensities={intensities[0]},{intensities[1]},{intensities[2]}";
        Debug.Log("Sending request to URL: " + url); // Log the URL

        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
        request.Method = "GET";

        try
        {
            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            {
                Debug.Log("Response status code: " + (int)response.StatusCode);
                using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                {
                    string responseText = reader.ReadToEnd();
                    Debug.Log("Received response: " + responseText); // Log the response
                }
            }
        }
        catch (WebException ex)
        {
            Debug.LogError("Error in sending request: " + ex.Message);
            Debug.LogError("Status: " + ex.Status);
            if (ex.Response != null)
            {
                using (var errorResponse = (HttpWebResponse)ex.Response)
                {
                    using (var reader = new StreamReader(errorResponse.GetResponseStream()))
                    {
                        string errorText = reader.ReadToEnd();
                        Debug.LogError("Error response: " + errorText);
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Debug.LogError("General exception: " + ex.Message);
        }
    }

    private void Update()
    {
        Debug.Log("SendVibrationData script is active");
    }
}
