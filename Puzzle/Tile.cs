using System.Drawing;

namespace Puzzle
{
    /// <summary>
    /// represents a Tile from the game, storing the image and the coresponding initial indices in the tile matrix
    /// </summary>
    class Tile
    {
        public Image TileImage { get; set; }
        public int I { get; set; }
        public int J { get; set; }

        public Tile(Image tileImage, int i, int j)
        {
            TileImage = tileImage;
            I = i;
            J = j;
        }
    }
}
