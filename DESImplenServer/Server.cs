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
            if (listBox1.Items.Count != server.UserList.Count)
            {
                listBox1.Items.Clear();

                foreach (UserData udat in server.UserList)
                {
                    listBox1.Items.Add(udat.username + "(" + udat.key.n + "," + udat.key.e + ")");
                }
            }

            if (listBox2.Items.Count != server.BufferMessage.Count)
            {
                listBox2.Items.Clear();

                foreach (Message msg in server.BufferMessage)
                {
                    if (msg.user != null)
                        listBox2.Items.Add(msg.user.username + " = " + msg.message);
                    else
                        listBox2.Items.Add("# " + msg.message);
                }
            }
        }
    }
}
