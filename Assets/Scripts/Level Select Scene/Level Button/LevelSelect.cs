using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class LevelSelect : MonoBehaviour
{
    [SerializeField] uint btnIndex;

    public void SetButtonIndex(uint i)
    {
        btnIndex = i;
        transform.Find("Index").gameObject.GetComponent<Text>().text = btnIndex.ToString();
    }

    [SerializeField] bool isLocked = true;

    public void Unlock()
    {
        isLocked = false;
        if (LevelManager.Instance)
        {
            SetStageSprite(LevelManager.Instance.GetUnLockedSprite());
        }
        transform.Find("Locked").gameObject.SetActive(false);
    }

    public void UnLockWithStar(int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            transform.Find((i + 1) + "Star").gameObject.SetActive(true);
        }
    }

    public void StarLock()
    {
        for (int i = 0; i < 3; i++)
        {
            transform.Find((i + 1) + "Star").gameObject.SetActive(false);
        }
    }

    public void UnlockWithSprite(Sprite sprite)
    {
        isLocked = false;
        transform.Find("Locked").gameObject.SetActive(false);
        SetStageSprite(sprite);
    }

    public void Lock()
    {
        isLocked = true;
        if (LevelManager.Instance) SetStageSprite(LevelManager.Instance.GetLockedSprite());
        transform.Find("Locked").gameObject.SetActive(true);
    }

    public void LockWithSprite(Sprite sprite)
    {
        isLocked = true;
        SetStageSprite(sprite);
        StarLock();

        if (btnIndex != 1)
        {
            transform.Find("Locked").gameObject.SetActive(true);
        }
    }

    public void LoadLevel()
    {
        if (isLocked) 
        {
            Debug.Log("Locked");
            return; 
        }
        else
        {
            SceneManager.LoadScene("GamePlay");
            Debug.Log(btnIndex);
        }
    }

    public void SetStageSprite(Sprite n_sprite)
    {
        Transform btn = transform.Find("Stage");

        if (btn)
        {
            btn.gameObject.GetComponent<Image>().sprite = n_sprite;
        }
        else
        {
            Debug.LogError("Stage button name \"stage\" not found");
        }
    }
}

