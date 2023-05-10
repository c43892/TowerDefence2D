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

            BattleMap map = new(80, 100);
            MapRenderer.Map = map;

            map.FillArea(0, map.Width, 20, 1, BattleMap.GridType.Uncovered);
            map.CompeteFilling(map.Width / 2, 19, map.Width / 2, 21);

            MapRenderer.UpdateMap();
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
