using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Andy.Functional
{
    public static class ExtensionsForBox
    {
        /// <summary>
        /// Andy -> B(T) -> (T -> bool) bool ? 
        /// </summary>
        public static Box<T> Where<T>(this Box<T> box, Func<T, bool> match)
             => box.Match<Box<T>>((ex) => box,
                                (value) =>
                                {
                                    try { return match(value) ? box : null; }
                                    catch (Exception ex) { return ex; }
                                });

        /// <summary>
        /// Andy => Right: B(T) -> ( T -> R ) -> B(R)
        /// </summary>
        public static Box<R> Map<T, R>(this Box<T> box, Func<T, R> Right)
            => box.Match<Box<R>>((ex) => ex,
                                (value) =>
                                {
                                    try { return Right(value); }
                                    catch (Exception ex) { return ex; }
                                });

        /// <summary>
        /// Andy => Right: B(T) -> (T -> B(R)) -> B(T). 也可以進階成 B(T) -> (T -> B(R).OrElse( _ -> B(R)).OrElse...) -> B(T)
        /// </summary>
        public static Box<R> Bind<T, R>(this Box<T> box, Func<T, Box<R>> Right)
            => box.Match<Box<R>>((ex) => ex,
                                (value) =>
                                {
                                    try { return Right(value); }
                                    catch (Exception ex) { return ex; }
                                });

        /// <summary>
        /// Andy => 只在Box狀態=None執行. Left: B(T) -> (ex -> B(T)) -> B(T)  
        /// </summary>
        public static Box<T> OrElse<T>(this Box<T> box, Func<Exception, Box<T>> Left)
            => box.Match<Box<T>>((ex) => Left(ex), (ex) => box);

        /// <summary>
        /// Andy => 根據目前Box的狀態. 決定是要執行 Left(ex) or Right(value). 一旦執行此指令, 會結整個 Box chain.
        /// </summary>
        public static void Then<T>(this Box<T> box, Action<Exception> Left, Action<T> Right)
        {
            var _ = box.Match<ValueTuple>(ex => Left.ToFunc().Invoke(ex), value => Right.ToFunc().Invoke(value));
        }

        /// <summary>
        /// Andy => 根據目前Box的狀態. 決定是要執行 Left() or Right(value). 一旦執行此指令, 會結整個 Box chain.
        /// </summary>
        public static void Then<T>(this Box<T> box, Action Left, Action<T> Right)
        {
            var _ = box.Match<ValueTuple>(ex => Left.ToFunc().Invoke(), value => Right.ToFunc().Invoke(value));
        }

        /// <summary>
        /// Andy => T -> Box
        /// </summary>
        public static Box<T> Return<T>(this T source) => source;

        /// <summary>
        /// Andy -> 針對 IEnumerable  Bind 後會過濾 Noe 的 Box. 再輸入
        /// </summary>
        public static IEnumerable<R> Bind<T, R>(this IEnumerable<Box<T>> source, Func<T, Box<R>> selector)
            => source.SelectMany(b => b.Bind(selector).AsEnumerable());

        /// <summary>
        /// Andy -> 針對 IEnumerable會過濾掉 Noe 的 Box 
        /// </summary>
        public static IEnumerable<Box<T>> Filter<T, R>(this IEnumerable<Box<T>> source)
            => source.Where(box => box.AsEnumerable().Any());

    }
}
