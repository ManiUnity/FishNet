using System.Collections;
using System.Collections.Generic;
using FishNet.Broadcast;
using UnityEngine;

public struct EventBroadcast : IBroadcast
{
    public string FromNetworkId;
    public string EventName;
}