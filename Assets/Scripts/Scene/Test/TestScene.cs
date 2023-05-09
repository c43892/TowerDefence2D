using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GalPanic
{
    public class TestScene : MonoBehaviour
    {
        public BattleMapRenderer MapRenderer;

        // Start is called before the first frame update
        IEnumerator Start()
        {
            yield return ConfigManager.Init();

            BattleMap map = new BattleMap(800, 600);
            MapRenderer.Map = map;
            MapRenderer.UpdateMap();
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
