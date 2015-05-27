using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Numerics;
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
        private int latestMsg = 0;
        private RSA rsa = new RSA();
        public RSAKey publicKey;
        public string publicDes = "";
        private RSAKey certificateKey = null;
        public string certificate = "";
        private string ecbKey = "";

        public Chat()
        {
            InitializeComponent();

            rsa.GenerateKey();
            ecbKey = ECB.GenerateKey();

            Package verf = new Package();
            verf.SetHeader("Command", "Register");
            verf.SetHeader("Public Key n", rsa.Key.n.ToString());
            verf.SetHeader("Public Key e", rsa.Key.e.ToString());
            Package certf = SendCommand(verf, 2745);
            BigInteger n = BigInteger.Parse(certf.GetHeader("Public Key n"));
            BigInteger e = BigInteger.Parse(certf.GetHeader("Public Key e"));
            certificateKey = new RSAKey();
            certificateKey.d = e;
            certificateKey.n = n;
            certificate = certf.GetContent();

            Console.WriteLine("Register Certificate: "+certificate);
            //certf.Print();
        }

        private String SendMessage(String message)
        {
            Package pck = new Package();
            RSA rsaPub = new RSA(publicKey);
            pck.SetHeader("Command", "Message");
            pck.SetContent(message);
            return SendCommand(pck).GetContent();
        }

        private Package SendCommand(Package pck)
        {
            pck.SetHeader("Certificate", certificate);
            return SendCommand(pck, 1234);
        }

        private Package SendCommand(Package pck, int port)
        {
            TcpClient client = new TcpClient();
            IPEndPoint serverEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), port);
            client.Connect(serverEndPoint);
            NetworkStream clientStream = client.GetStream();
            ASCIIEncoding encoder = new ASCIIEncoding();

            if (pck.GetContent() != "")
                pck.SetContent(ECB.encrypt(pck.GetContent(), ecbKey));

            String s = pck.GetString();

            byte[] buffer = encoder.GetBytes(s);

            clientStream.Write(buffer, 0, buffer.Length);
            clientStream.Flush();

            byte[] bufferResponse = new byte[4096];
            clientStream.Read(bufferResponse, 0, 4096);

            String response = encoder.GetString(bufferResponse).Replace("\0", "");
            client.Close();

            Package resp = new Package();
            resp.SetByString(response);

            //Console.WriteLine("#Response From Server: ");
            //resp.Print();

            if (port == 1234 && resp.GetContent() != "")
            {
                resp.SetContent(ECB.decrypt(resp.GetContent(), publicDes));
            }

            return resp;
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
                Package pck = new Package();
                pck.SetHeader("Command", "Connect");
                pck.SetHeader("Username", textBox2.Text);
                Package response = SendCommand(pck);
                RSA certRSA = new RSA(certificateKey);
                string[] respCertf = certRSA.decrypt(response.GetHeader("Certificate")).Split(';');
                publicKey = new RSAKey(BigInteger.Parse(respCertf[2]), BigInteger.Parse(respCertf[3]));

                // Send once more for des key
                pck = new Package();
                pck.SetHeader("Command", "DES");
                RSA rsaPub = new RSA(publicKey);
                pck.SetHeader("DES Key", rsaPub.encrypt(ecbKey));
                response = SendCommand(pck);
                this.publicDes = response.GetHeader("DES Key");
                this.publicDes = rsa.decrypt(this.publicDes);
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
                Package pck = new Package();
                pck.SetHeader("Command", "Disconnect");
                SendCommand(pck);
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
                
                Package pck = new Package();
                pck.SetHeader("Command", "New Message");
                pck.SetContent(latestMsg.ToString());
                Package response = SendCommand(pck);

                if (response.GetContent() != "")
                {
                    listBox1.Items.Add(response.GetContent());
                    latestMsg++;
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
            RSAKey key = rsa.Key;
            listBox1.Items.Add("RSA n: " + key.n);
            listBox1.Items.Add("RSA e: " + key.e);
            listBox1.Items.Add("RSA d: " + key.d);
            listBox1.Items.Add("ECB Key: " + ecbKey);
            listBox1.Items.Add("Public ECB Key: " + publicDes);
            listBox1.Items.Add("Certificate: " + certificate);
        }
    }
}
