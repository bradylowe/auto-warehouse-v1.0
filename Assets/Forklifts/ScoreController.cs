using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreController : MonoBehaviour
{
    // Bool to tell whether board has changed
    private bool boardChanged;

    // Flags for showing info on screen
    private bool showStats;
    private bool showControls;

    // Make world timer for all to use
    private float timer;
    public UnityEngine.UI.Text gameTimer;
    public UnityEngine.UI.Text controlsDisplay;
    public UnityEngine.UI.Text statsDisplay;

    // Reference to forklifts and stats boxes
    public GameObject[] lifts;
    private UnityEngine.UI.Text[] statsBoxes;
    public UnityEngine.UI.Text statsBoxPrefab;

    // Stats on forklifts
    private int[] nCollisions;
    private int[] nDeliveries;
    private float[][] lastTenPickupTimes;
    private float[][] lastTenDropoffTimes;

    // Start is called before the first frame update
    void Start()
    {
        // Initialize timer
        timer = 0f;
        gameTimer.text = "Time:  ---";
        boardChanged = true;
        showStats = false;
        showControls = false;

        // Initialize stats arrays arrays
        nCollisions = new int[lifts.Length];
        nDeliveries = new int[lifts.Length];
        lastTenPickupTimes = new float[lifts.Length][];
        lastTenDropoffTimes = new float[lifts.Length][];
        statsBoxes = new UnityEngine.UI.Text[lifts.Length];

        nCollisions = new int[lifts.Length];
        nDeliveries = new int[lifts.Length];
        lastTenPickupTimes = new float[lifts.Length][];
        lastTenDropoffTimes = new float[lifts.Length][];

        // Initialize arrays for each individual forklift in scene
        for (int i = 0; i < lifts.Length; i++)
        {
            // Initialize collision count
            nCollisions[i] = 0;
            nDeliveries[i] = 0;
            // Instantiate stats boxes and initialize them (disable them too)
            Vector3 thisBox = new Vector3(0f, -70f * i);
            statsBoxes[i] = Instantiate(statsBoxPrefab, thisBox, Quaternion.identity) as UnityEngine.UI.Text;
            statsBoxes[i].transform.SetParent(transform, false);
            statsBoxes[i].fontSize = 13;
            statsBoxes[i].enabled = false;
            // Initialize arrays
            lastTenPickupTimes[i] = new float[10];
            lastTenDropoffTimes[i] = new float[10];
            // Initialize the last ten times to negative 1
            for (int j = 0; j < 10; j++)
            {
                lastTenPickupTimes[i][j] = -1f;
                lastTenDropoffTimes[i][j] = -1f;
            }
        }
    }

    // Update is run once per frame
    void Update()
    {
        // Update timer
        timer += Time.deltaTime;
        gameTimer.text = "Time:  " + timer.ToString("F2");

        // Take in user input
        if (Input.GetKeyDown("c"))
        {
            showControls = !showControls;
            if (showControls) controlsDisplay.text = GetControls();
            else controlsDisplay.text = "Press [c] for \nplayer controls";
        }
        if (Input.GetKeyDown("x"))
        {
            // Switch flag
            showStats = !showStats;
            // Set the stats key message to off when stats are on
            statsDisplay.enabled = !showStats; 
            // When we are showing stats, turn on stats boxes
            foreach (UnityEngine.UI.Text thisText in statsBoxes)
                thisText.enabled = showStats;
        }

        // Update board if user wants to see
        if (boardChanged && showStats) UpdateBoard();
    }

    // Allow everyone to get time
    public float GetTime()
    {
        return timer;
    }

    // Add entries to the stats arrays for given lift
    private void AddDelivery(int lift)
    {
        nDeliveries[lift]++;
    }
    public void AddCollision(int lift)
    {
        nCollisions[lift]++;
        boardChanged = true;
    }

    // float start and end tell us the start and end times of transit
    public void AddPickupTime(int lift)
    {
        // Shift all the times down an index
        for (int i = 9; i >= 1; i--)
        {
            lastTenPickupTimes[lift][i] = lastTenPickupTimes[lift][i - 1];
        }
        // Add the new time
        lastTenPickupTimes[lift][0] = timer;
        boardChanged = true;
    }
    public void AddDropoffTime(int lift)
    {
        AddDelivery(lift);
        // Shift all the times down an index
        for (int i = 9; i >= 1; i--)
        {
            lastTenDropoffTimes[lift][i] = lastTenDropoffTimes[lift][i - 1];
        }
        // Add the new time
        lastTenDropoffTimes[lift][0] = timer;
        boardChanged = true;
    }

    // Update the game board with the new values, recalculate all
    private void UpdateBoard()
    {
        // Turn off flag
        boardChanged = false;

        // Update the screen with first 4 forklift stats
        string outputText;
        float avg;
        for (int i = 0; i < 5; i++)
        {
            // Print deliveries and collisions
            outputText = "Deliveries:  " + nDeliveries[i].ToString();
            outputText += "\nCollisions:  " + nCollisions[i].ToString();
            // Calculate average and print appropriately
            avg = GetAveragePickupToPickup(i);
            if (avg > 0f) outputText += "\nAverage time:  " + avg.ToString("F2");
            else outputText += "\nAverage time:  ---";
            // Update appropriate stats box
            statsBoxes[i].text = outputText;
        }
    }

    // Calculate average of a given array
    private float GetAveragePickupToPickup(int lift)
    {
        // Get the length of the array
        int len = lastTenPickupTimes[lift].Length;
        float avg = 0.0f;
        int count = 0;

        // Loop through the whole array starting at index 1
        for ( var i = 1; i < len; i++)
        {
            // Only do the good ones
            if (lastTenPickupTimes[lift][i] > 0f)
            {
                // Find the difference between pickup times
                avg += lastTenPickupTimes[lift][i - 1] - lastTenPickupTimes[lift][i];
                count++;
            }
        }
        // Return the average
        if (count == 0) return -1f;
        else return avg / count;
    }

    private string GetControls()
    {
        string ret;
        ret = "Forklift Controls \n";
        ret += "accelerate -- w \n";
        ret += "acc. backward -- s \n";
        ret += "left -- a    right -- d \n";
        ret += "lift/lower pallet -- u / j \n";
        ret += "drop pallet -- (space) \n";
        ret += "\n";
        ret += "Camera Controls \n";
        ret += "pan left / right -- [ / ] \n";
        ret += "zoom in/out -- + / - \n";
        ret += "toggle sky view -- \\ ";
        return ret;
    }
}
