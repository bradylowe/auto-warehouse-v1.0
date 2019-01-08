using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreTextController : MonoBehaviour
{
    // Variables for animation
    private float riseRate;
    private float shrinkRate;

    // Start is called before the first frame update
    void Start()
    {
        Destroy(this.gameObject, 3f);
    }

    // LateUpdate is called after Update()
    void LateUpdate()
    {
        
    }
}
