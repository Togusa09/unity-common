    using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;

namespace Anarks2.UnityCommon.Gameplay.Dialog
{   
    public class DialogueWindowController : MonoBehaviour
    {
        private VisualElement _root;
        private Label _speakerLabel;
        private Label _dialogueLabel;
        private Button _continueButton;

        private List<string> _lines = new List<string>();
        private int _currentLine;
        private string _speakerName;
        
        // Configurable typing speed (characters per second)
        [SerializeField] private float _typingSpeed = 30f; 

        private CancellationTokenSource _typingCancellationTokenSource;

        private void Awake()
        {
            var uiDocument = GetComponent<UIDocument>();
            _root = uiDocument.rootVisualElement;

            // Query elements based on names given in UXML
            _speakerLabel = _root.Q<Label>("Speaker");
            _dialogueLabel = _root.Q<Label>("Dialogue");
            _continueButton = _root.Q<Button>("Continue");

            _continueButton.clicked += OnContinuePressed;
            Close();
        }

        public void Open()
        {
            _root.style.display = DisplayStyle.Flex;
            _continueButton.Focus();
        }

        public void Close()
        {
            CancelTyping();
            _root.style.display = DisplayStyle.None;
        }

        public void DisplayLine(string line, string speaker = "")
        {
            DisplayMultiline(new List<string> { line }, speaker);
        }

        public void DisplayMultiline(List<string> lines, string speaker = "")
        {
            _lines = lines;
            _speakerName = speaker;
            _currentLine = 0;
            Open();
            _ = _NextLineAsync(); // Fire and forget standard async method
        }

        private async Task _NextLineAsync()
        {
            // Handle Speaker visibility and text
            _speakerLabel.style.display = string.IsNullOrEmpty(_speakerName) ? DisplayStyle.None : DisplayStyle.Flex;
            _speakerLabel.text = _speakerName;

            string targetText = _lines[_currentLine];
            _dialogueLabel.text = "";

            CancelTyping();
            _typingCancellationTokenSource = new CancellationTokenSource();

            try
            {
                await TypeTextRoutineAsync(targetText, _typingCancellationTokenSource.Token);
            }
            catch (OperationCanceledException)
            {
                // Handled when skipped
            }
        }

        private async Task TypeTextRoutineAsync(string targetText, CancellationToken cancellationToken)
        {
            int totalChars = targetText.Length;
            float currentChars = 0;

            while (currentChars < totalChars)
            {
                cancellationToken.ThrowIfCancellationRequested();

                currentChars += _typingSpeed * Time.deltaTime;
                int charsToShow = Mathf.Clamp(Mathf.FloorToInt(currentChars), 0, totalChars);
                
                _dialogueLabel.text = targetText.Substring(0, charsToShow);

                if (charsToShow >= totalChars) break;

                // Yield control back to Unity's main thread loop for the next frame
                await Task.Yield();
            }

            // Ensure full text is set at the end
            _dialogueLabel.text = targetText;
        }

        private void OnContinuePressed()
        {
            if (_typingCancellationTokenSource != null && !_typingCancellationTokenSource.IsCancellationRequested)
            {
                // Skip typing animation: cancel current typewriter task and display full text instantly
                CancelTyping();
                _dialogueLabel.text = _lines[_currentLine];
            }
            else
            {
                // Move to next line or close
                _currentLine++;
                if (_currentLine < _lines.Count)
                {
                    _ = _NextLineAsync();
                }
                else
                {
                    Close();
                }
            }
        }

        private void CancelTyping()
        {
            if (_typingCancellationTokenSource != null)
            {
                _typingCancellationTokenSource.Cancel();
                _typingCancellationTokenSource.Dispose();
                _typingCancellationTokenSource = null;
            }
        }
    }
}