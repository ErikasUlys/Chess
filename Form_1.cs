using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Chess
{
    public partial class Form1 : Form
    {
        private Color darkSquares = Color.FromArgb(195, 85, 75);
        private Color lightSquares = Color.FromArgb(245, 230, 210);
        private Panel[,] _chessBoardPanels;
        private Tile[,] chessBoard;
        private string selectedPiece;
        private bool playAsWhite = true;

        /// <summary>
        /// Loads the board to the winform
        /// </summary>
        private void LoadBoard()
        {
            HideLabel("Shadow", true);
            HideLabel("Result", true);

            this.Width = 1116;
            this.Height = 886;

            const int tileSize = 100;
            const int boardSize = 8;

            _chessBoardPanels = new Panel[boardSize, boardSize];

            for (int i = 0; i < boardSize; i++)
            {
                for (int j = 0; j < boardSize; j++)
                {
                    Panel newPanel = new Panel
                    {
                        Size = new Size(tileSize, tileSize),
                        Location = new Point(tileSize * i + 25, tileSize * j + 25)
                    };

                    newPanel.Controls.Add(SquareLocation(i, j));

                    if (chessBoard[i, j].IsOccupied())
                        newPanel.Controls.Add(PieceSprite(i, j));

                    Controls.Add(newPanel);
                    _chessBoardPanels[i, j] = newPanel;
                    //_chessBoardPanels[i, j].MouseClick += panel_Click;
                }
            }
        }
        /// <summary>
        /// Flips the board vertically
        /// </summary>
        /// <param name="changeSides">bool to change the color of the player</param>
        private void Flip(bool changeSides)
        {
            int boardSize = 8;

            for (int i = 0; i < boardSize / 2; i++)
            {
                for (int j = 0; j < boardSize; j++)
                {
                    Point tmp = _chessBoardPanels[i, j].Location;
                    _chessBoardPanels[i, j].Location = _chessBoardPanels[7 - i, 7 - j].Location;
                    _chessBoardPanels[7 - i, 7 - j].Location = tmp;
                }
            }
            if (changeSides)
                playAsWhite = !playAsWhite;
        }
        /// <summary>
        /// Makes a player move on a winform
        /// </summary>
        /// <param name="selectedPiece">coordinates of the selected piece</param>
        /// <param name="currentLocation">coordinates of target square</param>
        private void UpdateBoard(string selectedPiece, string currentLocation)
        {
            int source_i, source_j;
            Game.GetCoordinates(selectedPiece, out source_i, out source_j);

            int target_i, target_j;
            Game.GetCoordinates(currentLocation, out target_i, out target_j);
            Panel panel = _chessBoardPanels[target_i, target_j];
            
            //removes opponents pawn if EnPassant was performed
            if (Game.IsEnPassant(target_i, target_j, chessBoard[source_i, source_j].GetPiece().name))
            {
                RemovePictureBox(_chessBoardPanels[target_i, source_j]);
            }
            //moves rook if castling was performed
            else if (Game.IsCastle(source_i, target_i, chessBoard[source_i, source_j].GetPiece().name))
            {
                int direction = target_i - source_i;
                if (direction < 0)
                {
                    AddPictureBox(_chessBoardPanels[target_i + 1, target_j], 0, target_j);
                    RemovePictureBox(_chessBoardPanels[0, target_j]);
                }
                else
                {
                    AddPictureBox(_chessBoardPanels[target_i - 1, target_j], 7, target_j);
                    RemovePictureBox(_chessBoardPanels[7, target_j]);
                }
            }

            //makes the move on hte game board
            Game.Move(ref chessBoard, selectedPiece, currentLocation, true);
            AddPictureBox(panel, target_i, target_j);
            RemovePictureBox(_chessBoardPanels[source_i, source_j]);

            this.Refresh();
            selectedPiece = null;
        }
        /// <summary>
        /// Makes the AI move in winform
        /// </summary>
        private void AI_Move()
        {
            int depth = 3;
            string selectedPiece, currentLocation;

            Chess_ai.FindMove(depth, chessBoard, playAsWhite, out selectedPiece, out currentLocation);
            if (selectedPiece != "-1")
                UpdateBoard(selectedPiece, currentLocation);
        }

        /// <summary>
        /// Resets the boards colors
        /// </summary>
        /// <param name="lightSquares">color of the light squares</param>
        /// <param name="darkSquares">color of the dark squares</param>
        private void ResetTiles(Color lightSquares, Color darkSquares)
        {
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    Panel p = _chessBoardPanels[i, j];
                    if ((i + j) % 2 == 0)
                        PaintPanel(p, lightSquares);
                    else
                        PaintPanel(p, darkSquares);
                }
            }
        }
        /// <summary>
        /// Paints the panel with the given color in given opacity
        /// </summary>
        /// <param name="panel">Panel element</param>
        /// <param name="color">color</param>
        /// <param name="opacity">opacity</param>
        private void PaintPanelOpaque(Panel panel, Color color, double opacity)
        {
            int r = (int)(panel.BackColor.R * (1 - opacity) + color.R * opacity);
            int g = (int)(panel.BackColor.G * (1 - opacity) + color.G * opacity);
            int b = (int)(panel.BackColor.B * (1 - opacity) + color.B * opacity);

            panel.BackColor = Color.FromArgb(r, g, b);
        }
        /// <summary>
        /// Paints the given panel in given color
        /// </summary>
        /// <param name="panel">Panel element</param>
        /// <param name="color">color</param>
        private void PaintPanel(Panel panel, Color color)
        {
            panel.BackColor = color;
        }

        /// <summary>
        /// Creates new label element and sets its text to the gicen coordinates
        /// </summary>
        /// <param name="i">y coordinate</param>
        /// <param name="j">x coordinate</param>
        /// <returns>
        /// newly created label
        /// </returns>
        private Label SquareLocation(int i, int j)
        {
            Label squareLocation = new Label();

            squareLocation.Name = "Location";
            squareLocation.Text = Convert.ToString(i) + " " + Convert.ToString(j);
            squareLocation.Visible = false;

            return squareLocation;
        }
        /// <summary>
        /// Creates a new PictureBox and adds piece(by given coordinates) sprite to it
        /// </summary>
        /// <param name="i">y coordinate</param>
        /// <param name="j">x coordinate</param>
        /// <returns>
        /// newly created PictureBox
        /// </returns>
        private PictureBox PieceSprite(int i, int j)
        {
            const int spriteSize = 80;
            const int spriteOffset = 10;

            PictureBox sprite = new PictureBox();

            string piece = chessBoard[i, j].GetPiece().color + Convert.ToString(chessBoard[i, j].GetPiece().name);
            sprite.Image = Image.FromFile("../../Pieces/" + piece + ".png");

            sprite.Name = "Piece";
            sprite.SizeMode = PictureBoxSizeMode.Zoom;
            sprite.Size = new Size(spriteSize, spriteSize);
            sprite.Location = new Point(spriteOffset, spriteOffset);
            sprite.Enabled = false;

            return sprite;
        }
        /// <summary>
        /// Adds profile pictures to winform to given location
        /// </summary>
        /// <param name="x">x coordinate</param>
        /// <param name="y">y coordinate</param>
        /// <param name="name">name of the picture</param>
        private void ProfileIcon(int x, int y, string name)
        {
            PictureBox sprite = new PictureBox();
            sprite.Image = Image.FromFile("../../Pieces/" + name + ".png");

            sprite.SizeMode = PictureBoxSizeMode.Zoom;
            sprite.Size = new Size(80, 80);
            sprite.Location = new Point(x, y);
            this.Controls.Add(sprite);
        }
        /// <summary>
        /// Adds PictureBox to the Panel
        /// </summary>
        /// <param name="panel">Panel element</param>
        /// <param name="source_i">y coordinate</param>
        /// <param name="source_j">x coordinate</param>
        private void AddPictureBox(Panel panel, int source_i, int source_j)
        {
            if (panel.Controls.OfType<PictureBox>().Any(l => l.Name == "Piece"))
            {
                string pp = chessBoard[source_i, source_j].GetPiece().color + Convert.ToString(chessBoard[source_i, source_j].GetPiece().name);

                panel.Controls.OfType<PictureBox>()
                    .Single(l => l.Name == "Piece")
                    .Image = Image.FromFile("../../Pieces/" + pp + ".png"); ;
            }
            else
                panel.Controls.Add(PieceSprite(source_i, source_j));
        }
        /// <summary>
        /// Removes PictureBox from the Panel
        /// </summary>
        /// <param name="panel">Panel element</param>
        private void RemovePictureBox(Panel panel)
        {
            foreach (Control control in panel.Controls)
            {
                if (control.Name == "Piece")
                    panel.Controls.Remove(control);
            }
        }
        /// <summary>
        /// Creates the end game message
        /// </summary>
        private void FinishScreen()
        {
            Label shadow = new Label
            {
                Size = new Size(250, 150),
                Location = new Point(302, 352)
            };
            shadow.Name = "Shadow";
            shadow.BackColor = Color.FromArgb(90, 90, 90);
            Controls.Add(shadow);
            shadow.BringToFront();

            Label Result = new Label
            {
                Size = new Size(250, 150),
                Location = new Point(300, 350)
            };
            Result.Name = "Result";
            Result.Font = new Font("Arial", 16);
            Result.TextAlign = ContentAlignment.MiddleCenter;
            Result.BorderStyle = BorderStyle.FixedSingle;
            Result.BackColor = Color.FromArgb(230, 232, 229);

            Controls.Add(Result);
            Result.BringToFront();
        }
        /// <summary>
        /// Hides label
        /// </summary>
        /// <param name="name">name of the label</param>
        /// <param name="hide">bool to which to set label visibility</param>
        private void HideLabel(string name, bool hide)
        {
            this.Controls.OfType<Label>()
                    .Single(l => l.Name == name)
                    .Visible = !hide;
        }

    }
}
