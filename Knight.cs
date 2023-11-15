using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess
{
    class Knight : Piece
    {
        // Table of values the piece gets for standing of a specific square
        private int[,] PositionValue = new int[8, 8] {
            { -10, -8, -6, -6, -6, -6, -8, -10 },
            {  -8, -4,  0,  1,  1,  0, -4,  -8 },
            {  -6,  1,  2,  3,  3,  2,  1,  -6 },
            {  -6,  1,  3,  3,  3,  3,  1,  -6 },
            {  -6,  1,  3,  3,  3,  3,  1,  -6 },
            {  -6,  1,  4,  3,  3,  4,  1,  -6 },
            { -8,  -4,  0,  2,  2,  0,  -4, -8 },
            { -10, -8, -6, -6, -6, -6, -8, -10 }};

        public Knight(PieceType name, PieceColor color) : base(name, color)
        {
        }
        public Knight(Knight copyPiece) : base(copyPiece)
        {
        }
        /// <summary>
        /// Makes a copy of the piece
        /// </summary>
        /// <returns>copy of the piece</returns>
        public override Piece Clone()
        {
            return new Knight(this);
        }

        /// <summary>
        /// Checks if the piece can move to the given coordinates on the board
        /// </summary>
        /// <param name="currentColumn">x coordinate of the piece current position</param>
        /// <param name="currentRow">y coordinate of the piece current position</param>
        /// <param name="targetColumn">x coordinate of the target position</param>
        /// <param name="targetRow">y coordinate of the target position position</param>
        /// <param name="isOccupied">bool whether or not target square is occupied by another piece</param>
        /// <param name="color">color of the piece that occupies target position,
        /// if the target position is not occupied, you would use enum variable PieceColor.None</param>
        /// <returns>
        /// true - if the given piece can be placed on the target square
        /// false - otherwise
        /// </returns>
        protected override bool IsValidMove(int currentColumn, int currentRow, int targetColumn, int targetRow, bool isOccupied, PieceColor color)
        {
            if (isOccupied && this.color == color) return false;
            return (Math.Abs(currentColumn - targetColumn) == 1 && Math.Abs(currentRow - targetRow) == 2)
                || (Math.Abs(currentColumn - targetColumn) == 2 && Math.Abs(currentRow - targetRow) == 1);
        }
        /// <summary>
        /// Gets the value for the piece standing on a given square on the board
        /// </summary>
        /// <param name="x">x coordinate of the board square</param>
        /// <param name="y">y coordinate of the board square</param>
        /// <returns>
        /// Board square value from the PositionValue table
        /// if the piece is black color return Board square value from the vertically 
        /// flipped PositionValue table
        /// </returns>
        public override int GetPositionValue(int x, int y)
        {
            if (color == PieceColor.Black) return PositionValue[7 - y, x];
            return PositionValue[y, x];
        }
        /// <summary>
        /// Finds all possible moves for the given piece on the board
        /// </summary>
        /// <param name="currentColumn">x coordinate of the piece current position</param>
        /// <param name="currentRow">y coordinate of the piece current position</param>
        /// <param name="chessBoard">The game board</param>
        /// <returns>
        /// The list of all possible moves of the given piece
        /// </returns>
        public override List<KeyValuePair<int, int>> MovementControl(int currentColumn, int currentRow, Tile[,] chessBoard)
        {
            List<KeyValuePair<int, int>> Moves = new List<KeyValuePair<int, int>>();

            Moves.AddRange(Movement(currentColumn, currentRow, 1, 2, chessBoard));
            Moves.AddRange(Movement(currentColumn, currentRow, 1, -2, chessBoard));
            Moves.AddRange(Movement(currentColumn, currentRow, -1, 2, chessBoard));
            Moves.AddRange(Movement(currentColumn, currentRow, -1, -2, chessBoard));
            Moves.AddRange(Movement(currentColumn, currentRow, 2, 1, chessBoard));
            Moves.AddRange(Movement(currentColumn, currentRow, 2, -1, chessBoard));
            Moves.AddRange(Movement(currentColumn, currentRow, -2, 1, chessBoard));
            Moves.AddRange(Movement(currentColumn, currentRow, -2, -1, chessBoard));

            return Moves;
        }
    }
}
