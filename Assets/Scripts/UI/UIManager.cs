using GalPanic;
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
    public Text ArmorValue;

    public event Action OnRestartClicked;

    private Battle bt;

    public void SetBattle(Battle battle)
    {
        bt = battle;

        bt.OnWon += () => ShowResult(true);
        bt.OnLost += () => ShowResult(false);
    }

    private void Update()
    {
        if (bt==null)
            return;

        HpValue.text = bt.Cursor.Hp.ToString();
        ArmorValue.text = string.Format("{0:.00}", (float)bt.Cursor.Armor);
        CompletionValue.text = string.Format("{0:.00}", bt.Map.Completion * 100);
    }

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
