using System;
using System.Drawing;
using System.Windows.Forms;

namespace Puzzle
{
    public partial class PuzzleView : Form
    {
        private FlowLayoutPanel panel;
        private Panel grid;
        private FlowLayoutPanel scorePanel;
        private Controller ctrl;
        private String imagePath;
        private Label scoreLabel;
        private int gridSize;
        private readonly int gridWidth = 440;
        

        /// <summary>
        /// ctor
        /// </summary>
        public PuzzleView()
        {
            InitializeComponent();
            Text = "Puzzle";
            Width = 700;
            Height = 500;
            LoadGameStart();
        }


        /// <summary>
        /// method that initilizes the start page of the game view
        /// </summary>
        private void LoadGameStart()
        {
            ctrl = new Controller();

            //initialize panel
            panel = new FlowLayoutPanel
            {
                Width = 700,
                Height = 500
            };

            //add label
            Label label = new Label()
            {
                Text = "Choose an image to start",
                TextAlign = ContentAlignment.MiddleCenter,
                BorderStyle = BorderStyle.FixedSingle,
                Width = 200,
                Height = 200,
                Margin = new Padding(250, 70, 250, 70)
            };
            label.Click += Label_Click;

            panel.Controls.Add(label);
            Controls.Add(panel);
        }


        private void Label_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.InitialDirectory = "Desktop";
                openFileDialog.Filter = "Image files (*.jpg, *.jpeg, *.jpe, *.jfif, *.png) | *.jpg; *.jpeg; *.jpe; *.jfif; *.png";
                openFileDialog.FilterIndex = 2;
                openFileDialog.RestoreDirectory = true;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    //Get the path of specified file
                    imagePath = openFileDialog.FileName;

                    //load image preview
                    var image = Image.FromFile(imagePath);
                    int size = image.Width < image.Height ? image.Width : image.Height;
                    var imageCropped = new Bitmap(200, 200);
                    var graphics = Graphics.FromImage(imageCropped);
                    graphics.DrawImage(image, new Rectangle(0, 0, 200, 200), new Rectangle(0, 0, size, size), GraphicsUnit.Pixel);
                    graphics.Flush();
                    var imagePreview = new PictureBox
                    {
                        Image = imageCropped,
                        Size = new Size(200, 200),
                        Margin = new Padding(250, 70, 250, 70),
                        BorderStyle = BorderStyle.FixedSingle,
                        SizeMode = PictureBoxSizeMode.StretchImage
                    };

                    //add comboBox
                    var comboBox = new ComboBox
                    {
                        Text = "Split image into..."
                    };
                    comboBox.Items.AddRange(new object[] 
                    {
                        "2",
                        "3",
                        "4",
                        "5",
                        "6",
                    });
                    int cbHorizontalMargin = (int)(0.5 * (panel.Width - comboBox.Width));
                    comboBox.Margin = new Padding(cbHorizontalMargin, 0, cbHorizontalMargin, 0);
                    comboBox.SelectedIndexChanged += ComboBox_SelectedIndexChanged;

