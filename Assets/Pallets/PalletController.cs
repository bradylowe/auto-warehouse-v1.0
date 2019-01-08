using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PalletController : MonoBehaviour
{
    // This pallet index (from filename)
    private int palletIndex;

    // Reference to destination box
    private GameObject destination;
    public GameObject destBoxPrefab;

    // Called only once, initialize everything here
    private void Start()
    {
        palletIndex = GetThisObjectIndex();
    }
    
    // Called when there is a collision (interaction), do lot's of logic here
    private void OnCollisionEnter(Collision collision)
    {
        // Check for collision with ground plane (fell off world)
        if (collision.collider.tag == "OffWorld")
        {
            // Respawn
            RespawnPallet();
        }
    }

    // Respawn this pallet
    private void RespawnPallet()
    {
        // Get reference to spawn zone
        GameObject spawnZone = GameObject.Find("SpawnZone");
        // Get center
        float x0 = spawnZone.transform.position.x;
        float y0 = spawnZone.transform.position.y;
        float z0 = spawnZone.transform.position.z;
        // Get width 
        float dx = spawnZone.transform.localScale.x / 2f;
        float dy = spawnZone.transform.localScale.y / 2f;
        float dz = spawnZone.transform.localScale.z / 2f;
        // Get random location in spawn zone
        float x = x0 + Random.Range(-dx, dx);
        float y = y0 + Random.Range(-dy, dy);
        float z = z0 + Random.Range(-dz, dz);
        // Set the location to that location
        transform.position = new Vector3(x, y, z);
    }

    // Respawn this destination box
    public GameObject SpawnDestinationBox()
    {
        destination = Instantiate(destBoxPrefab, Vector3.zero, Quaternion.identity);
        SetDestBoxPositionByIndex();
        destination.transform.rotation = Quaternion.identity;
        return destination;
    }

    // Find partner in pallet/destination pair
    private int GetThisObjectIndex()
    {
        // Extract index from auto-generated part name
        int start, end, n;
        start = name.IndexOf("(");
        // If there is no (, just return 0
        if (start <= 0) return 0;
        // Get the number from in the parantheses
        end = name.IndexOf(")");
        if (int.TryParse(name.Substring(start + 1, end - start - 1), out n))
            return n;
        else return -1;
    }

    private void SetDestBoxPositionByIndex()
    {
        GameObject[] gos = GameObject.FindGameObjectsWithTag("Shelf");
        // Shelves can hold 15 pallets
        int shelfIndex = palletIndex / 5;
        int slotIndex = palletIndex % 5;
        GameObject curShelf = gos[shelfIndex];

        float width = curShelf.transform.localScale.z / 5f;
        Vector3 offset = new Vector3(0f, 0.55f, -2f + slotIndex * width);
        destination.transform.position = curShelf.transform.position + offset;
    }

    // Allow others to grab the dest box reference
    public GameObject GetDestinationBox()
    {
        return destination;
    }
}
