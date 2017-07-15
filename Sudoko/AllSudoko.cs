using System.Collections.Generic;

namespace Sudoko
{
    /// <summary>
    /// Recursive backtracking, brute force algorithm
    /// found at: https://stackoverflow.com/questions/16671791/sudoku-solver-algorithm-for-int
    /// </summary>
    /// <remarks>
    /// This class will solve a sudoko grid using serial recursion
    /// </remarks>
    public class AllSudokoSolver
    {
        private List<Grid> SolvedGrids { get; set; }
        private Grid Grid { get; }
        private bool TriedSolving { get; set; }

        public AllSudokoSolver(Grid grid)
        {
            this.Grid = grid;
        }

        public List<Grid> SolvePuzzle()
        {
            if (!this.TriedSolving)
            {
                this.SolvedGrids = Solve(this.Grid);
                this.TriedSolving = true;
            }

            return this.SolvedGrids; // TODO: make a copy
        }

        public static List<Grid> Solve(Grid toSolve)
        {
            Grid copy = new Grid(toSolve.Data);
            var solutions = new List<Grid>();
            SolveRecurse(copy, solutions);
            return solutions;
        }

        protected static void SolveRecurse(Grid grid, List<Grid> solutions)
        {
            int row, col;
            if (!grid.TryFindUnassignedLocation(out row, out col))
            {
                solutions.Add(new Grid(grid.Data));
                return;
            }

            for (int num = 1; num <= grid.Size; num++)
            {
                if (grid.TryAssign(row, col, num))
                {
                    // try to find solutions with the newly assigned value
                    SolveRecurse(grid, solutions);

                    grid.Unassign(row, col);
                }
            }
        }
    }
}