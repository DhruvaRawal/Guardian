using UnityEngine;
using TMPro;

public class RuntimeConsole : MonoBehaviour
{
    public TMP_Text consoleText;

    void OnEnable()
    {
        Application.logMessageReceived += HandleLog;
    }

    void OnDisable()
    {
        Application.logMessageReceived -= HandleLog;
    }

    void HandleLog(string logString, string stackTrace, LogType type)
    {
        // show only latest line
        consoleText.text = logString;
    }
}
