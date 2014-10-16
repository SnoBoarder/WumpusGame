using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WumpusProject
{
    public class BoardCreation
    {

        //separate variables to use for board creation
        //we do not need the entire node data structure
        private bool[,] creationBoard_;
        private int[,] creationDist_;

        List<string> commandList_;

        public BoardCreation(int rows, int cols)
        {
            //so that usage of this makeBoard is invisible to distTo

            bool solved = false;
            int row = 0;
            int col = 0;
            int startR = 0;
            int startC = 0;

            int goldR = 0;
            int goldC = 0;
            bool wumpusPlaced = false;
            bool goldPlaced = false;

            commandList_ = new List<string>();
            Random rnd = new Random();

            //max pits is rows * cols(all the squares) - 3(where 3 is player, gold, wumpus, and forbidden squares max 8);
            int numPits = rnd.Next(0, (rows * cols) - 8);
            while (!solved)
            {
                creationBoard_ = new bool[rows, cols];

                for (int x = 0; x < rows; x++)
                {
                    for (int y = 0; y < cols; y++)
                    {
                        //everything starts off as safe and as bad stuff goes in, we remove it.
                        creationBoard_[x, y] = true;
                    }
                }
                row = rnd.Next(1);
                col = rnd.Next(1);

                //get a random corner to start
                startR = (row == 0) ? 0 : rows - 1;
                startC = (col == 0) ? 0 : cols - 1;

                goldPlaced = false;

                while (!goldPlaced)
                {
                    goldR = rnd.Next(0, rows - 1);
                    goldC = rnd.Next(0, cols - 1);

                    if (Math.Abs(goldR - startR) > 1 && Math.Abs(goldC - startC) > 1)
                    {
                        //if the gold is at least 2 squares away from the player. valid.
                        goldPlaced = true;
                    }
                }

                //set up traps
                wumpusPlaced = false;
                while (!wumpusPlaced)
                {
                    row = rnd.Next(0, rows - 1);
                    col = rnd.Next(0, cols - 1);

                    if (Math.Abs(row - startR) > 1 && Math.Abs(col - startC) > 1 && Math.Abs(goldR - row) > 1 && Math.Abs(goldC - col) > 1)
                    {
                        wumpusPlaced = true;
                        creationBoard_[row, col] = false;
                    }
                }

            }
            commandList.Add("null");
        }


    }
}
