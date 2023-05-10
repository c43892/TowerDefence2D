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
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
