using System.Collections.Generic;
using UnityEngine;

namespace FAILSAFE.Gameplay.Puzzles
{
    /// <summary>
    /// A puzzle solved by activating interactive elements in the correct order.
    /// E.g. pressing buttons/levers in a specific sequence.
    /// </summary>
    public class SequencePuzzle : PuzzleBase
    {
        [Header("Sequence")]
        [SerializeField] private string[] correctSequence;
        [SerializeField] private bool resetOnWrong = true;

        private readonly List<string> _playerSequence = new List<string>();

        public void RegisterStep(string stepID)
        {
            _playerSequence.Add(stepID);

            // Validate current progress
            int index = _playerSequence.Count - 1;
            if (_playerSequence[index] != correctSequence[index])
            {
                if (resetOnWrong) Reset();
                return;
            }

            if (_playerSequence.Count == correctSequence.Length)
                Attempt();
        }

        protected override bool ValidateSolution()
        {
            if (_playerSequence.Count != correctSequence.Length) return false;
            for (int i = 0; i < correctSequence.Length; i++)
                if (_playerSequence[i] != correctSequence[i]) return false;
            return true;
        }

        protected override void OnReset() => _playerSequence.Clear();
    }
}
