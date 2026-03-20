using UnityEngine;
using UnityEngine.Events;

namespace FAILSAFE.Narrative
{
    /// <summary>
    /// Tracks and progresses through the game's chapters.
    /// Each chapter has a name, scene, and optional intro epigraph.
    /// </summary>
    public class ChapterManager : MonoBehaviour
    {
        [System.Serializable]
        public class Chapter
        {
            public string chapterID;
            public string chapterName;
            public string sceneName;
            [TextArea(2, 4)] public string epigraphQuote;
            public string epigraphAuthor;
        }

        [Header("Chapters")]
        [SerializeField] private Chapter[] chapters;

        [Header("Events")]
        public UnityEvent<Chapter> onChapterStarted;

        public int CurrentChapterIndex { get; private set; } = 0;
        public Chapter CurrentChapter => chapters.Length > 0 ? chapters[CurrentChapterIndex] : null;

        public void AdvanceChapter()
        {
            if (CurrentChapterIndex < chapters.Length - 1)
            {
                CurrentChapterIndex++;
                onChapterStarted?.Invoke(CurrentChapter);
            }
        }

        public void GoToChapter(int index)
        {
            if (index < 0 || index >= chapters.Length) return;
            CurrentChapterIndex = index;
            onChapterStarted?.Invoke(CurrentChapter);
        }
    }
}
