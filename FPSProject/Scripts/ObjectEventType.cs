using System;
using System.Collections;
using System.Collections.Generic;
using FishNet;
using FishNet.Connection;
using FishNet.Object;
using FishNet.Transporting;
using UnityEngine;

public class ObjectEventType : MonoBehaviour
{
    /// <summary>
    /// Event Type of an Object 
    /// </summary>
    public string EventType;
    /// <summary>
    /// Start of a Object Event Type 
    /// </summary>
    public bool IsMove;
    public bool IsOwner;
    /// <summary>
    /// is Server 
    /// </summary>
    public bool IsServer = false;
    private Transform previousTransform;
    public bool ShowChangeTransform;
    public NetworkObject Network;

    /// <summary>
    /// Start 
    /// </summary>
    private void Start()
    {
        InstanceFinder.ServerManager.OnRemoteConnectionState += OnRemoteConnection;
        InstanceFinder.ServerManager.OnServerConnectionState += ServerConnectionState;
        InstanceFinder.ClientManager.OnClientConnectionState += ClientConnectState;
        InstanceFinder.ClientManager.OnConnectedClients += ConectedClientsCallback;
        Network = gameObject.GetComponent<NetworkObject>();
    }

    private void ConectedClientsCallback(ConnectedClientsArgs obj)
    {
        IsServer = InstanceFinder.IsServerStarted;
    }

    private void ServerConnectionState(ServerConnectionStateArgs obj)
    {
        IsServer = InstanceFinder.IsServerStarted;
    }

    private void ClientConnectState(ClientConnectionStateArgs obj)
    {
        IsServer = InstanceFinder.IsServerStarted;
    }

    private void OnRemoteConnection(NetworkConnection arg1, RemoteConnectionStateArgs arg2)
    {
        IsServer = InstanceFinder.IsServerStarted;
    }
    public int OwnerId = -1;

    bool HasMoved(Transform t1, Transform t2)
    {
        if ((t1.position - t2.position).sqrMagnitude > float.Epsilon)
        {
            return true;
        }
        if (t1.localScale != t2.localScale)
        {
            return false; // Object has not moved
        }
        if (Quaternion.Dot(t1.rotation, t2.rotation) < 1.0f - float.Epsilon)
        {
            return true;
        }
        return false;
    }

    bool IsTranslate(Vector3 previous, Vector3 current)
    {
        return (previous - current).sqrMagnitude == 0;
    }

    [ContextMenu("Authority CHeck")]
    void AuhorityGive()
    {
    }
    internal void CallEvent()
    {
        PlayerEvents.Register(gameObject, EventType);
    }
    void LateUpdate()
    {
        if (Network != null && Network.Owner != null)
        {
            OwnerId = Network.Owner.ClientId;
            IsOwner = true;
        }
        else
        {
            IsOwner = false;
        }
    }
    internal void SetLocalOwnership(int clientId)
    {
        if (OwnerId != clientId)
            OwnerId = clientId;
    }
}


