namespace FAILSAFE.Utilities
{
    /// <summary>
    /// Project-wide constants: tags, layer names, scene names, animator parameters.
    /// </summary>
    public static class Constants
    {
        // Tags
        public const string TAG_INTERACTABLE = "Interactable";
        public const string TAG_PLAYER       = "Player";
        public const string TAG_LADDER       = "Ladder";

        // Layers
        public const string LAYER_DEFAULT     = "Default";
        public const string LAYER_PLAYER      = "Player";
        public const string LAYER_INTERACTABLE = "Interactable";
        public const string LAYER_UI          = "UI";

        // Scene Names
        public const string SCENE_MAIN_MENU       = "MainMenu";
        public const string SCENE_CHAPTER_01      = "Chapter01_Awakening";
        public const string SCENE_CHAPTER_02      = "Chapter02_Descent";
        public const string SCENE_CHAPTER_03      = "Chapter03_Truth";
        public const string SCENE_CHAPTER_04      = "Chapter04_Failsafe";
        public const string SCENE_END_GAME        = "EndGame";

        // Animator Parameters
        public const string ANIM_IS_WALKING  = "IsWalking";
        public const string ANIM_IS_CROUCHING = "IsCrouching";
        public const string ANIM_IS_RUNNING  = "IsRunning";
        public const string ANIM_OPEN        = "Open";
        public const string ANIM_FADE_IN     = "FadeIn";
        public const string ANIM_FADE_OUT    = "FadeOut";

        // Input Axes / Buttons
        public const string INPUT_HORIZONTAL = "Horizontal";
        public const string INPUT_VERTICAL   = "Vertical";
        public const string INPUT_INTERACT   = "Interact";
        public const string INPUT_PAUSE      = "Pause";

        // PlayerPrefs Keys
        public const string PREFS_SETTINGS   = "FAILSAFE_Settings";
    }
}
