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
        public static IEnumerator<HashSet<KeyValuePair<int, int>>> Fill2DAreaCoroutine(
            List<KeyValuePair<int, int>> seeds, 
            Func<int, int, bool> fillable,
            Func<int, int, bool> cancelChecker,
            Func<int, int, KeyValuePair<int, int>[]> getNeighbours, 
            Action<int, int> fill)
        {
            var filled = new HashSet<KeyValuePair<int, int>>();
            Queue<KeyValuePair<int, int>> q = new();

            void internalFill(int x, int y)
            {
                var pt = new KeyValuePair<int, int>(x, y);
                q.Enqueue(pt);

                if (filled.Contains(pt))
                    return;

                fill(x, y);
                filled.Add(pt);
            }

            foreach (var kv in seeds)
            {
                if (fillable(kv.Key, kv.Value))
                {
                    internalFill(kv.Key, kv.Value);
                    yield return filled;
                }
            }

            while (q.Count > 0)
            {
                var cur = q.Dequeue();
                var neighbours = getNeighbours(cur.Key, cur.Value);
                foreach (var n in neighbours)
                {
                    var x = n.Key;
                    var y = n.Value;
                    if (cancelChecker != null && cancelChecker(x, y))
                        break;

                    if (fillable(x, y))
                    {
                        internalFill(x, y);
                        yield return filled;
                    }
                }
            }
        }

        // 填充指定区域
        public static HashSet<KeyValuePair<int, int>> Fill2DArea(int cx, int cy, Func<int, int, bool> fillable, Func<int, int, KeyValuePair<int, int>[]> getNeighbours, Action<int, int> fill)
        {
            var iter = Fill2DAreaCoroutine(new() { new(cx, cy) }, fillable, null, getNeighbours, fill);
            while (iter.MoveNext())
                ;
            return iter.Current;
        }
    }
}
