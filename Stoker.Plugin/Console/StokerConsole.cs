using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Collections;
using BepInEx.Logging;
using Stoker.Base.Interfaces;

namespace Stoker.Plugin.Console
{
    public class LogListener : ILogListener
    {
        private readonly StokerConsole console;

        public LogListener(StokerConsole console)
        {
            this.console = console;
        }

        public void LogEvent(object sender, LogEventArgs eventArgs)
        {
            console.AddLog($"{eventArgs.Data}", eventArgs.Level switch
            {
                LogLevel.Info => LogType.Info,
                LogLevel.Warning => LogType.Warning,
                LogLevel.Error => LogType.Error,
                _ => LogType.Info,
            });
        }

        public void Dispose()
        {
        }
    }

    public enum LogType
    {
        Info,
        Warning,
        Error,
    }

    public class StokerConsole : MonoBehaviour
    {
        private bool isVisible;
        private string inputText = "";
        private Vector2 scrollPosition;
        private readonly List<(string, LogType)> logHistory = [];
        private readonly List<string> commandHistory = [];
        private int historyIndex = -1;
        private const int MAX_LOG_LINES = 1000;
        private const float CONSOLE_HEIGHT = 0.5f;
        private const float CONSOLE_WIDTH = 0.98f;
        // private StreamReader? outputReader;
        private readonly StringBuilder currentLine = new();
        // private Coroutine? readCoroutine;
        public ICommandExecutor? CommandExecutor { get; set; }

        private int completionIndex = 0;
        private List<string> completions = [];
        private string tabCompletionText = "";
        private string lastInputText = "";

        // Rect fields for UI elements
        private Rect consoleRect;
        private Rect logRect;
        private Rect inputRect;
        private Rect scrollViewRect;
        private Rect tabCompletionRect;
        private float width;
        private Vector2 lastScreenSize;

        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
            StartReadingOutput();
            UpdateRects();
        }

        private void UpdateRects()
        {
            float height = Screen.height * CONSOLE_HEIGHT;
            width = Screen.width * CONSOLE_WIDTH;
            float fullWidth = Screen.width;
            float offset = (fullWidth - width) / 2;
            float offsetY = Screen.height * 0.01f;

            consoleRect = new Rect(offset, offsetY, width, height);
            logRect = new Rect(5 + consoleRect.x, 5 + consoleRect.y, consoleRect.width - 10, consoleRect.height - 30);
            inputRect = new Rect(5 + consoleRect.x, consoleRect.height - 25 + consoleRect.y, consoleRect.width - 10, 20);
            tabCompletionRect = new Rect(8 + consoleRect.x, inputRect.y, inputRect.width, inputRect.height);
        }

        private void OnDestroy()
        {
            // if (readCoroutine != null)
            // {
            //     StopCoroutine(readCoroutine);
            // }
            // outputReader?.Dispose();
        }

        private void StartReadingOutput()
        {
            var listener = new LogListener(this);
            BepInEx.Logging.Logger.Listeners.Add(listener);

            // var outputStream = System.Console.OpenStandardOutput();
            // outputReader = new StreamReader(outputStream, Encoding.UTF8, true, -1, true);
            // readCoroutine = StartCoroutine(ReadOutputCoroutine());
        }

        // private IEnumerator ReadOutputCoroutine()
        // {
        //     if (outputReader == null)
        //         yield break;

        //     // @TODO: Use a circular buffer to do more than per character reads. 

        //     var buffer = new StringBuilder();
        //     while (true)
        //     {
        //         while (outputReader.Peek() > -1)
        //         {
        //             char c = (char)outputReader.Read();
        //             if (c == '\n')
        //             {
        //                 var line = buffer.ToString();
        //                 AddLog(line);
        //                 buffer.Clear();
        //             }
        //             else
        //             {
        //                 buffer.Append(c);
        //             }
        //         }
        //         yield return new WaitForSeconds(0.1f); // poll less aggressively
        //     }
        // }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.BackQuote))
                isVisible = !isVisible;

