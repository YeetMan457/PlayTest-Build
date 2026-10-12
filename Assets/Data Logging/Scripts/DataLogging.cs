using UnityEngine;

public class DataLogging
{
    public static DataLogging currentLog = new DataLogging();
    public static void Reset() => currentLog = new DataLogging();

    private float startTime;

    public DataLogging()
    {
        startTime = Time.time;
    }

    // --- Metrics ---
    public int totalMerges = 0;
    
    // Actions
    public int timesFolded = 0;
    public int timesMoulded = 0;
    public int timesShaped = 0;
    public int timesSmithed = 0;

    // Progression
    public int wasteCollected = 0;
    public int finalFormsDiscovered = 0; // Target: 24

    public string BuildLogString()
    {
        float totalTimePlayed = Time.time - startTime;

        return
            $"```What It Was? Run Log\n" +
            $"User ID: {SystemInfo.deviceUniqueIdentifier.Substring(0, 4)}\n" +
            $"Time Played: {totalTimePlayed:F1}s\n" +
            $"-------------------------------\n" +
            $"Total Merges: {totalMerges}\n" +
            $"Times Folded: {timesFolded}\n" +
            $"Times Moulded: {timesMoulded}\n" +
            $"Times Shaped: {timesShaped}\n" +
            $"Times Smithed: {timesSmithed}\n" +
            $"-------------------------------\n" +
            $"Waste Collected: {wasteCollected}\n" +
            $"Final Forms Discovered: {finalFormsDiscovered} / 24\n" +
            $"```";
    }
}