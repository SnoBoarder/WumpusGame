using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WumpusProject
{
    public class BoardCreation
    {
        /*
         * Usage:
         * BoardCreation creator = new BoardCreation();
         * creator.makeBoard(rows, cols);
         * list<string> newBoard = creator.getBoard();
         */

        //separate variables to use for board creation
        //we do not need the entire node data structure
        private bool[,] creationBoard_;
        private int[,] dists_;

        List<string> commandList_;

        public BoardCreation()
        {
           
        }

        public void makeBoard(int rows, int cols)
        { 
            //so that usage of this makeBoard is invisible to distTo
            bool solved = false;
            int row = 0;
            int col = 0;
            int startR = 0;
            int startC = 0;

            int wumpR = 0;
            int wumpC = 0;
            List<int[]> pits = new List<int[]>();
            List<int[]> safePlaces = new List<int[]>();

            int goldR = 0;
            int goldC = 0;
            bool wumpusPlaced = false;
            bool goldPlaced = false;
            int pitsPlaced = 0;

            int[] pitLoc = new int[2];

            commandList_ = new List<string>();
            Random rnd = new Random();

            //max pits is rows * cols(all the squares) - 3(known safe squares) - 3 (player,gold,wumpus)
            int numPits = rnd.Next(0, (rows * cols) - 3 - 3);
            while (!solved)
            {
                creationBoard_ = new bool[rows, cols];
                dists_ = new int[rows, cols];
                pits.Clear();
                safePlaces.Clear();

                for (int x = 0; x < rows; x++)
                {
                    for (int y = 0; y < cols; y++)
                    {
                        //everything starts off as safe and as bad stuff goes in, we remove it.
                        int[] safeLoc = new int[2];
                        safeLoc[0] = x;
                        safeLoc[1] = y;

                        safePlaces.Add(safeLoc);

                        creationBoard_[x, y] = true;
                    }
                }
                row = rnd.Next(2);
                col = rnd.Next(2);

                //get a random corner to start
                startR = (row == 0) ? 0 : rows - 1;
                startC = (col == 0) ? 0 : cols - 1;

                goldPlaced = false;

                while (!goldPlaced)
                {
                    int ind = rnd.Next(0, safePlaces.Count);

                    goldR = safePlaces[ind][0];
                    goldC = safePlaces[ind][1];

                    if (Math.Abs(goldR - startR) > 1 && Math.Abs(goldC - startC) > 1)
                    {
                        //if the gold is at least 2 squares away from the player. valid.
                        goldPlaced = true;
                        safePlaces.RemoveAt(ind);
                    }
                    else
                    {
                        //if it is too near the start, nothing else will fall here anyways.
                        //take it out of the possible safe places that can change.
                        safePlaces.RemoveAt(ind);
                    }
                }

                //set up traps
                wumpusPlaced = false;
                while (!wumpusPlaced)
                {
                    int ind = rnd.Next(0, safePlaces.Count);
                    wumpR = safePlaces[ind][0];
                    wumpC = safePlaces[ind][1];

                    if (Math.Abs(wumpR - startR) > 1 && Math.Abs(wumpC - startC) > 1 && Math.Abs(goldR - wumpR) > 0 && Math.Abs(goldC - wumpC) > 0)
                    {
                        wumpusPlaced = true;
                        creationBoard_[row, col] = false;
                        safePlaces.RemoveAt(ind);
                    }
                }

                pitsPlaced = 0;
                while (pitsPlaced < numPits)
                {
                    int ind = rnd.Next(0, safePlaces.Count);
                    row = safePlaces[ind][0];
                    col = safePlaces[ind][1];

                    if (creationBoard_[row, col] && row != goldR && col != goldC
                        && Math.Abs(row - startR) > 1 && Math.Abs(col - startC) > 1)
                    {
                        //if it is a safe spot, there is no gold, and not a starting area. We are good to go
                        creationBoard_[row, col] = false;
                        safePlaces.RemoveAt(ind);

                        pitLoc = new int[2];
                        pitLoc[0] = row;
                        pitLoc[1] = col;
                        pits.Add(pitLoc);
                        pitsPlaced++;
                    }

                    else
                    {
                        //this is safe place that cannot hold a pit, remove it from the list
                        safePlaces.RemoveAt(ind);
                    }

                    if(safePlaces.Count == 0)
                    {
                        //the current board set up doesn't let us place any more pits
                        //break out and go with the board we have so far. 
                        break;
                    }
                }

                //now make sure there is a solution
                clearDist();
                flood(startR, startC, 0);

                if (dists_[goldR, goldC] != -1)
                {
                    //we have a solution
                    solved = true;
                    break;
                }

            }

            
            //go through all and place attributes
            for (int x = 0; x < rows; x++)
            {
                for (int y = 0; y < cols; y++)
                {
                    string atts = "";

                    if (Math.Abs(wumpC - y) == 1 || Math.Abs(wumpR - x) == 1)
                    {
                        atts += "S";
                    }

                    for (int z = 0; z < pits.Count; z++)
                    {
                        if (Math.Abs(pits[z][0] - x) == 1 || Math.Abs(pits[z][1] - y) == 1)
                        {
                            atts += "B";
                            break;
                        }
                    }

                    commandList_.Add(x.ToString() + "," + y.ToString() + "," + atts);
                }
            }

            //add start location
            commandList_[startR * cols + startC] += "E";

            //add wumpus location
            commandList_[wumpR * cols + wumpC] += "W";

            //add gold location
            commandList_[goldR * cols + goldC] += "G";

            //add pits location(if not gold or wumpus, pit)
            for (int x = 0; x < pits.Count; x++)
            {
                commandList_[pits[x][0] * cols + pits[x][1]] += "P";
            }

           
            int index = 0;
            while (index < commandList_.Count())
            {
                int last = commandList_[index].LastIndexOf(',');

                if ((last + 1) == commandList_[index].Length)
                {
                    //comma was the last thing, no attributes
                    commandList_.RemoveAt(index);
                }
                else
                {
                    index++;
                }
            }

            //add in board size
            commandList_.Insert(0, rows.ToString() + "," + cols.ToString());
        }

        public List<string> getBoard()
        {
            return commandList_;
        }

        private void flood(int curRow, int curCol, int dist)
        {
            if (dists_[curRow, curCol] > dist || dists_[curRow, curCol] == -1)
            {
                dists_[curRow, curCol] = dist;

                //flood to neighbors that we have visited. This will take us back to the node we came from.
                //distance will be less so that path will stop
                if (inBounds(curRow - 1, curCol) && creationBoard_[curRow - 1, curCol])
                {
                    flood(curRow - 1, curCol, dist+1);
                }

                if (inBounds(curRow + 1, curCol) && creationBoard_[curRow + 1, curCol])
                {
                    flood(curRow + 1, curCol, dist+1);
                }

                if (inBounds(curRow, curCol - 1) && creationBoard_[curRow, curCol - 1])
                {
                    flood(curRow, curCol - 1, dist+1);
                }

                if (inBounds(curRow, curCol + 1) && creationBoard_[curRow, curCol + 1])
                {
                    flood(curRow, curCol + 1, dist+1);
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
