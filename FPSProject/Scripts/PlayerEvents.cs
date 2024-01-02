using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Player Events for register and unregister events 
/// </summary>
public static class PlayerEvents
{
    // Hit Delegate
    public delegate void HitDelegate(object data, GameObject hitter);
    // Hit Event
    public static event HitDelegate HitEvent;
    // Dictionary to store registered events for each GameObject
    // Register method
    public static void Register(this GameObject gameObject, object eventType)
    {
        HitEvent?.Invoke(eventType, gameObject);
    }
}