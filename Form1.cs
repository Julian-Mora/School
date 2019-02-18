using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GameOfLife2._0
{
    public partial class Form1 : Form
    {
        // The universe array
        bool[,] universe = new bool[5, 5];
        bool[,] universe2 = new bool[5, 5];

        bool notTorus = true;
        float storedCellX = -1;
        float storedCellY = -1;


        // Drawing colors
        Color gridColor = Color.Black;
        Color cellColor = Color.Gray;
        Color deadCell = Color.White;

        // The Timer class
        Timer timer = new Timer();

        // Generation count
        int generations = 0;

        public Form1()
        {
            InitializeComponent();

            //Read in the Settings
            cellColor = Properties.Settings.Default.CellColor;
            gridColor = Properties.Settings.Default.GridColor;
            universe = new bool[Properties.Settings.Default.GridSize, Properties.Settings.Default.GridSize];
            universe2 = new bool[Properties.Settings.Default.GridSize, Properties.Settings.Default.GridSize];

            notTorus = Properties.Settings.Default.notTorus;

            // Setup the timer
            timer.Interval = 100; // milliseconds
            timer.Tick += Timer_Tick;
            timer.Enabled = false; // start timer running
        }

        #region WHAT TO EDIT
        // Calculate the next generation of cells
        private void NextGeneration()
        {
            // Increment generation count
            generations++;

            // Update status strip generations
            toolStripStatusLabelGenerations.Text = "Generations = " + generations.ToString();

            //get neighbor count and apply all the rules here
            for (int y = 0; y < universe.GetLength(1); y++)
            {
                // Iterate through the universe in the x, left to right
                for (int x = 0; x < universe.GetLength(0); x++)
                {
                    // find neighbor count and place it in new universe
                    int howManyAlive = 0;
                    if (notTorus)
                        howManyAlive = getNeighbor(x, y);
                    if (!notTorus)
                        howManyAlive = torusGetNeighbor(x, y);
                    //use the rules and say if the current thing is alive or dead
                    if (universe[x, y] == true && howManyAlive < 2)
                        universe2[x, y] = false;

                    if (universe[x, y] == true && howManyAlive > 3)
                        universe2[x, y] = false;

                    if (universe[x, y] == true && (howManyAlive == 3 || howManyAlive == 2))
                        universe2[x, y] = true;

                    if (!universe[x, y] && (howManyAlive == 3))
                        universe2[x, y] = true;

                }
            }

            //swap the array with alive and dead things
            for (int i = 0; i < universe.GetLength(1); i++)
            {
                for (int j = 0; j < universe.GetLength(0); j++)
                {
                    swap(ref universe[i, j], ref universe2[i, j]);
                }
            }
            graphicsPanel1.Invalidate();
        }

        private void swap(ref bool u1, ref bool u2)
        {
            u1 = u2;
        }

        private int torusGetNeighbor(int x, int y)
        {
            int count = 0;
            //get top left
            if (x > 0 && y > 0)
                if (universe[x - 1, y - 1] == true)
                    count++;
            //get top middle
            if (y > 0)
                if (universe[x, y - 1] == true)
                    count++;
            //get top right
            if (x < universe.GetLength(0) - 1 && y > 0)
                if (universe[x + 1, y - 1] == true)
                    count++;
            //get middle left
            if (x > 0)
                if (universe[x - 1, y] == true)
                    count++;
            //get middle right
            if (x == universe.GetLength(0))
                if (universe[x + 1, y] == true)
                    count++;
            //get bottom left
            if (x == 0 && y == universe.GetLength(1))
                if (universe[universe.GetLength(0), universe.GetLength(1)])
                    count++;
            //get bottom middle
            if (y == universe.GetLength(1))
                if (universe[x, 0])
                    count++;
            //get bottom right
            if (x == universe.GetLength(0) && y == universe.GetLength(1))
                if (universe[0, 0])
                    count++;
            return count;
        }

        private int getNeighbor(int x, int y)
        {
            int count = 0;
            //get top left
            if (x > 0 && y > 0)
                if (universe[x - 1, y - 1] == true)
                    count++;
            //get top middle
            if (y > 0)
                if (universe[x, y - 1] == true)
                    count++;
            //get top right
            if (x < universe.GetLength(0) - 1 && y > 0)
                if (universe[x + 1, y - 1] == true)
                    count++;
            //get middle left
            if (x > 0)
                if (universe[x - 1, y] == true)
                    count++;
            //get middle right
            if (x < universe.GetLength(0) - 1)
                if (universe[x + 1, y] == true)
                    count++;
            //get bottom left
            if (x > 0 && y < universe.GetLength(1) - 1)
                if (universe[x - 1, y + 1])
                    count++;
            //get bottom middle
            if (y < universe.GetLength(1) - 1)
                if (universe[x, y + 1])
                    count++;
            //get bottom right
            if (x < universe.GetLength(0) - 1 && y < universe.GetLength(1) - 1)
                if (universe[x + 1, y + 1])
                    count++;
            return count;
        }
        #endregion

        #region graphic things
        // The event called by the timer every Interval milliseconds.
        private void Timer_Tick(object sender, EventArgs e)
        {
            NextGeneration();
        }

        private void graphicsPanel1_Paint(object sender, PaintEventArgs e)
        {
            // Calculate the width and height of each cell in pixels
            // CELL WIDTH = WINDOW WIDTH / NUMBER OF CELLS IN X
            float cellWidth = (float)graphicsPanel1.ClientSize.Width / universe.GetLength(0);
            // CELL HEIGHT = WINDOW HEIGHT / NUMBER OF CELLS IN Y
            float cellHeight = (float)graphicsPanel1.ClientSize.Height / universe.GetLength(1);

            // A Pen for drawing the grid lines (color, width)
            Pen gridPen = new Pen(gridColor, 2);

            // A Brush for filling living cells interiors (color)
            Brush cellBrush = new SolidBrush(cellColor);

            // A Brush for filling a dead cell (color)
            Brush deadBrush = new SolidBrush(deadCell);

            // Iterate through the universe in the y, top to bottom
            for (int y = 0; y < universe.GetLength(1); y++)
            {
                // Iterate through the universe in the x, left to right
                for (int x = 0; x < universe.GetLength(0); x++)
                {
                    // A rectangle to represent each cell in pixels
                    RectangleF cellRect = RectangleF.Empty;
                    cellRect.X = x * cellWidth;
                    cellRect.Y = y * cellHeight;
                    cellRect.Width = cellWidth;
                    cellRect.Height = cellHeight;

                    // Fill the cell with a brush if alive
                    if (universe[x, y] == true)
                    {
                        e.Graphics.FillRectangle(cellBrush, cellRect);
                    }
                    else
                    {
                        e.Graphics.FillRectangle(deadBrush, cellRect);
                    }

                    // Outline the cell with a pen
                    e.Graphics.DrawRectangle(gridPen, cellRect.X, cellRect.Y, cellRect.Width, cellRect.Height);
                }
            }

            // Cleaning up pens and brushes
            gridPen.Dispose();
            cellBrush.Dispose();
        }

        private void graphicsPanel1_MouseClick(object sender, MouseEventArgs e)
        {
            // If the left mouse button was clicked
            if (e.Button == MouseButtons.Left)
            {
                // Calculate the width and height of each cell in pixels
                float cellWidth = (float)graphicsPanel1.ClientSize.Width / universe.GetLength(0);
                float cellHeight = (float)graphicsPanel1.ClientSize.Height / universe.GetLength(1);

                // Calculate the cell that was clicked in
                // CELL X = MOUSE X / CELL WIDTH
                float x = e.X / cellWidth;
                // CELL Y = MOUSE Y / CELL HEIGHT
                float y = e.Y / cellHeight;

                // Toggle the cell's state
                if (y > universe.GetLength(1) - 1)
                    y = universe.GetLength(1) - 1;
                if (x > universe.GetLength(0) - 1)
                    x = universe.GetLength(0) - 1;
                int a = (int)x;
                int b = (int)y;
                universe[a, b] = !universe[a, b];

                // Tell Windows you need to repaint
                graphicsPanel1.Invalidate();
            }
        }
        #endregion

        
        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //set the array to empty
            for (int y = 0; y < universe.GetLength(1); y++)
            {
                // Iterate through the universe in the x, left to right
                for (int x = 0; x < universe.GetLength(0); x++)
                {
                    universe[x, y] = false;
                }
            }
            // redraw with invalidate
            //reset the generation 
            generations = 0;
            graphicsPanel1.Invalidate();
        }
        
        // start and stop button
        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            timer.Enabled = !timer.Enabled;
            if (timer.Enabled)
            {
                toolStripButton1.Image = GameOfLife2._0.Properties.Resources.playbtn;
            }
            else
            {
                toolStripButton1.Image = GameOfLife2._0.Properties.Resources.pausebtn;
            }
        }

        private void changeSizeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            for (int y = 0; y < universe.GetLength(1); y++)
            {
                // Iterate through the universe in the x, left to right
                for (int x = 0; x < universe.GetLength(0); x++)
                {
                    universe[x, y] = false;
                }
            }
            timer.Enabled = false;
            int a = 0;
            int b = 0;


            universe = new bool[a, b];
            universe2 = new bool[a, b];
        }
        // 100x100 box button

        private void x100BoxToolStripMenuItem_Click(object sender, EventArgs e)
        {
            universe = new bool[100, 100];
            universe2 = new bool[100, 100];
            generations = 0;
            toolStripStatusLabelGenerations.Text = "Generations = " + generations.ToString();
            graphicsPanel1.Invalidate();
        }

        // torus button
        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            notTorus = !notTorus;
        }

        // 10x10 box button
        private void x10BoxToolStripMenuItem_Click(object sender, EventArgs e)
        {
            universe = new bool[10, 10];
            universe2 = new bool[10, 10];
            generations = 0;
            toolStripStatusLabelGenerations.Text = "Generations = " + generations.ToString();
            graphicsPanel1.Invalidate();
        }

        // step to the next generation button
        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            NextGeneration();
        }

        // in progress for highlight for 
        private void graphicsPanel1_MouseMove(object sender, MouseEventArgs e)
        {
            //// Calculate the width and height of each cell in pixels
            //float cellWidth = (float)graphicsPanel1.ClientSize.Width / universe.GetLength(0);
            //float cellHeight = (float)graphicsPanel1.ClientSize.Height / universe.GetLength(1);


            //// Calculate the cell that was clicked in
            //// CELL X = MOUSE X / CELL WIDTH
            //float x = e.X / cellWidth;
            //// CELL Y = MOUSE Y / CELL HEIGHT
            //float y = e.Y / cellHeight;

            //if (storedCellX == -1 || storedCellY == -1)
            //{
            //    storedCellX = x;
            //    storedCellY = y;
            //}
                

            //// store the cell and check if it goes outside it
            //if (x == storedCellX && y == storedCellY)
            //{
            //        // Toggle the cell's state
            //        if (y > universe.GetLength(1) - 1)
            //            y = universe.GetLength(1) - 1;
            //        if (x > universe.GetLength(0) - 1)
            //            x = universe.GetLength(0) - 1;
            //        int a = (int)x;
            //        int b = (int)y;
            //        universe[a, b] = !universe[a, b];

            //        // Tell Windows you need to repaint
            //        graphicsPanel1.Refresh();
            //}
        }

        private void newToolStripButton_Click(object sender, EventArgs e)
        {
            universe = new bool[universe.GetLength(0), universe.GetLength(1)];
            universe2 = new bool[universe.GetLength(0), universe.GetLength(1)];
            generations = 0;
            toolStripStatusLabelGenerations.Text = "Generations = " + generations.ToString();
            graphicsPanel1.Invalidate();
        }

        private void colorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ColorDialog dlg = new ColorDialog();
            dlg.Color = cellColor;
            if (DialogResult.OK == dlg.ShowDialog())
            {
                cellColor = dlg.Color;
                graphicsPanel1.Invalidate();
            }

        }

        private void xBoxToolStripMenuItem_Click(object sender, EventArgs e)
        {
            universe = new bool[10, 10];
            universe2 = new bool[10, 10];
            generations = 0;
            toolStripStatusLabelGenerations.Text = "Generations = " + generations.ToString();
            graphicsPanel1.Invalidate();
        }

        private void newToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            universe = new bool[universe.GetLength(0), universe.GetLength(1)];
            universe2 = new bool[universe.GetLength(0), universe.GetLength(1)];
            generations = 0;
            toolStripStatusLabelGenerations.Text = "Generations = " + generations.ToString();
            graphicsPanel1.Invalidate();
        }

        private void boxToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //call another form to change size
        }

        private void settingsToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            //Settings in the context menu
        }

        private void gridColorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //change grid color
            ColorDialog dlg = new ColorDialog();
            dlg.Color = cellColor;
            if (DialogResult.OK == dlg.ShowDialog())
            {
                gridColor = dlg.Color;
                graphicsPanel1.Invalidate();
            }
        }

        private void cellColorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //change cell colors
            ColorDialog dlg = new ColorDialog();
            dlg.Color = cellColor;
            if (DialogResult.OK == dlg.ShowDialog())
            {
                cellColor = dlg.Color;
                graphicsPanel1.Invalidate();
            }
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            //Update The Settings Here
            Properties.Settings.Default.CellColor = cellColor;
            Properties.Settings.Default.GridColor = gridColor;
            Properties.Settings.Default.notTorus = notTorus;
            Properties.Settings.Default.GridSize = universe.GetLength(0);
            Properties.Settings.Default.Timer = timer.Enabled;
            //writing to the file
            Properties.Settings.Default.Save();
        }

        private void resetToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.Reset();
            cellColor = Properties.Settings.Default.CellColor;
            gridColor = Properties.Settings.Default.GridColor;
            universe = new bool[Properties.Settings.Default.GridSize, Properties.Settings.Default.GridSize];
            universe2 = new bool[Properties.Settings.Default.GridSize, Properties.Settings.Default.GridSize];
            notTorus = Properties.Settings.Default.notTorus;
            timer.Enabled = Properties.Settings.Default.Timer;
            graphicsPanel1.Invalidate();

        }

        private void reloadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.Reload();
            cellColor = Properties.Settings.Default.CellColor;
            gridColor = Properties.Settings.Default.GridColor;
            universe = new bool[Properties.Settings.Default.GridSize, Properties.Settings.Default.GridSize];
            universe2 = new bool[Properties.Settings.Default.GridSize, Properties.Settings.Default.GridSize];
            notTorus = Properties.Settings.Default.notTorus;
            timer.Enabled = Properties.Settings.Default.Timer;
            graphicsPanel1.Invalidate();
        }
    }
}

