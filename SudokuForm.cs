using Sudoku.Constants;
using Sudoku.Helpers;
using Sudoku.Models;
using System;
using System.Text;
using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Reflection;

namespace Sudoku
{
    /// <summary>
    /// Sudoku Form Events Implementation Class.
    /// </summary>
    public partial class SudokuForm : Form
    {
        int ticks = 0;
        bool timerStarted = false;
        bool gridCleared = false;
        int previousGridSize = 9;
        string gridMode = Constants.Constants.Easy;
        bool errorMode = false;
        bool noteMode = false;
        Grid grid;
        private int[] gridarr = new int[81];
        bool solved;

        readonly List<Label> cellControls = new List<Label>();
       
        /// SudokuForm Constructor
        public SudokuForm() => InitializeComponent();

       
        private void SudokuForm_Load(object sender, EventArgs e)
        {
            cmbBoxMode.SelectedIndex = 0;
            cmbBoxGrid.SelectedIndex = 0;
        }


        /// Timer Tick Event
        private void Timer_Tick(object sender, EventArgs e)
        {
            ticks = timerStarted ? ticks + 1 : 0;
            lblTimer.Text = TimeSpan.FromSeconds(ticks).ToString(@"hh\:mm\:ss");
            if (gridCleared) { gridCleared = false; timer.Stop(); }
        }

       
        /// Message Timer Tick Event
        private void MessageTimer_Tick(object sender, EventArgs e)
        {
            ResetStatus();
            messageTimer.Stop();
        }


        #region Click Events

        /// <summary>
        /// Generate Button Click Event
        /// </summary>
        /// <param name="sender">The Sender</param>
        /// <param name="e">The Event Args</param>
        private void BtnGenerate_Click(object sender, EventArgs e)
        {
            foreach (Label cell in cellControls)
            {
                cell.ForeColor = Color.Black;
            }
            timerStarted = false;
            ticks = 0;
            timer.Stop();

            timer.Start();
            timerStarted = true;

            ResetTheGrid();
            var generator = new Generator(grid, gridMode);
            generator.Generate();
            generator.GetSolvedArr(out solved, out gridarr);
            RefreshTheGrid();

            lblStatus.ForeColor = Color.DeepSkyBlue;
            lblStatus.Text = Constants.Constants.PuzzleGenerated;


            messageTimer.Start();
        }

        /// <summary>
        /// Validate Button Click Event
        /// </summary>
        /// <param name="sender">The Sender</param>
        /// <param name="e">The Event Args</param>
        private void BtnValidate_Click(object sender, EventArgs e)
        {
            string message;
            var messageColor = Color.DodgerBlue;

            foreach(Cell cell in grid.Cells)
            {
                if (cell.IsNote)
                {
                    messageColor = Color.OrangeRed;
                    message = Constants.Constants.Notes;

                    lblStatus.ForeColor = messageColor;
                    lblStatus.Text = message;

                    messageTimer.Interval = 10000;
                    messageTimer.Start();

                    return;
                }
            }


            if (grid.IsGridEmpty())
            {
               messageColor = Color.Orange;
               message = Constants.Constants.PuzzleGridEmpty;
            }
            else if (grid.IsGridFilled() && grid.Solver.ValidateGrid())
            {
                messageColor = Color.LawnGreen;
                message = Constants.Constants.PuzzleSolved;
                timer.Stop();
            }
            else if (grid.IsGridFilled() && !grid.Solver.ValidateGrid())
            {
                messageColor = Color.Red;
                message = Constants.Constants.PuzzleInvalidSolve;
            }
            else if (!grid.IsGridFilled() && grid.Solver.ValidateGrid())
                message = Constants.Constants.PuzzleValidButNotCompleted;
            else
            {
                messageColor = Color.Red;
                message = Constants.Constants.PuzzleInvalidSolveState;
            }

            lblStatus.ForeColor = messageColor;
            lblStatus.Text = message;

            messageTimer.Interval = 10000;
            messageTimer.Start();
            
        }

