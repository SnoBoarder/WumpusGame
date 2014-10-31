using System;

namespace WumpusProject
{
    public class Command
    {
        private int _row = -1;
        private int _col = -1;
        private WumpusGame.Direction _shootArrow;
        private bool _goldFoundHere = false;

        public Command(int row, int col, WumpusGame.Direction shootArrow = WumpusGame.Direction.NONE, bool goldFoundHere = false)
        {
            _row = row;
            _col = col;
            _shootArrow = shootArrow;
            _goldFoundHere = false;
        }

        public int row
        {
            get { return _row; }
        }

        public int col
        {
            get { return _col; }
        }

        public WumpusGame.Direction shootArrow
        {
            get { return _shootArrow; }

            set { _shootArrow = value; }
        }

        public bool goldFoundHere
        {
            set
            {
                if (!_goldFoundHere)
                    _goldFoundHere = true;
            }
        }

        public string output
        {
            get
            {
                string str = "Move to " + _row + ", " + _col;

                switch (_shootArrow)
                {
                    case WumpusGame.Direction.DOWN:
                        str += " and shoot arrow DOWN.";
                        break;
                    case WumpusGame.Direction.LEFT:
                        str += " and shoot arrow LEFT.";
                        break;
                    case WumpusGame.Direction.RIGHT:
                        str += " and shoot arrow RIGHT.";
                        break;
                    case WumpusGame.Direction.UP:
                        str += " and shoot arrow UP.";
                        break;
                    case WumpusGame.Direction.NONE:
                        str += ".";
                        break;
                }

                if (_goldFoundHere)
                    str += " Found gold here!";

                return str;
            }
        }
    }
}
