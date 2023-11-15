using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess
{
    class Bishop : Piece
    {
        // Table of values the piece gets for standing of a specific square
        private int[,] PositionValue = new int[8, 8] {
            { -4, -2, -2, -2, -2, -2, -2, -4 },
            { -2,  0,  0,  0,  0,  0,  0, -2 },
            { -2,  0,  1,  2,  2,  1,  0, -2 },
            { -2,  1,  1,  2,  2,  1,  1, -2 },
            { -2,  0,  3,  2,  2,  3,  0, -2 },
            { -2,  2,  2,  2,  2,  2,  2, -2 },
            { -2,  3,  0,  0,  0,  0,  3, -2 },
            { -4, -2, -2, -2, -2, -2, -2, -4} };



        public Bishop(PieceType name, PieceColor color) : base(name, color)
        {
        }
        public Bishop(Bishop copyPiece) : base(copyPiece)
        {
        }
        /// <summary>
        /// Makes a copy of the piece
        /// </summary>
        /// <returns>copy of the piece</returns>
        public override Piece Clone()
        {
            return new Bishop(this);
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
            return true;
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
            return DiagonalMovement(currentColumn, currentRow, chessBoard);
        }
    }
}