        /// <summary>
        /// Solve Button Click Event
        /// </summary>
        /// <param name="sender">The Sender</param>
        /// <param name="e">The Event Args</param>
        private void BtnSolve_Click(object sender, EventArgs e)
        {
            string message;
            var messageColor = Color.DodgerBlue;
            foreach (Cell cell in grid.Cells)
            {
                if (cell.IsNote)
                {
                    messageColor = Color.OrangeRed;
                    message = Constants.Constants.Notes;

                    lblStatus.ForeColor = messageColor;
                    lblStatus.Text = message;

                    messageTimer.Interval = 10000;
                    messageTimer.Start();

                    return;
                }
            }

            if (solved)
            {
                for (int i = 0; i < grid.Cells.Count; i++)
                {
                    grid.Cells[i].Value = gridarr[i];
                    cellControls[i].ForeColor = Color.Black;
                }
                RefreshTheGrid();
                lblStatus.ForeColor = Color.LawnGreen;
                lblStatus.Text = Constants.Constants.PuzzleSolved;
                timer.Stop();
            }
           

    
            messageTimer.Interval = 7000;
            messageTimer.Start();
        }

        /// <summary>
        /// Reset Button Click Event
        /// </summary>
        /// <param name="sender">The Sender</param>
        /// <param name="e">The Event Args</param>
        private void BtnReset_Click(object sender, EventArgs e)
        {
            timer.Start();
            timerStarted = false;
            gridCleared = true;

            ResetTheGrid();

            lblStatus.ForeColor = Color.White;
            lblStatus.Text = Constants.Constants.PuzzleCleared;

            messageTimer.Start();
        }








        private void BtnError_Click(object sender, EventArgs e)
        {
            lblStatus.ForeColor = Color.White;
            if(noteMode == false)
            {
                errorMode = !errorMode;

                if (errorMode == true)
                {
                    lblStatus.Text = Constants.Constants.ModErrorOn;
                    for (int i = 0; i < 81; i++)
                    {
                        if (gridarr[i] != grid.Cells[i].Value && !grid.Cells[i].IsNote)
                            cellControls[i].ForeColor = Color.Red;
                    }
                }
                else
                {
                    lblStatus.Text = Constants.Constants.ModErrorOff;
                    for (int i = 0; i < 81; i++)
                    {
                        if (gridarr[i] != grid.Cells[i].Value && !grid.Cells[i].IsNote)
                            cellControls[i].ForeColor = Color.Black;
                    }
                }
            }
            else 
            {
                lblStatus.Text = Constants.Constants.Error_Mod_Error;
            }
            
        }

        private void BtnNote_Click(object sender, EventArgs e)
        {
            lblStatus.ForeColor = Color.White;
            if (errorMode == false) 
            {
                noteMode = !noteMode;
                if (noteMode == true)
                {
                    lblStatus.Text = Constants.Constants.NoteOn;
                }
                else
                {
                    lblStatus.Text = Constants.Constants.NoteOff;
                }
            }
            else 
            {
                lblStatus.Text = Constants.Constants.Error_Note;
            }
        }







            #endregion

            #region Options Selection Events


            /// Mode ComboBox Selection Index Change Event

            private void CmbBoxMode_SelectedIndexChanged(object sender, EventArgs e)
        {
            gridMode = cmbBoxMode.SelectedIndex switch {
                2 => Constants.Constants.Hard,
                1 => Constants.Constants.Medium,
                _ => Constants.Constants.Easy
            };

        }

        
        /// Grid ComboBox Selection Index Change Event
    
        private void CmbBoxGrid_SelectedIndexChanged(object sender, EventArgs e)
        {
            grid = new Grid(9, 9);
            timerStarted = false;
            ResetStatus();
            CreateTheGrid();
        }

        #endregion

        #region Grid Events

