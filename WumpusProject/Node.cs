using System;

namespace WumpusProject
{
    public class Node
    {
        public bool potentialPit;
        
        private int _row;
        private int _col;
        private bool _visitedSlot;
        private int _wumpusCount;

        public bool visitedSlot
        {
            get { return _visitedSlot; }

            set
            {
                if (_visitedSlot)
                    return; // don't manipulate more than once

                _visitedSlot = value;
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
                    // TODO: Set static bool that the wumpus has been found
                }
            }
        }
    }
}
