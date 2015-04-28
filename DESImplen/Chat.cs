using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net.Sockets;
using System.Net;
using DESEncryption;
using System.Collections;

namespace DESImplen
{
    public partial class Chat : Form
    {
        private bool connected = false;
        private int userId = 0;
        private int latestMsg = 0;
        private RSA rsa = new RSA();
        private string publicKey = "";

        public Chat()
        {
            InitializeComponent();

            publicKey = rsa.GeneratePublicKey();
        }

        private String SendMessage(String message)
        {
            return SendCommand("message", message);
        }

        private String SendCommand(String command, String message)
        {
            TcpClient client = new TcpClient();
            IPEndPoint serverEndPoint = new IPEndPoint(IPAddress.Parse(textBox3.Text), 1234);
            client.Connect(serverEndPoint);
            NetworkStream clientStream = client.GetStream();
            ASCIIEncoding encoder = new ASCIIEncoding();

            Console.WriteLine(userId.ToString() + ";" + command + ";" + message);
            String s = userId.ToString() + ";" + ECB.encrypt(command, publicKey) + ";" + ECB.encrypt(message, publicKey) + ";" + publicKey;
            Console.WriteLine(s);
            
            //s = ECB.encrypt(s);

            byte[] buffer = encoder.GetBytes(s);

            clientStream.Write(buffer, 0, buffer.Length);
            clientStream.Flush();

            byte[] bufferResponse = new byte[4096];
            clientStream.Read(bufferResponse, 0, 4096);

            String response = encoder.GetString(bufferResponse).Replace("\0", "") ;
            client.Close();

            response = ECB.decrypt(response, publicKey);

            return response;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ChatSend();
        }

        private void ChatSend()
        {
            if (textBox1.Text != "")
            {
                String message = textBox1.Text;
                SendMessage(message);
            }

            textBox1.Text = "";
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (!connected)
            {
                String sUserId = SendCommand("connect", textBox2.Text);
                userId = int.Parse(sUserId);
                connected = true;

                button2.Text = "Disconnect";
                textBox3.Enabled = false;
                listBox1.Enabled = true;
                textBox1.Enabled = true;
                textBox2.Enabled = false;
                button1.Enabled = true;
            }
            else
            {
                SendCommand("disconnect", userId.ToString());
                connected = false;

                button2.Text = "Connect to server...";
                textBox3.Enabled = true;
                listBox1.Enabled = false;
                textBox1.Enabled = false;
                textBox2.Enabled = true;
                button1.Enabled = false;
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (connected == true)
            {
                timer1.Enabled = false;

                String latestMessage = SendCommand("getLatest", latestMsg.ToString());

                if (latestMessage != "" && latestMessage != "0")
                {
                    listBox1.Items.Add(latestMessage);
                    latestMsg++;
                }

                String user = SendCommand("getUser", latestMsg.ToString());

                if (user != "" && user != "0")
                {
                    String[] exp = user.Split(';');

                    listBox2.Items.Clear();
                    for (int i = 1; i < exp.Length; i++)
                    {
                        listBox2.Items.Add(exp[i]);
                    }
                }

                timer1.Enabled = true;
            }
        }

        private void textBox1_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                ChatSend();
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            listBox1.Items.Add("Public Key: " + publicKey);
            listBox1.Items.Add("Private Key: " + rsa.GeneratePrivateKey(publicKey));
        }
    }
}
