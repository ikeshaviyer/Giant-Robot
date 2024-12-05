using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using PCSC;
using PCSC.Iso7816;
using Mifare1kTest;

public class MifareCardReader : MonoBehaviour
{
    private const byte MSB = 0x00;
    private const byte LSB = 0x08;

    private MifareCard mifareCard;
    private string currentUID = null;
    private CancellationTokenSource cancellationTokenSource;

    // Dictionary to store UID with associated description
    private Dictionary<string, string> uidDataDictionary = new Dictionary<string, string>();

    // Dictionary to manage UID descriptions
    private Dictionary<string, string> uidDescriptions = new Dictionary<string, string>
    {
        { "04-63-B9-38-21-02-89", "Scrap Metal" },
        { "04-F3-CE-74-21-02-89", "Scrap Metal" },
        { "04-33-BB-68-21-02-89", "Scrap Metal" },
        { "04-E3-88-4A-21-02-89", "Scrap Metal" },

        { "04-63-4B-35-21-02-89", "Circuit" },
        { "04-53-9B-64-21-02-89", "Circuit" },
        { "04-33-0D-47-21-02-89", "Circuit" },
        { "04-63-6E-3F-21-02-89", "Circuit" },

        { "04-23-2D-74-21-02-89", "Energy Core" },
        { "04-F3-5B-45-21-02-89", "Energy Core" },
        { "04-F3-63-3B-21-02-89", "Energy Core" },
        { "04-43-01-63-21-02-89", "Energy Core" }

        // Add or remove UIDs as needed
    };

    // Singleton instance
    public static MifareCardReader Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
    }

    private void OnDestroy()
    {
        StopReading();  // Stop reading when the script is destroyed
    }

    public static void StartReading()
    {
        if (Instance == null)
        {
            Debug.LogError("MifareCardReader instance not found in the scene.");
            return;
        }

        Instance.StartReaderLoop();
    }

    public static void StopReading()
    {
        if (Instance == null)
        {
            Debug.LogError("MifareCardReader instance not found in the scene.");
            return;
        }

        Instance.StopReaderLoop();
    }

    public void ResetCards()
    {
        if (Instance == null)
        {
            Debug.LogError("MifareCardReader instance not found in the scene.");
            return;
        }

        Instance.ResetUIDData();
    }

    private void StartReaderLoop()
    {
        if (cancellationTokenSource != null && !cancellationTokenSource.IsCancellationRequested)
            return;

        cancellationTokenSource = new CancellationTokenSource();
        _ = ReadLoopAsync(cancellationTokenSource.Token);
    }

    private void StopReaderLoop()
    {
        cancellationTokenSource?.Cancel();
        ResetUID();
    }

    private async Task ReadLoopAsync(CancellationToken token)
    {
        while (!token.IsCancellationRequested)
        {
            await Task.Delay(500);  // Adjust delay as necessary for scan frequency

            using (var context = ContextFactory.Instance.Establish(SCardScope.System))
            {
                var readerNames = context.GetReaders();
                if (IsEmpty(readerNames))
                {
                    ResetUID();
                    continue;
                }

                var readerName = readerNames[0];
                try
                {
                    using (var isoReader = new IsoReader(context, readerName, SCardShareMode.Shared, SCardProtocol.Any, false))
                    {
                        bool isMifare = false;
                        bool isNTAG215 = false;

                        // Attempt to detect MIFARE or NTAG215 by sending a known command for each type
                        try
                        {
                            // Step 1: Try to identify MIFARE
                            mifareCard = new MifareCard(isoReader);
                            var mifareUID = mifareCard.GetData();
                            if (mifareUID != null)
                            {
                                isMifare = true;
                            }
                        }
                        catch
                        {
                            // Not a MIFARE card
                        }

                        if (!isMifare)
                        {
                            try
                            {
                                // Step 2: Attempt NTAG215 specific command
                                var command = new CommandApdu(IsoCase.Case2Short, isoReader.ActiveProtocol)
                                {
                                    CLA = 0x00,
                                    INS = 0x30, // READ command for NTAG215
                                    P1 = 0x00,  // Page number for UID
                                    Le = 16
                                };
                                var response = isoReader.Transmit(command);
                                if (response.HasData)
                                {
                                    isNTAG215 = true;
                                }
                            }
                            catch
                            {
                                // Not an NTAG215 card
                            }
                        }

                        if (isMifare)
                        {
                            // MIFARE-specific UID handling
                            var mifareUID = mifareCard.GetData();
                            if (mifareUID != null)
                            {
                                string uidString = BitConverter.ToString(mifareUID);
                                ProcessUID(uidString);
                            }
                        }
                        else if (isNTAG215)
                        {
                            // NTAG215-specific UID handling
                            var command = new CommandApdu(IsoCase.Case2Short, isoReader.ActiveProtocol)
                            {
                                CLA = 0x00,
                                INS = 0x30, // READ command for NTAG215
                                P1 = 0x00,  // Page number for UID
                                Le = 16
                            };
                            var response = isoReader.Transmit(command);
                            if (response.HasData)
                            {
                                string uidString = BitConverter.ToString(response.GetData());
                                ProcessUID(uidString);
                            }
                        }
                        else
                        {
                            ResetUID(); // Unknown card type or no card detected
                        }
                    }
                }
                catch (PCSC.Exceptions.RemovedCardException)
                {
                    ResetUID();
                    Debug.Log("Card removed. Resetting UID.");
                }
                catch (Exception ex)
                {
                    Debug.LogError($"An unexpected error occurred: {ex.Message}");
                    ResetUID();
                }
            }
        }
    }

    // Helper method to process and display UID
    private void ProcessUID(string uidString)
    {
        if (!uidDataDictionary.ContainsKey(uidString))
        {
            currentUID = uidString;
            uidDataDictionary[uidString] = GetUIDDescription(uidString); // Add UID with description
            DisplayUIDLabel(uidDataDictionary[uidString]);
        }
    }


    private void ResetUID()
    {
        currentUID = null;
        DisplayUIDLabel("None");
    }

    private void ResetUIDData()
    {
        uidDataDictionary.Clear();  // Clear the dictionary of UIDs and associated data
        Debug.Log("Scanned UIDs have been reset.");
    }

    private string GetUIDDescription(string uid)
    {
        // Try to get the description from the dictionary
        return uidDescriptions.TryGetValue(uid, out var description) ? description : "Unknown Device";
    }

    private void DisplayUIDLabel(string uidDescription)
    {
        Debug.Log("Detected Device: " + uidDescription);
    }
    public string GetLastScannedUIDDescription()
    {
        // Check if there is a current UID and return its description if available
        if (currentUID != null && uidDataDictionary.TryGetValue(currentUID, out var description))
        {
            return description;
        }
        return "No card scanned or unknown device";
    }

    public string GetLastScannedUID()
    {
        return currentUID;
    }


    private bool IsEmpty(ICollection<string> readerNames) =>
        readerNames == null || readerNames.Count < 1;
}
