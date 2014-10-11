using System;
using System.Collections.Generic;
using System.Windows;

namespace WumpusProject
{
    public class WumpusGame
    {
        public enum Direction { NONE, UP, LEFT, DOWN, RIGHT };

        private const string START_LOCATION = "E";
        private const string BREEZY = "B";
        private const string STENCH = "S";
        private const string GOLD = "G";
        private const string PIT = "P";
        private const string WUMPUS = "W";

        private static bool _runningSetup; // make sure no one manipulates PerfectNodes after setup

        public static PerfectNode[,] perfectMap;
        public static Node[,] playerMap;

        public static bool goldFound = false;
        public static bool wumpusKilled = false;

        public static int totalRows;
        public static int totalCols;

        public static List<Node> potentialWumpusNodes;

        private static int _wumpusRow = -1;
        private static int _wumpusCol = -1;

        private int _startRow = -1;
        private int _startCol = -1;

        private List<Command> _commands;
        private List<Node> _goals;

        public WumpusGame()
        {
            // reset static variables
            wumpusKilled = false;
            goldFound = false;

            if (potentialWumpusNodes == null)
                potentialWumpusNodes = new List<Node>();

            potentialWumpusNodes.Clear();

            _wumpusRow = -1;
            _wumpusCol = -1;

            Node.OnSafetyPitCheck += checkPitSafety;
            Node.OnSafetyWumpusCheck += checkWumpusSafety;
        }

        #region SetupRegion
        public static bool runningSetup
        {
            get { return _runningSetup; }
        }

        public string setupGridSize(string choice)
        {
            _runningSetup = true;

            // NOTE: Currently assumes a square grid
            string[] choices = choice.Split(',');
            totalRows = Convert.ToInt32(choices[0]);
            totalCols = Convert.ToInt32(choices[1]);

            perfectMap = new PerfectNode[totalRows, totalCols];
            playerMap = new Node[totalRows, totalCols];

            for (int row = 0; row < totalRows; row++)
            {
                for (int col = 0; col < totalCols; col++)
                {
                    perfectMap[row, col] = new PerfectNode();
                    playerMap[row, col] = new Node(row, col);
                }
            }

            return "\n\tGrid size is " + totalRows + " by " + totalCols + "\n";
        }

        public string addAttributes(string choice)
        {
            string retVal = "\n\t";

            string[] choices = choice.Split(',');
            
            int row = Int32.Parse(choices[0]);
            int col = Int32.Parse(choices[1]);
            string attributes = choices[2];

            char[] attributeList = attributes.ToCharArray();

            int len = attributeList.Length;
            string attribute;
            PerfectNode pNode;
            for (int i = 0; i < len; ++i)
            {
                attribute = attributeList[i].ToString().ToUpper();
                pNode = perfectMap[row, col];
                switch (attribute)
                {
                    case START_LOCATION:
                        _startRow = row;
                        _startCol = col;

                        retVal += "Player will enter at: (" + _startRow + "," + _startCol + ")\n";
                        break;
                    case WUMPUS:
                        pNode.isWumpus = true;
                        retVal += "Wumpus";
                        break;
                    case BREEZY:
                        pNode.isBreezy = true;
                        retVal += "Breezy";
                        break;
                    case STENCH:
                        pNode.isStench = true;
                        retVal += "Stench";
                        break;
                    case GOLD:
                        pNode.hasGold = true;
                        retVal += "Gold";
                        break;
                    case PIT:
                        pNode.isPit = true;
                        retVal += "Pit";
                        break;
                }

                if (i + 1 < len)
                {
                    retVal += ", ";
                }
            }

            if (attributeList[0].ToString().ToUpper() != START_LOCATION)
                retVal +=  " has been set at: (" + row + "," + col + ")\n";
            
            return retVal;
        }
        #endregion Setup

        public static void setWumpusPosition(int row, int col)
        {
            if (_wumpusRow != -1 || _wumpusCol != -1)
                throw new Exception("Wumpus position has already been set!");

            _wumpusRow = row;
            _wumpusCol = col;
        }

        private void checkPitSafety(Node node)
        {
            if (!node.visited && node.isSafe)
            { // add the node to the goal list since it's safe!

                // TODO: Discuss where it should be added! For now it's added to the top of the list
                _goals.Add(node);
            }
        }

