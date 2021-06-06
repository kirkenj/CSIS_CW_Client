using System;
using System.Windows.Forms;
using System.Diagnostics;
using System.Net;

namespace CSIS_CW_Client
{
    public partial class Form1 : Form
    {
        Client client;
        private const int SERVER_PERIOD = 15;
        public Form1()
        {
            InitializeComponent();
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            client = new Client(pictureBox1.Width, pictureBox1.Height);
            timer1.Interval = SERVER_PERIOD;
        }
        private void button1_Click(object sender, EventArgs e)
        {
            string[] buf;
            try
            {
                buf = textBox3.Text.Split(':');
                client.IPEndPoint = new IPEndPoint(IPAddress.Parse(buf[0]), int.Parse(buf[1]));
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            if (!client.Connected && client.IPEndPoint != null)
            {
                client.Start();
                timer1.Start();
            }
            else if (client.IPEndPoint != null) 
            {
                client = new Client(pictureBox1.Width, pictureBox1.Height);
            }
        }
        private void ReviewConsoleAndBitmap()
        {
            textBox1.Text = client.ConsoleStr;
            pictureBox1.Image = client.Bitmap;
            button1.Enabled = !client.Connected;
            textBox3.Enabled = !client.Connected;
            button2.Enabled = client.Connected;
            if (!client.GameStarted)
            {
                button4_Click(null, null);
            }
        }
        private void button2_Click(object sender, EventArgs e)
        {
            client.SendMsg(ServerCommands.GameMessage +": "+ textBox2.Text.Replace(' ','_'));
            textBox2.Text = "";
        }
        private void timer1_Tick(object sender, EventArgs e)
        {
            ReviewConsoleAndBitmap();
        }
        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            textBox1.SelectionStart = textBox1.TextLength;
            textBox1.ScrollToCaret();
        }
        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }
        private void button4_Click(object sender, EventArgs e)
        {
            if (client.GameStarted)
            {
                client.SendMsg(ServerCommands.Stop.ToString() + " .");
            }
            groupBox1.Enabled = true;
            button3.Enabled = true;
            client.GameStarted = false;
        }
        private void pictureBox1_MouseClick(object sender, MouseEventArgs e)
        {
            if (client.GameStarted)
            {
                client.SendMsg(ServerCommands.ClickTransport + ": " + '{' + $"{e.Location.X},{e.Location.Y}" + '}');
            }
            ReviewConsoleAndBitmap();
        }
        private void button3_Click(object sender, EventArgs e)
        {
            if (client.Connected)
            {
                if (radioButton1.Checked)
                {
                    client.StartLevel(1);
                 }
                else if (radioButton2.Checked)
                {
                    client.StartLevel(2);
                }
                else if (radioButton3.Checked)
                {
                    client.StartLevel(3);
                }
                client.ClearMessage();
                groupBox1.Enabled = false;
                button3.Enabled = false;
                client.GameStarted = true;
            }
        }
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            
        }
        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            client.Stop();
            Process.GetCurrentProcess().Close();
            Application.Exit();
        }
    }
}
