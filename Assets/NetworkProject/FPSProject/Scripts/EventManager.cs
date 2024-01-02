using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Component.Transforming;
using FishNet.Connection;
using FishNet.Object;
using FishNet.Transporting;
using System.Linq;
using System;
using FishNet.Managing.Server;
using FishNet;

public class EventManager : MonoBehaviour
{
    public bool IsServer, IsClient, IsOwner;
    private Dictionary<int, CharacterMove> NetworkPlayers = new Dictionary<int, CharacterMove>();
    Dictionary<int, NetworkConnection> Clients;
    public List<CharacterMove> Moves;
    public List<NetworkObject> Networkobjects = new List<NetworkObject>();
    public List<NetworkConnection> Conenctions = new List<NetworkConnection>();
    public GameObject missilePrefab;

    void Start()
    {
        Clients = InstanceFinder.ServerManager.Clients;
        //  InstanceFinder.ServerManager.Objects.SceneObjects
        //  InstanceFinder.ServerManager.Objects.Spawned
        InstanceFinder.ServerManager.OnRemoteConnectionState += OnRemoteConnection;
        InstanceFinder.ServerManager.OnServerConnectionState += ServerConnectionState;
        InstanceFinder.ServerManager.OnClientKick += ClientKick;
        InstanceFinder.ClientManager.OnClientConnectionState += ClientConnectState;
        InstanceFinder.ClientManager.OnConnectedClients += ConectedClientsCallback;
        InstanceFinder.ClientManager.OnRemoteConnectionState += OnRemoteConnectionClient;
    }
    public RemoteConnectionStateArgs CurrentConnectedClient;
    private void OnRemoteConnectionClient(RemoteConnectionStateArgs obj)
    {
        Debug.Log("Remote OnRemoteConnectionClient >>> M");
        Invoke("SpawnAt", 0.5f);
        CurrentConnectedClient = obj;
        Invoke("SendAllClientsDataTo", 0.6f);

    }
    void SendAllClientsDataTo()
    {
        Networkobjects = Networkobjects.Where(data => data != null).ToList();
        Networkobjects.ForEach(data => {
           if(data.GetComponent<PlayerAnimator>()!=null)
            {
                data.GetComponent<PlayerAnimator>().SendJoinScore();
            }
        });

    }
    private void SendAllDatasFromServerToClients()
    {
      
    }

    private void ConectedClientsCallback(ConnectedClientsArgs obj)
    {
        Debug.Log("Remote ConectedClientsCallback >>> M");
        Invoke("SpawnAt", 0.5f);
    }

    private void ClientConnectState(ClientConnectionStateArgs obj)
    {
        Debug.Log("Remote ClientConnectState >>> M");
        Invoke("SpawnAt", 0.5f);
    }

    private void ClientKick(NetworkConnection arg1, int arg2, KickReason arg3)
    {
        Debug.Log("Remote ClientKick >>> M");
        Invoke("SpawnAt", 0.5f);
    }

    private void ServerConnectionState(ServerConnectionStateArgs obj)
    {
        Debug.Log("Remote ServerConnectionState >>> M");
        Invoke("SpawnAt", 0.5f);
    }
    private NetworkConnection ClientConncetion;
    private void OnRemoteConnection(NetworkConnection arg1, RemoteConnectionStateArgs arg2)
    {
        ClientConncetion = arg1;
        Debug.Log("Arg2 " + arg2.ConnectionId+"  "+ arg1.ClientId);
        if(!Conenctions.Contains(arg1))
        Conenctions.Add(arg1);
        Debug.Log("Remote OnRemoteConnection >>> M");
        Invoke("SpawnAt", 0.5f);
    }

    private void ConnectState(ClientConnectionStateArgs obj)
    {
    }

