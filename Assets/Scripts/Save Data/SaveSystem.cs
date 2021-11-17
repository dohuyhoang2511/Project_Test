using System.Collections.Generic;
using UnityEngine;

public static class SaveSystem
{

    static SaveSystem()
    {

    }

    private static void SaveData()
    {
        PlayerPrefs.Save();
    }

    public static void ClearStageData()
    {
        PlayerPrefs.DeleteAll();
        SaveData();
    }

    //Kiểm tra xem stage đã tồn tại chưa
    public static bool IsStageExist(int id)
    {
        string stageName = "level_" + id.ToString();
        return PlayerPrefs.HasKey(stageName);
    }

    //Thêm stage
    public static void AddStage(int id, int numberOfStar)
    {
        if (!IsStageExist(id))
        {
            string stageName = "level_" + id.ToString();
            PlayerPrefs.SetInt(stageName, numberOfStar);
        }
        else
        {
            ChangeStar(id, numberOfStar);
        }
        SaveData();
    }

    //Mở Khóa Stage 
    public static void UnlockedStageExisted(int id, int numberOfStar)
    {
        ChangeStar(id, numberOfStar);
        SaveData();
    }

    //Khóa stage
    public static void LockStage(int id)
    {
        string stageName = "level_" + id.ToString();
        PlayerPrefs.DeleteKey(stageName);
        PlayerPrefs.SetInt(stageName, -1);
        SaveData();
    }

    //Lấy giá trị star của stage
    public static int GetStar(int id)
    {
        string stageName = "level_" + id.ToString();
        int numberOfStar = PlayerPrefs.GetInt(stageName);
        return numberOfStar;

    }

    public static void ChangeStar(int id, int numberStarAfter)
    {
        string stageName = "level_" + id.ToString();
        int numberStarBefore = PlayerPrefs.GetInt(stageName);
        if (numberStarBefore < numberStarAfter)
        {
            PlayerPrefs.DeleteKey(stageName);
            PlayerPrefs.SetInt(stageName, numberStarAfter);
        }
        else if (numberStarBefore >= numberStarAfter)
        {
            PlayerPrefs.DeleteKey(stageName);
            PlayerPrefs.SetInt(stageName, numberStarBefore);
        }
        SaveData();
    }
}