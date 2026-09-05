using System.Collections.Generic;

public static class GameScoreData
{
    public static int saveScore = 0;
    public static int treatScore = 0;
    public static HashSet<string> uniqueSpeciesAssisted = new HashSet<string>();

    public static void ResetData()
    {
        saveScore = 0;
        treatScore = 0;
        uniqueSpeciesAssisted.Clear();
    }

    public static int GetCombo()
    {
        int uniqueCount = uniqueSpeciesAssisted.Count;
        return uniqueCount >= 3 ? uniqueCount : 1;
    }

    public static int CalculateFinalScore()
    {
        return (saveScore + treatScore) * GetCombo();
    }
}