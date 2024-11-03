using System;
using System.Runtime.InteropServices;
using UnityEngine;

public class NFCReaderUnity : MonoBehaviour
{
    private IntPtr _context;
    private IntPtr _cardHandle;

    [DllImport("winscard.dll")]
    private static extern int SCardEstablishContext(uint dwScope, IntPtr pvReserved1, IntPtr pvReserved2, out IntPtr phContext);

    [DllImport("winscard.dll")]
    private static extern int SCardReleaseContext(IntPtr hContext);

    [DllImport("winscard.dll")]
    private static extern int SCardConnect(IntPtr hContext, string szReader, uint dwShareMode, uint dwPreferredProtocols, out IntPtr phCard, out IntPtr pdwActiveProtocol);

    [DllImport("winscard.dll")]
    private static extern int SCardDisconnect(IntPtr hCard, int dwDisposition);

    [DllImport("winscard.dll")]
    private static extern int SCardTransmit(IntPtr hCard, ref SCARD_IO_REQUEST pioSendPci, byte[] pbSendBuffer, int cbSendLength, IntPtr pioRecvPci, byte[] pbRecvBuffer, ref int pcbRecvLength);

    private const uint SCARD_SCOPE_USER = 0;
    private const uint SCARD_SHARE_SHARED = 2;
    private const uint SCARD_PROTOCOL_T0 = 1;
    private const uint SCARD_PROTOCOL_T1 = 2;
    private const int SCARD_LEAVE_CARD = 0;

    // Structure for protocol information
    [StructLayout(LayoutKind.Sequential)]
    public struct SCARD_IO_REQUEST
    {
        public uint dwProtocol;
        public uint cbPciLength;
    }

    private SCARD_IO_REQUEST _ioRequest = new SCARD_IO_REQUEST { dwProtocol = SCARD_PROTOCOL_T1, cbPciLength = (uint)Marshal.SizeOf(typeof(SCARD_IO_REQUEST)) };

    public bool InitializeReader()
    {
        int result = SCardEstablishContext(SCARD_SCOPE_USER, IntPtr.Zero, IntPtr.Zero, out _context);
        if (result != 0)
        {
            Debug.LogError("Failed to establish NFC context.");
            return false;
        }
        Debug.Log("NFC context established.");
        return true;
    }

    public bool ConnectToCard()
    {
        int result = SCardConnect(_context, "ACS ACR122U PICC Interface", SCARD_SHARE_SHARED, SCARD_PROTOCOL_T0 | SCARD_PROTOCOL_T1, out _cardHandle, out _);
        if (result != 0)
        {
            Debug.LogError("Failed to connect to NFC card.");
            return false;
        }
        Debug.Log("Connected to NFC card.");
        return true;
    }

    public void Disconnect()
    {
        SCardDisconnect(_cardHandle, SCARD_LEAVE_CARD);
        SCardReleaseContext(_context);
        Debug.Log("Disconnected and released NFC context.");
    }

    public string ReadData()
    {
        // Example command to read the UID of the NFC card, varies based on card type
        byte[] command = { 0xFF, 0xCA, 0x00, 0x00, 0x00 }; // APDU command for UID
        byte[] response = new byte[256];
        int responseLength = response.Length;

        int result = SCardTransmit(_cardHandle, ref _ioRequest, command, command.Length, IntPtr.Zero, response, ref responseLength);
        if (result != 0)
        {
            Debug.LogError("Failed to read data from NFC card.");
            return null;
        }

        // Convert the received bytes to a hexadecimal string
        string data = BitConverter.ToString(response, 0, responseLength).Replace("-", "");
        Debug.Log("NFC Card Data: " + data);
        return data;
    }
}
