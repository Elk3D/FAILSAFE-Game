using UnityEngine;

namespace FAILSAFE.Prologue
{
    /// <summary>
    /// Static wrapper around PlayerPrefs for all persistent prologue flags.
    ///
    /// Intentionally NOT a MonoBehaviour — it has no scene lifetime and
    /// can be called from anywhere, including Awake() before any manager
    /// has initialised. All keys are namespaced under "FAILSAFE_Prologue_"
    /// to avoid collisions with the main SaveManager JSON system, which
    /// handles chapter-level data separately.
    ///
    /// Usage examples:
    ///   if (PrologueSaveData.HasCompletedPrologue()) { ... }
    ///   PrologueSaveData.SetFlag("pager_seen", true);
    ///   bool seen = PrologueSaveData.GetFlag("pager_seen");
    /// </summary>
    public static class PrologueSaveData
    {
        // ── Key constants ────────────────────────────────────────────────────
        // Use these constants everywhere rather than raw strings so that a
        // typo can never silently create a ghost key.

        /// <summary>Set once when the prologue scene finishes (chapter title fade).</summary>
        public const string KEY_PROLOGUE_COMPLETE = "has_completed_prologue";

        /// <summary>Set when the player has triggered the pager sequence.</summary>
        public const string KEY_PAGER_SEEN = "pager_seen";

        /// <summary>Set when the bedroom door sequence reaches attempt 3.</summary>
        public const string KEY_BEDROOM_DOOR_DONE = "bedroom_door_done";

        // Internal prefix applied to every key before writing to PlayerPrefs.
        // Keeps prologue flags isolated from all other FAILSAFE PlayerPrefs data.
        private const string PREFIX = "FAILSAFE_Prologue_";

        // ── Primary flag helpers ─────────────────────────────────────────────

        /// <summary>Returns true if the player has finished the prologue at least once.</summary>
        public static bool HasCompletedPrologue()
        {
            return GetFlag(KEY_PROLOGUE_COMPLETE);
        }

        /// <summary>
        /// Call this exactly once — at the end of the pager sequence, just before
        /// SceneManager.LoadScene — to permanently record prologue completion.
        /// </summary>
        public static void SetPrologueComplete()
        {
            SetFlag(KEY_PROLOGUE_COMPLETE, true);
        }

        // ── Generic flag API ─────────────────────────────────────────────────

        /// <summary>
        /// Reads a named bool flag from PlayerPrefs.
        /// Returns false if the key has never been written.
        ///
        /// PlayerPrefs has no native bool type; we store 1 (true) / 0 (false)
        /// as an int, which is the standard Unity convention.
        /// </summary>
        /// <param name="key">One of the KEY_* constants above, or a custom string.</param>
        public static bool GetFlag(string key)
        {
            return PlayerPrefs.GetInt(PREFIX + key, 0) == 1;
        }

        /// <summary>
        /// Writes a named bool flag to PlayerPrefs and immediately flushes to disk.
        ///
        /// PlayerPrefs.Save() is called on every write. For a prologue that sets
        /// only a handful of flags this is acceptable; it ensures the flag survives
        /// an unexpected crash mid-sequence.
        /// </summary>
        /// <param name="key">One of the KEY_* constants above, or a custom string.</param>
        /// <param name="value">The value to store.</param>
        public static void SetFlag(string key, bool value)
        {
            PlayerPrefs.SetInt(PREFIX + key, value ? 1 : 0);
            PlayerPrefs.Save();
        }

        // ── Debug / development helpers ──────────────────────────────────────

        /// <summary>
        /// Deletes all prologue flags from PlayerPrefs.
        /// Useful for testing second-playthrough behaviour in the Editor.
        /// NOT called at runtime — wire to a debug menu or Editor button only.
        /// </summary>
        public static void ClearAllFlags()
        {
            PlayerPrefs.DeleteKey(PREFIX + KEY_PROLOGUE_COMPLETE);
            PlayerPrefs.DeleteKey(PREFIX + KEY_PAGER_SEEN);
            PlayerPrefs.DeleteKey(PREFIX + KEY_BEDROOM_DOOR_DONE);
            PlayerPrefs.Save();

            Debug.Log("[PrologueSaveData] All prologue flags cleared.");
        }

        /// <summary>
        /// Logs the current state of all known flags to the console.
        /// Handy during development; strip calls before shipping.
        /// </summary>
        public static void DebugLogAllFlags()
        {
            Debug.Log(
                $"[PrologueSaveData] " +
                $"prologue_complete={HasCompletedPrologue()}, " +
                $"pager_seen={GetFlag(KEY_PAGER_SEEN)}, " +
                $"bedroom_door_done={GetFlag(KEY_BEDROOM_DOOR_DONE)}"
            );
        }
    }
}
