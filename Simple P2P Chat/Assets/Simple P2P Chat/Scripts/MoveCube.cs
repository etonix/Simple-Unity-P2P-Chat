/*
 * Author: Thomas Pearce
 * Studio: Shadowed Souls Gaming
 * 
 * Project: Simple P2P Chat
 * Dependencies: Mirror Networking, Text Mesh Pro
 * Mirror URL: https://www.mirror-networking.com // https://assetstore.unity.com/packages/tools/network/mirror-129321
 * 
 * File: MoveCube.cs
 * Date: 11/08/2019
 * 
 * Visit us: https://www.shadowed-souls.net
 * 
 */
using Mirror;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using UnityEngine;
using System.Collections;

namespace ShadowedSouls.Core
{
    public class MoveCube : NetworkBehaviour 
    {
        public int maxMessages = 45;
        List<Message> listOfMessages = new List<Message>();
        string myName = "";

        public bool showTimestamps = true;

        public GameObject chatWindow, chatMessagePrefab;
        public TMP_InputField chatInput;

        private void Awake()
        {
            chatWindow = GameObject.Find("Chat Window");
            chatInput = GameObject.FindObjectOfType<TMP_InputField>();
        }


        private void Start()
        {
            ShowIntro();
        }

        private void ShowIntro()
        {
            if (!isLocalPlayer)
                return;

            transform.position = new Vector3(Random.Range(450, 650), Random.Range(250, 350), Random.Range(150, 225));
            AddMessage("SYSTEM", "Welcome! Please set your name using the /name command.", NetworkClient.connection.identity, 2);
        }

        public void ClientConnect()
        {
            ClientScene.RegisterPrefab(chatMessagePrefab);
        }

        void Update()
        {
            if (!isLocalPlayer)
                return;

            if (Input.GetKeyDown(KeyCode.Return))
            {
                if (chatInput.text != "")
                {
                    if (chatInput.text.StartsWith("/name "))
                    {
                        if (myName == "")
                        {
                            string tmp = chatInput.text.Replace("/name ", " ").TrimStart();
                            if (tmp != "")
                            {
                                myName = tmp;
                                chatInput.text = "";
                                AddMessage("SYSTEM", "Your name is now " + myName, NetworkClient.connection.identity, 2);
                                CmdSend("SYSTEM", myName + " has joined the server.", NetworkClient.connection.identity, 1);
                            }
                            return;
                        } else
                        {
                            string tmp = chatInput.text.Replace("/name ", " ").TrimStart();
                            if (tmp != "")
                            {
                                chatInput.text = "";
                                AddMessage("SYSTEM", "Your name is now " + tmp, NetworkClient.connection.identity, 2);
                                CmdSend("SYSTEM", myName + " is now known as " + tmp, NetworkClient.connection.identity, 1);
                                myName = tmp;
                            }
                            return;
                        }
                    }

                    if (myName == "")
                    {
                        AddMessage("SYSTEM", "You must first set a name!", NetworkClient.connection.identity, 2);
                        chatInput.text = "";
                        return;
                    }
                    else if (myName == "SYSTEM")
                    {
                        AddMessage("SYSTEM", "You cannot impersonate the server.", NetworkClient.connection.identity, 2);
                        chatInput.text = "";
                        return;
                    }

                    string msg = chatInput.text;
                    chatInput.text = "";
                    CmdSend(myName, msg, NetworkClient.connection.identity, 0);
                }
            } else
            {
                if (UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject != chatInput.gameObject)
                    UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(chatInput.gameObject);
            }

            StartCoroutine("ForceScrollDown");
        }
        
        IEnumerator ForceScrollDown()
        {
            yield return new WaitForEndOfFrame();
            chatWindow.GetComponentInParent<ScrollRect>().verticalNormalizedPosition = 0f;
        }

        [Command]
        private void CmdSend(string whoFrom, string message, NetworkIdentity identity, int ignoreLocal)
        {
            RpcReceive(whoFrom, message, identity, ignoreLocal);
        }

        [ClientRpc]
        public void RpcReceive(string whoFrom, string message, NetworkIdentity identity, int ignoreLocal)
        {
            AddMessage(whoFrom, message, identity, ignoreLocal);
        }

        private void AddMessage(string whoFrom, string message, NetworkIdentity identity, int ignoreLocal = 0)
        {
            if (ignoreLocal == 1)
            {
                if(identity == NetworkClient.connection.identity)
                    return;

            } else if(ignoreLocal == 2)
            {
                if (identity != NetworkClient.connection.identity)
                    return;
            }
            GameObject msgPrefab = Instantiate(chatMessagePrefab, chatWindow.transform);

            Message msg = new Message()
            {
                fromWho = whoFrom,
                message = message,
                dateTime = System.DateTime.Now.ToShortTimeString()
            };

            msg.messageTarget = msgPrefab.GetComponent<TextMeshProUGUI>();
            string msgTime;

            if (NetworkClient.connection.identity.GetComponent<MoveCube>().showTimestamps)
                msgTime = string.Format("[{0:t}]", System.DateTime.Now);
            else
                msgTime = "";

            string txt = string.Format("[{0}]{1}:{2}", whoFrom, msgTime, msg.message);
            msg.messageTarget.text = txt;


            if (listOfMessages.Count >= maxMessages)
                for (int i = 0; i < 5; i++)
                {
                    Destroy(listOfMessages[0].messageTarget.gameObject);
                    listOfMessages.Remove(listOfMessages[0]);
                }


            listOfMessages.Add(msg);
        }
    }

    [System.Serializable]
    public class Message
    {
        public string fromWho;
        public string message;
        public string dateTime;

        public TextMeshProUGUI messageTarget;
    }
}
