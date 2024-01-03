using System;
using System.Collections;
using System.Collections.Generic;
using FishNet;
using FishNet.Component.Animating;
using FishNet.Component.Transforming;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using UnityEngine;

public class PlayerAnimator : NetworkBehaviour
{
    public NetworkAnimator Animator;
    public TextMesh ScoreText;
    public readonly SyncVar<int> Score = new SyncVar<int>();
    public readonly SyncVar<float> Health = new SyncVar<float>(100);
    public GameObject HealthPivot;
    public Animator animator;
    public bool IsOwner;
    NetworkObject NetworkObject;
    NetworkTransform NetworkTransform;
    public bool IsServer, IsClient;
    public string walkClip = "Base Layer.walkclip";
    public string idleClip = "Base Layer.idleclip";
    public string clipname;
    private EventManager EventManager;
    public float xScale;
    // Start is called before the first frame update
    void Start()
    {
        NetworkObject = GetComponent<NetworkObject>();
        NetworkTransform = GetComponent<NetworkTransform>();
        Invoke("SetUpNetwork", 1f);
        xScale = HealthPivot.transform.localScale.x;
        Score.OnChange += ScoreChangeOperation;
        Health.OnChange += HealthChangeOperation;
        
    }
    public override void OnStartClient()
    {
        base.OnStartClient();
        ScoreText.text = Score.Value.ToString();
        float targetScale = ((float)(Health.Value) / (100)) * xScale;
        HealthPivot.transform.localScale = new Vector3(targetScale,
            HealthPivot.transform.localScale.y, HealthPivot.transform.localScale.z);
    }

    private void HealthChangeOperation(float prev, float next, bool asServer)
    {
        float targetScale = ((float)(next) / (100)) * xScale;
        HealthPivot.transform.localScale = new Vector3(targetScale,
            HealthPivot.transform.localScale.y, HealthPivot.transform.localScale.z);
    }

    private void ScoreChangeOperation(int prev, int next, bool asServer)
    {
        ScoreText.text = next.ToString();

    }

    [System.Obsolete]
    void SetUpNetwork()
    {
        IsServer = NetworkObject.IsServer;
        IsClient = NetworkObject.IsClient;
        IsOwner = NetworkObject.IsOwner;
        EventManager = FindObjectOfType<EventManager>();
    }
    Vector3 LastPosition;
    // Update is called once per frame
    void Update()
    {
        if (IsClient && IsOwner)
        {
            if (transform.position != LastPosition)
            {
                LastPosition = transform.position;
                if (!IsAnimationPlaying(walkClip))
                {
                    clipname = walkClip;
                    // Do something when the animation ends
                    animator.Play(walkClip);
                    SendClipAnimation(idleClip);
                }
            }
            else
            {
                // Check if the "idleclip" animation is at its end
                if (!IsAnimationPlaying(idleClip))
                {
                    clipname = idleClip;
                    // Do something when the animation ends
                    animator.Play(idleClip);
                    SendClipAnimation(idleClip);
                }
            }
        }
    }

    [ServerRpc]
    void SendClipAnimation(string clipname)
    {
        if (IsClient && IsOwner)
        {
            EventManager?.EventOperation(InstanceFinder.ClientManager.Connection,
                    NetworkObject.ObjectId.ToString(),
                    clipname);
        }
    }

    //[ServerRpc(RunLocally = true)]
    internal void SendJoinScore()
    {
        if (IsClient && IsOwner && IsServer)
        {
            //EventManager?.EventOperation(InstanceFinder.ClientManager.Connection,
            //        NetworkObject.ObjectId.ToString(),
            //        "score", Score.Value.ToString());
            //EventManager?.EventOperation(InstanceFinder.ClientManager.Connection,
            //       NetworkObject.ObjectId.ToString(),
            //       "health", Health.Value.ToString());
        }
    }

    bool IsAnimationPlaying(string animationName)
    {
        // Get the current state info for the specified animation layer
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);

        // Check if the specified animation is currently playing
        return stateInfo.IsName(animationName) && stateInfo.normalizedTime >= 0f && stateInfo.normalizedTime < 1f;
    }

    bool IsAnimationAtEnd(string animationName)
    {
        // Get the current state info for the specified animation layer
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);

        // Check if the specified animation has reached its end
        return stateInfo.IsName(animationName) && stateInfo.normalizedTime >= 1f;
    }

    internal void ScorePlus()
    {
        Score.Value++;
    }

    internal void HealthMinus()
    {
        Health.Value--;
        if (IsServer && Health.Value<1)
            GetComponent<NetworkObject>().GiveOwnership(InstanceFinder.ClientManager.Connection);
    }

    internal void SetHealth(string addonmessage)
    {
        Health.Value = int.Parse(addonmessage);
        int currentHealth =  int.Parse(addonmessage);
        float targetScale = ((float)(currentHealth) / (100)) * xScale;
        HealthPivot.transform.localScale = new Vector3(targetScale, HealthPivot.transform.localScale.y, HealthPivot.transform.localScale.z);
    }

    internal void SetScore(string addonmessage)
    {
        Score.Value = int.Parse(addonmessage);
        ScoreText.text = " " + addonmessage;
    }
#if UNITY_EDITOR
    //[EasyButtons.Button()]
    [ContextMenu(nameof(GetAnimatorStatesInfo))]
    void GetAnimatorStatesInfo()
    {

        if (animator != null)
        {
            var animatorController = animator.runtimeAnimatorController as UnityEditor.Animations.AnimatorController;

            string animatorInfoText = "<b>Animator:</b>" + animator.name + "\n";

            foreach (var animatorLayer in animatorController.layers)
            {
                animatorInfoText += "\n<b>Layer:</b>" + animatorLayer.name;
                foreach (var childAnimatorState in animatorLayer.stateMachine.states)
                {
                    animatorInfoText += "\n-----------------------";
                    animatorInfoText += "\n\t<b>State:</b>" + childAnimatorState.state.name;
                    animatorInfoText += "\n\t<b>State Name Hash:</b>" + childAnimatorState.state.nameHash;
                    string fullPath = animatorLayer.name + "." + childAnimatorState.state.name;
                    animatorInfoText += "\n\t<b>Full Path:</b>" + "<color=green>" + fullPath + "</color>";
                   // animatorInfoText += "\n\t<b>Full Path Hash:</b>" + animator.StringToHash(fullPath);
                }
            }

            Debug.Log(animatorInfoText);

        }
    }

    
#endif
}
