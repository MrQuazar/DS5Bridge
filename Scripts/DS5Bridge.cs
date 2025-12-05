using System.Runtime.InteropServices;
using UnityEngine;

/// <summary>
/// A bridge cs file to make the dll functions accessible to the project scripts.
/// </summary>
public static class DS5Bridge
{
    [DllImport("DS5Bridge")] private static extern int DS5_Init();
    [DllImport("DS5Bridge")] private static extern void DS5_Quit();
    [DllImport("DS5Bridge")] private static extern void DS5_SetLightbar(byte r, byte g, byte b);
    [DllImport("DS5Bridge")] private static extern void DS5_SetRumble(byte left, byte right);
    [DllImport("DS5Bridge")] private static extern void DS5_SetTriggerResistance(byte trigger, byte startPos, byte force);
    [DllImport("DS5Bridge")] private static extern void DS5_SetBothTriggerResistance(byte leftStartPos, byte leftForce, byte rightStartPos, byte rightForce);
    [DllImport("DS5Bridge")] private static extern void DS5_ClearTriggers();
    [DllImport("DS5Bridge")] private static extern void DS5_SetMicLed([MarshalAs(UnmanagedType.I1)] bool on);
    [DllImport("DS5Bridge")] private static extern void DS5_SetPlayerLeds(byte bitmask);

    public static bool Initialize()
    {
        int result = DS5_Init();
        if (result != 0)
        {
            Debug.LogError($"DualSense init failed (code {result})");
            return false;
        }
        Debug.Log("DualSense initialized");
        return true;
    }

    public static void Shutdown() => DS5_Quit();

    public static void SetLightbar(Color color) =>
        DS5_SetLightbar((byte)(Mathf.Clamp01(color.r) * 255), (byte)(Mathf.Clamp01(color.g) * 255), (byte)(Mathf.Clamp01(color.b) * 255));

    public static void Rumble(float left, float right) =>
        DS5_SetRumble((byte)(Mathf.Clamp01(left) * 255), (byte)(Mathf.Clamp01(right) * 255));

    public static void SetTriggerResistance(bool leftTrigger, byte startPos, byte force) =>
        DS5_SetTriggerResistance(leftTrigger ? (byte)0 : (byte)1, startPos, force);

    public static void SetBothTriggerResistance(byte leftStartPos, byte leftForce, byte rightStartPos, byte rightForce) =>
        DS5_SetBothTriggerResistance(leftStartPos, leftForce, rightStartPos, rightForce);

    public static void ClearTriggers() => DS5_ClearTriggers();

    public static void SetMicLed(bool on) => DS5_SetMicLed(on);

    public static void SetPlayerLeds(byte bitmask) => DS5_SetPlayerLeds(bitmask);

    // ---- Structs ----
    [StructLayout(LayoutKind.Sequential)]
    public struct DS5Vector3
    {
        public short x;
        public short y;
        public short z;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct DS5Touch
    {
        public uint x;
        public uint y;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct DS5Battery
    {
        [MarshalAs(UnmanagedType.I1)] public bool charging;
        [MarshalAs(UnmanagedType.I1)] public bool full;
        public byte level;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct DS5InputState
    {
        public sbyte leftStickX;
        public sbyte leftStickY;
        public sbyte rightStickX;
        public sbyte rightStickY;
        public byte leftTrigger;
        public byte rightTrigger;
        public byte buttonsAndDpad;
        public byte buttonsA;
        public byte buttonsB;
        public DS5Vector3 accelerometer;
        public DS5Vector3 gyroscope;
        public DS5Touch touch1;
        public DS5Touch touch2;
        public DS5Battery battery;
        [MarshalAs(UnmanagedType.I1)] public bool headphoneConnected;
        public byte leftTriggerFeedback;
        public byte rightTriggerFeedback;
    }

    [DllImport("DS5Bridge")] private static extern bool DS5_GetInputState(out DS5InputState state);
    public static bool TryGetInput(out DS5InputState state) => DS5_GetInputState(out state);
}
