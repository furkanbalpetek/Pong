using UnityEngine;

public static class GameSettings
{
    public static bool singlePlayer = true;
    public static int aiLevel = 1;

    public static bool IsBeaten(int level)
    {
        return PlayerPrefs.GetInt("beaten_" + level, 0) == 1;
    }

    public static void MarkBeaten(int level)
    {
        PlayerPrefs.SetInt("beaten_" + level, 1);
        PlayerPrefs.Save();
    }
}