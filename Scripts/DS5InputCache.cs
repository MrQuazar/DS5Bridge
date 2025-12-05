using UnityEngine;

/// <summary>
/// Polls the native DS5Bridge once per frame and stores the latest state.
/// </summary>
public class DS5InputCache : MonoBehaviour
{
#if UNITY_STANDALONE
    public DS5Bridge.DS5InputState State { get; private set; }
    public bool HasState { get; private set; }

    void Update()
    {
        if (DS5Bridge.TryGetInput(out var state))
        {
            State = state;
            HasState = true;
        }
        else
        {
            HasState = false;
        }
    }
#endif
}
