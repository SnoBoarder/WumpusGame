using System;

namespace WumpusProject
{
    public class PerfectNode
    {
        private bool _isPit = false;
        private bool _isWumpus = false;
        private bool _isBreezy = false;
        private bool _isStench = false;
        private bool _hasGold = false;

        public PerfectNode()
        {

        }

        public bool isPit
        {
            get { return _isPit; }
            set
            {
                if (WumpusGame.runningSetup)
                    _isPit = value;
            }
        }
        public bool isWumpus
        {
            get { return _isWumpus; }
            set
            {
                if (WumpusGame.runningSetup)
                    _isWumpus = value;
            }
        }
        public bool isBreezy
        {
            get { return _isBreezy; }
            set
            {
                if (WumpusGame.runningSetup)
                    _isBreezy = value;
            }
        }
        public bool isStench
        {
            get { return _isStench; }
            set
            {
                if (WumpusGame.runningSetup)
                    _isStench = value;
            }
        }
        public bool hasGold
        {
            get { return _hasGold; }
            set
            {
                if (WumpusGame.runningSetup)
                    _hasGold = value;
            }
        }

    }
}
