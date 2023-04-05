using System;
using System.Collections;
using System.Collections.Generic;
using Swift;
using Swift.Math;

namespace TowerDefance.Game
{

    // for a tower defence battle instance including the map, the towers, the enemies, the player, the game state, etc.
    public class TowerDefanceBattle : IFrameDrived
    {
        public BattleMap Map { get; protected set; }

        protected readonly StateMachineManager smm = new();

        public virtual void Init()
        {
            BattleMap.OnUnitRemoved += u => smm.Del(u.UID);
        }

        public virtual void OnTimeElapsed(int te)
        {
            smm.OnTimeElapsed(te);
            Map.OnTimeElapsed(te);
        }
    }
}
