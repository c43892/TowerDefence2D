using Swift.Math;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Swift;
using System;
using System.Linq;
using Unity.VisualScripting;
using System.Diagnostics;

namespace GalPanic
{
    public class Cursor : ITimeDriven
    {
        public Vec2 Pos { get; private set; }
        public Vec2 StartPos { get; set; }
        public List<Vec2> TraceLine { get; } = new List<Vec2>();
        public int X => (int)Pos.x;
        public int Y => (int)Pos.y;
        public int Hp { get; set; } = 0;
        public int MaxHp { get; } = 0;
        public Fix64 Armor { get; set; } = 0;
        public int MaxArmor { get; } = 0;
        public Fix64 ArmorDec { get; set; } = 0;
        public int ArmorHurtReset { get; set; } = 0;
        public Fix64 ArmorCompletionBonusPercent { get; set; } = 0;
        public Fix64 ArmorCompletionBonusConst { get; set; } = 0;
        public Fix64 CoolDown { get; set; }

        public Cursor(int maxHp, int maxArmor, Fix64 armorDec, int armorHurtReset, Fix64 armorCompletionBonusPercent, Fix64 armorCompletionBonusConst)
        {
            Pos = new(0, 0);
            Hp = maxHp;
            MaxHp = maxHp;
            Armor = maxArmor;
            MaxArmor = maxArmor;
            ArmorDec = armorDec;
            ArmorHurtReset = armorHurtReset;
            ArmorCompletionBonusPercent = armorCompletionBonusPercent;
            ArmorCompletionBonusConst = armorCompletionBonusConst;
        }

        public void SetPos(Vec2 pos, bool clearTraceLine = false)
        {
            Pos = pos;
            if (clearTraceLine)
                TraceLine.Clear();
        }

        public void SetPos(int x, int y, bool clearTraceLine = false) => SetPos(new(x, y), clearTraceLine);
        public void AddTracePos(int x, int y) => TraceLine.Add(new(x, y));

        public void StepBack()
        {
            if (TraceLine.Count > 0)
                TraceLine.Remove(TraceLine[^1]);

            Pos = TraceLine.Count() > 0 ? TraceLine[^1] : StartPos;
        }

        public void CursorHurt(int dhp = -1)
        {
            Hp += dhp;
            Armor = ArmorHurtReset;
        }

        public void Reset2StartPos()
        {
            var startPos = StartPos;
            TraceLine.Clear();
            Pos = startPos;
        }

        public void OnTimeElapsed(Fix64 te)
        {
            CoolDown = CoolDown <= te ? 0 : CoolDown - te;
            Armor += ArmorDec * te;
            if (Armor < 0)
                Armor = 0;
            else if (Armor > MaxArmor)
                Armor = MaxArmor;
        }

        public void OnCompletion(Fix64 precent)
        {
            Armor = MathEx.Clamp(Armor + ArmorCompletionBonusConst + ArmorCompletionBonusPercent * precent, 0, MaxArmor);
        }
    }
}
