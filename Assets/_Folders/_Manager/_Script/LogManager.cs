using UnityEngine;

public static class LogManager
{
    // Log function that takes a message, an optional context object, and an optional color
    public static void Log(object message, Object context = null, string color = "white")
    {
        // Convert the message to a string and enclose it in color tags
        string coloredMessage = $"<color={color}>{message}</color>";

        // Append the calling script's name to the message
        string finalMessage = $"{coloredMessage} - Called from {context.GetType()}";

        // Output the final message to the Unity console
        Debug.Log(finalMessage, context);
    }
}