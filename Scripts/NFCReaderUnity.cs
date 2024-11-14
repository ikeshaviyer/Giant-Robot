using System;
using System.Runtime.InteropServices;
using UnityEngine;

public class NFCReaderUnity : MonoBehaviour
{
    private IntPtr _context;
    private IntPtr _cardHandle;
    private bool _cardPresent = false;

    [DllImport("winscard.dll")]
    private static extern int SCardEstablishContext(uint dwScope, IntPtr pvReserved1, IntPtr pvReserved2, out IntPtr phContext);

    [DllImport("winscard.dll")]
    private static extern int SCardReleaseContext(IntPtr hContext);

    [DllImport("winscard.dll")]
    private static extern int SCardConnect(IntPtr hContext, string szReader, uint dwShareMode, uint dwPreferredProtocols, out IntPtr phCard, out uint pdwActiveProtocol);

    [DllImport("winscard.dll")]
    private static extern int SCardDisconnect(IntPtr hCard, int dwDisposition);

    [DllImport("winscard.dll")]
    private static extern int SCardTransmit(IntPtr hCard, ref SCARD_IO_REQUEST pioSendPci, byte[] pbSendBuffer, int cbSendLength, IntPtr pioRecvPci, byte[] pbRecvBuffer, ref int pcbRecvLength);

    private const uint SCARD_SCOPE_USER = 0;
    private const uint SCARD_SHARE_SHARED = 2;
    private const uint SCARD_PROTOCOL_T0 = 1;
    private const uint SCARD_PROTOCOL_T1 = 2;
    private const int SCARD_LEAVE_CARD = 0;

    [StructLayout(LayoutKind.Sequential)]
    public struct SCARD_IO_REQUEST
    {
        public uint dwProtocol;
        public uint cbPciLength;
    }

    private SCARD_IO_REQUEST _ioRequest = new SCARD_IO_REQUEST { dwProtocol = SCARD_PROTOCOL_T1, cbPciLength = (uint)Marshal.SizeOf(typeof(SCARD_IO_REQUEST)) };

    void Start()
    {
        if (InitializeReader())
        {
            StartCoroutine(CheckForCard());
        }
    }

    public bool InitializeReader()
    {
        int result = SCardEstablishContext(SCARD_SCOPE_USER, IntPtr.Zero, IntPtr.Zero, out _context);
        if (result != 0)
        {
            Debug.LogError("Failed to establish NFC context. Error Code: " + result);
            return false;
        }
        Debug.Log("NFC context established.");
        return true;
    }

    private System.Collections.IEnumerator CheckForCard()
    {
        while (true)
        {
            if (_cardPresent)
            {
                ReadAllData();
                yield return new WaitForSeconds(1f);
            }
            else
            {
                if (ConnectToCard())
                {
                    _cardPresent = true;
                    Debug.Log("Card detected.");
                }
                else
                {
                    yield return new WaitForSeconds(0.5f);
                }
            }
        }
    }

    public bool ConnectToCard()
    {
        int result = SCardConnect(_context, "ACS ACR122 0", SCARD_SHARE_SHARED, SCARD_PROTOCOL_T0 | SCARD_PROTOCOL_T1, out _cardHandle, out _);
        
        if (result != 0)
        {
            return false;
        }
        
        return true;
    }

    public void Disconnect()
    {
        if (_cardHandle != IntPtr.Zero)
        {
            SCardDisconnect(_cardHandle, SCARD_LEAVE_CARD);
            _cardHandle = IntPtr.Zero;
            Debug.Log("Card disconnected.");
        }

        if (_context != IntPtr.Zero)
        {
            SCardReleaseContext(_context);
            _context = IntPtr.Zero;
            Debug.Log("NFC context released.");
        }
    }

    public void ReadAllData()
    {
        for (int blockNumber = 0; blockNumber < 64; blockNumber++)
        {
            byte[] command = { 0xFF, 0xB0, 0x00, (byte)blockNumber, 0x10 };
            byte[] response = new byte[16];
            int responseLength = response.Length;

            int result = SCardTransmit(_cardHandle, ref _ioRequest, command, command.Length, IntPtr.Zero, response, ref responseLength);

            if (result == 0)
            {
                string hexData = BitConverter.ToString(response, 0, response.Length).Replace("-", " ");
                Debug.Log($"Block {blockNumber} Data (Hex): {hexData}");
            }
            else
            {
                Debug.LogError($"Failed to read from block {blockNumber}. Error Code: {result}");
                return;
            }
        }
    }

    void OnDestroy()
    {
        Disconnect();
    }
}