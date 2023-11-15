using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess
{
    class Chess_ai
    {
        /// <summary>
        /// Finds the best move in the game position
        /// </summary>
        /// <param name="depth">depth of how many moves ahead is calculated</param>
        /// <param name="chessBoard">The game board</param>
        /// <param name="playAsWhite">bool whether or not it is white turn</param>
        /// <param name="piecePosition">position of the piece that makes the best move</param>
        /// <param name="targetSquare">target square to which to move the piece</param>
        public static void FindMove(int depth, Tile[,] chessBoard, bool playAsWhite, out string piecePosition, out string targetSquare)
        {
            int bot = 1;
            if (playAsWhite)
                bot = -1;
            //set the piece and target position to -1 in case there are no legal moves possible
            piecePosition = targetSquare = "-1";

            var Moves = Game.AllPlayerMoves(chessBoard, bot, false);
            int bestPosition = int.MinValue;

            foreach(var move in Moves)
            {
                Tile[,] copy = Game.CopyBoard(chessBoard);
                Game.Move(ref copy, move.Key, move.Value, false);

                //searches for the move that leads to the best position evaluation
                int position = MinMaxSearch(depth - 1, copy, playAsWhite, int.MinValue, int.MaxValue);
                if (position * bot > bestPosition)
                {
                    bestPosition = position * bot;
                    piecePosition = move.Key;
                    targetSquare = move.Value;
                }
            }
        }
        /// <summary>
        /// Search that finds the best possible position evaluation in hte given depth
        /// </summary>
        /// <param name="depth">depth of the search</param>
        /// <param name="chessBoard">The game board</param>
        /// <param name="isWhiteTurn">bool whether or not it is white turn</param>
        /// <param name="alpha">minimal position evaluation</param>
        /// <param name="beta">maximal position evaluation</param>
        /// <returns>Position evaluation after a given depth of moves</returns>
        public static int MinMaxSearch(int depth, Tile[,] chessBoard, bool isWhiteTurn, int alpha, int beta)
        {
            int multiplier = 0;
            int pieceColor = -1;
            if (isWhiteTurn)
                pieceColor = 1;

            //base case where we just return position evaluation
            if (depth == 0)
                return -pieceColor * multiplier + PositionEvaluation(chessBoard);

            bool check = Game.SquareIsChecked(chessBoard, (PieceColor)pieceColor, Game.GetKingPosition(chessBoard, (PieceColor)pieceColor));
            var Moves = Game.AllPlayerMoves(chessBoard, pieceColor, false);

            //if there are no possible moves e.g. due to checkmate
            //we return the position evaluation before depth reaches 0 
            if (Moves.Count() == 0)
            {
                //encouraging position with no moves as it leads to checkmate
                multiplier = 200;
                if (check)
                    multiplier = 2000;
                return -pieceColor * multiplier + PositionEvaluation(chessBoard);
            }

            if (isWhiteTurn)
            {
                //when it is white's turn the search looks for the highest possible position evaluation
                Moves = Game.MoveOrdering(chessBoard, Moves);
                int bestPosition = int.MinValue;
                foreach(var move in Moves)
                {
                    Tile[,] copy = Game.CopyBoard(chessBoard);
                    Game.Move(ref copy, move.Key, move.Value, false);

                    bestPosition = Math.Max(bestPosition, MinMaxSearch(depth-1, copy, !isWhiteTurn, alpha, beta));

                    //alpha-beta pruning allows to stop branch search if the we find a move that leads to 
                    //a better possition for our opponent then we already had 
                    alpha = Math.Max(alpha, bestPosition);
                    if (beta <= alpha)
                        return bestPosition;
                }
                return bestPosition;
            }
            else
            {
                //when it is black's turn the search looks for the lowest possible possition evaluation
                int bestPosition = int.MaxValue;
                foreach (var move in Moves)
                {
                    Tile[,] copy = Game.CopyBoard(chessBoard);
                    Game.Move(ref copy, move.Key, move.Value, false);

                    bestPosition = Math.Min(bestPosition, MinMaxSearch(depth - 1, copy, !isWhiteTurn, alpha, beta));

                    //alpha-beta pruning allows to stop branch search if the we find a move that leads to 
                    //a better possition for our opponent then we already had 
                    beta = Math.Min(beta, bestPosition);
                    if (beta <= alpha)
                        return bestPosition;
                }
                return bestPosition;
            }
        }        
        /// <summary>
        /// Evaluates the position on the board,
        /// looking at how many, which type pieces are on the board and their positioning
        /// equal position would have a value of 0, white pieces add to this value and black subtracts from it
        /// </summary>
        /// <param name="chessBoard">The game board</param>
        /// <returns>
        /// value that represents position evaluation
        /// </returns>
        public static int PositionEvaluation(Tile[,] chessBoard)
        {
            int position = 0;

            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    var piece = chessBoard[j, i].GetPiece();
                    if(piece != null)
                        position += (int)piece.color * ((int)piece.name + piece.GetPositionValue(j, i));
                }
            }

            return position;
        }
    }
}
