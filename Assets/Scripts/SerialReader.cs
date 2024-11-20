using System.IO.Ports;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SerialReader : MonoBehaviour
{
    public static SerialReader Instance { get; private set; }

    private SerialPort serialPort;
    public string portName = "COM3"; // Default port (auto-detected in Start)
    public int baudRate = 9600;

    // Public booleans for external access
    public bool LArm { get; private set; }
    public bool RArm { get; private set; }
    public bool LLeg { get; private set; }
    public bool RLeg { get; private set; }
    public bool Button { get; private set; }

    private bool previousButtonState = false; // To track the last state of the button

    public bool ButtonPressed { get; private set; } // True only on key down

    private void Awake()
    {
        // Ensure the singleton instance
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Persist across scenes
        }
        else
        {
            Destroy(gameObject); // Enforce singleton
        }
    }

    void Start()
    {
        portName = FindArduinoPort(); // Auto-detect Arduino port
        if (!string.IsNullOrEmpty(portName))
        {
            Debug.Log($"Arduino found on port: {portName}");
            serialPort = new SerialPort(portName, baudRate);
            try
            {
                serialPort.Open();
                Debug.Log("Serial port opened successfully.");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Failed to open serial port: {e.Message}");
            }
        }
        else
        {
            Debug.LogError("Arduino not found. Please check the connection.");
        }
    }

    void Update()
    {
        if (serialPort != null && serialPort.IsOpen)
        {
            try
            {
                string data = serialPort.ReadLine(); // Read a line of data
                Debug.Log($"Raw Data: {data}");
                ParseData(data); // Parse the binary string
                HandleButtonPress(); // Detect button press
            }
            catch (System.Exception e)
            {
                Debug.LogWarning($"Serial read error: {e.Message}");
            }
        }
    }

    private string FindArduinoPort()
    {
        string[] ports = SerialPort.GetPortNames(); // Get all available ports
        foreach (string port in ports)
        {
            try
            {
                using (SerialPort testPort = new SerialPort(port, baudRate))
                {
                    testPort.Open();
                    testPort.ReadTimeout = 500; // Set a small timeout
                    string response = testPort.ReadLine();

                    if (response.Length == 5 && IsBinaryString(response))
                    {
                        testPort.Close();
                        return port; // Valid port found
                    }
                    testPort.Close();
                }
            }
            catch
            {
                // Ignore exceptions and continue checking other ports
            }
        }
        return null; // Return null if no valid port is found
    }

    private bool IsBinaryString(string data)
    {
        foreach (char c in data)
        {
            if (c != '0' && c != '1') return false;
        }
        return true;
    }

    private void ParseData(string data)
    {
        if (data.Length == 5)
        {
            LArm = data[0] == '1';
            RArm = data[1] == '1';
            LLeg = data[2] == '1';
            RLeg = data[3] == '1';
            Button = data[4] == '1';
        }
        else
        {
            Debug.LogWarning("Unexpected data format.");
        }
    }

    private void HandleButtonPress()
    {
        // Detect transition from false to true (like KeyDown)
        ButtonPressed = !previousButtonState && Button;

        // Update the previous button state
        previousButtonState = Button;

        if (ButtonPressed)
        {
            Debug.Log("Button Press Detected!");
        }
    }

    private void OnApplicationQuit()
    {
        if (serialPort != null && serialPort.IsOpen)
        {
            serialPort.Close();
        }
    }
}
