using UnityEngine;
using PCSC;
using PCSC.Exceptions;
using PCSC.Monitoring;
using PCSC.Iso7816;
using System;
using System.Threading;
using System.Linq;


public class NFCReader : MonoBehaviour
{
    private ISCardMonitor _monitor;
    private SynchronizationContext _syncContext;
    private readonly byte[] _defaultKey = { 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF };

    void Start()
    {
        var contextFactory = ContextFactory.Instance;
        using (var context = contextFactory.Establish(SCardScope.System))
        {
            // List all connected readers
            var readerNames = context.GetReaders();
            if (readerNames.Length == 0)
            {
                Debug.LogError("No NFC readers connected.");
                return;
            }

            var readerName = readerNames[0]; // Use the first reader

            // Create a monitor for the NFC reader
            var monitorFactory = MonitorFactory.Instance;
            _monitor = monitorFactory.Create(SCardScope.System);
            _monitor.CardInserted += CardInserted;
            _monitor.CardRemoved += CardRemoved;
            _monitor.MonitorException += MonitorException;

            // Start monitoring the reader
            Debug.Log($"Monitoring NFC Reader: {readerName}");
            _monitor.Start(readerName);

            _syncContext = SynchronizationContext.Current;
        }
    }

    private void CardInserted(object sender, CardStatusEventArgs args)
    {
        PostToMainThread(state => Debug.Log($"Card inserted in reader: {args.ReaderName}"));
        try
        {
            var contextFactory = ContextFactory.Instance;
            using (var context = contextFactory.Establish(SCardScope.System))
            using (var isoReader = new IsoReader(context, args.ReaderName, SCardShareMode.Shared, SCardProtocol.Any, false))
            {
                AuthenticateAndReadSector(isoReader, 1); // Read data from sector 1
            }
        }
        catch (PCSCException ex)
        {
            PostToMainThread(state => Debug.LogError($"Card read error: {ex.Message}"));
        }
    }

private void AuthenticateAndReadSector(IsoReader isoReader, int sector)
{
    var authenticateApdu = new CommandApdu(IsoCase.Case3Short, isoReader.ActiveProtocol)
    {
        CLA = 0xFF, // Class
        INS = 0x86, // INS code for authentication
        P1 = 0x00,
        P2 = 0x00,
        Data = new byte[] { 0x01, 0x00, (byte)sector, 0x60 }.Concat(_defaultKey).ToArray() // Using Key A
    };

    var authResponse = isoReader.Transmit(authenticateApdu);
    if (authResponse.SW1 == 0x90 && authResponse.SW2 == 0x00)
    {
        Debug.Log("Authentication successful.");
        for (int blockOffset = 0; blockOffset < 4; blockOffset++)
        {
            ReadBlockData(isoReader, sector * 4 + blockOffset);
        }
    }
    else
    {
        Debug.LogError($"Authentication failed: SW1 SW2 = {authResponse.SW1:X2} {authResponse.SW2:X2}");
    }
}

private void ReadBlockData(IsoReader isoReader, int block)
{
    var readApdu = new CommandApdu(IsoCase.Case2Short, isoReader.ActiveProtocol)
    {
        CLA = 0xFF,
        INS = 0xB0,
        P1 = 0x00,
        P2 = (byte)block,
        Le = 0x10 // Read 16 bytes
    };

    var response = isoReader.Transmit(readApdu);
    if (response.SW1 == 0x90 && response.SW2 == 0x00)
    {
        Debug.Log($"Block {block} data: {BitConverter.ToString(response.GetData())}");
    }
    else
    {
        Debug.LogError($"Failed to read block: SW1 SW2 = {response.SW1:X2} {response.SW2:X2}");
    }
}

    private void CardRemoved(object sender, CardStatusEventArgs args)
    {
        PostToMainThread(state => Debug.Log($"Card removed from reader: {args.ReaderName}"));
    }

    private void MonitorException(object sender, PCSCException ex)
    {
        PostToMainThread(state => Debug.LogError($"Monitor error: {ex.Message}"));
    }

    private void OnDestroy()
    {
        _monitor?.Cancel();
        _monitor?.Dispose();
    }

    private void PostToMainThread(SendOrPostCallback callback)
    {
        _syncContext?.Post(callback, null);
    }
}
