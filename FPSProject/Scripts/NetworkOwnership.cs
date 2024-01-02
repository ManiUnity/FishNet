using System.Collections;
using System.Collections.Generic;
using FishNet.Component.Transforming;
using FishNet.Connection;
using FishNet.Object;
using FishNet.Transporting;
using UnityEngine;
using UnityEngine.EventSystems;

public class NetworkOwnership : NetworkBehaviour
{

    public bool IsServer, IsClientAuthorative,IsMaster;
    private NetworkObject NetworkObject;
    private NetworkTransform NetworkTransform;
    public bool IsClient, IsOwner;
    private EventManager EventManager;
    void Start()
    {
        // Get the CharacterController component attached to the GameObject
        NetworkObject = GetComponent<NetworkObject>();
        NetworkTransform = GetComponent<NetworkTransform>();
        Invoke("EventManagerSetup", 0.1f);
    }
    void EventManagerSetup()
    {
        EventManager = FindObjectOfType<EventManager>();
    }
    bool _reliable = false;
    // Update is called once per frame
    [System.Obsolete]
    void LateUpdate()
    {
        IsServer = NetworkObject.IsServer;
        IsClient= NetworkObject.IsClient;
        IsOwner = NetworkObject.IsOwner;
        FindObjectOfType<EventManager>().SetServerClient(IsServer, IsClient,IsOwner);
    }

    private void Update()
    {
        //If owner and space bar is pressed.
       
    }
    
    [ServerRpc(RequireOwnership =false)]
    private void RpcSendChat(string msg)
    {
        if(IsServer)
        Debug.Log($"Received {msg} on the server.");
    }

    /*
     * ObserversRpc allows the server to run logic on clients. Only observing clients will get and run the logic. 
     * Observers are set by using the Network Observer component. To use ObserversRpc add the ObserversRpc attribute to a method.
     */
    //[ObserversRpc(ExcludeOwner = true)]
    //private void RpcSetNumber(int next)
    //{
    //    Debug.Log($"Received number {next} from the server.");
    //}
   //Observer RPC send to client
   //by server 
    //[ObserversRpc(ExcludeOwner = false, BufferLast = true)]
    //public void PlayerCollideEvent(object next)
    //{
    //    //This won't run on owner and will send to new clients.
    //    Debug.Log($"Received number {next} from the server.");
    //}
    [ObserversRpc(ExcludeServer = true)]
    public void SendMessageA(NetworkConnection target, string sender, string message)
    {
        EventManager?.EventOperation(target,sender, message);
    }
    [ObserversRpc(ExcludeServer = true)]
    public void SendMessageA(NetworkConnection target, string sender, string message,string addon)
    {
        EventManager?.EventOperation(target, sender, message,addon);
    }
    [ServerRpc]
    public void SendToServerWithoutOwner(NetworkConnection target, string sender, string message)
    {
        EventManager?.EventOperation(target,sender, message);
    }
    [ServerRpc]
    public void SendToServerWithoutOwner(NetworkConnection target, string sender, string message,string addon)
    {
        EventManager?.EventOperation(target, sender, message,addon);
    }
    /*
     * Lastly is TargetRpc. This RPC is used to run logic on a specific client. You can implement this feature by adding the TargetRpc attribute. When sending a TargetRpc the first parameter of your method must always be a NetworkConnection; this is the connection the data goes to.
private void UpdateOwnersAmmo()
{
    /* Even though this example passes in owner, you can send to
    * any connection that is an observer.
    RpcSetAmmo(base.Owner, 10);
}

[TargetRpc]
private void RpcSetAmmo(NetworkConnection conn, int newAmmo)
{
    //This might be something you only want the owner to be aware of.
    _myAmmo = newAmmo;
}
*/

    /*
     * Multi-Purpose Rpc
It is possible to have a single method be both a TargetRpc,
    as well an ObserversRpc. This can be very useful if you sometimes want to send a RPC to all observers,
    or a single individual. A chat message could be an example of this.
[ObserversRpc][TargetRpc]
private void DisplayChat(NetworkConnection target, string sender, string message)
{
    Debug.Log($"{sender}: {message}."); //Display a message from sender.
}

[Server]
private void SendChatMessage()
{
    //Send to only the owner.
    DisplayChat(base.Owner, "Bob", "Hello world");
    //Send to everyone.
    DisplayChat(null, "Bob", "Hello world");
}
     */


    /* This example uses ServerRpc, but any RPC will work.
    * Although this example shows a default value for the channel,
    * you do not need to include it. */
    [ServerRpc]
    private void RpcTest(string txt, Channel channel = Channel.Reliable)
    {
        if (channel == Channel.Reliable)
            Debug.Log("Message received! I never doubted you.");
        else if (channel == Channel.Unreliable)
            Debug.Log($"Glad you got here, I wasn't sure you'd make it.");
    }
}
