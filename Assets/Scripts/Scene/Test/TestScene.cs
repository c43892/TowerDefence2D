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

            BattleMap map = new BattleMap(80, 60);
            MapRenderer.Map = map;

            map.CompeteFilling(map.Width / 2, map.Height / 2, 1, 1);

            MapRenderer.UpdateMap();
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
