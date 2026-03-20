using UnityEngine;

namespace FAILSAFE.Gameplay.Puzzles
{
    /// <summary>
    /// Puzzle solved by placing objects or aligning elements in the environment.
    /// E.g. moving crates onto pressure plates, aligning symbols on rotating rings.
    /// </summary>
    public class EnvironmentPuzzle : PuzzleBase
    {
        [Header("Conditions")]
        [SerializeField] private PuzzleCondition[] conditions;

        [System.Serializable]
        public class PuzzleCondition
        {
            public string conditionID;
            public bool isMet;
        }

        public void SetCondition(string conditionID, bool met)
        {
            foreach (var c in conditions)
            {
                if (c.conditionID == conditionID)
                {
                    c.isMet = met;
                    break;
                }
            }
            // Auto-check after each condition change
            if (AllConditionsMet()) Attempt();
        }

        private bool AllConditionsMet()
        {
            foreach (var c in conditions)
                if (!c.isMet) return false;
            return true;
        }

        protected override bool ValidateSolution() => AllConditionsMet();
    }
}
