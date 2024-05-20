using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using System.Net;
using System.IO;

public class SendVibrationData : MonoBehaviour
{
    public string deviceIP = "192.168.178.42"; // The IP address of the M5StickC Plus 192.168.178.42
    public int port = 8080;
    private XRSimpleInteractable simpleInteractable;

    // This method sends a vibration command to the M5StickC Plus
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
        }
    }

    private void Awake()
    {
        simpleInteractable = GetComponent<XRSimpleInteractable>();
    }

    private void OnEnable()
    {
        simpleInteractable.hoverEntered.AddListener(OnHoverEntered);
    }

    private void OnDisable()
    {
        simpleInteractable.hoverEntered.RemoveListener(OnHoverEntered);
    }

    private void OnHoverEntered(HoverEnterEventArgs args)
    {
        SendVibrationCommand(new int[] { 255, 0, 0 }); // Example: vibrate only the first motor
    }
}
