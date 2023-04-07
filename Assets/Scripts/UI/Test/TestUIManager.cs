using System.Collections;
using System.Collections.Generic;
using TowerDefance.Game;
using UnityEngine;

public class TestUIManager : MonoBehaviour
{
    public Transform StartScreen;
    public Transform WinnerScreen;
    public Transform LoserScreen;

    public TestBattleScene BtScene;

    TestBattle bt = null;

    public void Start()
    {
        StartScreen.gameObject.SetActive(true);
        WinnerScreen.gameObject.SetActive(false);
        LoserScreen.gameObject.SetActive(false);
    }
    public void OnStartBtn()
    {
        StartScreen.gameObject.SetActive(false);
        WinnerScreen.gameObject.SetActive(false);
        LoserScreen.gameObject.SetActive(false);

        bt = new TestBattle();
        bt.Init();

        BtScene.Init(bt);

        bt.OnWon += (bt) => WinnerScreen.gameObject.SetActive(true);
        bt.OnFailed += (bt) => LoserScreen.gameObject.SetActive(true);

        bt.Start();
    }
}
