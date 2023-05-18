using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public GameObject WinWnd;
    public GameObject LostWnd;
    public Text Completion;

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
}
