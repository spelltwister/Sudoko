using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SudokoTester
{
    using System.Diagnostics;
    using Sudoko;

    [TestClass]
    public class UnitTest1
    {
        // 1 solution in 65 ms
        public static int?[,] DifficultSudoku =
        {
            {    3, null,    4, null,    8, null, null, null, null },
            { null,    2,    9, null,    1, null, null,    8, null },
            {    1, null, null,    9, null,    3, null, null, null },
            {    9, null, null,    8, null, null,    6, null,    5 },
            { null, null, null, null, null, null, null, null, null },
            {    6, null,    8, null, null,    7, null, null,    1 },
            { null, null, null,    4, null,    6, null, null,    2 },
            { null,    5, null, null,    9, null,    7,    6, null },
            { null, null, null, null,    5, null,    8, null,    4 }
        };

        // 130 solutions in 550 ms
        public static int?[,] DifficultSudoku2 =
        {
            {    3, null,    4, null, null, null, null, null, null },
            { null,    2,    9, null,    1, null, null,    8, null },
            {    1, null, null,    9, null,    3, null, null, null },
            {    9, null, null,    8, null, null,    6, null,    5 },
            { null, null, null, null, null, null, null, null, null },
            {    6, null,    8, null, null,    7, null, null, null },
            { null, null, null,    4, null,    6, null, null,    2 },
            { null,    5, null, null,    9, null,    7,    6, null },
            { null, null, null, null, null, null,    8, null,    4 }
        };

        // 3543 solutions in 12 seconds
        public static int?[,] DifficultSudoku3 =
        {
            {    3, null,    4, null, null, null, null, null, null },
            { null,    2, null, null,    1, null, null,    8, null },
            {    1, null, null,    9, null,    3, null, null, null },
            { null, null, null,    8, null, null,    6, null,    5 },
            { null, null, null, null, null, null, null, null, null },
            {    6, null,    8, null, null,    7, null, null, null },
            { null, null, null,    4, null,    6, null, null,    2 },
            { null,    5, null, null,    9, null,    7,    6, null },
            { null, null, null, null, null, null,    8, null,    4 }
        };

        // 20073 solutions in 57 seconds
        public static int?[,] DifficultSudoku4 =
        {
            {    3, null,    4, null, null, null, null, null, null },
            { null,    2, null, null,    1, null, null,    8, null },
            {    1, null, null,    9, null,    3, null, null, null },
            { null, null, null,    8, null, null,    6, null,    5 },
            { null, null, null, null, null, null, null, null, null },
            {    6, null,    8, null, null, null, null, null, null },
            { null, null, null,    4, null,    6, null, null,    2 },
            { null,    5, null, null,    9, null,    7,    6, null },
            { null, null, null, null, null, null,    8, null,    4 }
        };

        // 46901 solutions in 142 seconds = 2 min 22 second
        public static int?[,] DifficultSudoku5 =
        {
            {    3, null,    4, null, null, null, null, null, null },
            { null,    2, null, null,    1, null, null,    8, null },
            {    1, null, null,    9, null,    3, null, null, null },
            { null, null, null,    8, null, null,    6, null,    5 },
            { null, null, null, null, null, null, null, null, null },
            {    6, null,    8, null, null, null, null, null, null },
            { null, null, null,    4, null,    6, null, null,    2 },
            { null,    5, null, null,    9, null, null,    6, null },
            { null, null, null, null, null, null,    8, null,    4 }
        };

        // 2344579 solutions in 3364 seconds = 56 min
        public static int?[,] DifficultSudoku6 =
        {
            {    3, null,    4, null, null, null, null, null, null },
            { null,    2, null, null,    1, null, null,    8, null },
            { null, null, null,    9, null,    3, null, null, null },
            { null, null, null, null, null, null,    6, null,    5 },
            { null, null, null, null, null, null, null, null, null },
            {    6, null,    8, null, null, null, null, null, null },
            { null, null, null,    4, null,    6, null, null, null },
            { null,    5, null, null,    9, null, null,    6, null },
            { null, null, null, null, null, null,    8, null,    4 }
        };

        [TestMethod]
        public void TestMethod1()
        {
            Grid g = new Grid(DifficultSudoku5);

            System.Diagnostics.Stopwatch sw = Stopwatch.StartNew();
            var solution = SudokoSolver.Solve(g);
            sw.Stop();

            string message = "Solution " + (solution == null ? "Not" : "") + $"Found in {sw.ElapsedMilliseconds} ms";
            Console.WriteLine(message);
        }

        [TestMethod]
        public void TestMethod2()
        {
            Grid g = new Grid(DifficultSudoku6);

            System.Diagnostics.Stopwatch sw = Stopwatch.StartNew();
            var solution = AllSudokoSolver.Solve(g);
            sw.Stop();

            string message = $"{solution.Count} Solution(s) Found in {sw.ElapsedMilliseconds} ms";
            Console.WriteLine(message);
        }
    }
}