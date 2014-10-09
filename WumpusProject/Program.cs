using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WumpusProject
{
    class Program
    {
        static void Main(string[] args)
        {
            List<string> test = new List<string>();
            populateTest(test);

            string choice = "";

            bool _runningGame = false;
            WumpusGame _game = null;
            bool _gridSet = false;

            do
            {
                if (!_runningGame && test != null)
                {
                    Console.WriteLine("Welcome to the Wumpus Game! A test has been set, use it? (Y/N):");
                    Console.Write("Input:");
                    choice = Console.ReadLine();
                }

                if (choice.ToUpper() == "Y")
                { // run test!
                    _game = new WumpusGame();

                    Console.WriteLine(_game.SetupGridSize(test[0]));

                    int len = test.Count;
                    for (int i = 1; i < len; ++i)
                    {
                        Console.WriteLine(_game.AddAttributes(test[i]));
                    }

                    _game.Run();
                    Console.WriteLine(); // just to add a new line
                    continue;
                }
                else
                {
                    _runningGame = true;
                }

                /* Not using test. Run game normally! */

                if (!_gridSet)
                {
                    _gridSet = true;

                    Console.WriteLine("Welcome to the Wumpus Game! Please set the board size (row,col i.e. \"5,5\" for a 5 by 5 board):");
                    Console.Write("Input:");
                    choice = Console.ReadLine();

                    _game = new WumpusGame();
                    Console.WriteLine(_game.SetupGridSize(choice));
                    continue;
                }

                // grid has been set, get rest of setup attributes or get run
                Console.WriteLine("Add attribute(s) [row,col,<attributes>] OR type \"run\" to start. Type -help for help.");
                Console.Write("Input:");
                choice = Console.ReadLine();

                switch (choice)
                {
                    case "-help":
                        Console.WriteLine("\tThe followin attributes are allowed (NOTE: Seperators for attributes are NOT needed!):\n\tE - Enter\n\tW - Wumpus\n\tB - Breezy\n\tS - Stench\n\tG - Gold\n\tP - Pit");
                        break;
                    case "run":
                        _gridSet = false;
                        _game.Run();
                        Console.WriteLine(); // just to add a new line
                        break;
                    case "exit":
                        break;
                    default:
                        Console.WriteLine(_game.AddAttributes(choice));
                        break;
                }
            } while (choice != "exit");
        }

        // Consider additional static functions for unit testing
        private static void populateTest(List<string> test)
        {
            test.Add("5,5"); // grid size
            test.Add("0,0,E");
            test.Add("0,1,B");
            test.Add("0,2,P");
            test.Add("0,3,B");
            test.Add("1,2,B");
            test.Add("2,0,B");
            test.Add("2,2,S");
            test.Add("2,4,B");
            test.Add("3,0,P");
            test.Add("3,1,BS");
            test.Add("3,2,W");
            test.Add("3,3,GSB");
            test.Add("3,4,P");
            test.Add("4,0,B");
            test.Add("4,1,P");
            test.Add("4,2,BS");
            test.Add("4,4,B");
        }
    }
}
