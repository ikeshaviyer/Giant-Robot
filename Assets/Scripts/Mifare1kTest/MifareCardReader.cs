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
    private HashSet<string> scannedUIDs = new HashSet<string>();  // Store scanned UIDs

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

    public static void ResetCards()
    {
        if (Instance == null)
        {
            Debug.LogError("MifareCardReader instance not found in the scene.");
            return;
        }

        Instance.ResetScannedUIDs();
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
                        mifareCard = new MifareCard(isoReader);

                        // Attempt to read only the UID of the card.
                        var uid = mifareCard.GetData();
                        if (uid != null)
                        {
                            string uidString = BitConverter.ToString(uid);

                            // Check if UID has already been scanned
                            if (!scannedUIDs.Contains(uidString))
                            {
                                currentUID = uidString;
                                scannedUIDs.Add(uidString);  // Add UID to the set of scanned UIDs
                                DisplayUIDLabel(currentUID);
                            }
                        }
                        else
                        {
                            ResetUID();  // Reset UID if no card is detected
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

    private void ResetUID()
    {
        currentUID = null;
        DisplayUIDLabel("None");
    }

    private void ResetScannedUIDs()
    {
        scannedUIDs.Clear();  // Clear the set of scanned UIDs
        Debug.Log("Scanned UIDs have been reset.");
    }

    private void DisplayUIDLabel(string uid)
    {
        Debug.Log("Current UID: " + uid);
    }

    private bool IsEmpty(ICollection<string> readerNames) =>
        readerNames == null || readerNames.Count < 1;
}
