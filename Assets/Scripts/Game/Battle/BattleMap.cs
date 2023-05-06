using Swift.Math;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GalPanic
{
    public class BattleMap
    {
        public enum GridType
        {
            Covered,
            Uncovered,
        }

        public int Width { get; private set; }
        public int Height { get; private set; }

        private readonly GridType[,] grids;

        public BattleMap(int width, int height)
        {
            Width = width;
            Height = height;
            grids = new GridType[width, height];
        }

        public GridType this[int x, int y] { get => grids[x, y]; }
    }
}