        /// <summary>
        /// Creates the Sudoku Grid in Window Form UI
        /// </summary>
        private void CreateTheGrid()
        {
            ClearTheGrid();
            previousGridSize = grid.GridSize + 1;

            var cellTopLocation = 6;
            var cellWidth = 39; 
            var cellHeight = 39;  
            var cellFontFamily = Constants.Constants.FontFamily;
            var cellFontSize = 16;

            // Iterates through Rows
            for (var x = 0; x < grid.TotalRows; x++)
            {
                var cellLeftLocation = 5;

                // Iterates through Columns and Place each cell side by side for the current row.
                for (var y = 0; y < grid.TotalColumns; y++)
                {
                    // Control Label within cell
                    var cell = new Label
                    {
                        // Index of the cell
                        Tag = x * grid.TotalRows + y,

                        // UI Properties
                        Width = cellWidth,
                        Height = cellHeight,
                        Left = cellLeftLocation,
                        Top = cellTopLocation,
                        Cursor = Cursors.Hand,
                        ForeColor = Color.Black,
                        TextAlign = ContentAlignment.MiddleCenter,
                        Font = new Font(cellFontFamily, cellFontSize),
                        BackColor = Color.GhostWhite,
                    };

                    // Click Event for the cell.
                    cell.MouseClick += Cell_Click;
                    // Mouse Hover Event for the cell
                    cell.MouseHover += Cell_Hover;
                    // Mouse Leave Event for the cell
                    cell.MouseLeave += Cell_Leave;

                    // Modify 'cellLeftLocation' with left padding for other cells w.r.t current column.
                    cellLeftLocation += cellWidth + ((y + 1) % grid.SubGridSize == 0 ? 5 : 1);

                    // Add the cell to the 'CellControls' list and to the Grid View.
                    cellControls.Add(cell);


                    gridView.Controls.Add(cell);

                }

                // Modify 'cellTopLocation' with top padding for other cells w.r.t current row.
                cellTopLocation += cellHeight + ((x + 1) % grid.SubGridSize == 0 ? 5 : 2);
            }
        }






        /// <summary>
        /// Hover Event for the cell.
        /// </summary>
        /// <param name="sender">The Sender</param>
        /// <param name="e">The Mouse Event Args</param>
        /// 



        private void HighlightCells(int value)
        {
            for (int i = 0; i < cellControls.Count; i++)
            {
                if (!string.IsNullOrEmpty(cellControls[i].Text))
                {
                    // Если значение совпадает с заданным, подсвечиваем ячейку
                    if (int.Parse(cellControls[i].Text) == value && !grid.Cells[i].IsNote)
                    {
                        cellControls[i].BackColor = Color.Yellow;
                        if (grid.Cells[i].IsNote)
                        {
                            cellControls[i].ForeColor = Color.Gray;
                        }
                       
                    }
                }
            }
        }

        private void Cell_Hover(object sender, EventArgs e)
        {
            Label cellControl = (sender as Label);
            cellControl.BackColor = Color.Yellow; //вот тута цвет меняется на зеленый при наведении

            if (!string.IsNullOrEmpty(cellControl.Text))
            {
                cellControl.BackColor = Color.Yellow; //вот тута цвет меняется на зеленый при наведении
                int value = int.Parse(cellControl.Text);
                // Подсвечиваем ячейки с тем же значением
                HighlightCells(value);
            }


        }


        /// <summary>
        /// Leave Event for the cell.
        /// </summary>
        /// <param name="sender">The Sender</param>
        /// <param name="e">The Mouse Event Args</param>
        private void Cell_Leave(object sender, EventArgs e)
        {
            Label cellControl = (sender as Label);
            cellControl.BackColor = Color.GhostWhite;

            for (int i = 0; i < cellControls.Count; i++)
            {
                cellControls[i].BackColor = Color.GhostWhite;
                if (cellControl != cellControls[i])
                {
                    cellControls[i].BackColor = Color.GhostWhite;
                    if (grid.Cells[i].IsNote) 
                    {
                        cellControls[i].ForeColor = Color.Gray;
                    }
                    if (!grid.Cells[i].IsNote)
                    {
                        if (errorMode == true && !string.IsNullOrEmpty(cellControls[i].Text) && grid.Cells[i].Value != gridarr[i])
                            cellControls[i].ForeColor = Color.Red;
                        else if (grid.Cells[i].IsNote)
                            cellControls[i].ForeColor = Color.Black;

                    }    
                }
            }
        }

        /// <summary>
        /// Click Event for the cell.
        /// </summary>
        /// <param name="sender">The Sender</param>
        /// <param name="e">The Mouse Event Args</param>
        
