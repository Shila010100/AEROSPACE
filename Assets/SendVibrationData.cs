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
        SendVibrationCommand(new int[] { 255, 0, 0 }); // Example: vibrate only the first motor
    }

    public async void SendVibrationCommand(int[] intensities)
    {
        string message = $"GET /vibrate?intensities={intensities[0]},{intensities[1]},{intensities[2]} HTTP/1.1\r\nHost: {deviceIP}:{port}\r\nConnection: close\r\n\r\n";
        Debug.Log("Sending message: " + message); // Log the message

        try
        {
            using (TcpClient client = new TcpClient())
            {
                await client.ConnectAsync(deviceIP, port);
                Debug.Log("Connected to server");

                NetworkStream stream = client.GetStream();
                byte[] data = Encoding.ASCII.GetBytes(message);
                await stream.WriteAsync(data, 0, data.Length);
                Debug.Log("Message sent to server");

                byte[] responseData = new byte[256];
                int bytes = await stream.ReadAsync(responseData, 0, responseData.Length);
                string response = Encoding.ASCII.GetString(responseData, 0, bytes);
                Debug.Log("Received response: " + response);
            }
        }
        catch (SocketException ex)
        {
            Debug.LogError("SocketException: " + ex.Message);
        }
        catch (Exception ex)
        {
            Debug.LogError("Exception: " + ex.Message);
        }
    }

    private void Update()
    {
        Debug.Log("SendVibrationData script is active");
    }
}
