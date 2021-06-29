using System;
using Unit = System.ValueTuple;

namespace Andy.Functional
{
    /// <summary>
    /// Andy -> Action -> Function
    /// </summary>
    public static class ExtensionsForAction
    {
        /// <summary>
        /// Andy -> 提供 Action To Fnnc 的回傳值. Unit = System.ValueTuple
        /// </summary>
        public static Unit Unit() => default(Unit);

        /// <summary>
        /// Andy -> Action -> Func(Action, Unit)  
        /// </summary>
        public static Func<Unit> ToFunc(this Action action)
            => () => { action(); return Unit(); };

        /// <summary>
        /// Andy -> Action[t] -> Func(Action[t], Unit)  
        /// </summary>
        public static Func<T, Unit> ToFunc<T>(this Action<T> action)
            => (t) => { action(t); return Unit(); };

        /// <summary>
        /// Andy -> Action[t1, t2] -> Func(Action[t1, t2], Unit)  
        /// </summary>
        public static Func<T1, T2, Unit> ToFunc<T1, T2>(this Action<T1, T2> action)
                => (t1, t2) => { action(t1, t2); return Unit(); };
    }
}
