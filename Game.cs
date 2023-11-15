using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Chess
{
    /// <summary>
    /// class to store const game states
    /// </summary>
    static class GameState
    {
        public const string whiteWon = "White won!";
        public const string blackWon = "Black won!";
        public const string stalemate = "Draw by stalemate";
        public const string insufficiantMaterial = "Draw due to insufficiant material";
        public const string repetition = "Draw by repetition";
        public const string move50 = "Draw by 50 move rule";
        public const string playOn = "play on";
    }

    class Game
    {
        //-------------------------------------------------------------------
        //Game control
        //-------------------------------------------------------------------
        public static bool isWhiteTurn = true;

        /// <summary>
        /// Determines if its player's or computer's turn
        /// </summary>
        /// <param name="pieceCoordinates">coordinates of teh piece</param>
        /// <param name="chessBoard">The game board</param>
        /// <param name="playAsWhite">bool whether or not player plays as white</param>
        /// <returns>
        /// true - if it is players turn
        /// false - otherwise
        /// </returns>
        public static bool IsMyTurn(string pieceCoordinates, Tile[,] chessBoard, bool playAsWhite)
        {
            int y, x;
            GetCoordinates(pieceCoordinates, out y, out x);

            if (chessBoard[y, x].GetPiece() == null)
            {

                if ((isWhiteTurn && playAsWhite)
                || (!isWhiteTurn && !playAsWhite))
                    return true;
                else
                    return false;
            }
            return ((isWhiteTurn && chessBoard[y, x].GetPiece().color == PieceColor.White && playAsWhite)
                || (!isWhiteTurn && chessBoard[y, x].GetPiece().color == PieceColor.Black && !playAsWhite));

        }
        /// <summary>
        /// Sets the pieces on the board to the starting position
        /// </summary>
        /// <param name="chessBoard">The game board</param>
        public static void ResetGame(ref Tile[,] chessBoard)
        {
            isWhiteTurn = true;
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    chessBoard[j, i] = new Tile();
                    if (i == 1) chessBoard[j, 1].Add(new Pawn(PieceType.Pawn, PieceColor.Black));
                    if (i == 6) chessBoard[j, 6].Add(new Pawn(PieceType.Pawn, PieceColor.White));
                }
            }

            chessBoard[0, 0].Add(new Rook(PieceType.Rook, PieceColor.Black));
            chessBoard[1, 0].Add(new Knight(PieceType.Knight, PieceColor.Black));
            chessBoard[2, 0].Add(new Bishop(PieceType.Bishop, PieceColor.Black));
            chessBoard[3, 0].Add(new Queen(PieceType.Queen, PieceColor.Black));
            chessBoard[4, 0].Add(new King(PieceType.King, PieceColor.Black));
            chessBoard[5, 0].Add(new Bishop(PieceType.Bishop, PieceColor.Black));
            chessBoard[6, 0].Add(new Knight(PieceType.Knight, PieceColor.Black));
            chessBoard[7, 0].Add(new Rook(PieceType.Rook, PieceColor.Black));

            chessBoard[0, 7].Add(new Rook(PieceType.Rook, PieceColor.White));
            chessBoard[1, 7].Add(new Knight(PieceType.Knight, PieceColor.White));
            chessBoard[2, 7].Add(new Bishop(PieceType.Bishop, PieceColor.White));
            chessBoard[3, 7].Add(new Queen(PieceType.Queen, PieceColor.White));
            chessBoard[4, 7].Add(new King(PieceType.King, PieceColor.White));
            chessBoard[5, 7].Add(new Bishop(PieceType.Bishop, PieceColor.White));
            chessBoard[6, 7].Add(new Knight(PieceType.Knight, PieceColor.White));
            chessBoard[7, 7].Add(new Rook(PieceType.Rook, PieceColor.White));
        }
        /// <summary>
        /// Makes a move on the game board
        /// </summary>
        /// <param name="chessBoard">The game board</param>
        /// <param name="selectedPiece">selected piece coordinates</param>
        /// <param name="targetSquare">target square coordinates</param>
        /// <param name="realMove">bool whether it is an actual move in the game or just a calculation</param>
        public static void Move(ref Tile[,] chessBoard, string selectedPiece, string targetSquare, bool realMove)
        {
            int fromY, fromX;
            GetCoordinates(selectedPiece, out fromY, out fromX);

            int toY, toX;
            GetCoordinates(targetSquare, out toY, out toX);

            Piece piece = chessBoard[fromY, fromX].GetPiece();

            try
            {
                piece.Moved();
                if (piece is Pawn)
                {
                    if (realMove)
                        fiftyMoves = 0;
                    //Makes a special capture id EnPassant was performed
                    if (IsEnPassant(toY, toX, piece.name))
                        chessBoard[toY, fromX].Remove();
                    //Creates a possibility for an EnPassant next move
                    if (Math.Abs(fromX - toX) == 2 && realMove)
                    {
                        PossibleEnPassant = new KeyValuePair<int, int>(toY, fromX + ((toX - fromX) / 2));
                        EnPassantColor = chessBoard[fromY, fromX].GetPiece().color;
                    }
                    //Pawn promotion
                    if ((piece.color == PieceColor.White && toX == 0) || (piece.color == PieceColor.Black && toX == 7))
                        piece = new Queen(PieceType.Queen, piece.color);
                }

                if (piece is King)
                {
                    if (IsCastle(fromY, toY, piece.name))
                    {
                        //if the move is castle moves rook
                        int direction = toY - fromY;
                        if (direction < 0)
                        {
                            chessBoard[toY + 1, toX].Add(chessBoard[0, toX].GetPiece());
                            chessBoard[0, toX].Remove();
                        }
                        else
                        {
                            chessBoard[toY - 1, toX].Add(chessBoard[7, toX].GetPiece());
                            chessBoard[7, toX].Remove();
                        }
                    }
                }
                //Resets possibility for EnPassant
                if (realMove && PossibleEnPassant.Value != -1
                    && chessBoard[fromY, fromX].GetPiece().color != EnPassantColor)
                    PossibleEnPassant = new KeyValuePair<int, int>(-1, -1);

                if (realMove && chessBoard[toY, toX].GetPiece() != null)
                    fiftyMoves = 0;

                chessBoard[toY, toX].Add(piece);
                chessBoard[fromY, fromX].Remove();

                if (realMove)
                {
                    isWhiteTurn = !isWhiteTurn;
                    fiftyMoves++;
                    previousPositions.Add(GetPositionHashCode(chessBoard));
                }
            }
            catch (Exception ex) { }
        }    

        /// <summary>
        /// Creates a copy of the board position
        /// </summary>
        /// <param name="chessBoard">The agme board</param>
        /// <returns>
        /// The copy to the board
        /// </returns>
        public static Tile[,] CopyBoard(Tile[,] chessBoard)
        {
            Tile[,] copy = new Tile[8, 8];
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    copy[j, i] = new Tile();
                    if (chessBoard[j, i].GetPiece() != null)
                        copy[j, i].Add(chessBoard[j, i].GetPiece().Clone());
                }
            }
            return copy;
        }
        /// <summary>
        /// Gets kings position
        /// </summary>
        /// <param name="chessBoard">The game board</param>
        /// <param name="color">color of the king</param>
        /// <returns>
        /// position of the given color king
        /// </returns>
        public static KeyValuePair<int, int> GetKingPosition(Tile[,] chessBoard, PieceColor color)
        {
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    var piece = chessBoard[j, i].GetPiece();
                    if (piece != null && piece.name == PieceType.King
                        && piece.color == color) return new KeyValuePair<int, int>(j, i);
                }
            }
            return new KeyValuePair<int, int>(-1, -1);
        }
        /// <summary>
        /// Converts coordinates from string to ints
        /// </summary>
        /// <param name="coordinates">string with the coordinates</param>
        /// <param name="i">y coordinate</param>
        /// <param name="j">x coordinate</param>
        public static void GetCoordinates(string coordinates, out int i, out int j)
        {
            string[] c = coordinates.Split(' ');
            i = Convert.ToInt32(c[0]);
            j = Convert.ToInt32(c[1]);
        }
        /// <summary>
        /// Gets teh hashcode of the current position on the board
        /// </summary>
        /// <param name="chessBoard">The game board</param>
        /// <returns>
        /// hashcode of the current position on the board
        /// </returns>
        public static int GetPositionHashCode(Tile[,] chessBoard)
        {
            int hash = 0;
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    int pieceHash = 0;
                    if (chessBoard[j, i].GetPiece() != null)
                        pieceHash = chessBoard[j, i].GetPiece().GetHashCode();
                    hash += chessBoard[j, i].GetHashCode() * pieceHash;
                }
            }
            return hash;
        }


        //-------------------------------------------------------------------
        //Moves
        //-------------------------------------------------------------------
        /// <summary>
        /// Gets all the possible moves by all one players pieces
        /// </summary>
        /// <param name="chessBoard">The game board</param>
        /// <param name="player">player color(1 - white, -1 - black)</param>
        /// <param name="ignoreCheck">bool should the search ignore checks</param>
        /// <returns>
        /// List of pieces and thier possible moves
        /// </returns>
        public static List<KeyValuePair<string, string>> AllPlayerMoves(Tile[,] chessBoard, int player, bool ignoreCheck)
        {
            List<KeyValuePair<string, string>> Moves = new List<KeyValuePair<string, string>>();
            PieceColor color = (PieceColor)player;

            for (int i = 7; i >= 0; i--)
            {
                for (int j = 7; j >= 0; j--)
                {
                    var piece = chessBoard[j, i].GetPiece();
                    if (piece != null && piece.color == color)
                    {
                        string currentPosition = j + " " + i;
                        var M = PossibleMoves(chessBoard, currentPosition, ignoreCheck);
                        foreach (var move in M)
                        {
                            string targetPosition = move.Key + " " + move.Value;
                            Moves.Add(new KeyValuePair<string, string>(currentPosition, targetPosition));
                        }
                    }
                }
            }

            return Moves;
        }
        /// <summary>
        /// Orderes moves
        /// </summary>
        /// <param name="chessBoard">The game board</param>
        /// <param name="Moves">List of moves</param>
        /// <returns>
        /// Ordered list of moves
        /// </returns>
        public static List<KeyValuePair<string, string>> MoveOrdering(Tile[,] chessBoard, List<KeyValuePair<string, string>> Moves)
        {
            List<KeyValuePair<string, string>> Ordered = new List<KeyValuePair<string, string>>();
            //first adds all moves that are captures
            for (int i = 0; i < Moves.Count - 1; i++)
            {
                int x, y;
                GetCoordinates(Moves[i].Value, out y, out x);
                if (chessBoard[y, x].GetPiece() != null)
                {
                    Ordered.Add(Moves[i]);
                    Moves.RemoveAt(i);
                    i--;
                }
            }
            //then adds all movse that result in checks
            for (int i = 0; i < Moves.Count - 1; i++)
            {
                int x, y;
                GetCoordinates(Moves[i].Key, out y, out x);

                int xx, yy;
                GetCoordinates(Moves[i].Value, out yy, out xx);

                if (KingInCheck(chessBoard, Moves[i].Key, new KeyValuePair<int, int>(yy, xx), chessBoard[y, x].GetPiece().color))
                {
                    Ordered.Add(Moves[i]);
                    Moves.RemoveAt(i);
                    i--;
                }
            }
            //finally adds the rest moves
            Ordered.AddRange(Moves);
            return Ordered;
        }
        /// <summary>
        /// Finds all the possible moves from the given square
        /// </summary>
        /// <param name="chessBoard">The game board</param>
        /// <param name="coordinates">coordinates of the piece</param>
        /// <param name="ignoreCheck">bool whether of not to ignore checks</param>
        /// <returns>
        /// List of moves by the piece on the given square
        /// </returns>
        public static List<KeyValuePair<int, int>> PossibleMoves(Tile[,] chessBoard, string coordinates, bool ignoreCheck)
        {
            List<KeyValuePair<int, int>> Moves = new List<KeyValuePair<int, int>>();
            List<KeyValuePair<int, int>> LegalMoves = new List<KeyValuePair<int, int>>();

            int x, y;
            GetCoordinates(coordinates, out y, out x);

            Piece piece = chessBoard[y, x].GetPiece();
            try
            {
                Moves = piece.MovementControl(y, x, chessBoard);
                if (piece.name == PieceType.Pawn && PossibleEnPassant.Value != -1)
                {
                    Pawn pawn = piece as Pawn;
                    if (pawn.color != EnPassantColor && pawn.IsValidEnPassant(y, x, PossibleEnPassant.Key, PossibleEnPassant.Value))
                        Moves.Add(PossibleEnPassant);
                }
            }
            catch (Exception ex) { }

            if (!ignoreCheck)
            {
                foreach (KeyValuePair<int, int> move in Moves)
                {
                    if (!KingInCheck(chessBoard, coordinates, move, piece.color))
                        LegalMoves.Add(move);
                }
            }
            else LegalMoves = Moves;

            return LegalMoves;
        }
        /// <summary>
        /// Checks if the move is legal
        /// </summary>
        /// <param name="pieceCoords">coordinates of the piece we want to move</param>
        /// <param name="targetCoordinates">coordinates of the target square</param>
        /// <param name="chessBoard">The game board</param>
        /// <returns>
        /// true - if the move we want to perform is legal
        /// false - otherwise
        /// </returns>
        public static bool IsValidMove(string pieceCoords, string targetCoordinates, Tile[,] chessBoard)
        {
            int y, x;
            GetCoordinates(targetCoordinates, out y, out x);

            List<KeyValuePair<int, int>> Moves = PossibleMoves(chessBoard, pieceCoords, false);

            foreach (KeyValuePair<int, int> move in Moves)
            {
                if (move.Key == y && move.Value == x) return true;
            }
            return false;
        }
        /// <summary>
        /// Checks if the given square is in check be the player with the given color 
        /// </summary>
        /// <param name="copy">The game board</param>
        /// <param name="color">the color of the player</param>
        /// <param name="squarePosition">coordinates of the target square</param>
        /// <returns>
        /// true - if the given square is under attack by the given color player
        /// false - otherwise
        /// </returns>
        public static bool SquareIsChecked(Tile[,] copy, PieceColor color, KeyValuePair<int, int> squarePosition)
        {
            int opponent = -Convert.ToInt32(color);
            List<KeyValuePair<string, string>> Moves = AllPlayerMoves(copy, opponent, true);

            foreach (KeyValuePair<string, string> move in Moves)
            {
                if (move.Value.Equals(squarePosition.Key + " " + squarePosition.Value))
                    return true;
            }

            return false;
        }
        /// <summary>
        /// Checks if the king is in check
        /// </summary>
        /// <param name="chessBoard">The game board</param>
        /// <param name="pieceCoordinates">piece coordinates</param>
        /// <param name="move">move that given piece want to make</param>
        /// <param name="color">color of the king</param>
        /// <returns>
        /// true - if after a wanted move king would be in check or player want to do illegal castle move
        /// false - otherwise
        /// </returns>
        public static bool KingInCheck(Tile[,] chessBoard, string pieceCoordinates, KeyValuePair<int, int> move, PieceColor color)
        {
            Tile[,] copy = CopyBoard(chessBoard);
            string target = move.Key + " " + move.Value;
            Move(ref copy, pieceCoordinates, target, false);

            int i, j;
            GetCoordinates(pieceCoordinates, out i, out j);
            //king is in check
            if (SquareIsChecked(copy, color, GetKingPosition(copy, color)))
                return true;

            //restricts castling 
            if (IsCastle(i, move.Key, chessBoard[i, j].GetPiece().name))
            {
                //if king is currently in check
                if (SquareIsChecked(chessBoard, color, GetKingPosition(chessBoard, color)))
                    return true;

                //or would castle through check
                if (i - move.Key < 0)
                {
                    if (SquareIsChecked(copy, chessBoard[i, j].GetPiece().color, new KeyValuePair<int, int>(i + 1, j))
                        || SquareIsChecked(copy, chessBoard[i, j].GetPiece().color, new KeyValuePair<int, int>(i + 2, j)))
                        return true;
                }
                else
                {
                    if (SquareIsChecked(copy, chessBoard[i, j].GetPiece().color, new KeyValuePair<int, int>(i - 1, j))
                        || SquareIsChecked(copy, chessBoard[i, j].GetPiece().color, new KeyValuePair<int, int>(i - 2, j)))
                        return true;
                }
            }

            return false;
        }

        //-------------------------------------------------------------------
        //Special game rules
        //-------------------------------------------------------------------
        private static KeyValuePair<int, int> PossibleEnPassant = new KeyValuePair<int, int>(-1, -1);
        private static PieceColor EnPassantColor = PieceColor.None;

        /// <summary>
        /// Checks if the move is castle
        /// </summary>
        /// <param name="fromY">current y coordinates</param>
        /// <param name="toY">target y coordinates</param>
        /// <param name="piece">piece type</param>
        /// <returns>
        /// whether or not the move is castle
        /// </returns>
        public static bool IsCastle(int fromY, int toY, PieceType piece)
        {
            return (Math.Abs(fromY - toY) == 2 && piece == PieceType.King);
        }
        /// <summary>
        /// Checks if the move is EnPassant
        /// </summary>
        /// <param name="toY">target y coordinates</param>
        /// <param name="toX">target x coordinates</param>
        /// <param name="piece">piece type</param>
        /// <returns>
        /// whether or not the move is EnPassant
        /// </returns>
        public static bool IsEnPassant(int toY, int toX, PieceType piece)
        {
            return toY == PossibleEnPassant.Key && toX == PossibleEnPassant.Value
                && piece == PieceType.Pawn;
        }

        //------------------------------------------------------------------
        //Ending conditions
        //------------------------------------------------------------------
        private static int fiftyMoves = 0;
        private static List<int> previousPositions = new List<int>();

        /// <summary>
        /// Gets the state of the game
        /// </summary>
        /// <param name="chessBoard">The game board</param>
        /// <returns>
        /// the state of the game
        /// </returns>
        public static string GameEnding(ref Tile[,] chessBoard)
        {
            int player = 1;
            if (!isWhiteTurn) player = -1;

            int moveCount = AllPlayerMoves(chessBoard, player, false).Count(); // realus judesiai kiekis
            var checkedKing = SquareIsChecked(chessBoard, (PieceColor)player, GetKingPosition(chessBoard, (PieceColor)player));

            if (moveCount == 0)
            {
                if (checkedKing)
                {
                    int color = -player;
                    if ((PieceColor)color == PieceColor.Black)
                        return GameState.blackWon;
                    else
                        return GameState.whiteWon;
                }
                else
                    return GameState.stalemate;
            }
            if (fiftyMoves == 100)
                return GameState.move50;
            if (IsInsufficienMaterial(chessBoard))
                return GameState.insufficiantMaterial;
            if (IsRepetition(chessBoard))
                return GameState.repetition;

            return GameState.playOn;
        }
        /// <summary>
        /// checks if there are enough pieces in the game to make a checkmate
        /// </summary>
        /// <param name="chessBoard">The game board</param>
        /// <returns>
        /// true - if at least one player still has pawns or more that 1 minor piece
        /// false - otherwise
        /// </returns>
        public static bool IsInsufficienMaterial(Tile[,] chessBoard)
        {
            int whitePieces = 0;
            int blackPieces = 0;

            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    var piece = chessBoard[j, i].GetPiece();
                    try
                    {
                        if (!(piece.name == PieceType.King || piece.name == PieceType.Knight
                            || piece.name == PieceType.Bishop))
                            return false;
                        if (piece.name == PieceType.Knight || piece.name == PieceType.Bishop)
                        {
                            if (piece.color == PieceColor.White) whitePieces++;
                            else blackPieces++;
                        }
                    }
                    catch (Exception ex) { }
                }
            }

            if (whitePieces < 2 && blackPieces < 2) return true;
            return false;
        }
        /// <summary>
        /// Checks if the position repeated in the gale 3 times
        /// </summary>
        /// <param name="chessBoard">The game board</param>
        /// <returns>
        /// true - if the position in the game repeated 3 or more times
        /// false - otherwise
        /// </returns>
        public static bool IsRepetition(Tile[,] chessBoard)
        {
            int x = GetPositionHashCode(chessBoard);
            int counter = 0;

            foreach (int position in previousPositions)
            {
                if (position == x) counter++;
            }
            if (counter >= 3) return true;
            return false;
        }
    }
}
