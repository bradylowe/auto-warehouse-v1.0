using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextController : MonoBehaviour
{
    // Variables for animation
    private float riseRate;
    private float shrinkRate;
    private float destroyTime = 3f;

    // Start is called before the first frame update
    void Start()
    {
        // Destroy object after a short time
        Destroy(gameObject, destroyTime);
    }

    // LateUpdate is called after Update()
    void LateUpdate()
    {

    }
}