            // Update rects if screen size changes
            if (Screen.width != lastScreenSize.x || Screen.height != lastScreenSize.y)
            {
                UpdateRects();
                lastScreenSize = new Vector2(Screen.width, Screen.height);
            }
        }

        private void OnGUI()
        {
            if (!isVisible) return;

            // Handle keyboard input
            var e = Event.current;
            if (e.type == EventType.KeyDown && GUI.GetNameOfFocusedControl() == "ConsoleInput")
            {
                switch (e.keyCode)
                {
                    case KeyCode.UpArrow:
                        NavigateHistory(1);
                        break;
                    case KeyCode.DownArrow:
                        NavigateHistory(-1);
                        break;
                    case KeyCode.Return:
                        if (!string.IsNullOrWhiteSpace(inputText))
                        {
                            commandHistory.Add(inputText);
                            historyIndex = -1;
                            ProcessCommand(inputText);
                            inputText = "";
                            tabCompletionText = "";
                            completions.Clear();
                        }
                        break;
                    case KeyCode.Tab:
                        if (completions.Count > 0)
                        {
                            if (e.shift) // Shift+Tab cycles backwards
                            {
                                completionIndex--;
                                if (completionIndex < 0) completionIndex = completions.Count - 1;
                            }
                            else
                            {
                                completionIndex++;
                                if (completionIndex >= completions.Count) completionIndex = 0;
                            }

                            // Get the base text up to the last space or the whole text if no spaces
                            var lastSpaceIndex = inputText.LastIndexOf(" ");
                            var baseText = lastSpaceIndex > -1 ? inputText[..(lastSpaceIndex + 1)] : inputText + " ";
                            tabCompletionText = baseText + completions[completionIndex];
                            e.Use(); // Prevent the tab from changing focus
                        }
                        break;
                    case KeyCode.RightArrow:
                        if (e.shift && !string.IsNullOrEmpty(tabCompletionText))
                        {
                            inputText = tabCompletionText;
                            e.Use(); // Prevent the arrow from moving the cursor
                        }
                        break;
                }
            }

            // Draw console background
            GUI.SetNextControlName("ConsoleBackground");
            GUI.Box(consoleRect, "");

            // Draw log area
            GUI.SetNextControlName("ConsoleLogArea");
            GUI.Box(logRect, "");

            // Create a scroll view within the log area
            GUI.SetNextControlName("ConsoleScrollView");
            
            scrollViewRect = new Rect(0, 0, width - 25, logHistory.Count * 20);
            scrollPosition = GUI.BeginScrollView(logRect, scrollPosition, scrollViewRect);
            for (int i = 0; i < logHistory.Count; i++)
            {
                var (message, logType) = logHistory[i];
                GUI.color = logType switch
                {
                    LogType.Info => Color.white,
                    LogType.Warning => Color.yellow,
                    LogType.Error => Color.red,
                    _ => Color.white,
                };
                GUI.Label(new Rect(5, i * 20, logRect.width - 10, 20), message);
            }
            GUI.color = Color.white;
            GUI.EndScrollView();

            // Draw input field
            // Draw tab completion text in the background with semi-transparent gray
            if (!string.IsNullOrEmpty(tabCompletionText))
            {
                GUI.SetNextControlName("ConsoleTabCompletion");
                GUI.color = new Color(1f, 0.8f, 0.8f, 0.8f);
                GUI.Label(tabCompletionRect, tabCompletionText);
                GUI.color = Color.white;
            }

            GUI.SetNextControlName("ConsoleInput");
            var newInputText = GUI.TextField(inputRect, inputText);
            
            // Update completions when input changes
            if (newInputText != lastInputText)
            {
                inputText = newInputText;
                lastInputText = newInputText;
                
                // Reset tab completion state when input changes
                if (!string.IsNullOrEmpty(inputText))
                {
                    var newCompletions = CommandExecutor?.GetCompletions(inputText) ?? [];
                    completions = [.. newCompletions];
                    completionIndex = 0;
                    
                    // Only show tab completion if we have completions and aren't actively cycling
                    if (completions.Count > 0)
                    {
                        var lastSpaceIndex = inputText.LastIndexOf(" ");
                        var baseText = lastSpaceIndex > -1 ? inputText[..(lastSpaceIndex + 1)] : inputText + " ";
                        bool found = false;
                        foreach (var completion in completions)
                        {
                            if (completion.StartsWith(baseText))
                            {
                                tabCompletionText = completion;
                                found = true;
                                break;
                            }
                        }
                        if (!found)
                        {
                            tabCompletionText = baseText + completions[0];
                        }
                    }
                }
                else
                {
                    tabCompletionText = "";
                    completions.Clear();
                }
            }
        }

        private void NavigateHistory(int direction)
        {
            if (commandHistory.Count == 0) return;

            historyIndex = Mathf.Clamp(historyIndex + direction, -1, commandHistory.Count - 1);
            inputText = historyIndex == -1 ? "" : commandHistory[commandHistory.Count - 1 - historyIndex];
        }

        private void ProcessCommand(string command)
        {
            CommandExecutor?.ExecuteAsync(command);
        }

        public void AddLog(string message, LogType logType)
        {
            var messagesMultiLine = message.Split(["\n", "\r\n"], StringSplitOptions.None);
            foreach (var msg in messagesMultiLine)
            {
                logHistory.Add((msg, logType));
            }
            if (logHistory.Count > MAX_LOG_LINES)
                logHistory.RemoveRange(0, logHistory.Count - MAX_LOG_LINES);
            scrollPosition.y = float.MaxValue;
        }
    }
}