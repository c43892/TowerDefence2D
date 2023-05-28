using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public GameObject WinWnd;
    public GameObject LostWnd;
    public Text CompletionValue;
    public Text HpValue;

    public event Action OnRestartClicked;

    public void ShowResult(bool win)
    {
        WinWnd.SetActive(win);
        LostWnd.SetActive(!win);
    }

    public void HideResult()
    {
        WinWnd.SetActive(false);
        LostWnd.SetActive(false);
    }

    public void Restart()
    {
        OnRestartClicked?.Invoke();
    }

    public void SetHp(int hp)
    {
        HpValue.text = hp.ToString();
    }

    public void SetCompletion(float completion)
    {
        CompletionValue.text = string.Format("{0:.00}", completion * 100);
    }
}
