using UnityEngine;
using UnityEngine.Events;

namespace FAILSAFE.Gameplay.Puzzles
{
    /// <summary>
    /// Abstract base for all puzzles. Subclass this and implement ValidateSolution().
    /// </summary>
    public abstract class PuzzleBase : MonoBehaviour
    {
        [Header("Puzzle")]
        [SerializeField] private string puzzleID;
        [SerializeField] private bool solvable = true;

        [Header("Events")]
        public UnityEvent onSolved;
        public UnityEvent onFailed;
        public UnityEvent onReset;

        public bool IsSolved { get; private set; }
        public string PuzzleID => puzzleID;

        public void Attempt()
        {
            if (!solvable || IsSolved) return;

            if (ValidateSolution())
            {
                IsSolved = true;
                OnSolved();
                onSolved?.Invoke();
            }
            else
            {
                OnFailed();
                onFailed?.Invoke();
            }
        }

        public void Reset()
        {
            IsSolved = false;
            OnReset();
            onReset?.Invoke();
        }

        /// <summary>Override to implement the validation logic.</summary>
        protected abstract bool ValidateSolution();

        protected virtual void OnSolved() { }
        protected virtual void OnFailed() { }
        protected virtual void OnReset()  { }
    }
}
