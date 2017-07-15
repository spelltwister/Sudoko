namespace Sudoko
{
    /// <summary>
    /// Recursive backtracking, brute force algorithm
    /// found at: https://stackoverflow.com/questions/16671791/sudoku-solver-algorithm-for-int
    /// </summary>
    /// <remarks>
    /// This class will solve a sudoko grid using serial recursion
    /// </remarks>
    public class SudokoSolver
    {
        private Grid SolvedGrid { get; set; }
        private Grid Grid { get; }
        private bool TriedSolving { get; set; }

        public SudokoSolver(Grid grid)
        {
            this.Grid = grid;
        }

        public int?[,] SolvePuzzle()
        {
            if (!this.TriedSolving)
            {
                this.SolvedGrid = Solve(this.Grid);
                this.TriedSolving = true;
            }

            return this.SolvedGrid?.Data; // TODO: make a copy
        }

        public static Grid Solve(Grid toSolve)
        {
            Grid copy = new Grid(toSolve.Data);
            return SolveRecurse(copy);
        }

        protected static Grid SolveRecurse(Grid grid)
        {
            int row, col;
            if (!grid.TryFindUnassignedLocation(out row, out col))
            {
                return grid;
            }

            for (int num = 1; num <= grid.Size; num++)
            {
                if (grid.TryAssign(row, col, num))
                {
                    // try to find a solution with the newly assigned value
                    Grid test = SolveRecurse(grid);
                    if (test != null)
                    {
                        return test;
                    }

                    // no solution could be found with the newly assigned value
                    // so it must not be correct, unassign it
                    grid.Unassign(row, col);
                }
            }

            // there was no solution for the given board
            return null;
        }
    }
}