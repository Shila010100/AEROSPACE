using System.IO.Ports;
using UnityEngine;

public class TouchVibrate : MonoBehaviour
{
    SerialPort serialPort = new SerialPort("COM3", 115200); // Adjust port and baud rate

    void Start()
    {
        serialPort.Open();
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Touchable")
        {
            serialPort.WriteLine("triggerVibrationEffect"); // Command the Arduino to vibrate
        }
    }

    void OnDestroy()
    {
        if (serialPort != null && serialPort.IsOpen)
            serialPort.Close();
    }
}
