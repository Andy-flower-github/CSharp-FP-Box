//===============================================================================
// Copyright © 花志民(Andy.flower)  All rights reserved.
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND.
//===============================================================================
// 功能說明:
//   (1) 將一般的 Primitive Type 或是 DTO 放入一個 Box. 提供 Functional Programming 的操作
//   (2) c# 是強型別的語言, 是無法定義出 maybe T
//   (3) 使用 Box 可以包裝在Box裡的 value 模擬出 maybe T的功能. 讓 Monad function 操作時. 會自動做出 match(L, R) 的動作
//   (4) 提供的operator  有 Where, Map, Bind, OrElse, Match, Then
//   (5) 使用 F.Boxing<T>(value) 來建立Box,
//
// 範例: 判定輸入的陣列值是否可否構成 Bingo 遊戲的一條線
// 判定規則
// (1) => 陣列由小到大排序
// (2) => 起始點必須是在 Top or Left 的線上
// (3) => 最後一點必須是在 Bottom or Right 的線上
// (4) => 計算前後兩點間的距離
// (5) => 所有的距離必須相同.
//  1   2   3   4   5
//  6   7   8   9   10
//  11  12  13  14  15
//  16  17  18  19  20
//  21  22  23  24  25
//
// internal bool Bingo(params int[] positions)
// {
//    Func<int, bool> IsTop(int size) => (loc) => loc <= size;
//    Func<int, bool> IsLeft(int size) => (loc) => loc % size == 1;
//    Func<int, bool> IsRight(int size) => (loc) => loc % size == 0;
//    Func<int, bool> IsBottom(int size) => (loc) => loc > size * (size - 1);
//    Func<IEnumerable<int>, IEnumerable<int>> GetGaps = ary => ary.Zip(ary.Skip(1), (x, y) => y - x);
//    Func<IEnumerable<int>, bool> GapsIsSame = ary => !ary.Any(x => x != ary.First());

//    return F.Boxing(positions)
//                .Map(ary => ary.OrderBy(x => x).ToArray())
//                .Where(ary => IsTop(ary.Length)(ary[0]) || IsLeft(ary.Length)(ary[0]))
//                .Where(ary => IsBottom(ary.Length)(ary.Last()) || IsRight(ary.Length)(ary.Last()))
//                .Map(GetGaps)
//                .Map(GapsIsSame)
//                .GetValue();
// }
//===============================================================================
// Version. 1.0.0.0  6/27/2021 
//===============================================================================

using System;
using System.Collections.Generic;

namespace Andy.Functional
{
    /// <summary>
    /// Andy => 提供一個 Box 來將資料物件(DTO). 變成可以使用 FP 的功能物件
    /// </summary>
    public struct Box<T>
    {
        /// <summary>
        /// Andy => 提供給[隱含轉型]時建立一個新的 Box(value)
        /// </summary>
        private static Box<T> OfSome(T value) => new Box<T>(value);

        /// <summary>
        /// Andy -> 提供給[隱含轉型]建立一個新的 Box() value = null, exception = null;
        /// </summary>
        private static Box<T> OfNone(Exception ex = null)
            => ex == null ? new Box<T>() : new Box<T>(ex);

        // 讓Value 可以模擬出 maybe monad 的功能. 讓後繼的 Functor 操作可以做出 Match(L, R) 的動作 
        readonly bool isSome;
        readonly T value;

        // 加後一個 exception 的欄位. 記錄造成None 的原因.(非必要)
        readonly Exception exception;

        private Box(T value)
        {
            this.isSome = true;
            this.value = value;
            this.exception = null;
        }

        private Box(Exception ex)
        {
            this.isSome = false;
            this.value = default(T);
            this.exception = ex;
        }

        /// <summary>
        /// Andy => 當程式要做 隱含轉型時. 會自動判斷
        /// </summary>
        public static implicit operator Box<T>(T value)
            => value == null ? Box<T>.OfNone() : Box<T>.OfSome(value);

        /// <summary>
        /// Andy => 當程式要做 隱含的轉型時. 會自動判斷
        /// </summary>
        public static implicit operator Box<T>(Exception ex)
            => Box<T>.OfNone(ex);

        /// <summary>
        /// Andy => isSome ? Right(value) : Left(exception)
        /// </summary>
        public R Match<R>(Func<Exception, R> Left, Func<T, R> Right) => isSome ? Right(value) : Left(exception);

        /// <summary>
        /// Andy => if (isSome) yield return value
        /// </summary>
        public IEnumerable<T> AsEnumerable() { if (isSome) yield return value; }

        /// <summary>
        /// Andy => Match((ex) => ex == null ? "None" : ex.Message, x => x.ToString())
        /// </summary>
        public override string ToString()
            => Match((ex) => ex == null ? "None" : ex.Message, x => x.ToString());

        /// <summary>
        /// Andy => 取出放在Box的值
        /// </summary>
        public T GetValue() => value;

        /// <summary>
        /// Andy => 取出放在Box的Exception
        /// </summary>
        public Exception GetException() => exception;

        /// <summary>
        /// Andy => 觀查目前是否有 Exception.
        /// </summary>
        public bool HasExcception => exception != null;
    }

    public static partial class F
    {
        /// <summary>
        /// Andy -> Return(value, Box(value))
        /// </summary>
        public static Box<T> Boxing<T>(T value) => value;

        /// <summary>
        /// Andy -> Return(ex, Box(none)(有Exception) or Return(Box T(none))
        /// </summary>
        public static Box<T> Boxing<T>(Exception ex = null) => ex;
    }
}
