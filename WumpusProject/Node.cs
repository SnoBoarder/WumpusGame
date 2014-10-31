using System;

namespace WumpusProject
{
    public class Node
    {
        public delegate void HandleSafetyPitCheck(Node node);
        public static event HandleSafetyPitCheck OnSafetyPitCheck;

        public delegate void HandleSafetyWumpusCheck(Node node);
        public static event HandleSafetyWumpusCheck OnSafetyWumpusCheck;

        public delegate void HandleGoldFound(int row, int col);
        public static event HandleGoldFound OnGoldFound;

        private enum PitState { UNKNOWN, POTENTIAL_PIT, NO_PIT, PIT_HERE };
        private enum WumpusState { UNKNOWN, POTENTIAL_WUMPUS, NO_WUMPUS, WUMPUS_HERE };
        private enum UpdateType { NO_PIT, NO_WUMPUS, POTENTIAL_PIT, POTENTIAL_WUMPUS };

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
            get {
                if (visited)
                    return true;

                if (_pitState != PitState.NO_PIT)
                    return false;

                if (_wumpusState != WumpusState.NO_WUMPUS)
                    return WumpusGame.wumpusKilled;

                return true;
            }
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
                    if (!isAdjacent(row, col))
                        continue;

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
                setStateNoPit();
            }
        }

        public void setStateNoPit()
        {
            _pitState = PitState.NO_PIT;

            if (OnSafetyPitCheck != null)
                OnSafetyPitCheck(this); // dispatch event to make the wumpus game check the safety of this node again
        }

        public void handlePotentialWumpus()
        {
            if (WumpusGame.wumpusKilled)
                return; // IT'S DEAD

            int neighborCount = 0;
            int stenchCount = 0;

            Node neighbor = null;

            for (int row = _row - 1; row <= _row + 1; row++)
            {
                for (int col = _col - 1; col <= _col + 1; col++)
                {
                    if (!isAdjacent(row, col))
                        continue;

                    neighbor = WumpusGame.playerMap[row, col];

                    if (neighbor.visited)
                    {
                        neighborCount++;

                        if (neighbor.perfectNode.isStench)
                            stenchCount++;
                    }
                }
            }

            if (stenchCount == 1 && neighborCount == 1)
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
                setStateNoWumpus();
            }
        }

        public void setStateNoWumpus()
        {
            _wumpusState = WumpusState.NO_WUMPUS;

            if (OnSafetyWumpusCheck != null)
                OnSafetyWumpusCheck(this); // dispatch event to make the wumpus game check the safety of this node again
        }

        public void updateFromPerfectNode(PerfectNode perfectNode)
        {
            if (_perfectNode != null)
                return; // we've already visited this node. ignore updating

            _perfectNode = perfectNode;

            if (_perfectNode.isBreezy)
            { // notify adjacent nodes that they could be potential pits
                updateAdjacentNodesWith(UpdateType.POTENTIAL_PIT);
            }
            else
            {
                updateAdjacentNodesWith(UpdateType.NO_PIT);
            }

            if (_perfectNode.isPit)
            { // player falls into a pit... player loses 1000 points...
                throw new Exception("YOU FELL INTO A PIT");
            }

            if (_perfectNode.isWumpus && !WumpusGame.wumpusKilled)
            { // player is killed by the wumpus... player loses 1000 points...
                throw new Exception("YOU WALKED INTO A WUMPUS");
            }

            if (_perfectNode.isStench)
            { // notify adjacent nodes that their wumpusCount needs to increase
                updateAdjacentNodesWith(UpdateType.POTENTIAL_WUMPUS);
            }
            else
            {
                updateAdjacentNodesWith(UpdateType.NO_WUMPUS);
            }

            if (_perfectNode.hasGold)
            { // WE GOT GOLD
                WumpusGame.goldFound = true;

                if (OnGoldFound != null)
                    OnGoldFound(_row, _col);
            }

            // since we have visited this location, we know that there is no pit and no wumpus
            _pitState = PitState.NO_PIT;
            _wumpusState = WumpusState.NO_WUMPUS;
        }

        private void updateAdjacentNodesWith(UpdateType type)
        {
            // go through all adjacent nodes and set their information
            for (int row = _row - 1; row <= _row + 1; row++)
            {
                for (int col = _col - 1; col <= _col + 1; col++)
                {
                    if (!isAdjacent(row, col))
                        continue;

                    if (WumpusGame.playerMap[row, col].visited)
                        continue; // we've already visisted this... ignore

                    switch (type)
                    {
                        case UpdateType.NO_PIT:
                            WumpusGame.playerMap[row, col].setStateNoPit();
                            break;
                        case UpdateType.NO_WUMPUS:
                            WumpusGame.playerMap[row, col].setStateNoWumpus();
                            break;
                        case UpdateType.POTENTIAL_PIT:
                            WumpusGame.playerMap[row, col].handlePotentialPit();
                            break;
                        case UpdateType.POTENTIAL_WUMPUS:
                            WumpusGame.playerMap[row, col].handlePotentialWumpus();
                            break;
                    }
                }
            }
        }

        private bool isAdjacent(int row, int col)
        {
            if (row < 0 || row >= WumpusGame.totalRows)
                return false;

            if (col < 0 || col >= WumpusGame.totalCols)
                return false; // column not within bounds. ignore.

            if (row == _row && col == _col)
                return false; // ignore self

            if (_row - 1 == row && _col - 1 == col)
                return false; // ignore top left

            if (_row - 1 == row && _col + 1 == col)
                return false; // ignore top right
            
            if (_row + 1 == row && _col + 1 == col)
                return false; // ignore bottom right

            if (_row + 1 == row && _col - 1 == col)
                return false; // ignore bottom left

            return true;
        }
    }
}
