using System;
using System.Collections.Generic;

public static class DayFactory
{
    private static readonly Dictionary<int, IDay> _days = new()
    {
        { 1, new Day1() },
        {102, new Day1Part2() },
        { 2, new Day2() },
        {202, new Day2Part2() },
        {3, new Day3() },
        {302, new Day3Part2()},
        {4, new Day4()},
        {402, new Day4Part2()},
        {5, new Day5()},
        {502, new Day5Part2()},
        {6, new Day6()},
        {601, new Day6Visualization()},
        {602, new Day6Part2()},
        {603, new Day6Part2Visualization()},
        {7, new Day7()},
        {701, new Day7Visualization()},
        {702, new Day7Part2()},
        {703, new Day7Part2Visualization()},
    };

    public static IDay? GetDaySolution(int day)
    {
        return _days.ContainsKey(day) ? _days[day] : null;
    }
}