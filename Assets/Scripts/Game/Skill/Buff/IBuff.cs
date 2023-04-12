using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Swift;
using Swift.Math;

namespace TowerDefance.Game
{
    public interface IBuff : ISkill
    {
        public interface IBuffOwner
        {
            void AddBuff(IBuff buff);
            void RemoveBuff(IBuff buff);
        }

        public IBuffOwner Owner { get; set; }
    }
}
