using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess
{
    class Tile
    {
        private Piece piece;
        private bool isOccupied;

        public Tile()
        {
            piece = null;
            isOccupied = false; 
        }

        /// <summary>
        /// Sets the this.piece to the given piece
        /// and isOccupied to true
        /// </summary>
        /// <param name="piece">Piece class element</param>
        public void Add(Piece piece)
        {
            this.piece = piece;
            if(this.piece != null) isOccupied = true;
        }
        /// <summary>
        /// Sets piece and isOccupied to null
        /// </summary>
        public void Remove()
        {
            piece = null;
            isOccupied = false;
        }
        /// <summary>
        /// Gets piece element
        /// </summary>
        /// <returns>piece</returns>
        public Piece GetPiece() { return piece; }
        /// <summary>
        /// Gets isOccupied element
        /// </summary>
        /// <returns>isOccupied</returns>
        public bool IsOccupied() { return isOccupied; }
    }
}
