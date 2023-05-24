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
    public class Cursor
    {
        public Vec2 Pos { get; private set; }
        public Vec2 StartPos { get; set; }
        public List<Vec2> TraceLine { get; } = new List<Vec2>();
        public int X => (int)Pos.x;
        public int Y => (int)Pos.y;
        public int Hp { get; private set; } = 0;

        public Cursor(int maxHp)
        {
            Pos = new(0, 0);
            Hp = maxHp;
        }

        public void SetPos(int x, int y) => Pos = new(x, y);
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
        }

        public void Reset2StartPos()
        {
            var startPos = StartPos;
            TraceLine.Clear();
            Pos = startPos;
        }
    }
}
