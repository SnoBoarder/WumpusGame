using System;
using System.Collections.Generic;

namespace WumpusProject
{
    public class WumpusGame
    {
        private const string START_LOCATION = "E";
        private const string BREEZY = "B";
        private const string STENCH = "S";
        private const string GOLD = "G";
        private const string PIT = "P";
        private const string WUMPUS = "W";

        private static bool _runningSetup; // make sure no one manipulates PerfectNodes after setup

        public static bool wumpusFound = false;

        public static int boardRows;
        public static int boardCols;

        public static PerfectNode[,] perfectMap;
        public static Node[,] playerMap; // TODO: Although it's a double list, I suggest we set it up like perfect node where it's a double array for simplicity

        private int _startRow = -1;
        private int _startCol = -1;

        public WumpusGame()
        {
            // reset static variables
            wumpusFound = false;
        }
        
        public static bool runningSetup
        {
            get { return _runningSetup; }
        }

        public string SetupGridSize(string choice)
        {
            _runningSetup = true;

            // NOTE: Currently assumes a square grid
            string[] choices = choice.Split(',');
            boardRows = Convert.ToInt32(choices[0]);
            boardCols = Convert.ToInt32(choices[1]);

            perfectMap = new PerfectNode[boardRows, boardCols];
            playerMap = new Node[boardRows, boardCols];

            for (int row = 0; row < boardRows; row++)
            {
                for (int col = 0; col < boardCols; col++)
                {
                    perfectMap[row, col] = new PerfectNode();
                    playerMap[row, col] = null; // initialize to null so that we can populate anything at any point
                }
            }


            return "\n\tGrid size is " + boardRows + " by " + boardCols + "\n";
        }

        public string AddAttributes(string choice)
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

        public void Run()
        {
            _runningSetup = false;

            if (_startRow == -1 || _startCol == -1)
                throw new Exception("Start location hasn't been set!");

            // NOTE: Although we should have additional checks on making sure
            // the board is valid, it may be overkill in terms of this project.

            Console.WriteLine("~~NOW SOLVING WUMPUS GAME~~");
            
            // TODO: SETUP AND RUN ALGORITHM HERE
        }
    }
}