                    //add elements to panel
                    panel.Controls.Clear();
                    panel.Controls.AddRange(new Control[]
                    {
                        imagePreview,
                        comboBox,
                    });
                }
            }
        }


        private void ComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            //set grid size
            var comboBox = (ComboBox)sender;
            gridSize = Convert.ToInt32(comboBox.SelectedItem);

            //add start button
            var startButton = new Button()
            {
                Text = "Start"
            };
            int startHorizontalMargin = (int)(0.5 * (panel.Width - startButton.Width));
            startButton.Margin = new Padding(startHorizontalMargin, 20, startHorizontalMargin, 20);
            startButton.Click += StartButton_Click;
            panel.Controls.Add(startButton);
        }


        private void StartButton_Click(object sender, EventArgs e)
        {
            ctrl.SplitImage(imagePath, gridSize);
            ctrl.ShuffleTiles();
            SetGameView();
            KeyPreview = true;
            KeyDown += PuzzleView_KeyDown;
            PreviewKeyDown += PuzzleView_PreviewKeyDown;
        }


        private void SetGameView()
        {
            panel.Controls.Clear();
            DrawGrid();

            scorePanel = new FlowLayoutPanel
            {
                Width = 200,
                Height = 500
            };

            //add image preview
            var image = Image.FromFile(imagePath);
            int size = image.Width < image.Height ? image.Width : image.Height;
            var imageCropped = new Bitmap(150, 150);
            var graphics = Graphics.FromImage(imageCropped);
            graphics.DrawImage(image, new Rectangle(0, 0, 150, 150), new Rectangle(0, 0, size, size), GraphicsUnit.Pixel);
            graphics.Flush();
            var imagePreview = new PictureBox
            {
                Image = imageCropped,
                Size = new Size(150, 150),
                Margin = new Padding(25, 60, 25, 0),
                BorderStyle = BorderStyle.FixedSingle,
                SizeMode = PictureBoxSizeMode.StretchImage
            };

            //add score
            var textLabel = new Label
            {
                Text = "Moves",
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Arial", 12),
                Size = new Size(150, 50),
                Margin = new Padding(25, 20, 25, 0)
            };

            scoreLabel = new Label
            {
                Text = ctrl.Score.ToString(),
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Arial", 24, FontStyle.Bold),
                Size = new Size(150, 50),
                Margin = new Padding(25, 5, 25, 0)
            };

            scorePanel.Controls.AddRange(new Control[]
            {
                imagePreview,
                textLabel,
                scoreLabel
            });

            panel.Controls.Add(scorePanel);
        }


        /// <summary>
        /// method that draws the grid using the backend Matrix from the controller
        /// </summary>
        private void DrawGrid()
        {
            grid = new Panel
            {
                Width = gridWidth,
                Height = gridWidth,
                Margin = new Padding(10, 10, 10, 10)
            };

            int size = gridWidth / gridSize;    //cellsize

            for (int i = 0; i < gridSize; i++)
            {
                for (int j = 0; j < gridSize; j++)
                {
                    var pictureBox = new PictureBox
                    {
                        Width = size,
                        Height = size,
                        BorderStyle = BorderStyle.FixedSingle,
                        Location = new Point(j * size, i * size),
                    };
                    if (i != ctrl.VoidJ || j != ctrl.VoidI)
                    {
                        pictureBox.Image = ctrl.Matrix[j, i].TileImage;
                    }
                    pictureBox.SizeMode = PictureBoxSizeMode.StretchImage;
                    grid.Controls.Add(pictureBox);
                }
            }
            panel.Controls.Add(grid);
        }


        private void PuzzleView_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Down:
                case Keys.Up:
                case Keys.Left:
                case Keys.Right:
                    e.IsInputKey = true;
                    break;
            }
        }


        private void PuzzleView_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Down:
                    {
                        ctrl.Move(0, -1);
                        ChangeMovedTileLocation();
                        scoreLabel.Text = ctrl.Score.ToString();
                        if (ctrl.GameOver())
                        {
                            EndGame();
                        }
                        break;
                    }
                case Keys.Up:
                    {
                        ctrl.Move(0, 1);
                        ChangeMovedTileLocation();
                        scoreLabel.Text = ctrl.Score.ToString();
                        if (ctrl.GameOver())
                        {
                            EndGame();
                        }
                        break;
                    }
                case Keys.Left:
                    {
                        ctrl.Move(1, 0);
                        ChangeMovedTileLocation();
                        scoreLabel.Text = ctrl.Score.ToString();
                        if (ctrl.GameOver())
                        {
                            EndGame();
                        }
                        break;
                    }
                case Keys.Right:
                    {
                        ctrl.Move(-1, 0);
                        ChangeMovedTileLocation();
                        scoreLabel.Text = ctrl.Score.ToString();
                        if (ctrl.GameOver())
                        {
                            EndGame();
                        }
                        break;
                    }
            }
        }


        /// <summary>
        /// method that visually shifts tiles which changed location in the Move method from the controller
        /// </summary>
        private void ChangeMovedTileLocation()
        {
            int index = 0;
            foreach (Control pb in grid.Controls)
            {
                double fraction = index / gridSize;
                int i = Convert.ToInt32(Math.Floor(fraction));
                int j = index % gridSize;

                if (i != ctrl.VoidJ || j != ctrl.VoidI)
                {
                    ((PictureBox)pb).Image = ctrl.Matrix[j, i].TileImage;
                }
                else
                {
                    ((PictureBox)pb).Image = null;
                }
                index++;
            }
        }


        /// <summary>
        /// method that executes on game over
        /// </summary>
        private void EndGame()
        {
            var image = Image.FromFile(imagePath);
            int size = image.Width < image.Height ? image.Width : image.Height;
            var imageCropped = new Bitmap(gridWidth, gridWidth);
            var graphics = Graphics.FromImage(imageCropped);
            graphics.DrawImage(image, new Rectangle(0, 0, gridWidth, gridWidth), new Rectangle(0, 0, size, size), GraphicsUnit.Pixel);
            graphics.Flush();
            var completedPicture = new PictureBox
            {
                Width = gridWidth,
                Height = gridWidth,
                Image = imageCropped,
            };
            grid.Controls.Clear();
            grid.Controls.Add(completedPicture);

            //add you won label
            var textLabel = new Label
            {
                Text = "You won!",
                TextAlign = ContentAlignment.MiddleCenter,
                ForeColor = Color.Olive,
                Font = new Font("Arial", 14),
                Size = new Size(150, 50),
                Margin = new Padding(25, 15, 25, 0)
            };

            //add restart button
            var restartButton = new Button()
            {
                Text = "Play again"
            };
            int restartHorizontalMargin = (int)(0.5 * (scorePanel.Width - restartButton.Width));
            restartButton.Margin = new Padding(restartHorizontalMargin, 10, restartHorizontalMargin, 20);
            restartButton.Click += RestartButton_Click;

            scorePanel.Controls.AddRange(new Control[]
            {
                textLabel,
                restartButton
            });
        }

        private void RestartButton_Click(object sender, EventArgs e)
        {
            Controls.Clear();
            LoadGameStart();
        }
    }
}
