using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DriveForklift : MonoBehaviour
{
    // Forklift variables and objects
    [SerializeField]
    private bool humanControl = true;
    private int forkliftIndex;
    private Rigidbody rb;
    private UnityEngine.AI.NavMeshAgent agent;
    public GameObject forks;
    public GameObject drivePoint;
    public GameObject frontWheel1;
    public GameObject frontWheel2;
    public GameObject driveWheel;
    private Vector3 initialForkPos;

    // Variables controlling qualifications for scoring
    private float scoreBoxBounds = 1.45f;

    private GameObject pallet;
    private GameObject destination;
    // Flag for successful dropoff
    private bool success;

    // Universal constants
    private float moveSpeed = 200f;
    private float rotateSpeed = 80f;
    private float forkSpeed = 1f;
    private float maxLiftingHeight = 3.5f;

    // Movement variables
    private bool movingForward;
    private bool movingBackward;
    private bool turningLeft;
    private bool turningRight;
    // Load manipulation variables
    private bool raisingForks;
    private bool loweringForks;
    private bool droppingPallet;

    // Runs once
    void Start()
    {
        // We haven't failed a delivery yet
        success = true;
        forkliftIndex = GetThisObjectIndex();
        initialForkPos = forks.transform.position;
        rb = GetComponent<Rigidbody>();
        agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        agent.enabled = !humanControl;
            
    }

    // Update is called once per frame
    private void Update()
    {
        // Take input from human
        if (humanControl)
        {
            // Grab forward/backward movements
            if (Input.GetKey("w"))
            {
                movingForward = true;
            }
            else if (Input.GetKey("s"))
                movingBackward = true;
            // Grab left/right movements
            if (Input.GetKey("a"))
                turningLeft = true;
            else if (Input.GetKey("d"))
                turningRight = true;
            // Grab load manipulation movements
            if (Input.GetKey("u"))
                raisingForks = true;
            else if (Input.GetKey("j"))
                loweringForks = true;
            // Find out if we are dropping the load
            if (Input.GetKeyDown("space") && pallet != null)
                droppingPallet = true;
        }
        else TakeNextStep();
    }

    // Do physics in here
    private void FixedUpdate()
    {
        /*
        // Move forward/backward
        if (movingForward)
        {
            rb.AddForce(transform.forward * moveSpeed);
        }
        else if (movingBackward)
        {
            rb.AddForce(-transform.forward * moveSpeed);
        }

        // Move left/right
        if (turningLeft)
        {
            transform.Rotate(-Vector3.up * Time.deltaTime * rotateSpeed);
            driveWheel.transform.Rotate(Vector3.forward * Time.deltaTime * rotateSpeed);
        }
        else if (turningRight)
        {
            transform.Rotate(Vector3.up * Time.deltaTime * rotateSpeed);
        }
        */
        if (movingForward) driveWheel.GetComponent<Rigidbody>().AddTorque(Vector3.right * moveSpeed);

        // Reset flags
        movingForward = false;
        movingBackward = false;
        turningLeft = false;
        turningRight = false;

        // Update the forks and pallet
        if (raisingForks) RaiseForks();
        else if (loweringForks) LowerForks();
        if (pallet != null)
        {
            if (droppingPallet) DropPallet();
            else MovePallet();
        }

        // Reset flags
        raisingForks = false;
        loweringForks = false;
        droppingPallet = false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.tag == "OffWorld")
        {
            Scene scene = SceneManager.GetActiveScene();
            SceneManager.LoadScene(scene.name);
        }
        else if (collision.collider.tag == "Shelf")
        {
            //FindObjectOfType<ScoreController>().AddCollision(forkliftIndex);
        }
        else if (collision.collider.tag == "Forklift")
        {
            //FindObjectOfType<ScoreController>().AddCollision(forkliftIndex);
        }
        else if (collision.collider.tag == "Pallet")
        {
            // If we don't have a pallet yet, pick this one up
            if (pallet == null) PickupPallet(collision.collider.gameObject);
            else
            {
                //FindObjectOfType<ScoreController>().AddCollision(forkliftIndex);
            }
        }
    }

    // This algorithm heuristically makes decisions for the agent to
    // pick up pallets and deliver them to their destinations.
    private void TakeNextStep()
    {
        // Check if we are in transit via nav mesh agent
        if (agent.pathPending || agent.hasPath)
        {
            if (agent.remainingDistance > agent.stoppingDistance)
                return;
        }

        // If we don't have a pallet, go get one
        if (pallet == null)
        {
            // Find the closest pallet and go get it
            GameObject closest = FindClosestPallet();
            agent.SetDestination(closest.transform.position);
            return;
        }
        else
        {
            // Grab height of destination zone
            float h = destination.transform.position.y;
            // Set destination as just in front of the shelf on floor
            float x = destination.transform.position.x;
            float y = 1f;
            float z = destination.transform.position.z;
            if (x > 0f) x -= 1.1f;
            else x += 1.1f;
            // If we aren't there yet, go there
            Vector3 destPosition = new Vector3(x, y, z);
            Vector3 distance = pallet.transform.position - destPosition;
            if (distance.sqrMagnitude > 3f && pallet.transform.position.y < 1.5f)
            {
                agent.SetDestination(destPosition);
                return;
            }

            // Now, make sure we are facing the box
            Vector3 orientation = transform.forward;
            if (Mathf.Abs(orientation.x) < 10f * Mathf.Abs(orientation.z))
            {
                // Find out if turning right or left will bring the pallet 
                // closer to the destinationZone
                float delta = 0.1f;
                Vector3 curDist = pallet.transform.position - destPosition;
                Vector3 right = transform.right * delta;
                Vector3 rightDist = curDist + right;
                Vector3 leftDist = curDist - right;

                // Turn 
                if (rightDist.sqrMagnitude < leftDist.sqrMagnitude)
                    turningRight = true;
                else turningLeft = true;
                return;
            }

            // Finally, we are ready to lift the load and then drop it
            if (PalletIsTooLow())
            {
                raisingForks = true;
                return;
            }
            // If the load is too high, lower it
            else if (PalletIsTooHigh())
            {
                loweringForks = true;
                return;
            }
            // If the load is good, drop it
            else
            {
                droppingPallet = true;
                return;
            }
        }
    }

    // Return position of nearest pallet
    private GameObject FindClosestPallet()
    {
        GameObject[] gos;
        gos = GameObject.FindGameObjectsWithTag("Pallet");
        GameObject closest = null;
        float distance = Mathf.Infinity;
        foreach (GameObject go in gos)
        {
            Vector3 diff = go.transform.position - transform.position;
            float curDistance = diff.sqrMagnitude;
            if (curDistance < distance)
            {
                closest = go;
                distance = curDistance;
            }
        }
        return closest;
    }

    // Run this function to pick up a pallet
    private void PickupPallet(GameObject newPallet)
    {
        // Set pallet reference
        pallet = newPallet;
        // Turn off physics, control motion through pallet script
        pallet.GetComponent<Rigidbody>().isKinematic = true;
        pallet.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePositionY;
        // Make pallet un-pickupable
        pallet.tag = "Load";
        // Grab destinationZone reference
        destination = pallet.GetComponent<PalletController>().SpawnDestinationBox();
        // Make destination box stand out for human
        if (humanControl)
            destination.GetComponent<Renderer>().material.SetFloat("_Metallic", 0.5f);
        // Update scoreboard if the last drop was successful
        //if (success)
            //FindObjectOfType<ScoreController>().AddPickupTime(forkliftIndex);
    }

    // Run this to drop a load, return true upon success
    private void DropPallet()
    {
        // Turn physics back on
        pallet.GetComponent<Rigidbody>().isKinematic = false;
        pallet.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
        // Make the pallet pickupable again
        pallet.tag = "Pallet";

        // Find out if successful delivery or failure
        Bounds bounds = new Bounds(pallet.transform.position, new Vector3(scoreBoxBounds, scoreBoxBounds, scoreBoxBounds));
        if (bounds.Contains(destination.transform.position))
        {
            success = true;
            // Update scoreboard and respawn
            //FindObjectOfType<ScoreController>().AddDropoffTime(forkliftIndex);
            //pallet.SetActive(false);
        }
        else
        {
            success = false;
        }
        Destroy(destination);
        pallet = null;
        destination = null;
    }

    // Move the load according to the forklift position
    private void MovePallet()
    {
        // Update the position of the pallet to be in front of the lift
        //Vector3 pos = transform.position + forkLength * transform.forward;
        // Keep its height the same
        //pos.y = pallet.transform.position.y;
        Vector3 offset = new Vector3(0f, 0.1f, 0.05f);
        pallet.transform.position = forks.transform.position + offset;
        pallet.transform.rotation = transform.rotation;
        // Rotate the current destination box
        if (humanControl)
            destination.transform.RotateAround(destination.transform.position, Vector3.up, 30f * Time.deltaTime);
    }

    // Get location to display text bubble
    private Vector3 GetTextPosition()
    {
        // Create the point text at the location above the destination box
        // plus the defined offset
        Vector3 center = transform.position;
        float x = transform.forward.x * 0.5f;
        float y = 1f;
        float z = transform.forward.z * 0.5f;
        Vector3 offset = new Vector3(x, y, z);
        return center + offset;
    }

    private void RaiseForks()
    {
        if (forks.transform.position.y > initialForkPos.y + maxLiftingHeight) return;
        forks.transform.Translate(new Vector3(0f, forkSpeed * Time.deltaTime, 0f));
    }

    private void LowerForks()
    {
        if (forks.transform.position.y <= initialForkPos.y) return;
        forks.transform.Translate(new Vector3(0f, -forkSpeed * Time.deltaTime, 0f));
    }

    // Find partner in pallet/destination pair
    private int GetThisObjectIndex()
    {
        // Extract index from auto-generated part name
        int start, end, n;
        start = name.IndexOf("(");
        // If there is no (, just return 0
        if (start <= 0) return 0;
        end = name.IndexOf(")");
        // Get pair index
        if (int.TryParse(name.Substring(start + 1, end - start - 1), out n))
            return n;
        else return -1;
    }

    private bool PalletIsTooLow()
    {
        if (pallet.transform.position.y < destination.transform.position.y + 0.05f)
            return true;
        else return false;
    }

    private bool PalletIsTooHigh()
    {
        if (pallet.transform.position.y > destination.transform.position.y + 0.15f)
            return true;
        else return false;
    }

}
