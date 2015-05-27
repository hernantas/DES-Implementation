using DESEncryption;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Numerics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Certificate
{
    public partial class Form1 : Form
    {
        private TcpListener listener = null;
        private Thread thread = null;
        private bool listening = true;
        private RSA rsa = new RSA();
        private List<UserCertificate> listCert = new List<UserCertificate>();
        int lastId = 0;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            rsa.GenerateKey();
            this.listener = new TcpListener(IPAddress.Any, 2745);
            this.listener.Start();
            this.thread = new Thread(new ThreadStart(ListenToClient));
            this.thread.Start();
        }

        private void ListenToClient()
        {
            while (listening)
            {
                TcpClient client = this.listener.AcceptTcpClient();

                Thread clientThread = new Thread(new ParameterizedThreadStart(HandleClient));
                clientThread.Start(client);
            }

            this.listener.Stop();
        }

        private void HandleClient(object client)
        {
            TcpClient tcpClient = (TcpClient)client;

            NetworkStream clientStream = tcpClient.GetStream();

            byte[] message = new byte[4096];
            int bytesRead = 0;

            while (true)
            {
                try
                {
                    //blocks until a client sends a message
                    bytesRead = clientStream.Read(message, 0, 4096);
                }
                catch
                {
                    //a socket error has occured
                    break;
                }

                if (bytesRead == 0)
                    break;

                //message has successfully been received
                ASCIIEncoding encoder = new ASCIIEncoding();
                String clientMessage = encoder.GetString(message, 0, bytesRead);
                //Console.WriteLine(clientMessage);

                Package response = ProcessMessage(new Package(clientMessage));
                byte[] byteResponse = encoder.GetBytes(response.GetString());

                clientStream.Write(byteResponse, 0, byteResponse.Length);

                clientStream.Flush();
            }

            tcpClient.Close();
        }

        public String GetTimestamp(DateTime value)
        {
            return value.ToString("yyyyMMddHHmmssfff");
        }

        private void AddCertificate(UserCertificate cert)
        {
            if (InvokeRequired)
            {
                this.Invoke(new Action<UserCertificate>(AddCertificate), new object[] { cert });
                return;
            }

            listCert.Add(cert);
            listBox1.Items.Add("ID: " + cert.Id + " Public Key: " + cert.publicKey.n+cert.publicKey.e);
        }

        private Package ProcessMessage(Package pck)
        {
            //Console.WriteLine("#Certifiate Client: ");
            //pck.Print();

            Package response = new Package();

            switch (pck.GetHeader("Command"))
            {
                case "Register":
                    BigInteger n = BigInteger.Parse(pck.GetHeader("Public Key n"));
                    BigInteger e = BigInteger.Parse(pck.GetHeader("Public Key e"));
                    UserCertificate newCertf = new UserCertificate(lastId++);
                    newCertf.publicKey = new RSAKey(n, e);
                    newCertf.timestamp = GetTimestamp(new DateTime());
                    AddCertificate(newCertf);
                    response.SetHeader("Public Key n", rsa.Key.n.ToString());
                    response.SetHeader("Public Key e", rsa.Key.e.ToString());
                    response.SetContent(newCertf.ToString());
                    break;

                case "Verify":
                    int index = -1;
                    for (int i = 0; i < listCert.Count; i++)
                    {
                        if (listCert[i].ToString().Equals(pck.GetHeader("Certificate")))
                        {
                            index = 1;
                        }
                    }

                    if (index != -1)
                        response.SetContent("TRUE");
                    else
                        response.SetContent("FALSE");

                    break;
            }

            if (response.GetContent() != "")
            {
                string enc = rsa.encrypt(response.GetContent(), rsa.Key.d, rsa.Key.n);
                // string dec = rsa.decrypt(enc, rsa.Key.e, rsa.Key.n);
                // Console.WriteLine(enc + " => " +dec);
                response.SetContent(enc);
            }

            return response;
        }
    }
}
