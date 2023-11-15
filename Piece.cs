using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess
{
    enum PieceColor
    {
        White = 1,
        Black = -1,
        None = 0
    }
    enum PieceType
    {
        King = 1800,
        Queen = 180,
        Rook = 100,
        Knight = 60,
        Bishop = 61,
        Pawn = 20
    }

    abstract class Piece
    {
        public PieceType name { get; private set; }
        public PieceColor color { get; private set; }
        public bool firstMove { get; private set; }

        public Piece(PieceType name, PieceColor color)
        {
            this.name = name;
            this.color = color;
            firstMove = true;
        }
        public Piece(Piece copyPiece)
        {
            this.name = copyPiece.name;
            this.color = copyPiece.color;
            this.firstMove = copyPiece.firstMove;
        }

        /// <summary>
        /// Sets the firstMove to false
        /// </summary>
        public void Moved() { firstMove = false; }

        /// <summary>
        /// Makes a copy of the piece
        /// </summary>
        /// <returns>copy of the piece</returns>
        public abstract Piece Clone();
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
        protected abstract bool IsValidMove(int currentColumn, int currentRow, int targerColumn, int targetRo, bool occupied, PieceColor color);
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
        public abstract int GetPositionValue(int x, int y);
        /// <summary>
        /// Finds all possible moves for the given piece on the board
        /// </summary>
        /// <param name="currentColumn">x coordinate of the piece current position</param>
        /// <param name="currentRow">y coordinate of the piece current position</param>
        /// <param name="chessBoard">The game board</param>
        /// <returns>
        /// The list of all possible moves of the given piece
        /// </returns>
        public abstract List<KeyValuePair<int, int>> MovementControl(int currentColumn, int currentRow, Tile[,] chessBoard);

        /// <summary>
        /// Finds all possible diagonal moves
        /// </summary>
        /// <param name="currentColumn">x coordinate of the piece current position</param>
        /// <param name="currentRow">y coordinate of the piece current position</param>
        /// <param name="chessBoard">The game board</param>
        /// <returns>
        /// List of possible diagonal moves
        /// </returns>
        protected List<KeyValuePair<int, int>> DiagonalMovement(int currentColumn, int currentRow, Tile[,] chessBoard)
        {
            List<KeyValuePair<int, int>> Moves = new List<KeyValuePair<int, int>>();

            Moves.AddRange(Movement(currentColumn, currentRow, 1, 1, chessBoard));
            Moves.AddRange(Movement(currentColumn, currentRow, 1, -1, chessBoard));
            Moves.AddRange(Movement(currentColumn, currentRow, -1, 1, chessBoard));
            Moves.AddRange(Movement(currentColumn, currentRow, -1, -1, chessBoard));

            return Moves;
        }
        /// <summary>
        /// Finds all possible horizontal moves
        /// </summary>
        /// <param name="currentColumn">x coordinate of the piece current position</param>
        /// <param name="currentRow">y coordinate of the piece current position</param>
        /// <param name="chessBoard">The game board</param>
        /// <returns>
        /// List of possible horizontal moves
        /// </returns>
        protected List<KeyValuePair<int, int>> HorizontalMovement(int currentColumn, int currentRow, Tile[,] chessBoard)
        {
            List<KeyValuePair<int, int>> Moves = new List<KeyValuePair<int, int>>();

            Moves.AddRange(Movement(currentColumn, currentRow, 1, 0, chessBoard));
            Moves.AddRange(Movement(currentColumn, currentRow, -1, 0, chessBoard));
            Moves.AddRange(Movement(currentColumn, currentRow, 0, 1, chessBoard));
            Moves.AddRange(Movement(currentColumn, currentRow, 0, -1, chessBoard));

            return Moves;
        }
        /// <summary>
        /// Finds all possible moves in the given direction
        /// </summary>
        /// <param name="currentColumn">x coordinate of the piece current position</param>
        /// <param name="currentRow">y coordinate of the piece current position</param>
        /// <param name="direction_X">x direction</param>
        /// <param name="direction_Y">y direction</param>
        /// <param name="chessBoard">The game board</param>
        /// <returns>
        /// List of all possible moves in the given direction
        /// </returns>
        protected List<KeyValuePair<int, int>> Movement(int currentColumn, int currentRow, int direction_X, int direction_Y, Tile[,] chessBoard)
        {
            List<KeyValuePair<int, int>> Moves = new List<KeyValuePair<int, int>>();

            int steps = 1;
            int x = currentRow + direction_X;
            int y = currentColumn + direction_Y;
            //check if we are in game board bounds
            if (y > 7 || y < 0 || x > 7 || x < 0) return Moves;

            PieceColor color = PieceColor.None;
            if (chessBoard[y, x].GetPiece() != null) color = chessBoard[y, x].GetPiece().color;
            bool isOccupied = chessBoard[y, x].IsOccupied();


            while (IsValidMove(currentColumn, currentRow, y, x, isOccupied, color))
            {
                Moves.Add(new KeyValuePair<int, int>(y, x));
                steps++;
                color = PieceColor.None;

                x = currentRow + (direction_X * steps);
                y = currentColumn + (direction_Y * steps);

                //check if we are in game board bounds
                if (y > 7 || y < 0 || x > 7 || x < 0 || isOccupied)
                    break;

                if (chessBoard[y, x].GetPiece() != null)
                    color = chessBoard[y, x].GetPiece().color;

                isOccupied = chessBoard[y, x].IsOccupied();
            }

            return Moves;
        }
    }
}
