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
        private int[,] dists_;

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
            int pitsPlaced = 0;

            commandList_ = new List<string>();
            Random rnd = new Random();

            //max pits is rows * cols(all the squares) - 3(known safe squares) - 3 (player,gold,wumpus)
            int numPits = rnd.Next(0, (rows * cols) - 3 - 3);
            while (!solved)
            {
                creationBoard_ = new bool[rows, cols];
                dists_ = new int[rows, cols];

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

                pitsPlaced = 0;
                while(pitsPlaced < numPits)
                {
                    row = rnd.Next(0, rows - 1);
                    col = rnd.Next(0, cols - 1);

                    if (creationBoard_[row, col] && row != goldR && col != goldC 
                        && Math.Abs(row - startR) > 1 && Math.Abs(col - startC) > 1)
                    {
                        //if it is a safe spot, there is no gold, and not a starting area. We are good to go
                        creationBoard_[row, col] = false;
                        pitsPlaced++;
                    }
                }

                //now make sure there is a solution
                flood(startR, startC, 0);

                if(dists_[goldR, goldC] != -1)
                {
                    //we have a solution
                    solved = true;
                }
            }
            commandList_.Add("null");
        }

        private void flood(int curRow, int curCol, int dist)
        {
            if (dists_[curRow, curCol] > dist)
            {
                dists_[curRow, curCol] = dist;

                //flood to neighbors that we have visited. This will take us back to the node we came from.
                //distance will be less so that path will stop
                if (inBounds(curRow - 1, curCol) && creationBoard_[curRow - 1, curCol])
                {
                    flood(curRow - 1, curCol, dist++);
                }

                if (inBounds(curRow + 1, curCol) && creationBoard_[curRow + 1, curCol])
                {
                    flood(curRow + 1, curCol, dist++);
                }

                if (inBounds(curRow, curCol - 1) && creationBoard_[curRow, curCol - 1])
                {
                    flood(curRow, curCol - 1, dist++);
                }

                if (inBounds(curRow, curCol + 1) && creationBoard_[curRow, curCol + 1])
                {
                    flood(curRow, curCol + 1, dist++);
                }
            }
        }

        //[inputs] : node 1 = where you are, node 2 = where you want to go
        //[outputs] : shortest path distance between the two. -1 if no path exists.

        private void clearDist()
        {
            for (int x = 0; x < dists_.GetLength(0); x++)
            {
                for (int y = 0; y < dists_.GetLength(1); y++)
                {
                    dists_[x, y] = -1;
                }
            }
        }

        private bool inBounds(int row, int col)
        {
            if (row < 0 || row >= creationBoard_.GetLength(0))
                return false;

            if (col < 0 || col >= creationBoard_.GetLength(1))
                return false; // column not within bounds. ignore.

            return true;
        }
    }
}
