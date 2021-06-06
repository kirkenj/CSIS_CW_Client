using System;
using System.Net.Sockets;
using System.Text;
using System.Net;
using System.Threading;
using System.Drawing;

namespace CSIS_CW_Client
{
    class Client
    {
        private const string IP_STR = "127.0.0.1";
        private const int PORT = 11000;
        public IPEndPoint IPEndPoint { get; set; }
        private const int SERVER_PERIOD = 15;

        private Thread _listenThread;
        private readonly Thread _mainThread;
        private readonly TcpClient _client;
        private readonly GameMessage _message = new GameMessage();
        private readonly int _poleWidth;
        private readonly int _poleHeigth;
        private readonly Bitmap _bitmap;
        private ClientCircle _circle = new ClientCircle();

        public Client( int poleWidth, int poleHeigth)
        {
            _client = new TcpClient();
            _mainThread = new Thread(BootAndOutputLogic);
            this._poleHeigth = poleHeigth;
            this._poleWidth = poleWidth;
            _bitmap = new Bitmap(_poleWidth, _poleHeigth);
        }

        public bool GameStarted { get; set; } = false;
        public string ConsoleStr { get; private set; }
        public bool Connected { get { return _client.Connected; } }
        public Bitmap Bitmap
        {
            get
            {
                Graphics g = Graphics.FromImage(_bitmap);
                g.Clear(Color.White);
                if (_circle.Show)
                {
                    Rectangle rectangle = _circle.Rectangle;
                    g.FillEllipse(Brushes.Red, rectangle);
                }
                if (_message.Show)
                {
                    g.DrawString(_message.Text, new Font(FontFamily.GenericSerif, 10), Brushes.Blue, new Point(10, 10));
                }
                return _bitmap;
            }
        }
        
        public void Start()
        {
            try
            {
                _client.Connect(IPEndPoint);
                SendMsg(ServerCommands.InitData.ToString() + ": " + '{' + $"{this._poleWidth},{this._poleHeigth}" + '}');
                _mainThread.Start();
            }
            catch (Exception ex)
            {
                OutlnConsole(ex.Message);
            }
        }
        private void BootAndOutputLogic()
        {
            _listenThread = new Thread(RecieveMessage);
            _listenThread.Start();
            try
            {
                while (_client.Client != null && _client.Client.Connected)
                {

                    if (_circle.Show)
                    {
                        _circle.LifeTime -= SERVER_PERIOD;
                    }
                    if (_message.Show)
                    {
                        _message.LifeTime -= SERVER_PERIOD;
                    }
                    Thread.Sleep(SERVER_PERIOD);
                }

            }
            catch (Exception ex)
            {
                OutlnConsole(ex.Message + ex.Source);
            }
        }
        private void RecieveMessage()
        {
            byte[] byteRes;
            int arrLen;
            string recStr;
            OutlnConsole("Жду сообщений...");
            while (_client.Connected)
            {
                Thread.Sleep(SERVER_PERIOD);


                byteRes = new byte[1024];
                try
                {
                    arrLen = _client.Client.Receive(byteRes);
                    if (arrLen > 0)
                    {
                        recStr = Encoding.UTF8.GetString(byteRes);
                        while (!(char.IsLetterOrDigit(recStr[recStr.Length - 1]) || char.IsPunctuation(recStr[recStr.Length - 1])))
                        {
                            recStr = recStr.Remove(recStr.Length - 1);
                        }
                        OutlnConsole("Сообщение от сервера: " + recStr);
                        HandleMessage(recStr);
                    }
                }
                catch (SocketException)
                {
                    break;
                }
                catch (Exception ex)
                {
                    ConsoleStr += ex.GetType().Name + " " + ex.Message + '\n';
                }
            }
            OutlnConsole("Соединение потеряно");   
        }
        public void SendMsg(string str)
        {
            if (_client.Connected)
            {
                byte[] sendArr = Encoding.UTF8.GetBytes(str);
                this._client.Client.Send(sendArr);
            }
        }
        public void ClearMessage()
        {
            _message.LifeTime = -1;
        }
        public void Stop()
        {
            SendMsg(ServerCommands.CloseSocket + " .");
            _client.Close();
            Thread.Sleep(30);
            if (!(_listenThread == null) && _listenThread.IsAlive)
            {
                _listenThread.Abort();
                _ = _listenThread;
            }
            if (!(_mainThread == null) && _mainThread.IsAlive)
            {
                _mainThread.Abort();
            }
        }
        private void OutlnConsole(string msg)
        {
            ConsoleStr += msg + "\r\n";
        }
        private void HandleMessage(string str)
        {
            string[] wordsArr = str.Split();
            if (wordsArr[0] == ServerCommands.TransportPoint.ToString() + ':')
            {
                _circle = ClientCircle.Parse(wordsArr[1]);
                OutlnConsole($"Got circle: {_circle.ToString()}");
            }
            else if (wordsArr[0] == ServerCommands.Stop.ToString())
            {
                OutlnConsole("Игра остановлена");
                GameStarted = false;
                _message.Text = wordsArr[1].Replace('_', ' ');
            }
            else if (wordsArr[0] == ServerCommands.GameMessage.ToString() + ':')
            {
                _message.Text = wordsArr[1].Replace('_', ' ');
            }
        }
        public void StartLevel(int levelIDSince1)
        {
            SendMsg($"StartLevel{levelIDSince1} .");
        }
    }
}