        private void Cell_Click(object sender, MouseEventArgs e)
        {
            Label clickedCellControl = (sender as Label);
            int cellIndex = (int)clickedCellControl.Tag;

            var numpadGrid9Dialog = new NumpadGrid9Dialog();
            #region Cell Click Location Calcutaion to display Numpad Grid 9 Dialog

            int numpadLocationX = clickedCellControl.Location.X - (numpadGrid9Dialog.Width / 4) + this.Location.X;
            int numpadLocationY = clickedCellControl.Location.Y - (numpadGrid9Dialog.Height / 4) + this.Location.Y;

            if (numpadLocationX < 0) numpadLocationX = 0;
            if (numpadLocationY < 0) numpadLocationY = 0;

            if (Screen.PrimaryScreen.WorkingArea.Width < numpadGrid9Dialog.Width + numpadLocationX)
                numpadLocationX = Screen.PrimaryScreen.WorkingArea.Width - numpadGrid9Dialog.Width;

            if (Screen.PrimaryScreen.WorkingArea.Height < numpadGrid9Dialog.Height + numpadLocationY)
                numpadLocationY = Screen.PrimaryScreen.WorkingArea.Height - numpadGrid9Dialog.Height;

            Point numpadLocation = new Point(numpadLocationX, numpadLocationY);
            numpadGrid9Dialog.Location = numpadLocation;

            #endregion

            // Show the numpad dialog.

            //numpadGrid9Dialog.Show();


            Label cellControl = (sender as Label);
            int index = -1;
            for (int i = 0; i < cellControls.Count; i++)
            {
                if (cellControls[i] == cellControl)
                {
                    index = i; break;
                }
            }
            if (index != -1 && grid.Cells[index].IsMutable)
                numpadGrid9Dialog.Show();




                // Handle the closed event of the numpad dialog to get the result.
            numpadGrid9Dialog.FormClosed += (object s, FormClosedEventArgs ea) =>
            {

                        if (numpadGrid9Dialog.Value != -1 && numpadGrid9Dialog.Value != 0)
                        {
                            if ((errorMode == true && gridarr[cellIndex] == numpadGrid9Dialog.Value) || (errorMode == false))
                            {
                                grid.SetCellValue(cellIndex, numpadGrid9Dialog.Value);
                                if (noteMode)
                                {
                                    grid.Cells[index].IsNote = true;
                                    cellControl.ForeColor = Color.Gray;
                                }
                                else 
                                {
                                    grid.Cells[index].IsNote = false;
                                    cellControl.ForeColor = Color.Black;
                                   }
                            }
                        }
                        else if (numpadGrid9Dialog.Value == 0)
                        {
                            grid.SetCellValue(cellIndex, -1);
                        }

                    RefreshTheGrid();
                    // Dispose the numpad dialog.
                    numpadGrid9Dialog.Dispose();

            };
        }

        /// <summary>
        /// Reset the Status Label
        /// </summary>
        private void ResetStatus() => lblStatus.Text = string.Empty;






        /// <summary>
        /// Refresh the Sudoku Grid in Window Form UI
        /// </summary>
        private void RefreshTheGrid() 
            => cellControls.ForEach(cell =>
                {
                    var cellIndex = (int)cell.Tag;
                    var cellValue = grid.GetCell(cellIndex).Value;
                     cell.Text = cellValue != -1 ? cellValue.ToString() : string.Empty; 
                });






        /// <summary>
        /// Reset the Sudoku Grid in Window Form UI
        /// </summary>
        private void ResetTheGrid()
            => Parallel.Invoke(
                () => cellControls.ForEach(cell => cell.Text = string.Empty),
                () => grid.Cells.ForEach(cell => cell.IsNote = false),
                () => grid.Cells.ForEach(prop => { prop.Value = -1; prop.IsMutable = false; }));

        /// <summary>
        /// Clear the Sudoku Grid
        /// </summary>
        private void ClearTheGrid()
        {
            cellControls.Clear();
            for (int i = 0; i < previousGridSize; i++) gridView.Controls.Clear();
            gridView.BackgroundColor = Color.FromArgb(47, 47, 47);
            gridView.Refresh();
        }

        #endregion
    }
}