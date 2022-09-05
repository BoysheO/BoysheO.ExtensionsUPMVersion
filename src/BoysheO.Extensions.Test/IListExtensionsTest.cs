﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using NUnit.Framework;

namespace BoysheO.Extensions.Test;

public class IListExtensionsTest
{
    [TestCase]
    public void QSort()
    {
        var rand = new Random();
        var list = new List<string>(rand.Next(0, 40000));
        var count = list.Capacity;
        var compareList = new List<int>(count);
        for (int i = 0; i < count; i++)
        {
            var v = rand.Next();
            list.Add(v.ToString());
            compareList.Add(v);
        }

        var clone = new List<string>(list);
        list.QSort(compareList, 0, count - 1);

        //valid
        clone.Sort((a, b) => int.Parse(a) - int.Parse(b));
        Assert.AreEqual(list, clone);
    }

    [TestCase]
    public void Main()
    {
        Random rand = new Random();
        int[] array = Enumerable.Range(0, 400000).Select(v => rand.Next(400000)).ToArray();
        var lst = new List<int>(array);

        var st = new Stopwatch();
        st.Start();
        int[] result = IListExtensions.KSsort(array, 0, array.Length - 1);
        st.Stop();
        var str = "q排序耗时 " + st.ElapsedMilliseconds;

        st.Restart();
        lst.Sort();
        st.Stop();
        var str2 = "l排序耗时 " + st.ElapsedMilliseconds;

        Assert.AreEqual(lst, array);

        var foo = str + str2;
        // Console.WriteLine(foo);
    }
}