using System;
using System.Collections.Generic;
using System.Linq;

namespace Sudoko
{
    public class Grid
    {
        public int?[,] Data { get; }
        public int Size { get; }
        public int SubSize { get; }
        private int _curC = 0;
        private int _curR = 0;
        public int Assigns { get; protected set; }

        public Grid(int sqrt) : this(new int?[sqrt*sqrt, sqrt*sqrt])
        {
        }

        public Grid(int?[,] data)
        {
            if (data == null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            this.Size = data.GetLength(0);
            if (this.Size != data.GetLength(1))
            {
                throw new ArgumentException("Grid is not square.");
            }

            double subSize = Math.Sqrt(this.Size);
            if (subSize % 1 != 0) // this is fine for small numbers
            {
                throw new ArgumentException("Grid is not a valid size.");
            }

            this.SubSize = (int) subSize;

            Data = data;

            if (!IsLegal())
            {
                throw new Exception("Illigal numbers populated!");
            }
        }

        public bool TryFindUnassignedLocation(out int row, out int col)
        {
            while (Data[_curR, _curC].HasValue)
            {
                _curC++;

                if (_curC == this.Size)
                {
                    _curR++;
                    _curC = 0;
                }

                if (_curR == this.Size)
                {
                    row = -1;
                    col = -1;
                    return false;
                }
            }

            row = _curR;
            col = _curC;

            return true;
        }

        protected bool NoConflicts(int row, int col, int num)
        {
            for (int r = 0; r < this.Size; ++r)
            {
                if (Data[r, col] == num)
                {
                    return false;
                }
            }

            for (int c = 0; c < this.Size; ++c)
            {
                if (Data[row, c] == num)
                {
                    return false;
                }
            }

            return GetSubSquare(row / this.SubSize, col / this.SubSize).All(x => !x.HasValue || x != num);
        }

        public bool TryAssign(int row, int col, int num)
        {
            if (NoConflicts(row, col, num))
            {
                this.Assigns = this.Assigns + 1;
                Data[row, col] = num;
                return true;
            }

            return false;
        }

        public void Unassign(int row, int col)
        {
            Data[row, col] = null;
            _curC = col;
            _curR = row;
        }

        protected IEnumerable<int?> GetSubSquare(int row, int col)
        {
            if (row >= this.SubSize)
            {
                throw new ArgumentOutOfRangeException(nameof(row));
            }

            if (col >= this.SubSize)
            {
                throw new ArgumentOutOfRangeException(nameof(col));
            }

            for (int rowPos = row * this.SubSize; rowPos < (row + 1) * this.SubSize; ++rowPos)
            {
                for (int colPos = col * this.SubSize; colPos < (col + 1) * this.SubSize; ++colPos)
                {
                    yield return Data[rowPos, colPos];
                }
            }
        }

        public bool IsLegal()
        {
            var container = new HashSet<int>();
            
            //vertical check 
            for (int c = 0; c < this.Size; ++c)
            {
                container.Clear();
                for (int r = 0; r < this.Size; ++r)
                {
                    if (Data[r, c].HasValue && !container.Add(Data[r, c].Value))
                    {
                        return false;
                    }
                }
            }

            // horizontal check
            for (int r = 0; r < this.Size; ++r)
            {
                container.Clear();
                for (int c = 0; c < this.Size; ++c)
                {
                    if (Data[r, c].HasValue && !container.Add(Data[r, c].Value))
                    {
                        return false;
                    }
                }
            }

            // square check
            for (int r = 0; r < this.SubSize; ++r)
            {
                container.Clear();
                for (int c = 0; c < this.SubSize; ++c)
                {
                    container.Clear();
                    foreach (var subSquareValue in GetSubSquare(r, c))
                    {
                        if (subSquareValue.HasValue && !container.Add(subSquareValue.Value))
                        {
                            return false;
                        }
                    }
                }
            }

            return true;
        }
    }
}