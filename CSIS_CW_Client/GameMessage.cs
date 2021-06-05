namespace CSIS_CW_Client
{
    class GameMessage
    {
        private string _text;
        public GameMessage()
        {
            _text = "";
            LifeTime = 3000;
        }
        public int LifeTime { get; set; }
        public bool Show
        {
            get
            {
                return (LifeTime >= 0);
            }
        }
        public string Text
        {
            get
            {
                return _text;
            }
            set
            {
                _text = value;
                LifeTime = 3000;
            }
        }
    }
}
