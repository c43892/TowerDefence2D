using Swift;
using Swift.Math;
using System.Collections;
using System.Collections.Generic;

namespace TowerDefance.Game
{
    // a tower instance in the tower defence battle
    public partial class BattleUnit : AIUnitExt.IUnit
    {
        public virtual StateMachine CreateAI()
        {
            return this.SimpleState();
        }
    }
}
