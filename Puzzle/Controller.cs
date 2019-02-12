using System;
using System.Drawing;


namespace Puzzle
{
    class Controller
    {
        public Tile[,] Matrix { get; set; }
        public int VoidI { get; set; }
        public int VoidJ { get; set; }
        public int Score { get; set; }


        /// <summary>
        /// method that splits the selected image into a matrix of Tiles
        /// </summary>
        /// <param name="imagePath">path of selected image </param> 
        /// <param name="gridSize"> number of rows and columns to split the image into </param> 
        public void SplitImage(String imagePath, int gridSize)
        {
            //load image and crop it to a 1:1 ratio
            var image = Image.FromFile(imagePath);
            int w = image.Width;
            int h = image.Height;
            int size = w < h ? w / gridSize : h / gridSize;     

            //split Image image into pieces
            var imageArray = new Image[gridSize * gridSize];
            for (int i = 0; i < gridSize; i++)
            {
                for (int j = 0; j < gridSize; j++)
                {
                    var index = i * gridSize + j;
                    imageArray[index] = new Bitmap(size, size);
                    var graphics = Graphics.FromImage(imageArray[index]);
                    graphics.DrawImage(image, new Rectangle(0, 0, size, size), new Rectangle(i * size, j * size, size, size), GraphicsUnit.Pixel);
                    graphics.Flush();
                }
            }

            //create a tile for each image and assign it to the Matrix
            Matrix = new Tile[gridSize,gridSize];
            for (int i = 0; i < gridSize; i++)
            {
                for (int j = 0; j < gridSize; j++)
                {
                    Matrix[i, j] = new Tile(imageArray[i * gridSize + j], i, j);
                }
            }

            //eliminate one tile
            VoidI = gridSize - 1;
            VoidJ = gridSize - 1;
            Matrix[VoidI, VoidJ] = null;
        }


        /// <summary>
        /// method that shuffles the tiles randomly
        /// </summary>
        public void ShuffleTiles()
        {
            Random rand = new Random();
            int i = 0;
            double gridsize = Math.Sqrt(Matrix.Length);
            //shuffle at least 100 times, until the empty tile is at bottom right corner
            while (i < 100 || VoidI != gridsize - 1 || VoidJ != gridsize - 1)
            {
                int randomHShift = rand.Next(-1, 2);
                int randomVShift = rand.Next(-1, 2);
                if (Math.Abs(randomHShift + randomVShift) == 1)
                {
                    Move(randomHShift, randomVShift);
                    i++;
                }
            }
            Score = 0;
        }


        /// <summary>
        /// method that moves the coresponding tile into the empty space
        /// </summary>
        /// <param name="horizontalShift"> specifies horizontal movement direction </param> 
        /// <param name="verticatShift"> specifies vertical movement direction </param>
        public void Move(int horizontalShift, int verticatShift)
        {
            //get indices of tile to move
            int movedTileI = VoidI + horizontalShift;
            int movedTileJ = VoidJ + verticatShift;

            //ensure that tile indices are not out of Matrix bounds
            if ((movedTileI >= 0 && movedTileI < Math.Sqrt(Matrix.Length)) && (movedTileJ >= 0 && movedTileJ < Math.Sqrt(Matrix.Length)))
            {
                Matrix[VoidI, VoidJ] = Matrix[movedTileI, movedTileJ];
                Matrix[movedTileI, movedTileJ] = null;
                VoidI = movedTileI;
                VoidJ = movedTileJ;
                Score++;
            }
        }


        /// <summary>
        /// method that checks if the place of all tiles in the Matrix matches their initial place
        /// </summary>
        /// <returns> true when condition is met for all tiles, which means the game is over,
        ///           false otherwise   </returns>
        public bool GameOver()
        {
            for (int i = 0; i < Math.Sqrt(Matrix.Length); i++)
            {
                for (int j = 0; j < Math.Sqrt(Matrix.Length); j++)
                {
                    if (Matrix[i, j] != null && (Matrix[i, j].I != i || Matrix[i, j].J != j))
                    {
                        return false;
                    }
                }
            }
            return true;
        }
    }
}
