using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DESImplenServer
{
    public partial class Server : Form
    {
        private bool serverStarter = false;
        private ServerChat server = new ServerChat();

        public Server()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (!serverStarter)
            {
                timer1.Start();
                server.StartServer();
                button1.Text = "Stop Server";
                serverStarter = true;
            }
            else
            {
                timer1.Stop();
                server.StopServer();
                button1.Text = "Start Server";
                serverStarter = false;
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            listBox1.Items.Clear();

            foreach (KeyValuePair<int, String> s in server.ClientList)
            {
                listBox1.Items.Add(s.Value + "(" + s.Key + ")");
            }

            listBox2.Items.Clear();

            foreach (Message s in server.BufferMessage)
            {
                listBox2.Items.Add(server.ClientList[s.UserId]+ ": " +s.message);
            }
        }
    }
}
