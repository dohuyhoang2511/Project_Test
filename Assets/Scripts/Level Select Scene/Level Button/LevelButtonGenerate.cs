using UnityEngine;
using UnityEngine.UI;


public class LevelButtonGenerate : MonoBehaviour
{
    [Header("Properties")]
    [SerializeField] int numberOfLevel;

    [Header("Refs")]
    [SerializeField] Transform content;
    [SerializeField] GameObject buttonPrefab;
    [Tooltip("Empty RectTransform, which contain Horizontal Layout")]
    [SerializeField] GameObject rowPrefab;
    [Tooltip("Dummy obj, just for filling the blank spot in last row")]
    [SerializeField] GameObject emptyButton;
    [SerializeField] Sprite lockedSprite;
    [SerializeField] Sprite unlockedSprite;
    [SerializeField] LevelSelect[] allLevels;

    private void Start()
    {
        LoadMap();
    }

    ///<summary>
    ///Clear all child inside content of scrollview
    ///</summary>
    public bool ClearAllLevel()
    {
        allLevels = new LevelSelect[numberOfLevel];
        if (content != null)
        {
            while (content.childCount > 0)
            {
                DestroyImmediate(content.GetChild(0).gameObject);
            }
            return true;
        }
        else
        {
            Debug.LogError("Content transform not found.");
            return false;
        }
    }

    ///<summary>
    ///Generate levels base on numOfLevel
    ///</summary>
    public void GenerateLevel()
    {
        int rowCount = Mathf.CeilToInt((float)numberOfLevel / 4.0f);
        int btnIndex = 1;

        for (int i = 1; i <= rowCount; i++)
        {
            GameObject newRow = Instantiate(rowPrefab);
            newRow.transform.SetParent(content, false);
            newRow.transform.SetAsFirstSibling();
            if (i % 2 == 0) newRow.GetComponent<HorizontalLayoutGroup>().reverseArrangement = true;

            for (int j = 1; j <= 4; j++)
            {
                if (btnIndex > numberOfLevel)
                {
                    GameObject dummR = Instantiate(emptyButton);
                    dummR.transform.SetParent(newRow.transform, false);
                    continue;
                }

                GameObject newLevel = Instantiate(buttonPrefab);
                newLevel.transform.SetParent(newRow.transform, false);

                if (i % 2 == 0)
                {
                    if (j != 1) SetRightLine(newLevel, btnIndex, true);
                }
                else
                {
                    if (j != 4) SetRightLine(newLevel, btnIndex, false);
                }

                //Set index to button
                LevelSelect levelSelect = newLevel.GetComponent<LevelSelect>();
                if (levelSelect)
                {
                    if (btnIndex == 1)
                    {
                        newLevel.transform.Find("Tutorial").gameObject.SetActive(true);
                        newLevel.transform.Find("Index").gameObject.SetActive(false);
                        levelSelect.UnlockWithSprite(unlockedSprite);

                        SaveSystem.AddStage(1, -1);
                    }
                    levelSelect.SetButtonIndex((uint)btnIndex);
                    newLevel.transform.gameObject.name = "LevelSelect_" + btnIndex;

                    //cache this button
                    allLevels[btnIndex - 1] = levelSelect;

                    //Set vertical line
                    if (btnIndex % 4 == 0 && btnIndex != numberOfLevel)
                    {
                        newLevel.transform.Find("LineUp").gameObject.SetActive(true);
                    }
                    btnIndex++;

                    SaveSystem.AddStage(btnIndex, -1);
                }
                else
                {
                    Debug.LogError("LevelSelect script not found in prefab. Fixing by adding another one.");
                    continue;
                }
            }
        }
    }

    public void RandomStageUnlock()
    {
        //ResetStages();

        for (int i = 1; i < allLevels.Length; i++)
        {
            SaveSystem.LockStage(i);
        }

        int randomIndex = Random.Range(1, allLevels.Length);

        for (int i = 0; i < randomIndex; i++)
        {
            allLevels[i].UnlockWithSprite(unlockedSprite);
            int randomStar = Random.Range(1, 4);
            allLevels[i].UnLockWithStar(randomStar);

            SaveSystem.UnlockedStageExisted(i+1, randomStar);
        }
    }

    public void ResetStages()
    {
        allLevels[0].UnlockWithSprite(unlockedSprite);
        allLevels[0].StarLock();
        for (int i = 1; i < allLevels.Length; i++)
        {
            allLevels[i].LockWithSprite(lockedSprite);
        }

        for (int i = 1; i < allLevels.Length; i++)
        {
            SaveSystem.LockStage(i);
        }
    }

    void SetRightLine(GameObject obj, int btnIndex, bool isEven)
    {
        if (btnIndex == numberOfLevel && !isEven)
        {
            return;
        }

        if (btnIndex % 2 == 0)
        {
            obj.transform.Find("LineLeft").gameObject.SetActive(true);
        }
        else
        {
            obj.transform.Find("LineRight").gameObject.SetActive(true);
        }
    }

    public void LoadMap()
    {
        //int numberStagesUnlocked = SaveSystem.GetNumberStages();

        for (int i = 0; i < allLevels.Length-1; i++)
        {
            int numberOfStar = SaveSystem.GetStar(i+1);
            if(numberOfStar >= 0)
            {
                allLevels[i].UnlockWithSprite(unlockedSprite);
                allLevels[i].UnLockWithStar(numberOfStar);
            }
        }
    }

    public void ClearData()
    {
        SaveSystem.ClearStageData();
    }
}

