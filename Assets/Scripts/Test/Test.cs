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

        // Start is called before the first frame update
        IEnumerator Start()
        {
            yield return ConfigManager.Init();
            BattleUnit.IDGen = (prefix) => RandomUtils.RandomString(prefix);

            var bt = new Battle(80, 100);
            var map = bt.Map;

            map.AddUnit(new BattleUnit(
                map,
                "Slime",
                "MoveAndReflect", 
                new Vec2(map.Width / 2, map.Height / 2), 
                new Vec2(
                    RandomUtils.RandomNext(5, 20, true), 
                    RandomUtils.RandomNext(5, 20, true))
                ));

            SceneRenderer.Bt = bt;
            SceneRenderer.MapRenderer.Map = map;
            SceneRenderer.GetCursorSpeed = () => 50;

            SceneRenderer.UpdateMap();
        }
    }
}
