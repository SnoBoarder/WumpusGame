using System;

namespace WumpusProject
{
    public class Command
    {
        private int _row = -1;
        private int _col = -1;
        private WumpusGame.Direction _shootArrow;

        public Command(int row, int col, WumpusGame.Direction shootArrow = WumpusGame.Direction.NONE)
        {
            _row = row;
            _col = col;
            _shootArrow = shootArrow;
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

        public string output
        {
            get
            {
                string str = "Moving to " + _row + ", " + _col;

                switch (_shootArrow)
                {
                    case WumpusGame.Direction.DOWN:
                        str += " and shooting arrow DOWN";
                        break;
                    case WumpusGame.Direction.LEFT:
                        str += " and shooting arrow LEFT";
                        break;
                    case WumpusGame.Direction.RIGHT:
                        str += " and shooting arrow RIGHT";
                        break;
                    case WumpusGame.Direction.UP:
                        str += " and shooting arrow UP";
                        break;
                }

                return str;
            }
        }
    }
}
