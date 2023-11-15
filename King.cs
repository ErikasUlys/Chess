using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess
{
    class King : Piece
    {
        // Table of values the piece gets for standing of a specific square
        private int[,] PositionValue = new int[8, 8] {
            { -6, -8, -8, -10, -10, -8, -8, -6 },
            { -6, -8, -8, -10, -10, -8, -8, -6 },
            { -6, -8, -8, -10, -10, -8, -8, -6 },
            { -6, -8, -8, -10, -10, -8, -8, -6 },
            { -4, -6, -6,  -8,  -8, -6, -6, -4 },
            { -2, -4, -4,  -4,  -4, -4, -4, -2 },
            {  1,  1,  0,   0,   0,  0,  1,  1 },
            {  4,  6,  3,   0,   0,  2,  6,  4 }};

        public King(PieceType name, PieceColor color) : base(name, color)
        {
        }
        public King(King copyPiece) : base(copyPiece)
        {
        }
        /// <summary>
        /// Makes a copy of the piece
        /// </summary>
        /// <returns>copy of the piece</returns>
        public override Piece Clone()
        {
            return new King(this);
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
            if(isOccupied && this.color == color) return false;
            return Math.Abs(currentColumn-targetColumn)<2 
                && Math.Abs(currentRow-targetRow)<2
                && !(currentRow == targetRow && currentColumn == targetColumn);
        }
        /// <summary>
        /// Checks is it possible to castle
        /// </summary>
        /// <param name="chessBoard">The game board</param>
        /// <param name="direction">direction in which we want to castle</param>
        /// <returns>
        /// true - if castling in the given direction is possible 
        /// (king and rook had not yet moved and the squares between those pieces are not occupied)
        /// false - otherwise
        /// </returns>
        private bool IsValidCastle(Tile[,] chessBoard, int direction)
        {
            if (firstMove)
            {
                int step = 4 + direction;
                int row = 0;
                if (color == PieceColor.White) row = 7;

                while (step > 0 && step < 7)
                {
                    if (chessBoard[step, row].GetPiece() != null)
                        return false;
                    step = step + direction;
                }

                if (chessBoard[step, row].GetPiece() != null
                    && chessBoard[step, row].GetPiece().name == PieceType.Rook
                    && chessBoard[step, row].GetPiece().firstMove)
                    return true;
            }
            return false;
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

            Moves.AddRange(Movement(currentColumn, currentRow, 0, 1, chessBoard));
            Moves.AddRange(Movement(currentColumn, currentRow, 0, -1, chessBoard));
            Moves.AddRange(Movement(currentColumn, currentRow, 1, 0, chessBoard));
            Moves.AddRange(Movement(currentColumn, currentRow, 1, 1, chessBoard));
            Moves.AddRange(Movement(currentColumn, currentRow, 1, -1, chessBoard));
            Moves.AddRange(Movement(currentColumn, currentRow, -1, 0, chessBoard));
            Moves.AddRange(Movement(currentColumn, currentRow, -1, 1, chessBoard));
            Moves.AddRange(Movement(currentColumn, currentRow, -1, -1, chessBoard));

            //conditions for castling
            if (IsValidCastle(chessBoard, -1) && Movement(currentColumn, currentRow, 0, -1, chessBoard).Count() > 0)
                Moves.Add(new KeyValuePair<int, int>(currentColumn - 2, currentRow));
            if (IsValidCastle(chessBoard, 1) && Movement(currentColumn, currentRow, 0, 1, chessBoard).Count() > 0)
                Moves.Add(new KeyValuePair<int, int>(currentColumn + 2, currentRow));

            return Moves;
        }
    }
}
