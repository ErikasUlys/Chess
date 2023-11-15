using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace Chess
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            this.Text = "Chess";
            this.MaximizeBox = false;

            ProfileIcon(855, 35, "bot");
            ProfileIcon(855, 735, "player");

            FinishScreen();
            chessBoard = new Tile[8, 8];
            Game.ResetGame(ref chessBoard);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            LoadBoard();
            ResetTiles(lightSquares, darkSquares);
            this.BackColor = Color.FromArgb(42, 43, 45);
        }
        private void panel_Click(object sender, EventArgs e)
        {
            ResetTiles(lightSquares, darkSquares);
            Panel panel = (Panel)sender;

            string currentLocation = panel.Controls.OfType<Label>().Single(l => l.Name == "Location").Text;
            bool isPiece = panel.Controls.OfType<PictureBox>().Any(l => l.Name == "Piece");



            if (selectedPiece != null && Game.IsValidMove(selectedPiece, currentLocation, chessBoard))
            {
                UpdateBoard(selectedPiece, currentLocation);
                AI_Move();
            }
            else if (Game.IsMyTurn(currentLocation, chessBoard, playAsWhite))
            {
                if (isPiece)
                {
                    selectedPiece = currentLocation;
                    PaintPanelOpaque(panel, Color.OrangeRed, 0.8);
                }
                else
                    selectedPiece = null;

                List<KeyValuePair<int, int>> Moves = Game.PossibleMoves(chessBoard, currentLocation, false);
                foreach (KeyValuePair<int, int> move in Moves)
                {
                    PaintPanelOpaque(_chessBoardPanels[move.Key, move.Value], Color.Red, 0.6);
                }
            }

            string text = Game.GameEnding(ref chessBoard);
            if (text != GameState.playOn)
            {
                for (int i = 0; i < 8; i++)
                {
                    for (int j = 0; j < 8; j++)
                    {
                        _chessBoardPanels[i, j].MouseClick -= panel_Click;
                    }
                }
                HideLabel("Shadow", false);
                HideLabel("Result", false);
                this.Controls.OfType<Label>()
                    .Single(l => l.Name == "Result").Text = text;
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            button2.Enabled = true;
            button3.Enabled = true;

            foreach (Control c in this.Controls.OfType<Control>().Reverse())
            {
                if (c is Panel)
                    this.Controls.Remove(c);
            }

            Game.ResetGame(ref chessBoard);
            LoadBoard();
            ResetTiles(lightSquares, darkSquares);

            if (!playAsWhite)
                Flip(false);
        }
        private void button2_Click(object sender, EventArgs e)
        {
            Flip(true);
        }
        private void button3_Click(object sender, EventArgs e)
        {
            button1.Enabled = true;
            button2.Enabled = false;
            button3.Enabled = false;

            if (!playAsWhite)
                AI_Move();


            for(int i=0; i<8; i++)
            {
                for(int j=0; j < 8; j++)
                {
                    _chessBoardPanels[i, j].MouseClick += panel_Click;
                }
            }
        }
    }
}
