using System;
using Number = System.Single;

namespace Swift
{
    public class State
    {
        // 状态名称
        public string Name { get; private set; }

        // 要执行的任务
        public Action<State, Number> DoRun { get; private set; }

        // 进入状态时的额外动作
        public Action<string> RunIn { get; private set; }

        // 离开状态时的额外动作
        public Action<string> RunOut { get; private set; }

        // 是否是默认状态
        public bool IsDefault { get; set; }

        public State(string name)
        {
            Name = name;
        }

        public State Run(Action<State, Number> doRun)
        {
            DoRun = doRun;
            return this;
        }

        public State OnRunIn(Action<string> runIn)
        {
            RunIn = runIn;
            return this;
        }

        public State OnRunOut(Action<string> runOut)
        {
            RunOut = runOut;
            return this;
        }

        public State AsDefault(bool b = true)
        {
            IsDefault = b;
            return this;
        }

        public void Log(Action<string> logger)
        {
            var runIn = RunIn;
            RunIn = (st) =>
            {
                logger((st == null ? "null " : st) + " => " + Name);
                runIn?.Invoke(st);
            };
        }

        public State Clone()
        {
            var s = new State(Name);
            s.DoRun = DoRun;
            s.RunIn = RunIn;
            s.RunOut = RunOut;
            s.IsDefault = IsDefault;

            return s;
        }
    }
}
