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
        public GizmoManager GizmoManager;
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

                UIManager.SetBattle(bt);

                // setup scene renderer
                SceneRenderer.Bt = bt;
                SceneRenderer.SetAvatar(btCfg.frontAni, btCfg.backAni);
                SceneRenderer.UpdateMap();
                SceneRenderer.GetCursorSpeed = () => 50;

                // setup gizmo manager
                BattleExt.OnAbortAddUnitAI += GizmoManager.OnAbortAddUnitAI;
                bt.Map.OnUnitRemoved += GizmoManager.OnBattleUnitRemoved;
                bt.OnWon += GizmoManager.OnBattleEnded;
                bt.OnLost += GizmoManager.OnBattleEnded;

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
