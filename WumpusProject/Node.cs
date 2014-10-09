using System;

namespace WumpusProject
{
    public class Node
    {
        private const int POTENTIAL_PIT = 0;
        private const int POTENTIAL_WUMPUS = 1;

        private bool _potentialPit = false;
        
        private int _row;
        private int _col;
        private bool _visitedNode = false;
        private int _wumpusCount;

        private PerfectNode _pNode;

        public Node(int row, int col)
        {
            _row = row;
            _col = col;
        }

        public bool visitedNode
        {
            get { return _visitedNode; }
        }

        public bool potentialPit
        {
            get { return _potentialPit; }

            set
            {
                if (_visitedNode)
                    return; // we already know what this node is (and because we know, it definitely shouldn't be a pit!)

                _potentialPit = value;
            }
        }

        // probably don't need this...
        public bool isWumpus
        {
            get { return _wumpusCount == 2; }
        }

        public int wumpusCount
        {
            set
            {
                if (WumpusGame.wumpusFound)
                    return; // we've already found the wumpus. ignore.

                _wumpusCount = value;
                if (_wumpusCount == 2)
                { // WE HAVE A WUMPUS!
                    // TODO: tell the game that THIS node row/col has a wumpus (may not be needed?)

                    // Set static bool that the wumpus has been found
                    WumpusGame.wumpusFound = true;
                }
            }
        }

        public void updateFromPerfectNode(PerfectNode pNode)
        {
            if (_visitedNode)
                return; // we've already visited this node. ignore updating

            _visitedNode = true;

            // TODO: Discuss if the perfect node needs to be stored
            _pNode = pNode;

            if (_pNode.isBreezy)
            { // notify adjacent nodes that they could be potential pits
                setAdjacentNodesWith(POTENTIAL_PIT);
            }

            if (_pNode.isPit)
            { // player falls into a pit... player loses 1000 points...

            }

            if (_pNode.isWumpus)
            { // player is killed by the wumpus... player loses 1000 points...

            }

            if (_pNode.isStench)
            { // notify adjacent nodes that their wumpusCount needs to increase
                setAdjacentNodesWith(POTENTIAL_WUMPUS);
            }

            if (_pNode.hasGold)
            { // WE GOT GOLD
                WumpusGame.goldFound = true;
            }
        }

        private void setAdjacentNodesWith(int type)
        {
            // go through all adjacent nodes and set their information

            for (int row = _row - 1; row <= _row + 1; row++)
            {
                for (int col = _col - 1; col <= _col + 1; col++)
                {
                    if (row < 0 || row >= WumpusGame.boardRows)
                        continue; // row not within bounds. ignore.

                    if (col < 0 || col >= WumpusGame.boardCols)
                        continue; // column not within bounds. ignore.

                    if (row == _row || col == _col)
                        continue; // ignore self

                    switch (type)
                    {
                        case POTENTIAL_PIT:
                            WumpusGame.playerMap[row, col].potentialPit = true;
                            break;
                        case POTENTIAL_WUMPUS:
                            WumpusGame.playerMap[row, col]._wumpusCount++;
                            break;
                    }
                }
            }
        }
    }
}
