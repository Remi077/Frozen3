using UnityEngine;
using System.Runtime.InteropServices;

public class FullscreenButton : MonoBehaviour
{
#if UNITY_WEBGL && !UNITY_EDITOR
    [DllImport("__Internal")]
    private static extern void RequestFullscreen();

    [DllImport("__Internal")]
    private static extern bool IsFullscreen();
#endif

    public void ToggleFullscreen()
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        if (!IsFullscreen())
            RequestFullscreen();
#else
        Screen.fullScreen = !Screen.fullScreen;
#endif
    }
}
