using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WumpusProject
{
    public class ManipBoard
    {
        private Node[,] board_;
        private int[,] dists_;
        private int rows_;
        private int cols_;

        private Node curNode_;

        public ManipBoard()
        {
        }

        public void updateBoard(Node[,] input)
        {
            board_ = input;
            dists_ = new int[input.GetLength(0), input.GetLength(1)];
 
            curNode_ = null;
            clearDist();
        }

        public void makeBoard(int rows, int cols)
        {
            board_ = new Node[rows, cols];
            dists_ = new int[rows, cols];

            for (int x = 0; x < rows; x++)
            {
                for (int y = 0; y < cols; y++)
                {
                    dists_[x,y] = -1;
                }
            }
            
            rows_ = rows;
            cols_ = cols;
        }

        private void flood(Node node, int dist)
        {
            int curRow = node.row;
            int curCol = node.col;

            if(dists_[curRow, curCol] > dist)
            {
                dists_[curRow, curCol] = dist;

                //flood to neighbors that we have visited. This will take us back to the node we came from.
                //distance will be less so that path will stop
                if (inBounds(curRow - 1, curCol) && board_[curRow - 1, curCol].visited)
                {
                    flood(board_[curRow - 1, curCol], dist++);
                }

                if (inBounds(curRow + 1, curCol) && board_[curRow + 1, curCol].visited)
                {
                    flood(board_[curRow + 1, curCol], dist++);
                }

                if (inBounds(curRow, curCol - 1) && board_[curRow, curCol - 1].visited)
                {
                    flood(board_[curRow, curCol - 1], dist++);
                }

                if (inBounds(curRow, curCol + 1) && board_[curRow, curCol + 1].visited)
                {
                    flood(board_[curRow, curCol + 1], dist++);
                }
            }
        }

        //[inputs] : node 1 = where you are, node 2 = where you want to go
        //[outputs] : shortest path distance between the two. -1 if no path exists.
        public int distTo(Node node1, Node node2)
        {
            if(node1 != curNode_)
            {
                //we are at a different place, recalculate
                clearDist();
                flood(node1, 0);
            }

            curNode_ = node1;

            return dists_[node2.row, node2.col];
        }

        private void clearDist()
        {
            for (int x = 0; x < rows_; x++)
            {
                for (int y = 0; y < cols_; y++)
                {
                    dists_[x, y] = -1;
                }
            }
        }

        private bool inBounds(int row, int col)
        {
            if (row < 0 || row >= rows_)
                return false;

            if (col < 0 || col >= cols_)
                return false; // column not within bounds. ignore.

            return true;
        }
    }
}