        private void checkWumpusSafety(Node node)
        {
            if (!node.visited  && node.isSafe)
            { // add the node to the goal list since it's safe!

                // TODO: Discuss where it should be added! For now it's added to the top of the list
                _goals.Add(node);
            }

            // remove node from potentialWumpusNodes
            potentialWumpusNodes.Remove(node);

            if (potentialWumpusNodes.Count == 1)
            { // WE FOUND THE WUMPUS BY REMOVING ALL OTHER OPTIONS!
                Node wumpusNode = potentialWumpusNodes[0];
                setWumpusPosition(wumpusNode.row, wumpusNode.col);
                potentialWumpusNodes.Clear();
            }
        }

        public void run()
        {
            _runningSetup = false;

            if (_startRow == -1 || _startCol == -1)
                throw new Exception("Start location hasn't been set!");

            // NOTE: Although we should have additional checks on making sure
            // the board is valid, it may be overkill in terms of this project.

            Console.WriteLine("~~NOW SOLVING WUMPUS GAME~~");

            Node currentNode = null;
            int currRow = _startRow;
            int currCol = _startCol;

            Node goalNode = null;
            _goals = new List<Node>();
            _commands = new List<Command>();

            // TODO: SETUP AND RUN ALGORITHM HERE
            while (true)
            {
                // Get perfect node information and set it to the current node
                // current node will handle updating the adjacent neighbors accordingly
                currentNode = playerMap[currRow, currCol];
                currentNode.updateFromPerfectNode(perfectMap[currRow, currCol]);

                // Get updated goals
                tryToAddToGoals(currRow - 1, currCol); // try adding UP
                tryToAddToGoals(currRow, currCol - 1); // try adding LEFT
                tryToAddToGoals(currRow + 1, currCol); // try adding DOWN
                tryToAddToGoals(currRow, currCol + 1); // try adding RIGHT
                
                // Handle goal
                if (_goals.Count == 0)
                { // WE HAVE NO GOALS
                    if (_wumpusRow != -1 && _wumpusCol != -1)
                    { // we know where the wumpus is! KILL IT

                        // Step 1: find shortest path to kill the wumpus (populate the commands)

                        // Step 2: if goldFound is false, then use distance method to find shortest path to get to the wumpus location (populate the commands)

                        // Step 3: if goldFound is true, GO HOME
                            // find shortest path from current location to the entrance and store the commands

                            // we're done with the algorithm so break out
                            //break;

                    }
                    else
                    { // we don't know where the wumpus is!
                        // GO HOME! WE GIVE UP!

                        // find shortest path from current location to the entrance and store the commands

                        // we're done with the algorithm so break out
                        break;
                    }
                }
                else
                { // WE HAVE GOALS
                    // pop the last node out of the goals list
                    goalNode = _goals[_goals.Count - 1];
                    _goals.Remove(goalNode);

                    // if goal is next to current goal
                    if (getDirection(currentNode, goalNode) != Direction.NONE)
                    { // set the command and then continue
                        _commands.Add(new Command(goalNode.row, goalNode.col));
                    }
                    else
                    { // otherwise calculate the shortest visited path to the goal and populate the command list
                        throw new Exception("IMPLEMENT SHORTEST VISITED PATH METHOD");
                    }

                    // set current row and current column
                    currRow = goalNode.row;
                    currCol = goalNode.col;
                }
            }

            // TODO: Display commands here!
        }

        private void tryToAddToGoals(int row, int col)
        {
            if (row < 0 || row >= totalRows)
                return; // row not within bounds. ignore.

            if (col < 0 || col >= totalCols)
                return; // column not within bounds. ignore.

            Node node = playerMap[row, col];

            if (node.visited)
                return; // has been visited. ignore.

            if (!node.isSafe)
                return; // not safe. ignore.

            if (_goals.IndexOf(node) != -1)
                _goals.Remove(node); // remove from the list to then add it to the end

            _goals.Add(node); // add to the end of the list
        }

        private Direction getDirection(Node fromNode, Node toNode)
        {
            if (fromNode.row - 1 >= 0 && playerMap[fromNode.row - 1, fromNode.col] == toNode)
                return Direction.UP;

            if (fromNode.col - 1 >= 0 && playerMap[fromNode.row, fromNode.col - 1] == toNode)
                return Direction.LEFT;

            if (fromNode.row + 1< totalRows && playerMap[fromNode.row + 1, fromNode.col] == toNode)
                return Direction.DOWN;

            if (fromNode.col + 1 < totalCols && playerMap[fromNode.row, fromNode.col + 1] == toNode)
                return Direction.RIGHT;

            return Direction.NONE;
        }
    }
}
