using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using FishNet;
using FishNet.Component.Transforming;
using FishNet.Connection;
using FishNet.Object;
using UnityEngine;

/// <summary>
/// Character Move of an object 
/// </summary>
public class CharacterMove : MonoBehaviour
{
    public List<CharacterMove> CharacterMoves;
    public GameObject Missile, spawnpoint;
    /// <summary>
    /// Move Speed of a character controller 
    /// </summary>
    public float moveSpeed = 5f;
    /// <summary>
    /// Rotation speed 
    /// </summary>
    public float rotationSpeed = 400f;
    /// <summary>
    /// Character Controller 
    /// </summary>
    private CharacterController characterController;
    private NetworkObject NetworkObject;
    private NetworkTransform NetworkTransform;
    public bool IsClient, IsOwner,IsServer;
    public GameObject missilePrefab;
    public List<string> Events;

    void Start()
    {
        // Get the CharacterController component attached to the GameObject
        characterController = GetComponent<CharacterController>();
        PlayerEvents.HitEvent += EventOpreation;
        NetworkObject = GetComponent<NetworkObject>();
        NetworkTransform = GetComponent<NetworkTransform>();
        Invoke("SetUpNetwork", 1f);
    }
    void SetUpNetwork()
    {
        IsServer = NetworkObject.IsServer;
        IsClient = NetworkObject.IsClient;
        IsOwner = NetworkObject.IsOwner;
    }
  
    
    bool _reliable = false;
    // Update is called once per frame
  
     
 void   CharacterMovesInsert()
    {
        // Find all objects of type CharacterMove in the scene
        CharacterMove[] foundMoves = GameObject.FindObjectsOfType<CharacterMove>();

        // Loop through the found objects
        foreach (CharacterMove move in foundMoves)
        {
            // Check if the object is not already in the list
            if (!CharacterMoves.Contains(move))
            {
                // Insert the object into the list
                CharacterMoves.Add(move);
            }
        }
    }

    private void EventOpreation(object data, GameObject hitter)
    {
        
        if (!Events.Contains(data.ToString()))
        {
            Events.Add(data.ToString());
         //   Destroy(hitter.GetComponent<ObjectEventType>());//.enabled = false;
        }
    }
    public List<string> Weapons;
    public GameObject ChildWeapon;
    internal void SetWeapon(string v)
    {
        Weapons ??= new List<string>();
        if (!Weapons.Contains(v))
        {
            Weapons.Add(v);
            try
            {
                Debug.Log("v " + v);
                GameObject gobj = GameObject.Find(v);
                Debug.Log("obj " + gobj.transform.name);
                ChildWeapon = gobj;
                if (ChildWeapon != null)
                {
                    var owner = ChildWeapon.GetComponent<ObjectEventType>();
                    int OwnerId = owner.OwnerId;
                    bool IsOwner = owner.IsOwner;
                    if (OwnerId < 0)
                        ChildWeapon.transform.SetParent(transform);
                }
            }catch(Exception exa)
            {
                Debug.Log("Exception 2" + exa.Message);
            }
        }
    }




    /// <summary>
    /// Update of a Character Controller
    /// </summary>
    void Update()
    {
        if (!IsOwner)
            return;
        // Get input from the keyboard
        float horizontalInput = Input.GetAxis("Horizontal");
        // Get input from the keyboard vertical 
        float verticalInput = Input.GetAxis("Vertical");
        // Calculate the movement direction based on input
        Vector3 movement = new Vector3(horizontalInput, 0f, verticalInput).normalized;
        // Rotate the character towards the movement direction
        if (movement.magnitude > 0.1f)
        {
            Quaternion toRotation = Quaternion.LookRotation(movement, Vector3.up);
            //Rotate towards of an object 
            transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation, rotationSpeed * Time.deltaTime);
        }
        // Move the character using CharacterController
        characterController.Move(movement * moveSpeed * Time.deltaTime);
        if(Input.GetKeyDown(KeyCode.Space))
        {
           if(IsOwner && IsServer)
            {
                GameObject missileobject = Instantiate(Missile,
                     spawnpoint.transform.position, spawnpoint.transform.rotation);
                missileobject.SetActive(true);
                InstanceFinder.ServerManager.Spawn(missileobject);
                missileobject.GetComponent<MissileFire>().FireOwner = this;
            }
            else
            {
                ClientMissileSpawn();
            }
        }
       
    }

    void SendMissileSpawn()
    {

        GetComponent<NetworkOwnership>().SendMessageA(InstanceFinder.ClientManager.Connection, NetworkObject.ObjectId.ToString(), "MissileSpawn");
    }
    void ClientMissileSpawn()
    {
        GetComponent<NetworkOwnership>().SendToServerWithoutOwner(InstanceFinder.ClientManager.Connection, NetworkObject.ObjectId.ToString(), "MissileSpawn");
    }
    private void OnDestroy()
    {
        PlayerEvents.HitEvent -= EventOpreation;
    }
    public void OnControllerColliderHit(ControllerColliderHit hit)
    {
        ObjectEventType eventType = hit.gameObject.GetComponent<ObjectEventType>();
        eventType?.CallEvent();
        //if (hit.transform.name.Contains("Grenade1"))
        //{
        //    if(hit.gameObject.GetComponent<BoxCollider>())
        //    {
        //        Destroy(hit.gameObject.GetComponent<BoxCollider>());
        //        hit.transform.SetParent(transform);
        //    }
        //}
        //if(hit.transform.name.Contains("MachineGun1"))
        //{
        //    if (hit.gameObject.GetComponent<BoxCollider>())
        //    {
        //        Destroy(hit.gameObject.GetComponent<BoxCollider>());
        //        hit.transform.SetParent(transform);
        //    }
        //}
        //if(hit.transform.name.Contains("MachineGun2"))
        //{
        //    if (hit.gameObject.GetComponent<BoxCollider>())
        //    {
        //        Destroy(hit.gameObject.GetComponent<BoxCollider>());
        //        hit.transform.SetParent(transform);
        //    }
        //}
        //if(hit.transform.name.Contains("Grenade2"))
        //{
        //    if (hit.gameObject.GetComponent<BoxCollider>())
        //    {
        //        Destroy(hit.gameObject.GetComponent<BoxCollider>());
        //        hit.transform.SetParent(transform);
        //    }
        //}
       if(eventType!=null)
        {
            //Client won't send their username, server will already know it.
            EventBroadcast msg = new EventBroadcast()
            {
               FromNetworkId=NetworkObject.ObjectId.ToString(),
               EventName = eventType.EventType
            };
            try {
                if(IsOwner && IsClient)
                   GetComponent<NetworkOwnership>().SendToServerWithoutOwner(InstanceFinder.ClientManager.Connection, msg.FromNetworkId, msg.EventName);
                if(IsServer)
                    GetComponent<NetworkOwnership>().SendMessageA(InstanceFinder.ClientManager.Connection, msg.FromNetworkId, msg.EventName);
            
            }
            catch (Exception exa)
            {

            }
        }
    }
    

}
