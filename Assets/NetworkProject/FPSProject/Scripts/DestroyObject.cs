using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyObject : MonoBehaviour
{
    float deathtime = 1f;
    float spawntime = 0f;
    // Start is called before the first frame update
    void Start()
    {
        spawntime = Time.unscaledTime;
    }

    // Update is called once per frame
    void Update()
    {
        if((Time.unscaledTime- spawntime)> deathtime)
        {
            Destroy(gameObject);
        }
    }
}
