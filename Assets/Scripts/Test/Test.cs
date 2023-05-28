using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Swift.Math;
using Swift;

namespace GalPanic
{
    public class Test : MonoBehaviour
    {
        public BattleSceneRender SceneRenderer;
        public UIManager UIManager;

        // Start is called before the first frame update
        IEnumerator Start()
        {
            yield return ConfigManager.Init();
            BattleUnit.IDGen = (prefix) => RandomUtils.RandomString(prefix);
            void StartNewGame()
            {
                var btCfg = ConfigManager.GetBattleConfig("Test01");
                var bt = Battle.Create(btCfg);

                bt.OnWon += () => UIManager.ShowResult(true);
                bt.OnLost += () => UIManager.ShowResult(false);
                bt.OnCompletionChanged += () => UIManager.SetCompletion(bt.Map.Completion);
                bt.OnCursorHpChanged += (_) => UIManager.SetHp(bt.Cursor.Hp);

                // setup scene renderer
                SceneRenderer.Bt = bt;
                SceneRenderer.SetAvatar(btCfg.frontAni, btCfg.backAni);
                SceneRenderer.UpdateMap();
                SceneRenderer.GetCursorSpeed = () => 50;

                // init battle
                bt.Map.FillArea(0, 10, 0, 10, BattleMap.GridType.Uncovered);
                bt.Cursor.StartPos = new(9, 9);
                bt.Cursor.SetPos(9, 9);
                bt.Load();
            };

            UIManager.OnRestartClicked += () =>
            {
                UIManager.HideResult();
                SceneRenderer.Clear();
                StartNewGame();
            };

            StartNewGame();
        }
    }
}
