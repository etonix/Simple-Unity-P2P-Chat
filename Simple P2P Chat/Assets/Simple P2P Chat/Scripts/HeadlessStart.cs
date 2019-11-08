using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using Mirror;

namespace ShadowedSouls.Core
{
    public class HeadlessStart : MonoBehaviour
    {
        // detect headless mode (which has graphicsDeviceType Null)
        bool IsHeadless()
        {
            return SystemInfo.graphicsDeviceType == GraphicsDeviceType.Null;
        }

        void Awake()
        {
            if (IsHeadless())
            {
                Debug.Log("Headless mode detected. Starting server.");
                GetComponent<NetworkManager>().StartServer();
            }
        }
    }
}