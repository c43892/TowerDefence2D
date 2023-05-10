using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GalPanic
{
    public class Test : MonoBehaviour
    {
        public BattleSceneRender SceneRenderer;

        // Start is called before the first frame update
        IEnumerator Start()
        {
            yield return ConfigManager.Init();

            var bt = new Battle(80, 100);
            var map = bt.Map;

            SceneRenderer.Bt = bt;
            SceneRenderer.MapRenderer.Map = map;

            SceneRenderer.UpdateMap();

            var points = new List<Vector3>();

            points.Add(new(0, 0, 0));
            points.Add(new(1, 0, 0));
            points.Add(new(1, 1, 0));
            points.Add(new(0, 1, 0));

            SceneRenderer.Trace.UpdateLine(points);
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
