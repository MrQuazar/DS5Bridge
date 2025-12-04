/*
 * DS5Bridge
 * -----------------
 * A simple C++ wrapper to allow Unity (or any Windows game) to interface with 
 * the PS5 DualSense controller without a PS5 devkit. 
 *
 * Based on ohjkrut's ds5w library: https://github.com/Ohjurot/DualSense-Windows
 *
 * License: MIT-style
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE, AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES, OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 * SOFTWARE.
 *
 * Author: Aartem Singh
 * GitLab: https://github.com/MrQuazar
 */

#include "pch.h"
#include <Windows.h>
#include <stdint.h>
#include <string.h>
#include "ds5w.h"

using namespace DS5W;

extern "C"
{
    static DeviceEnumInfo deviceInfos[4];
    static DeviceContext deviceCtx;

    // Cached output state so each call updates only fields we want and doesn't overwrite others.
    static DS5OutputState g_cachedOutput{};
    static bool g_cachedOutputInitialized = false;

    // Synchronization for thread-safety
    static CRITICAL_SECTION g_cs;
    static bool g_csInitialized = false;

    // Helper to ensure cached output is initialised with sensible defaults
    static void EnsureCachedOutputInitialized()
    {
        if (g_cachedOutputInitialized) return;
        memset(&g_cachedOutput, 0, sizeof(g_cachedOutput));

        // defaults:
        g_cachedOutput.leftRumble = 0;
        g_cachedOutput.rightRumble = 0;
        g_cachedOutput.microphoneLed = MicLed::OFF;
        g_cachedOutput.disableLeds = false;
        g_cachedOutput.playerLeds.bitmask = 0;
        g_cachedOutput.playerLeds.playerLedFade = false;
        g_cachedOutput.playerLeds.brightness = LedBrightness::MEDIUM;
        g_cachedOutput.lightbar.r = 0;
        g_cachedOutput.lightbar.g = 0;
        g_cachedOutput.lightbar.b = 0;

        // Set trigger effect types to NoResitance by default
        g_cachedOutput.leftTriggerEffect.effectType = TriggerEffectType::NoResitance;
        g_cachedOutput.rightTriggerEffect.effectType = TriggerEffectType::NoResitance;

        g_cachedOutputInitialized = true;
    }

    // Helper to send cached state to device
    static void ApplyCachedOutput()
    {
        if (!g_csInitialized)
        {
            // fallback
            setDeviceOutputState(&deviceCtx, &g_cachedOutput);
            return;
        }

        EnterCriticalSection(&g_cs);
        setDeviceOutputState(&deviceCtx, &g_cachedOutput);
        LeaveCriticalSection(&g_cs);
    }

    // Helper to build a continuous resistance TriggerEffect
    static TriggerEffect MakeContinuousResistance(uint8_t startPos, uint8_t force)
    {
        TriggerEffect eff{};
        eff.effectType = TriggerEffectType::ContinuousResitance;
        eff.Continuous.startPosition = startPos;
        eff.Continuous.force = force;
        return eff;
    }

    __declspec(dllexport) int DS5_Init()
    {
        if (!g_csInitialized)
        {
            InitializeCriticalSection(&g_cs);
            g_csInitialized = true;
        }

        unsigned int count = 0;
        if (DS5W_FAILED(enumDevices(deviceInfos, 4, &count, true)) || count == 0)
            return -1;
        if (DS5W_FAILED(initDeviceContext(&deviceInfos[0], &deviceCtx)))
            return -2;

        EnsureCachedOutputInitialized();

        return 0;
    }

    __declspec(dllexport) void DS5_Quit()
    {
        EnterCriticalSection(&g_cs);
        memset(&g_cachedOutput, 0, sizeof(g_cachedOutput));

        // set default trigger types
        g_cachedOutput.leftTriggerEffect.effectType = TriggerEffectType::NoResitance;
        g_cachedOutput.rightTriggerEffect.effectType = TriggerEffectType::NoResitance;

        setDeviceOutputState(&deviceCtx, &g_cachedOutput);
        LeaveCriticalSection(&g_cs);

        freeDeviceContext(&deviceCtx);

        // delete critical section
        if (g_csInitialized)
        {
            DeleteCriticalSection(&g_cs);
            g_csInitialized = false;
        }

        g_cachedOutputInitialized = false;
    }

    __declspec(dllexport) void DS5_SetLightbar(uint8_t r, uint8_t g, uint8_t b)
    {
        EnsureCachedOutputInitialized();

        EnterCriticalSection(&g_cs);
        g_cachedOutput.lightbar.r = r;
        g_cachedOutput.lightbar.g = g;
        g_cachedOutput.lightbar.b = b;

        setDeviceOutputState(&deviceCtx, &g_cachedOutput);
        LeaveCriticalSection(&g_cs);
    }

    __declspec(dllexport) void DS5_SetRumble(uint8_t left, uint8_t right)
    {
        EnsureCachedOutputInitialized();

        EnterCriticalSection(&g_cs);
        g_cachedOutput.leftRumble = left;
        g_cachedOutput.rightRumble = right;
        setDeviceOutputState(&deviceCtx, &g_cachedOutput);
        LeaveCriticalSection(&g_cs);
    }

    // Set resistance for single trigger
    __declspec(dllexport) void DS5_SetTriggerResistance(uint8_t trigger, uint8_t startPos, uint8_t force)
    {
        EnsureCachedOutputInitialized();

        EnterCriticalSection(&g_cs);
        TriggerEffect eff = MakeContinuousResistance(startPos, force);
        if (trigger == 0)
        {
            g_cachedOutput.leftTriggerEffect = eff;
        }
        else
        {
            g_cachedOutput.rightTriggerEffect = eff;
        }
        setDeviceOutputState(&deviceCtx, &g_cachedOutput);
        LeaveCriticalSection(&g_cs);
    }

    //set both trigger resistances atomically in one call
    __declspec(dllexport) void DS5_SetBothTriggerResistance(uint8_t leftStartPos, uint8_t leftForce, uint8_t rightStartPos, uint8_t rightForce)
    {
        EnsureCachedOutputInitialized();

        EnterCriticalSection(&g_cs);
        g_cachedOutput.leftTriggerEffect = MakeContinuousResistance(leftStartPos, leftForce);
        g_cachedOutput.rightTriggerEffect = MakeContinuousResistance(rightStartPos, rightForce);
        setDeviceOutputState(&deviceCtx, &g_cachedOutput);
        LeaveCriticalSection(&g_cs);
    }

    __declspec(dllexport) void DS5_ClearTriggers()
    {
        EnsureCachedOutputInitialized();

        EnterCriticalSection(&g_cs);
        g_cachedOutput.leftTriggerEffect.effectType = TriggerEffectType::NoResitance;
        g_cachedOutput.rightTriggerEffect.effectType = TriggerEffectType::NoResitance;

        // also clear feedback bytes if present
        g_cachedOutput.leftTriggerEffect._u1_raw[0] = 0;
        g_cachedOutput.rightTriggerEffect._u1_raw[0] = 0;
        setDeviceOutputState(&deviceCtx, &g_cachedOutput);
        LeaveCriticalSection(&g_cs);
    }

    __declspec(dllexport) void DS5_SetMicLed(bool on)
    {
        EnsureCachedOutputInitialized();

        EnterCriticalSection(&g_cs);
        g_cachedOutput.microphoneLed = on ? MicLed::ON : MicLed::OFF;
        setDeviceOutputState(&deviceCtx, &g_cachedOutput);
        LeaveCriticalSection(&g_cs);
    }

    __declspec(dllexport) void DS5_SetPlayerLeds(uint8_t bitmask)
    {
        EnsureCachedOutputInitialized();

        EnterCriticalSection(&g_cs);
        g_cachedOutput.playerLeds.bitmask = bitmask;
        g_cachedOutput.playerLeds.playerLedFade = true;
        g_cachedOutput.playerLeds.brightness = LedBrightness::MEDIUM;
        setDeviceOutputState(&deviceCtx, &g_cachedOutput);
        LeaveCriticalSection(&g_cs);
    }

    // Read input state
    __declspec(dllexport) bool DS5_GetInputState(DS5InputState* outState)
    {
        if (DS5W_FAILED(getDeviceInputState(&deviceCtx, outState)))
            return false;
        return true;
    }

}
