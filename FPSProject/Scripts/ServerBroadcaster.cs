using System;
using System.Collections;
using System.Collections.Generic;
using FishNet;
using FishNet.Connection;
using FishNet.Managing.Server;
using FishNet.Object;
using FishNet.Transporting;
using UnityEngine;

public class ServerBroadcaster : MonoBehaviour
{
    // Start is called before the first frame update
    void OnEnable()
    {
       // InstanceFinder.ClientManager.RegisterBroadcast<EventBroadcast>(OnEventBroadcast);
    }

    private void OnEventBroadcast(EventBroadcast arg1, Channel arg2)
    {
        Debug.Log("Arg1 " + arg1.FromNetworkId + " " + arg1.EventName + "  " + arg2.ToString());
    }

    private void OnDisable()
    {
//        InstanceFinder.ClientManager.UnregisterBroadcast<EventBroadcast>(OnEventBroadcast);
    }
}
