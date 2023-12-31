using System.Collections;
using System.Collections.Generic;
using FishNet;
using FishNet.Connection;
using FishNet.Object;
using UnityEngine;

public class MissileFire : NetworkBehaviour
{
    public Vector3 startPoint, endPoint;
    public float curveHeight = 2f;
    public float speed = 2f;
    private float t = 0f;
    private bool IsAnimate = false;
    public float longdistance=20f;
    bool IsOneTimeCheck = false, isDelete=false;
    float dta = 0f;
    public GameObject Sparkle;
    private bool IsServer, IsClient,IsOwner;
    public CharacterMove FireOwner { get; set; }
    private EventManager EventManager;
    private NetworkObject NetworkObjectA;
    void Start()
    {
        EventManager = FindObjectOfType<EventManager>();
        NetworkObjectA = gameObject.GetComponent<NetworkObject>();
        IsServer = NetworkObjectA.IsServer;
        IsClient = NetworkObjectA.IsClient;
        IsOwner = NetworkObjectA.IsOwner;
    }
    void Update()
    {
        if (!IsOneTimeCheck)
        {
            IsOneTimeCheck = true;
            startPoint = transform.position;
            endPoint = transform.position + transform.forward * longdistance;
            IsAnimate = true;
            float groundHeight = 0f; // Default ground level
            RaycastHit hit;
            if (Physics.Raycast(endPoint + Vector3.up * 100f,
                Vector3.down, out hit, Mathf.Infinity))
            {
                endPoint = hit.point;
            }
            else
            {
                // If no ground is hit, set a default ground height
                endPoint.y = groundHeight;
            }
            t = 0f;
        }
        if(IsAnimate)
        {
            // Increment the parameter 't' based on speed
            t += speed * Time.deltaTime;

            // Ensure 't' stays in the [0, 1] range
            t = Mathf.Clamp01(t);

            // Move the transform along the curved path
            MoveAlongCurvedPath();
            if (t >= 1f)
            {
                t = 0f;
                IsAnimate = false;
               var particle = Instantiate(Sparkle, transform.position, transform.rotation);
                particle.AddComponent<DestroyObject>();
                isDelete = true;
                dta = Time.unscaledTime;
            }
        }
      
        if (isDelete)
        {
            if((Time.unscaledTime-dta)>1f)
            {
                if(InstanceFinder.ServerManager.NetworkManager.IsServer)
                   InstanceFinder.ServerManager.Despawn(gameObject);
             //   Destroy(gameObject);
            }
        }
    }
    private void OnDestroy()
    {
        Destroy(gameObject);
    }
    bool IsReturn = false;
    void OnCollisionEnter(Collision collision)
    {
        // This method is called when a collision occurs
        if(collision.transform.name.ToLower().Contains("player") )
        {
            IsAnimate = false;
            var particle = Instantiate(Sparkle, transform.position, transform.rotation);
            particle.AddComponent<DestroyObject>();
            isDelete = true;
            dta = Time.unscaledTime;
            // Now get object id for both player
            if (IsServer && !IsReturn)
            {
                IsReturn = true;
                NetworkObject to = collision.gameObject.GetComponent<NetworkObject>();
                NetworkObject from = FireOwner.gameObject.GetComponent<NetworkObject>();
                PlayerAnimator toanimator = collision.gameObject.GetComponent<PlayerAnimator>();
                PlayerAnimator fromanimator = FireOwner.gameObject.GetComponent<PlayerAnimator>();
                toanimator.Health--;
                fromanimator.Score++;
                SendScoreOfPlayerToServer(null, to.ObjectId.ToString(), "health", toanimator.Health.ToString());
                SendScoreOfPlayerToServer(null, from.ObjectId.ToString(), "score", fromanimator.Score.ToString());
                if (toanimator.Health < 0)
                {
                    InstanceFinder.ServerManager.Despawn(toanimator.gameObject);
                }
            }

            //Now get object id of a hitted player
            // also fired id 
        }
    }
    void SendScoreOfPlayerToServer(NetworkConnection connection,string playerid,string scorehealth,string addon)
    {
        EventManager.EventOperation(connection, playerid, scorehealth, addon);
    }
    void MoveAlongCurvedPath()
    {
        // Use quadratic Bezier curve equation to create a curved path
        Vector3 p0 = startPoint;
        Vector3 p2 = endPoint;
        Vector3 p1 = (startPoint + endPoint) / 2f + Vector3.up * curveHeight;

        // Interpolate along the curve
        transform.position = BezierCurve(p0, p1, p2, t);
    }

    // Quadratic Bezier curve equation
    Vector3 BezierCurve(Vector3 p0, Vector3 p1, Vector3 p2, float t)
    {
        float u = 1f - t;
        float tt = t * t;
        float uu = u * u;
        float uuu = uu * u;
        float ttt = tt * t;

        Vector3 p = uuu * p0; // (1-t)^3 * p0
        p += 3f * uu * t * p1; // 3(1-t)^2 * t * p1
        p += 3f * u * tt * p2; // 3(1-t) * t^2 * p2
        p += ttt * p2; // t^3 * p2

        return p;
    }
}
