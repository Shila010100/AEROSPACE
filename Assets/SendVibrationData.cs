using UnityEngine;
using System;
using System.Net.Sockets;
using System.Text;
using UnityEngine.XR.Interaction.Toolkit;

public class SendVibrationData : MonoBehaviour
{
    public string deviceIP = "192.168.4.1"; // The IP address of the M5StickC Plus for WiFi Direct
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
        SendVibrationCommand(new byte[] { 255, 0, 0 }); // Example: vibrate only the first motor
    }

    public void SendVibrationCommand(byte[] intensities)
    {
        string message = $"GET /vibrate?intensities={intensities[0]},{intensities[1]},{intensities[2]} HTTP/1.1\r\nHost: {deviceIP}:{port}\r\nConnection: close\r\n\r\n";
        Debug.Log("Sending message: " + message); // Log the message

        try
        {
            Debug.Log("Attempting to connect to " + deviceIP + " on port " + port);
            using (TcpClient client = new TcpClient(deviceIP, port))
            {
                NetworkStream stream = client.GetStream();
                byte[] data = Encoding.ASCII.GetBytes(message);
                stream.Write(data, 0, data.Length);
                Debug.Log("Message sent to server");

                byte[] responseData = new byte[256];
                int bytes = stream.Read(responseData, 0, responseData.Length);
                string response = Encoding.ASCII.GetString(responseData, 0, bytes);
                Debug.Log("Received response: " + response);
            }
        }
        catch (SocketException ex)
        {
            Debug.LogError("SocketException: " + ex.Message + "\n" + ex.StackTrace);
        }
        catch (Exception ex)
        {
            Debug.LogError("Exception: " + ex.Message + "\n" + ex.StackTrace);
        }
    }

    private void Update()
    {
        // Reduced frequency of the update log message to avoid cluttering the log output
        if (Time.frameCount % 60 == 0) // Log every 60 frames
        {
            Debug.Log("SendVibrationData script is active");
        }
    }
}
