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
        public static Distance tool_;

        public static bool goldFound = false;
        public static bool wumpusKilled = false;
        public static bool goHome = false;

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
            goHome = false;

            tool_ = new Distance();
            
            if (potentialWumpusNodes == null)
                potentialWumpusNodes = new List<Node>();

            potentialWumpusNodes.Clear();

            _wumpusRow = -1;
            _wumpusCol = -1;

            Node.OnSafetyPitCheck += checkPitSafety;
            Node.OnSafetyWumpusCheck += checkWumpusSafety;
            Node.OnGoldFound += setGoHome;
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

            if (attributes == "")
                return "No attributes were set at: (" + row + "," + col + ")\n";

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

        public static bool wumpusFound
        {
            get
            {
                return _wumpusRow != -1 && _wumpusCol != -1;
            }
        }

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

            if (wumpusFound)
                return; // we've already found the wumpus. ignore

            // remove node from potentialWumpusNodes
            potentialWumpusNodes.Remove(node);

            if (potentialWumpusNodes.Count == 1)
            { // WE FOUND THE WUMPUS BY REMOVING ALL OTHER OPTIONS!
                Node wumpusNode = potentialWumpusNodes[0];
                setWumpusPosition(wumpusNode.row, wumpusNode.col);
                potentialWumpusNodes.Clear();
            }
        }

        private void setGoHome()
        {
            // since top and only priority is to find the gold, go home once we find the gold.
            if (goldFound && !goHome)
                goHome = true;
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

            // add the first move to the list(?) TODO: Consider not adding the first move to the list
            addCommand(currRow, currCol);

            // TODO: SETUP AND RUN ALGORITHM HERE
            while (true)
            {
                // Get perfect node information and set it to the current node
                // current node will handle updating the adjacent neighbors accordingly
                currentNode = playerMap[currRow, currCol];
                currentNode.updateFromPerfectNode(perfectMap[currRow, currCol]);

                if (goHome)
                { // we've found the gold! go home!
                    tool_.updateBoard(playerMap);

                    // find shortest path from current location to the entrance and store the commands
                    populateCommandList(tool_.distTo(currentNode, playerMap[_startRow, _startCol]));

                    // we're done with the algorithm so break out
                    break;
                }

                cleanupGoals();

                // Get updated goals
                tryToAddToGoals(currRow - 1, currCol); // try adding UP
                tryToAddToGoals(currRow, currCol - 1); // try adding LEFT
                tryToAddToGoals(currRow + 1, currCol); // try adding DOWN
                tryToAddToGoals(currRow, currCol + 1); // try adding RIGHT

                // Handle goal
                if (_goals.Count == 0)
                { // WE HAVE NO GOALS
                    if (!wumpusKilled && _wumpusRow != -1 && _wumpusCol != -1)
                    { // we know where the wumpus is! KILL IT

                        // Step 1: find shortest path to kill the wumpus (populate the commands)
                        currentNode = killWumpus(currentNode); // populates command and makes final command kill the wumpus. make it the current node

                        // Step 2: if goldFound is false, then use distance method to find shortest path to get to the wumpus location (populate the commands)
                        if (!goldFound)
                        {
                            // update the distance function's board
                            tool_.updateBoard(playerMap);

                            Node wumpusNode = playerMap[_wumpusRow, _wumpusCol];
                            populateCommandList(tool_.distTo(currentNode, wumpusNode));

                            currentNode = wumpusNode;
                            currRow = currentNode.row;
                            currCol = currentNode.col;
                        }
                        else
                        { // Step 3: if goldFound is true, GO HOME
                            
                            // NOTE: Since we are now handling goHome as soon as we find gold, this condition should never occur
                            throw new Exception("Gold has already been found. We should've left already. How are we here?");

                            // update the distance function's board
                            tool_.updateBoard(playerMap);

                            // find shortest path from current location to the entrance and store the commands
                            populateCommandList(tool_.distTo(currentNode, playerMap[_startRow, _startCol]));

                            // we're done with the algorithm so break out
                            break;
                        }
                    }
                    else
                    { // we've already killed the wumpus OR we don't know where the wumpus is!
                        // GO HOME! WE GIVE UP!
                        // update the distance function's board
                        tool_.updateBoard(playerMap);

                        // find shortest path from current location to the entrance and store the commands
                        populateCommandList(tool_.distTo(currentNode, playerMap[_startRow, _startCol]));

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
                        addCommand(goalNode.row, goalNode.col);
                    }
                    else
                    { // otherwise calculate the shortest visited path to the goal and populate the command list
                        // update the distance function's board
                        tool_.updateBoard(playerMap);

                        populateCommandList(tool_.distTo(currentNode, goalNode));
                    }

                    // set current row and current column
                    currRow = goalNode.row;
                    currCol = goalNode.col;
                }
            }

            // TODO: Display commands here!
            int len = _commands.Count;
            for (int i = 0; i < len; ++i)
            {
                Console.WriteLine(_commands[i].output);
            }
        }

        private void addCommand(int row, int col)
        {
            //Console.WriteLine("Command: " + row + ", " + col);
            _commands.Add(new Command(row, col));
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

        private void populateCommandList(string cmdStr)
        {
            populateCommandList(cmdStr.Split(Distance.SEPARATOR));
        }

        /// <summary>
        /// Populate the command list based on the passed on command list
        /// </summary>
        /// <param name="cmdList"></param>
        private void populateCommandList(string[] cmdList)
        {
            string[] pos;

            int len = cmdList.Length;
            for (int i = 0; i < len; ++i)
            {
                if (cmdList[i] == "")
                    continue;

                pos = cmdList[i].Split(',');

                addCommand(Convert.ToInt32(pos[0]), Convert.ToInt32(pos[1]));
            }
        }

        /// <summary>
        /// Clean up the goal list by removing all visited nodes from the goal list
        /// </summary>
        private void cleanupGoals()
        {
            int len = _goals.Count;
            for (int i = len - 1; i >= 0; --i)
            {
                if (_goals[i].visited)
                    _goals.RemoveAt(i);
            }
        }

        /// <summary>
        /// kill wumpus method moves the user to a location to then shoot an arrow at the wumpus to kill int he following steps:
        /// 1. go to nearest row or column of the wumpus.
        /// 2. once there, update the last command to shoot the arrow in the appropriate direction
        /// 3. update that the wumpus has been killed and set the state of the node that had the wumpus to have no wumpus
        /// </summary>
        /// <param name="currentNode"></param>
        /// <returns>return the NEWEST current node position after shooting the wumpus</returns>
        private Node killWumpus(Node currentNode)
        {
            // update the distance function's board
            tool_.updateBoard(playerMap);

            string[] nearestRow = null;

            Node potentialNode = playerMap[_wumpusRow, currentNode.col];
            if (potentialNode.visited)
            { // we've visited the node that is in the same row as the wumpus. get it's distance
                nearestRow = tool_.distTo(currentNode, potentialNode).Split(Distance.SEPARATOR);
            }

            string[] nearestCol = null;
            potentialNode = playerMap[currentNode.row, _wumpusCol];
            if (potentialNode.visited)
            {
                nearestCol = tool_.distTo(currentNode, potentialNode).Split(Distance.SEPARATOR);
            }

            string[] shortestDist;
            if (nearestRow == null)
            { // can't reach by row. take the col option
                shortestDist = nearestCol;
            }
            else if (nearestCol == null)
            { // can't reach by col. take the row option
                shortestDist = nearestRow;
            }
            else
            { // take the option that is the shortest distance to get in the same row or col of the wumpus
                shortestDist = nearestCol.Length <= nearestRow.Length ? nearestCol : nearestRow;
            }

            populateCommandList(shortestDist);

            // set arrow direction for the last command (which should be the same row or column as the wumpus)
            Command lastCommand = _commands[_commands.Count - 1];
            if (lastCommand.row == _wumpusRow)
                lastCommand.shootArrow = _wumpusCol - lastCommand.col > 0 ? Direction.RIGHT : Direction.LEFT;
            else
                lastCommand.shootArrow = _wumpusRow - lastCommand.row > 0 ? Direction.DOWN : Direction.UP;

            // wumpus has been killed! set it!
            wumpusKilled = true;

            // update the wumpus node to have no more wumpus!
            playerMap[_wumpusRow, _wumpusCol].setStateNoWumpus();

            // return new node position
            return playerMap[lastCommand.row, lastCommand.col];
        }
    }
}
