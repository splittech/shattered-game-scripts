using UnityEngine;

public static class GameManager
{
    public static PlayerMovement playerMovement;

    public static void QuitApplication()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
}