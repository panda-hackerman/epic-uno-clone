using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class AutoHostClient : NetworkBehaviour //This script automatically tries to connect to the server, otherwise will 
{
    public NetworkManager networkManager;

    private void Start()
    {
        if (!Application.isBatchMode) //If not a headless build
        {
            Debug.Log("Client Build");
            networkManager.StartClient();
        }
        else
        {
            //If this is a server build then StartHost() will automatically be called
            //So this can remain blank
            Debug.Log("Server Build");
        }
    }

    public void JoinLocal()
    {
        networkManager.networkAddress = "localhost";
        networkManager.StartClient();
    }
}
