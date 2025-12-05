
using UnityEngine;

/// <summary>
/// Input manager for DS5W to initialize DS5Bridge and DS5InputMapper
/// </summary>
public class DS5InputManager : MonoBehaviour
{
#if UNITY_STANDALONE
    public bool createCacheIfMissing = true;
    public bool isMainMenu = true;
    void Start()
    {
        bool ok = DS5Bridge.Initialize();
        if (ok)
        {
            Debug.Log("DS5Bridge initialized successfully. Setting lightbar to magenta.");
            if (isMainMenu)
            {
                DS5Bridge.SetLightbar(Color.magenta);
            }
            else
            {
                DS5Bridge.SetLightbar(Color.blue);
            }
        }
        else
        {
            Debug.LogError("DS5Bridge initialization failed.");
        }

        // Find existing DS5InputCache in scene
        DS5InputCache cache = FindAnyObjectByType<DS5InputCache>();
        if (cache == null && createCacheIfMissing)
        {
            var go = new GameObject("DS5InputCache");
            cache = go.AddComponent<DS5InputCache>();
            DontDestroyOnLoad(go);
        }

        if (cache != null)
        {
            DS5InputMapper.Init(cache);
            Debug.Log("DS5InputMapper initialized with DS5InputCache.");
        }
        else
        {
            Debug.LogWarning("No DS5InputCache found or created; DS5InputMapper not initialized.");
        }
    }
    void OnApplicationQuit()
    {
        DS5Bridge.Shutdown();
    }
#endif
}
