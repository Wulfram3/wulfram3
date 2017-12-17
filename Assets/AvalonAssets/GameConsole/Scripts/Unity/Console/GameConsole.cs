using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AvalonAssets.Console;
using UnityEngine;
using UnityEngine.UI;

// ReSharper disable CheckNamespace

namespace AvalonAssets.Unity.Console
{
    /// <summary>
    ///     A command line console implementation in Unity.
    /// </summary>
    public class GameConsole : MonoBehaviour
    {
        private IEnumerable<Button> _autoCompleteOptions;
        private CommandLineConsole _console;

        /// <summary>
        ///     Enable auto complete in console.
        /// </summary>
        [Tooltip("Enable console in game.")]
        public bool AllowAutoComplete = true;

        /// <summary>
        ///     Enable console in game.
        /// </summary>
        [Tooltip("Enable console in game.")]
        public bool AllowConsole = true;

        /// <summary>
        ///     Auto Complete of the console.
        /// </summary>
        [Tooltip("Auto Complete of the console.")]
        public GameObject AutoComplete;

        /// <summary>
        ///     Color of the command.
        /// </summary>
        [Tooltip("Color of the command.")]
        public Color CommandColor;

        /// <summary>
        ///     Prefix of the command.
        /// </summary>
        [Tooltip("Prefix of the command.")]
        public string CommandPrefix = "> ";

        /// <summary>
        ///     Console command.
        /// </summary>
        [Tooltip("Console command.")]
        public List<RegisterCommand> Commands;

        /// <summary>
        ///     Input command to console.
        /// </summary>
        [Tooltip("Input command to console.")]
        public InputField InputField;

        /// <summary>
        ///     Triggered when a non-exist command is called.
        /// </summary>
        [Tooltip("Triggered when a non-exist command is called.")]
        public ConsoleEvent NonExistCommand;

        /// <summary>
        ///     Key to open console
        /// </summary>
        [Tooltip("Key to open console.")]
        public KeyCode OpenConsoleKey = KeyCode.BackQuote;

        /// <summary>
        ///     Output text to console.
        /// </summary>
        [Tooltip("Output text to console.")]
        public Text OutputText;

        /// <summary>
        ///     Print the input command.
        /// </summary>
        [Tooltip("Print the input command.")]
        public bool PrintCommand = true;

        /// <summary>
        ///     Scroll bar of the console.
        /// </summary>
        [Tooltip("Scroll bar of the console.")]
        public Scrollbar Scrollbar;

        /// <summary>
        ///     Scroll Rect of the console.
        /// </summary>
        [Tooltip("Scroll Rect of the console.")]
        public ScrollRect ScrollRect;

        private void Awake()
        {
			InputField.gameObject.SetActive(!InputField.gameObject.activeSelf);
			Scrollbar.gameObject.SetActive(!Scrollbar.gameObject.activeSelf);
			OutputText.gameObject.SetActive(!OutputText.gameObject.activeSelf);
			ScrollRect.gameObject.SetActive(!ScrollRect.gameObject.activeSelf);
            _console = new CommandLineConsole(OutputMessage);
            if (InputField == null)
                throw new NullReferenceException("InputField cannot be null.");
            if (OutputText == null)
                throw new NullReferenceException("OutputText cannot be null.");
            if (Scrollbar == null)
                throw new NullReferenceException("Scrollbar cannot be null.");
            if (ScrollRect == null)
                throw new NullReferenceException("ScrollRect cannot be null.");

            // Register non-exist command
            _console.RegisterCommand(CommandLineConsole.CommandNotExist, NonExistCommand.Invoke);

            // Register normal command
            foreach (var command in Commands)
                _console.RegisterCommand(command.Name, command.Command.Invoke);

            InputField.onEndEdit.AddListener(OnSumbit);
            // Add Auto complete.
            // If you want more auto complete option duplicate the Button under Auto Complete.
            if (!AutoComplete) return;
            InputField.onValueChanged.AddListener(HandleAutoComplete);
            _autoCompleteOptions = AutoComplete.GetComponentsInChildren<Button>(true);

            foreach (var option in _autoCompleteOptions)
            {
                var currentOption = option;
                option.onClick.AddListener(() =>
                {
                    var text = currentOption.GetComponentInChildren<Text>(true).text;
                    InputField.text = text;
                    AutoComplete.SetActive(false);
                });
            }
        }

        private void Update()
        {
            if (!AllowConsole) return;
            if (!Input.GetKeyDown(OpenConsoleKey)) return;

            // Open and Close console
            InputField.gameObject.SetActive(!InputField.gameObject.activeSelf);
            Scrollbar.gameObject.SetActive(!Scrollbar.gameObject.activeSelf);
            OutputText.gameObject.SetActive(!OutputText.gameObject.activeSelf);
            ScrollRect.gameObject.SetActive(!ScrollRect.gameObject.activeSelf);
        }

        /// <summary>
        ///     Output a message to <see cref="OutputText" />.
        /// </summary>
        /// <param name="message">Message to be output.</param>
        public void OutputMessage(string message)
        {
            if (string.IsNullOrEmpty(message)) return;
            OutputText.text = OutputText.text.AppendLine(message);
            // Scroll to bottom
            StartCoroutine(ScrollToBottom());
        }

        /// <summary>
        ///     Get all the registered commands.
        /// </summary>
        /// <returns>Registered commands.</returns>
        public IEnumerable<string> GetCommands()
        {
            return _console.GetCommands();
        }

        private void OnSumbit(string message)
        {
            // Check if enter is pressed
            if (!Input.GetKeyDown(KeyCode.Return) || string.IsNullOrEmpty(message)) return;
            AutoComplete.SetActive(false);
            if (PrintCommand)
            {
                var outputMessage = CommandPrefix + message;
                // Add color and output message
                OutputMessage(outputMessage.AddColor(CommandColor));
                InputField.text = "";
            }
            _console.Read(message); // Prase command
        }

        private IEnumerator ScrollToBottom()
        {
            yield return new WaitForSeconds(0);
            ScrollRect.verticalNormalizedPosition = 0;
        }

        private void HandleAutoComplete(string message)
        {
            if(string.IsNullOrEmpty(message)) return;
            AutoComplete.SetActive(true);
            var commands = GetCommands().Where(command => command.StartsWith(message)).Take(_autoCompleteOptions.Count()).ToList();
            foreach (var options in _autoCompleteOptions)
            {
                options.gameObject.SetActive(false);
            }
            foreach (var command in commands)
            {
                foreach (var option in _autoCompleteOptions.Where(option => !option.gameObject.activeSelf))
                {
                    option.GetComponentInChildren<Text>().text = command;
                    option.gameObject.SetActive(true);
                    break;
                }
            }
        }
    }
}