    public List<int> PlayerIds;
    public int NetWorkCount;
    private void OnConnected(ConnectedClientsArgs obj)
    {
        PlayerIds = obj.ClientIds;
        
    }
    [ContextMenu("Chcngae Owner")]
    void ChangeOwner()
    {
        GameObject.Find("MachineGun1").gameObject.GetComponent<NetworkObject>()
                   .GiveOwnership(ClientConncetion);
    }

    
    public void EventOperation(NetworkConnection connection, string playerid, string message, string addonmessage )
    {
        if (Networkobjects == null)
            Debug.Log("Networkobjects is nullify " + playerid);
        Networkobjects = Networkobjects.Where(data => data != null).ToList();
        CharacterMove chm = Networkobjects.Find(data => data.ObjectId.Equals(int.Parse(playerid))).
            gameObject.GetComponent<CharacterMove>();
        switch (message)
        {
            case "score":
                chm.GetComponent<PlayerAnimator>().SetScore(addonmessage);
                break;
            case "health":
                chm.GetComponent<PlayerAnimator>().SetHealth(addonmessage);
                break;
        }
        if (IsServer)
        {

            FindObjectOfType<NetworkOwnership>().SendMessageA(InstanceFinder.ClientManager.Connection, playerid, message,addonmessage);
        }
    }
        public void EventOperation(NetworkConnection connection , string playerid , string message)
    {
        if (!Networkobjects.Select(xda => xda.ObjectId).ToList().Contains(int.Parse(playerid)))
            return;
        Networkobjects = Networkobjects.Where(data => data != null).ToList();
        CharacterMove chm = Networkobjects.Find(data => data.ObjectId.Equals(int.Parse(playerid))).
            gameObject.GetComponent<CharacterMove>();
   
        switch (message)
        {
            case "Door1":
                if (GameObject.Find("DoorPivot").transform.localScale.x > 0.5f)
                    StartCoroutine(ScaleDownToZero(GameObject.Find("DoorPivot")));
                break;
            case "Door2":
                if (GameObject.Find("DoorPivot").transform.localScale.x > 0.5f)
                    StartCoroutine(ScaleDownToZero(GameObject.Find("DoorPivot")));
                break;
            case "MachineGun1":
            case "MachineGun2":
            case "Grenade1":
            case "Grenade2":
                chm?.SetWeapon(message);
                GameObject gobj = GameObject.Find(message);
                try
                {
                    
                    int OwnerId = gobj.GetComponent<ObjectEventType>().OwnerId;
                    bool IsOwner = gobj.GetComponent<NetworkObject>().IsOwner;
                        if (!IsServer && !IsOwner)
                        {
                        
                           return;
                        }
                    if (IsServer && (!IsOwner))
                        {
                            Debug.Log("Parenting >>>>> ");
                            gobj.GetComponent<NetworkObject>().GiveOwnership(connection);
                        }
                }
                catch(Exception exa)
                {
                    Debug.Log("exa " + exa.Message);
                }
                break;
            case "MissileSpawn":
                if (IsServer) // from client 
                {
                      GameObject missileobject = Instantiate(
                        missilePrefab,
                        chm.spawnpoint.transform.position,
                        chm.spawnpoint.transform.rotation);
                        missileobject.SetActive(true);
                        MissileFire mfire;
                    if(missileobject.TryGetComponent(out mfire))
                    {
                        mfire.FireOwner = chm;
                    }
                    InstanceFinder.ServerManager.Spawn(missileobject);
                }
                break;
            case "Base Layer.walkclip":
                if (IsServer)
                    chm.GetComponent<PlayerAnimator>().Animator.GiveOwnership(connection);
                break;
            case "Base Layer.idleclip":
                if (IsServer)
                    chm.GetComponent<PlayerAnimator>().Animator.GiveOwnership(connection);
                break;
            case "score":
                
                break;
            case "health":
               
                break;
            
        }
        if (IsServer)
        {
            
            FindObjectOfType<NetworkOwnership>().SendMessageA(InstanceFinder.ClientManager.Connection, playerid, message);
        }
    }
    public void SetServerOwnerShip()
    {
       
    }
    void SpawnAt()
    {
     //   Moves = FindObjectsOfType<CharacterMove>().ToList();
        IEnumerable<KeyValuePair<int, NetworkObject>> datas = IsServer ?
            InstanceFinder.ServerManager.Objects.Spawned :
            InstanceFinder.ClientManager.Objects.Spawned;

        Networkobjects.Clear();
        foreach (var data in datas)
            Networkobjects.Add(data.Value);

    }
    private void Update()
    {
      
    }
    public void SetServerClient(bool isServer,bool isClient , bool isOwner)
    {
        IsServer = isServer;
        IsClient = isClient;
        IsOwner = isOwner;
    }
    IEnumerator ScaleDownToZero(GameObject Door)
    {
        float scaleX = 1;
        while (scaleX > 0)
        {
            // Adjust the x scale down based on unscaled deltaTime
            scaleX -= Time.unscaledDeltaTime * 5f;
            // Apply the new x scale to the object
            Door.transform.localScale = new Vector3(scaleX, Door.transform.localScale.y, Door.transform.localScale.z);
            // Wait for the next frame
            yield return null;
        }

        Door.transform.GetChild(0).GetComponent<BoxCollider>().enabled = false;

    }
    private void OnDestroy()
    {
    }

    

}

[Serializable]
public class PlayerData
{
    public string Id;
    public CharacterMove CharacterData;
}