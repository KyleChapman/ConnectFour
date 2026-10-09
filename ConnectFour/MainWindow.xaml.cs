// Author:  Kyle Chapman
// Created: October 15, 2024
// Updated: October 9, 2026
// Description:
// A WPF-based Connect Four game. Drop tokens into the board
// using buttons at the top. Any combination of four of the same-
// coloured tokens in a row counts as a win for whichever is the
// active player.

using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace ConnectFour
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // Declaration.
        const int MaxRow = 5;
        const int MaxColumn = 6;
        Button[] buttonArray;
        Label[,] labelArray;
        bool isTurnRed = true;
        int redScore = 0;
        int yellowScore = 0;
        int ties = 0;

        /// <summary>
        /// Constructor for the application's window.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();

            // Set up an array of the buttons that
            // drop tokens into columns.
            buttonArray = [buttonColumn0, buttonColumn1, buttonColumn2, buttonColumn3, buttonColumn4, buttonColumn5, buttonColumn6];

            // This monstrosity is declaring an array of the 42
            // game area labels.
            labelArray = new Label[,] {
                { labelRow0Column0, labelRow0Column1, labelRow0Column2, labelRow0Column3, labelRow0Column4, labelRow0Column5, labelRow0Column6},
                { labelRow1Column0, labelRow1Column1, labelRow1Column2, labelRow1Column3, labelRow1Column4, labelRow1Column5, labelRow1Column6},
                { labelRow2Column0, labelRow2Column1, labelRow2Column2, labelRow2Column3, labelRow2Column4, labelRow2Column5, labelRow2Column6},
                { labelRow3Column0, labelRow3Column1, labelRow3Column2, labelRow3Column3, labelRow3Column4, labelRow3Column5, labelRow3Column6},
                { labelRow4Column0, labelRow4Column1, labelRow4Column2, labelRow4Column3, labelRow4Column4, labelRow4Column5, labelRow4Column6},
                { labelRow5Column0, labelRow5Column1, labelRow5Column2, labelRow5Column3, labelRow5Column4, labelRow5Column5, labelRow5Column6}};
        }

        /// <summary>
        /// Handles events related to clicking any of the "drop here" buttons, which drop a token into the corresponding column.
        /// </summary>
        private void DropInColumnClick(object sender, RoutedEventArgs e)
        {
            // Set buttonClicked equal to whichever button send the event.
            Button buttonClicked = (Button)sender;
            // This is the column where the new piece will be dropped.
            int column = -1;
            // This is the row that the new piece will be dropped into.
            int row = -1;
            // Do we know the final row that piece falls to yet?
            bool foundRow = false;

            // Identify which column the button is in.
            // It is very possible that there's an easier way to do this.
            for (int checkColumn = 0; checkColumn <= MaxColumn; checkColumn++)
            {
                if (buttonClicked.Equals(buttonArray[checkColumn]))
                {
                    column = checkColumn;
                }
            }

            // Identify the row that the piece will drop into.
            for (int checkRow = MaxRow; checkRow >= 0 && !foundRow; checkRow--)
            {
                // Check whether the next row is still black.
                if (labelArray[checkRow, column].Foreground.ToString() == Brushes.Black.ToString())
                {
                    // If it is, that's the target row.
                    row = checkRow;
                    // We have now found the row, and the loop can exit.
                    foundRow = true;
                }
            }

            // If we've filled the last possible space in a column, disable the button to drop the piece.
            if (row == 0)
            {
                buttonClicked.IsEnabled = false;
            }

            // If it's red's turn, make the space red. Otherwise, make it yellow.
            if (isTurnRed)
            {
                labelArray[row, column].Foreground = Brushes.Red;
            }
            else
            {
                labelArray[row, column].Foreground = Brushes.Yellow;
            }

            // If nobody has won, switch the current turn.
            if (!EvaluateWin())
            {
                isTurnRed = !isTurnRed;
                if (isTurnRed)
                {
                    textCurrentPlayer.Text = "Red";
                }
                else
                {
                    textCurrentPlayer.Text = "Yellow";
                }

                // Check if all buttons are disabled (e.g. no more moves exist).
                // That means it's tied.
                if (!(buttonColumn0.IsEnabled || buttonColumn1.IsEnabled || buttonColumn2.IsEnabled || buttonColumn3.IsEnabled || buttonColumn4.IsEnabled || buttonColumn5.IsEnabled || buttonColumn6.IsEnabled))
                {
                    MessageBox.Show("Tie game. :(");
                    ties++;
                    textTies.Text = ties.ToString();
                    ResetBoard();
                }
            }
            // If there is a win, and it's red's turn, then red wins.
            else if (isTurnRed)
            {
                MessageBox.Show("Red player wins!");
                redScore++;
                textRedPoints.Text = redScore.ToString();
                ResetBoard();
            }
            // Otherwise, yellow has won.
            else
            {
                MessageBox.Show("Yellow player wins!");
                yellowScore++;
                textYellowPoints.Text = yellowScore.ToString();
                ResetBoard();
            }
        }

        /// <summary>
        /// Determine whether there's a win on the board right now (i.e. there are four of the same colour of tokens in a row).
        /// </summary>
        /// <returns>true if there's a winner, false otherwise</returns>
        private bool EvaluateWin()
        {
            // Store the colour for the active player.
            // We're going to check for this colour to see if they've won.
            Brush currentColour;

            if (isTurnRed)
            {
                currentColour = Brushes.Red;
            }
            else
            {
                currentColour = Brushes.Yellow;
            }
            
            // You could also do this in one line.
            //Brush currentPlayer = isTurnRed ? Brushes.Red : Brushes.Yellow;

            // Check horizontal runs of 4.
            // Start by checking each row.
            for (int row = 0; row <= MaxRow; row++)
            {
                int matches = 0;
                // Next, check each column.
                for (int column = 0; column <= MaxColumn; column++)
                {
                    // Increment the matches based on how many of the same colour are in this row, across the columns.
                    if (labelArray[row, column].Foreground == currentColour)
                    {
                        matches++;
                        // If there are four matches, somebody wins.
                        if (matches == 4)
                        {
                            return true;
                        }
                    }
                    // If the next token doesn't match, reset the matches.
                    else
                    {
                        matches = 0;
                    }
                }
            }

            // Check vertical runs of 4.
            // Go through the columns first.
            for (int column = 0; column <= MaxColumn; column++)
            {
                int matches = 0;
                // Within each column, check each row successively.
                for (int row = 0; row <= MaxRow; row++)
                {
                    // Increment the matches based on how many of the same colour are in this column, through the rows.
                    if (labelArray[row, column].Foreground == currentColour)
                    {
                        matches++;
                        // If there are four matches, somebody wins.
                        if (matches == 4)
                        {
                            return true;
                        }
                    }
                    // If the next token doesn't match, reset the matches.
                    else
                    {
                        matches = 0;
                    }
                }
            }

            // Check diagonals running down-right.
            // This starts 3 rows from the end, and 3 columns from the top.
            for (int row = 0; row <= MaxRow - 3; row++)
            {
                for (int column = 0; column <= MaxColumn - 3; column++)
                {
                    // Check the diagonal line from the current row and column.
                    if (labelArray[row, column].Foreground == currentColour &&
                        labelArray[row + 1, column + 1].Foreground == currentColour &&
                        labelArray[row + 2, column + 2].Foreground == currentColour &&
                        labelArray[row + 3, column + 3].Foreground == currentColour)
                    {
                        return true;
                    }
                }
            }

            // Check diagonals running up-right.
            // This starts 3 rows from the start, and 3 columns from the top.
            for (int row = 3; row <= MaxRow; row++)
            {
                for (int column = 0; column <= MaxColumn - 3; column++)
                {
                    // Check the diagonal line from the current row and column.
                    if (labelArray[row, column].Foreground == currentColour &&
                        labelArray[row - 1, column + 1].Foreground == currentColour &&
                        labelArray[row - 2, column + 2].Foreground == currentColour &&
                        labelArray[row - 3, column + 3].Foreground == currentColour)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// This method is tic-tac-toe related and not really necessary for Connect Four. Left in as demo material.
        /// </summary>
        /// <param name="controlToHighlight">Any form control to change the colour of.</param>
        private void HighlightControl(Control controlToHighlight)
        {
            controlToHighlight.Background = Brushes.LightSalmon;
            controlToHighlight.BorderBrush = Brushes.PeachPuff;
        }

        /// <summary>
        /// Change a label's foreground colour back to default black.
        /// </summary>
        /// <param name="labelToDeselect">A label to set back to black.</param>
        private void DeselectSpace(Label labelToDeselect)
        {
            labelToDeselect.Foreground = Brushes.Black;
        }

        /// <summary>
        /// Cycles through all labels and sets them back to foreground black as if to start a new game.
        /// </summary>
        private void ResetAllLabels()
        {
            for (int currentRow = 0; currentRow <= MaxRow; currentRow++)
            {
                for (int currentColumn = 0; currentColumn <= MaxColumn; currentColumn++)
                {
                    DeselectSpace(labelArray[currentRow, currentColumn]);
                }
            }
        }

        /// <summary>
        /// Reset the board including buttons as if to restart the game.
        /// </summary>
        private void ResetBoard()
        {
            ResetAllLabels();
            foreach (Button button in buttonArray)
            {
                button.IsEnabled = true;
            }
            textCurrentPlayer.Text = "Red";
        }

        /// <summary>
        /// Resets the scores. (Resetting the game is up to the user.)
        /// </summary>
        private void ResetScores(object sender, RoutedEventArgs e)
        {
            redScore = 0;
            yellowScore = 0;
            ties = 0;
            textRedPoints.Text = redScore.ToString();
            textYellowPoints.Text = yellowScore.ToString();
            textTies.Text = ties.ToString();
        }
    }
}