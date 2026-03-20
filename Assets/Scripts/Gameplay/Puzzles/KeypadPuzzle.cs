using UnityEngine;
using TMPro;

namespace FAILSAFE.Gameplay.Puzzles
{
    /// <summary>
    /// A numeric keypad puzzle. Player enters a code via on-screen buttons.
    /// </summary>
    public class KeypadPuzzle : PuzzleBase
    {
        [Header("Keypad")]
        [SerializeField] private string correctCode = "1234";
        [SerializeField] private int maxDigits = 4;
        [SerializeField] private TMP_Text displayText;
        [SerializeField] private AudioClip keyPressSFX;
        [SerializeField] private AudioClip correctSFX;
        [SerializeField] private AudioClip wrongSFX;

        private string _currentInput = "";

        public void PressKey(string digit)
        {
            if (_currentInput.Length >= maxDigits) return;
            _currentInput += digit;
            UpdateDisplay();
            Managers.AudioManager.Instance?.PlaySFX(keyPressSFX);

            if (_currentInput.Length == maxDigits)
                Attempt();
        }

        public void PressBackspace()
        {
            if (_currentInput.Length == 0) return;
            _currentInput = _currentInput[..^1];
            UpdateDisplay();
        }

        protected override bool ValidateSolution() => _currentInput == correctCode;

        protected override void OnSolved()
        {
            Managers.AudioManager.Instance?.PlaySFX(correctSFX);
        }

        protected override void OnFailed()
        {
            Managers.AudioManager.Instance?.PlaySFX(wrongSFX);
            _currentInput = "";
            UpdateDisplay();
        }

        protected override void OnReset()
        {
            _currentInput = "";
            UpdateDisplay();
        }

        private void UpdateDisplay()
        {
            if (displayText != null)
                displayText.text = _currentInput.PadRight(maxDigits, '_');
        }
    }
}
