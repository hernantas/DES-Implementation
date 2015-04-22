using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using DESEncryption;

namespace DESImplenServer
{
    // Message format
    // Connect
    // or
    // userId; message

    class Message
    {
        public int UserId;
        public String message;

        public Message(int userid, string message)
        {
            UserId = userid;
            this.message = message;
        }
    };
    class ServerChat
    {
        private int lastId = 0;
        private TcpListener listener;
        private Thread thread;
        private bool listening;
        private Dictionary<int, String> clientList = new Dictionary<int,String>();
        public Dictionary<int, String> ClientList
        {
            get
            {
                return clientList;
            }
        }
        private List<Message> bufferMessage = new List<Message>();
        public List<Message> BufferMessage
        {
            get
            {
                return bufferMessage;
            }
        }

        public ServerChat()
        {
            listening = false;
            bufferMessage.Clear();
        }

        public void StartServer()
        {
            this.listener = new TcpListener(IPAddress.Any, 1234);
            this.listener.Start();
            this.thread = new Thread(new ThreadStart(ListenToClient));
            this.thread.Start();

            listening = true;
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

                String[] response = ProcessMessage(clientMessage);

                // Process Message
                if (response[1] == "connect")
                {
                    clientList.Add(lastId, response[2]);
                    response[2] = lastId.ToString();
                    lastId++;
                }
                else if (response[1] == "disconnect")
                {
                    Console.WriteLine("DISCONNECT");
                    clientList.Remove(int.Parse(response[2]));
                }
                else if (response[1] == "getLatest")
                {
                    String resx = (bufferMessage.Count-1).ToString();
                    int latest = int.Parse(response[2]);

                    if (latest >= 0 && latest < bufferMessage.Count)
                    {
                        response[2] = clientList[bufferMessage[latest].UserId] + ": " + bufferMessage[latest].message;
                    }
                    else
                    {
                        response[2] = "0";
                    }
                }
                else if (response[1] == "getUser")
                {
                    String resx = "";

                    foreach (KeyValuePair<int, String> pair in clientList)
                    {
                        resx += ";" + pair.Value;
                    }

                    response[2] = resx;

                }
                else if (response[1] == "message")
                {
                    bufferMessage.Add(new Message(int.Parse(response[0]), response[2]));
                }


                String resEnc = ECB.encrypt(response[2]);

                byte[] byteResponse = encoder.GetBytes(resEnc);

                clientStream.Write(byteResponse, 0, byteResponse.Length);

                clientStream.Flush();
            }

            tcpClient.Close();
        }

        private String[] ProcessMessage(String message)
        {
            String[] exp = message.Split(';');
            String[] expFix = new String[3];

            if (exp.Length > 0)
                expFix[0] = exp[0];
            if (exp.Length > 1)
                expFix[1] = ECB.decrypt(exp[1]);
            if (exp.Length > 2)
                expFix[2] = ECB.decrypt(exp[2]);

            return expFix;
        }

        public void StopServer()
        {
            listening = false;
        }
    }
}
