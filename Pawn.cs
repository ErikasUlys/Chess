using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess
{
    class Pawn : Piece
    {
        // Table of values the piece gets for standing of a specific square
        private int[,] PositionValue = new int[8, 8] { 
            {  0,  0,  0,  0,  0,  0,  0,  0 }, 
            { 10, 10, 10, 10, 10, 10, 10, 10 }, 
            {  2,  2,  4,  6,  6,  4,  2,  2 }, 
            {  1,  1,  2,  5,  5,  2,  1,  1 }, 
            {  0,  0,  0,  5,  5,  0,  0,  0 },
            {  1,  0, -2,  0,  0, -2,  0,  1 },
            {  1,  2,  2, -6, -6,  2,  2,  1 },
            {  0,  0,  0,  0,  0,  0,  0,  0} };

        public Pawn(PieceType name, PieceColor color) : base(name, color)
        {
        }
        public Pawn(Pawn copyPiece) : base(copyPiece)
        {
        }
        /// <summary>
        /// Makes a copy of the piece
        /// </summary>
        /// <returns>copy of the piece</returns>
        public override Piece Clone()
        {
            return new Pawn(this);
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
            int step = 1;
            if (firstMove) step = 2;

            if (isOccupied) return false;
            return (this.color == PieceColor.White && currentColumn == targetColumn && currentRow <= targetRow + step && currentRow > targetRow)
                || (this.color == PieceColor.Black && currentColumn == targetColumn && currentRow >= targetRow - step && currentRow < targetRow);
        }
        /// <summary>
        /// Checks if the capture with the pawn is possible
        /// </summary>
        /// <param name="currentRow">y coordinate of the piece current position</param>
        /// <param name="targetRow">y coordinate of the target position</param>
        /// <param name="isOccupied">bool whether or not the target square is occupied</param>
        /// <param name="color">color the piece occupying the target square, 
        /// if the target position is not occupied, you would use enum variable PieceColor.None</param>
        /// <returns>
        /// true - if the capture is possible
        /// false - otherwise
        /// </returns>
        private bool IsValidCapture(int currentRow, int targetRow, bool isOccupied, PieceColor color)
        {
            if (!isOccupied) return false;
            return ((this.color == PieceColor.White && currentRow == targetRow + 1)
                || (this.color == PieceColor.Black && currentRow == targetRow - 1))
                && this.color != color;
        }
        /// <summary>
        /// Checks if the EnPassant move is possible
        /// </summary>
        /// <param name="currentColumn">x coordinate of the piece current position</param>
        /// <param name="currentRow">y coordinate of the piece current position</param>
        /// <param name="targetColumn">x coordinate of the target position</param>
        /// <param name="targetRow">y coordinate of the target position</param>
        /// <returns>
        /// true - if the move is possible
        /// (EnPassant is possible when opponent pawn moves two squares and lands to the side of players pawn)
        /// false - otherwise
        /// </returns>
        public bool IsValidEnPassant(int currentColumn, int currentRow, int targetColumn, int targetRow)
        {
            if (currentColumn > 7 || currentColumn < 0 || targetColumn > 7 || targetColumn < 0) return false;
            return ((this.color == PieceColor.White && currentRow == targetRow + 1) 
                || (this.color == PieceColor.Black && currentRow == targetRow - 1))
                && Math.Abs(currentColumn - targetColumn) == 1;
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

            Moves.AddRange(Movement(currentColumn, currentRow, 1, 0, chessBoard));
            Moves.AddRange(Movement(currentColumn, currentRow, -1, 0, chessBoard));

            Moves.AddRange(PawnCapture(currentColumn, currentRow, -1, -1, chessBoard));
            Moves.AddRange(PawnCapture(currentColumn, currentRow, -1, 1, chessBoard));
            Moves.AddRange(PawnCapture(currentColumn, currentRow, 1, -1, chessBoard));
            Moves.AddRange(PawnCapture(currentColumn, currentRow, 1, 1, chessBoard));

            return Moves;
        }
        /// <summary>
        /// Finds possible pawn capture
        /// </summary>
        /// <param name="currentColumn">x coordinate of the piece current position</param>
        /// <param name="currentRow">y coordinate of the piece current position</param>
        /// <param name="direction_X">x direction</param>
        /// <param name="direction_Y">y direction</param>
        /// <param name="chessBoard">The game board</param>
        /// <returns>
        /// List with the possible capture
        /// </returns>
        private List<KeyValuePair<int, int>> PawnCapture(int currentColumn, int currentRow, int direction_X, int direction_Y, Tile[,] chessBoard)
        {
            List<KeyValuePair<int, int>> Moves = new List<KeyValuePair<int, int>>();

            int x = currentRow + direction_X;
            int y = currentColumn + direction_Y;
            //check if we are in game board bounds
            if (y > 7 || y < 0 || x > 7 || x < 0) return Moves;

            PieceColor color = PieceColor.None;
            if (chessBoard[y, x].GetPiece() != null) color = chessBoard[y, x].GetPiece().color;
            bool isOccupied = chessBoard[y, x].IsOccupied();

            if (IsValidCapture(currentRow, x, isOccupied, color))
            {
                Moves.Add(new KeyValuePair<int, int>(y, x));
            }
            return Moves;
        }  
    }
}
