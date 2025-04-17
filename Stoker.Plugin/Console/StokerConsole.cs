using UnityEngine;
using System.Collections.Generic;

namespace Stoker.Plugin.Console
{
    public class StokerConsole : MonoBehaviour
    {
        private bool isVisible;
        private string inputText = "";
        private Vector2 scrollPosition;
        private readonly List<string> logHistory = [];
        private readonly List<string> commandHistory = [];
        private int historyIndex = -1;
        private const int MAX_LOG_LINES = 1000;
        private const float CONSOLE_HEIGHT = 0.5f;

        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
            Application.logMessageReceived += HandleLog;
        }

        private void OnDestroy() => Application.logMessageReceived -= HandleLog;

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.BackQuote))
                isVisible = !isVisible;

            if (isVisible)
            {
                if (Input.GetKeyDown(KeyCode.UpArrow))
                    NavigateHistory(1);
                else if (Input.GetKeyDown(KeyCode.DownArrow))
                    NavigateHistory(-1);
            }
        }

        private void OnGUI()
        {
            if (!isVisible) return;

            float height = Screen.height * CONSOLE_HEIGHT;
            Rect consoleRect = new(0, 0, Screen.width, height);

            // Draw console background
            GUI.Box(consoleRect, "");

            // Draw log area
            Rect logRect = new(5, 5, Screen.width - 10, height - 30);
            GUI.Box(logRect, "");
            scrollPosition = GUI.BeginScrollView(logRect, scrollPosition, new(0, 0, Screen.width - 25, logHistory.Count * 20));

            for (int i = 0; i < logHistory.Count; i++)
                GUI.Label(new(5, i * 20, Screen.width - 30, 20), logHistory[i]);

            GUI.EndScrollView();

            // Draw input field
            Rect inputRect = new(5, height - 25, Screen.width - 10, 20);
            GUI.SetNextControlName("ConsoleInput");
            inputText = GUI.TextField(inputRect, inputText);

            // Handle input
            if (Event.current.type == EventType.KeyDown && 
                Event.current.keyCode == KeyCode.Return && 
                GUI.GetNameOfFocusedControl() == "ConsoleInput" && 
                !string.IsNullOrWhiteSpace(inputText))
            {
                commandHistory.Add(inputText);
                historyIndex = -1;
                ProcessCommand(inputText);
                inputText = "";
                GUI.FocusControl("ConsoleInput");
            }
        }

        private void NavigateHistory(int direction)
        {
            if (commandHistory.Count == 0) return;

            historyIndex = Mathf.Clamp(historyIndex + direction, -1, commandHistory.Count - 1);
            inputText = historyIndex == -1 ? "" : commandHistory[commandHistory.Count - 1 - historyIndex];
            GUI.FocusControl("ConsoleInput");
        }

        private void ProcessCommand(string command) => AddLog($"Executing: {command}");

        private void HandleLog(string logString, string stackTrace, LogType type)
        {
            string color = type switch
            {
                LogType.Error or LogType.Exception => "red",
                LogType.Warning => "yellow",
                _ => "white"
            };

            AddLog($"<color={color}>{logString}</color>");
        }

        private void AddLog(string message)
        {
            logHistory.Add(message);
            if (logHistory.Count > MAX_LOG_LINES)
                logHistory.RemoveRange(0, logHistory.Count - MAX_LOG_LINES);
            scrollPosition.y = float.MaxValue;
        }
    }
} 