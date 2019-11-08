/*
 * Author: Thomas Pearce
 * Studio: Shadowed Souls Gaming
 * 
 * Project: Simple P2P Chat
 * Dependencies: Mirror Networking, Text Mesh Pro
 * Mirror URL: https://www.mirror-networking.com // https://assetstore.unity.com/packages/tools/network/mirror-129321
 * 
 * File: CameraMove.cs
 * Date: 11/08/2019
 * 
 * Visit us: https://www.shadowed-souls.net
 * 
 */
using Mirror;
using UnityEngine;
namespace ShadowedSouls.Core
{
    public class CameraMove : NetworkBehaviour
    {
        NetworkManager manager;
        public bool hasBeenSet = false;

        private void Awake()
        {
            manager = GetComponent<NetworkManager>();
        }
        void Start()
        {
            if (!isLocalPlayer)
                if (!isServer) return;

            if (!hasBeenSet)
            {
                MoveCube[] tmp = FindObjectsOfType<MoveCube>();

                foreach(MoveCube cube in tmp)
                {
                    if (cube.isLocalPlayer)
                    {
                        transform.parent = cube.transform;
                        hasBeenSet = true;
                        break;
                    } else
                    {
                        Debug.Log("<Camera>: Not found yet...");
                    }
                }
                if (hasBeenSet)
                {
                    transform.position = new Vector3(transform.parent.transform.position.x, 2f, -10f);
                    transform.rotation = Quaternion.Euler(new Vector3(18f, 0f, 0f));
                    transform.localScale = new Vector3(1f, 1f, 1f);
                }
            }
        }

        // Update is called once per frame
        void Update()
        {
            if (!isLocalPlayer)
                if (!isServer) return;

            if (!hasBeenSet)
            {
                MoveCube[] tmp = FindObjectsOfType<MoveCube>();

                foreach (MoveCube cube in tmp)
                {
                    if (cube.isLocalPlayer)
                    {
                        transform.parent = cube.transform;
                        hasBeenSet = true;
                        break;
                    }
                }
            }
            if (hasBeenSet)
            {
                transform.position = new Vector3(transform.parent.transform.position.x, transform.parent.transform.position.y + 2f, -10f);
                transform.rotation = Quaternion.Euler(new Vector3(12f, 0f, 0f));
                transform.localScale = new Vector3(1f, 1f, 1f);
            }
        }
    }
}