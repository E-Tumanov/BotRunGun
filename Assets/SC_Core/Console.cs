using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// A console to display Unity's debug logs in-game.
/// </summary>

public class Console : MonoBehaviour
{
    bool isCreated = false;

    struct Log
    {
        public string message;
        public string stackTrace;
        public LogType type;
    }

    // The hotkey to show and hide the console window.
    public KeyCode toggleKey = KeyCode.BackQuote;

    List<Log> logs = new List<Log>();
    Vector2 scrollPosition;
    public bool show;
    bool collapse;

    // Visual elements:

    static readonly Dictionary<LogType, Color> logTypeColors = new Dictionary<LogType, Color>()
	{
		{ LogType.Assert, Color.white },
		{ LogType.Error, Color.red },
		{ LogType.Exception, Color.red },
		{ LogType.Log, Color.white },
		{ LogType.Warning, Color.yellow },
	};

    const int margin = 20;

    Rect windowRect = new Rect(margin, margin, Screen.width - (margin * 2), Screen.height - (margin * 2));
    Rect titleBarRect = new Rect(0, 0, 10000, 20);
    GUIContent closeLabel = new GUIContent("Close", "close the console.");
    GUIContent clearLabel = new GUIContent("Clear", "Clear the contents of the console.");
    GUIContent collapseLabel = new GUIContent("Collapse", "Hide repeated messages.");

    private GUIStyle guiStyle = new GUIStyle(); //create a new variable

    private void Awake()
    {
        if (isCreated)
        {
            DestroyImmediate(this);
            return;
        }
        isCreated = true;

        DontDestroyOnLoad(gameObject);
        Application.logMessageReceived += HandleLog;

        guiStyle.fontSize = 14; //change the font size
        guiStyle.richText = true;

        guiStyle.wordWrap = false;
        guiStyle.stretchHeight = false;
        guiStyle.stretchWidth = false;
    }


    /// <summary>
    /// Records a log from the log callback.
    /// </summary>
    /// <param name="message">Message.</param>
    /// <param name="stackTrace">Trace of where the message came from.</param>
    /// <param name="type">Type of message (error, exception, warning, assert).</param>
    void HandleLog(string message, string stackTrace, LogType type)
    {
        logs.Add(new Log()
        {
            message = message,
            stackTrace = stackTrace,
            type = type,
        });
    }

    void OnDisable()
    {
        Application.logMessageReceived -= HandleLog;
    }


    public static bool __visible = false;

    void Update()
    {
        if (Input.GetKeyDown(toggleKey))
        {
            show = !show;
        }
        show = true;
    }

    void OnGUI()
    {
        if (!show && !__visible)
        {
            return;
        }
        //GUI.skin = skin;
        windowRect = GUILayout.Window(123456, windowRect, ConsoleWindow, "Console");
    }

    //public GUISkin skin;

    /// <summary>
    /// A window that displayss the recorded logs.
    /// </summary>
    /// <param name="windowID">Window ID.</param>
    void ConsoleWindow(int windowID)
    {
        scrollPosition = GUILayout.BeginScrollView(scrollPosition, guiStyle);

        // Iterate through the recorded logs.
        for (int i = 0; i < logs.Count; i++)
        {
            var log = logs[logs.Count - i - 1];

            // Combine identical messages if collapse option is chosen.
            if (collapse)
            {
                var messageSameAsPrevious = i > 0 && log.message == logs[i - 1].message;

                if (messageSameAsPrevious)
                {
                    continue;
                }
            }

            string str = "<color=";
            switch (log.type)
            {
                case LogType.Error:
                    str += "red";
                    break;
                case LogType.Assert:
                    str += "orange";
                    break;
                case LogType.Warning:
                    str += "yellow";
                    break;
                case LogType.Log:
                    str += "white";
                    break;
                case LogType.Exception:
                    str += "orange";
                    break;
                default:
                    break;
            }
            str += ">" + log.message + "</color>";
            //GUI.color = logTypeColors[log.type];
            guiStyle.fontSize = 12 + 4;
            GUILayout.Label(str, guiStyle);

            guiStyle.fontSize = 14 + 4;
            switch (log.type)
            {
                case LogType.Error:
                case LogType.Assert:
                case LogType.Exception:
                    GUILayout.Label(log.stackTrace, guiStyle);
                    break;
                default:
                    break;
            }

        }

        GUILayout.EndScrollView();

        GUI.contentColor = Color.white;

        GUILayout.BeginHorizontal();
        if (GUILayout.Button(closeLabel))
        {
            __visible = false;
        }
        if (GUILayout.Button(clearLabel))
        {
            logs.Clear();
        }

        collapse = GUILayout.Toggle(collapse, collapseLabel, GUILayout.ExpandWidth(false));

        GUILayout.EndHorizontal();

        // Allow the window to be dragged by its title bar.
        GUI.DragWindow(titleBarRect);
    }

}

