using Swift;
using Swift.Math;
using System.Collections;
using System.Collections.Generic;

namespace TowerDefance.Game
{
    // a tower instance in the tower defence battle
    public partial class BattleMapUnit : AIUnitExt.IUnit
    {
        public StateMachine AI { get; protected set; }

        public virtual void StartAI()
        {
            if (AI == null)
                AI = CreateAI();

            AI.Start();
        }

        public virtual StateMachine CreateAI()
        {
            return this.SimpleState();
        }
    }
}
