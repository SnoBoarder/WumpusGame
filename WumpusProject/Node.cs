using System;

namespace WumpusProject
{
    public class Node
    {
        public delegate void HandleSafetyPitCheck(Node node);
        public static event HandleSafetyPitCheck OnSafetyPitCheck;

        public delegate void HandleSafetyWumpusCheck(Node node);
        public static event HandleSafetyWumpusCheck OnSafetyWumpusCheck;

        private enum PitState { UNKNOWN, POTENTIAL_PIT, NO_PIT, PIT_HERE };
        private enum WumpusState { UNKNOWN, POTENTIAL_WUMPUS, NO_WUMPUS, WUMPUS_HERE };
        enum PotentialType { PIT, WUMPUS };

        private PitState _pitState = PitState.UNKNOWN;
        private WumpusState _wumpusState = WumpusState.UNKNOWN;

        private int _row;
        private int _col;

        private PerfectNode _perfectNode;

        public Node(int row, int col)
        {
            _row = row;
            _col = col;
        }
        
        public int row
        {
            get { return _row; }
        }

        public int col
        {
            get { return _col; }
        }

        public bool visited
        {
            get { return _perfectNode != null; }
        }

        public PerfectNode perfectNode
        {
            get { return _perfectNode; }
        }

        public bool isSafe
        {
            get { return _pitState == PitState.NO_PIT && _wumpusState == WumpusState.NO_WUMPUS; }
        }

        public void handlePotentialPit()
        {
            int neighborCount = 0;
            int breezyCount = 0;

            Node neighbor = null;

            for (int row = _row - 1; row <= _row + 1; row++)
            {
                for (int col = _col - 1; col <= _col + 1; col++)
                {
                    if (row < 0 || row >= WumpusGame.totalRows)
                        continue; // row not within bounds. ignore.

                    if (col < 0 || col >= WumpusGame.totalCols)
                        continue; // column not within bounds. ignore.

                    if (row == _row || col == _col)
                        continue; // ignore self

                    neighbor = WumpusGame.playerMap[row, col];

                    if (neighbor.visited)
                    {
                        neighborCount++;

                        if (neighbor.perfectNode.isBreezy)
                            breezyCount++;
                    }
                }
            }

            if (neighborCount == 1)
            { // i am the only one with breeze
                _pitState = PitState.POTENTIAL_PIT;
            }
            else if (breezyCount > 1)
            {
                _pitState = PitState.PIT_HERE;

                // TODO: Tell someone that there is a pit here? Not sure if we really need to tell anyone...
            }
            else
            {
                _pitState = PitState.NO_PIT;

                if (OnSafetyPitCheck != null)
                    OnSafetyPitCheck(this); // dispatch event to make the wumpus game check the safety of this node again
            }
        }

        public void handlePotentialWumpus()
        {
            int neighborCount = 0;
            int stenchCount = 0;

            Node neighbor = null;

            for (int row = _row - 1; row <= _row + 1; row++)
            {
                for (int col = _col - 1; col <= _col + 1; col++)
                {
                    if (row < 0 || row >= WumpusGame.totalRows)
                        continue; // row not within bounds. ignore.

                    if (col < 0 || col >= WumpusGame.totalCols)
                        continue; // column not within bounds. ignore.

                    if (row == _row || col == _col)
                        continue; // ignore self

                    neighbor = WumpusGame.playerMap[row, col];

                    if (neighbor.visited)
                    {
                        neighborCount++;

                        if (neighbor.perfectNode.isStench)
                            stenchCount++;
                    }
                }
            }

            if (neighborCount == 1)
            { // i am the only neighbor with stench
                _wumpusState = WumpusState.POTENTIAL_WUMPUS;

                // add to list of potential wumpus nodes
                WumpusGame.potentialWumpusNodes.Add(this);
            }
            else if (stenchCount > 1)
            {
                _wumpusState = WumpusState.WUMPUS_HERE;

                // tell the main game that the wumpus's location!
                WumpusGame.setWumpusPosition(_row, _col);
                WumpusGame.potentialWumpusNodes.Clear();
            }
            else
            {
                _wumpusState = WumpusState.NO_WUMPUS;

                if (OnSafetyWumpusCheck != null)
                    OnSafetyWumpusCheck(this); // dispatch event to make the wumpus game check the safety of this node again
            }
        }

        public void updateFromPerfectNode(PerfectNode perfectNode)
        {
            if (_perfectNode != null)
                return; // we've already visited this node. ignore updating

            _perfectNode = perfectNode;

            if (_perfectNode.isBreezy)
            { // notify adjacent nodes that they could be potential pits
                updateAdjacentNodesWith(PotentialType.PIT);
            }

            if (_perfectNode.isPit)
            { // player falls into a pit... player loses 1000 points...
                throw new Exception("YOU FELL INTO A PIT");
            }

            if (_perfectNode.isWumpus)
            { // player is killed by the wumpus... player loses 1000 points...
                throw new Exception("YOU WALKED INTO A WUMPUS");
            }

            if (_perfectNode.isStench)
            { // notify adjacent nodes that their wumpusCount needs to increase
                updateAdjacentNodesWith(PotentialType.WUMPUS);
            }

            if (_perfectNode.hasGold)
            { // WE GOT GOLD
                WumpusGame.goldFound = true;
            }
        }

        private void updateAdjacentNodesWith(PotentialType type)
        {
            // go through all adjacent nodes and set their information
            for (int row = _row - 1; row <= _row + 1; row++)
            {
                for (int col = _col - 1; col <= _col + 1; col++)
                {
                    if (row < 0 || row >= WumpusGame.totalRows)
                        continue; // row not within bounds. ignore.

                    if (col < 0 || col >= WumpusGame.totalCols)
                        continue; // column not within bounds. ignore.

                    if (row == _row || col == _col)
                        continue; // ignore self

                    switch (type)
                    {
                        case PotentialType.PIT:
                            WumpusGame.playerMap[row, col].handlePotentialPit();
                            break;
                        case PotentialType.WUMPUS:
                            WumpusGame.playerMap[row, col].handlePotentialWumpus();
                            break;
                    }
                }
            }
        }
    }
}
