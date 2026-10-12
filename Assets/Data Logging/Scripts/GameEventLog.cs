using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class GameEventLog : MonoBehaviour
{
    private static List<LogItem> items = new List<LogItem>();
    private struct LogItem
    {
        public string key;
        public float time;
        public string[] values;
    }

    /// <summary>
    /// Clear the current event log.
    /// </summary>
    public static void Clear ()
    {
        items.Clear();
    }

    /// <summary>
    /// Log an item to the event log with the given key and parameter values.
    /// </summary>
    /// <param name="key">The unique key for the event.</param>
    /// <param name="values">Any additional parameters for the event.</param>
    public static void Log(string key, params string[] values)
    {
        LogItem item;
        item.key = key;
        item.time = Time.time;
        item.values = values;
        items.Add(item);
    }

    /// <summary>
    /// Log an item to the event log with the given key and parameter values. Only use this signature if you need
    /// to specify a custom timestamp, otherwise use Log(string key, param string[] values) instead.
    /// </summary>
    /// <param name="key">The unique key for the event.</param>
    /// <param name="time">The unique timestamp for the event.</param>
    /// <param name="values">Any additional parameters for the event.</param>
    public static void Log(string key, float time, params string[] values)
    {
        LogItem item;
        item.key = key;
        item.time = time;
        item.values = values;
        items.Add(item);
    }

    /// <summary>
    /// Convers the current log to a CSV compatible string.
    /// </summary>
    /// <returns>Returns the complete formatted CSV string.</returns>
    public static string ToCSVString()
    {
        StringBuilder logContents = new StringBuilder();
        foreach (LogItem item in items)
        {

            logContents.Append($"{item.time},{item.key}");
            foreach (string value in item.values)
            {
                logContents.Append($",{FormatCsvItem(value)}");
            }
            logContents.Append('\n');
        }
        return logContents.ToString();
    }

    /// <summary>
    /// Returns a CSV file as a byte array.
    /// </summary>
    /// <returns>The array of bytes representing the CSV file.</returns>
    public static byte[] ToCSVBytes()
    {
        return Encoding.UTF8.GetBytes(ToCSVString());
    }

    /// <summary>
    /// Fully formats an item for the CSV file, ensuring that the contents are properly escaped.
    /// </summary>
    /// <param name="item">The text that needs to be formatted.</param>
    /// <returns>A formatted string.</returns>
    private static string FormatCsvItem(string item)
    {
        if (item == null)
            item = string.Empty;

        // Escape quotes by doubling them
        item = item.Replace("\"", "\"\"");

        // Always wrap the result in quotes
        return $"\"{item}\"";
    }
}
