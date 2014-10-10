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
        }
    }
}
