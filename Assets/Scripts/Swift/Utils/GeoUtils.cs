using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Runtime.ConstrainedExecution;

namespace Swift
{
    public class GeoUtils
    {
        // 填充指定区域
        public static IEnumerator<HashSet<KeyValuePair<int, int>>> Fill2DAreaCoroutine(int cx, int cy, Func<int, int, bool> fillable, Func<int, int, KeyValuePair<int, int>[]> getNeighbours, Action<int, int> fill)
        {
            Debug.Assert(cx >= 0 && cy >= 0);

            var filled = new HashSet<KeyValuePair<int, int>>();
            Queue<KeyValuePair<int, int>> q = new();

            void internalFill(int x, int y)
            {
                fill(x, y);
                var pt = new KeyValuePair<int, int>(x, y);
                q.Enqueue(pt);
                filled.Add(pt);
            }

            if (fillable(cx, cy))
            {
                internalFill(cx, cy);
                yield return filled;
            }

            while (q.Count > 0)
            {
                var cur = q.Dequeue();
                var neighbours = getNeighbours(cur.Key, cur.Value);
                foreach (var n in neighbours)
                {
                    if (fillable(n.Key, n.Value))
                    {
                        internalFill(n.Key, n.Value);
                        yield return filled;
                    }
                }
            }
        }

        // 填充指定区域
        public static HashSet<KeyValuePair<int, int>> Fill2DArea(int cx, int cy, Func<int, int, bool> fillable, Func<int, int, KeyValuePair<int, int>[]> getNeighbours, Action<int, int> fill)
        {
            var iter = Fill2DAreaCoroutine(cx, cy, fillable, getNeighbours, fill);
            while (iter.MoveNext())
                ;
            return iter.Current;
        }
    }
}
