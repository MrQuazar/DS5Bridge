# DS5Bridge

**Windows C++ wrapper for PS5 DualSense controllers** — fully Unity-compatible, enabling advanced DualSense features on Windows without a PS5 devkit.

---

## Overview

DS5Bridge extends the [DS5W](https://gitlab.com/ohjkrut/ds5w) Windows driver to provide Unity-safe access to DualSense controller features including:

- Adaptive trigger resistance
- Lightbar color control
- Rumble motors (left & right)
- Player LEDs
- Microphone LED
- Input polling (sticks, triggers, buttons, accelerometer, gyroscope, touchpad, battery)

Designed for developers who want to **test or implement DualSense features on Windows** without requiring a PS5 devkit.

**Purpose:** Extend DS5W to support Unity-safe DualSense operations including advanced triggers, microphone input, lightbar, and rumble.

**Techniques:**
- Thread-safe cached output using a global `DS5OutputState g_cachedOutput`
- `CRITICAL_SECTION` locking for hardware writes
- Atomic device updates using `setDeviceOutputState(...)`
- Continuous resistance trigger profiles via `MakeContinuousResistance()`

---

## Installation

1. Clone or download this repository:

```bash
git clone https://github.com/MrQuazar/DS5Bridge.git
```

2. Build the DLL using Visual Studio (or use the prebuilt DLL if available).  
3. Add the DLL to your Unity project (`Assets/Plugins` is recommended).  
4. Include the C# bridge scripts in your Unity project:
   - `DS5Bridge.cs`
   - `DS5Constants.cs`
   - `DS5InputCache.cs`

---

## Usage in Unity

### Initializing the DualSense

```csharp
using UnityEngine;

public class DualSenseDemo : MonoBehaviour
{
    void Start()
    {
        if (DS5Bridge.Initialize())
            Debug.Log("DualSense initialized!");
    }

    void OnApplicationQuit()
    {
        DS5Bridge.Shutdown();
    }
}
```

### Lightbar

```csharp
DS5Bridge.SetLightbar(Color.red);   // Red lightbar
DS5Bridge.SetLightbar(Color.green); // Green lightbar
```

### Rumble

```csharp
DS5Bridge.Rumble(0.5f, 1.0f); // Left 50%, Right 100%
```

### Adaptive Triggers

```csharp
DS5Bridge.SetTriggerResistance(true, 0, 200);   // Left trigger
DS5Bridge.SetTriggerResistance(false, 50, 150); // Right trigger
DS5Bridge.SetBothTriggerResistance(0, 200, 50, 150); // Both triggers
DS5Bridge.ClearTriggers();                            // Reset triggers
```

### Player LEDs & Mic LED

```csharp
DS5Bridge.SetPlayerLeds(DS5Constants.PLAYER_LED_LEFT | DS5Constants.PLAYER_LED_MIDDLE_RIGHT);
DS5Bridge.SetMicLed(true);
```

### Reading Input

```csharp
if (DS5Bridge.TryGetInput(out var state))
{
    Debug.Log($"Left Stick X: {state.leftStickX}, Cross Pressed: {(state.buttonsAndDpad & DS5Constants.BTX_CROSS) != 0}");
}
```

---

## Using DS5InputCache

Add the `DS5InputCache` component to a GameObject to automatically poll input each frame:

```csharp
public class PlayerController : MonoBehaviour
{
    public DS5InputCache inputCache;

    void Update()
    {
        if (inputCache.HasState)
        {
            var state = inputCache.State;
            float moveX = state.leftStickX / 127f;
            float moveY = state.leftStickY / 127f;
            // Use input to move player, rotate camera, etc.
        }
    }
}
```

---

## License

MIT-style license. See `DS5Bridge.cpp` for full license details.  
Use at your own risk. No warranty is provided, and the author is not responsible for any damage, loss, or other issues arising from the use of this software.

---

## Credits

- Based on [ohjkrut's DS5W](https://gitlab.com/ohjkrut/ds5w)  
- Author: Aartem Singh