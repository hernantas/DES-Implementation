using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using DESEncryption;
using System.Numerics;

namespace DESImplenServer
{
    // Message format
    // Connect
    // or
    // userId; message

    class Message
    {
        public UserData user = null;
        public String message = "";

        public Message(UserData user, string message)
        {
            this.user = user;
            this.message = message;
        }
    };

    class UserData
    {
        public RSAKey key = null;
        public string username = "";
        public string desKey = "";

        public UserData(BigInteger n, BigInteger e, string username)
        {
            this.key = new RSAKey(n, e);
            this.username = username;
        }
    }

    class ServerChat
    {
        private int lastId = 0;
        private TcpListener listener;
        private Thread thread;
        private bool listening;
        private string certificate = "";
        private RSAKey certificateKey = null;

        private List<UserData> users = new List<UserData>();
        public List<UserData> UserList
        {
            get
            {
                return users;
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

        private RSA rsa = new RSA();
        private string ecbKey = "";

        public ServerChat()
        {
            listening = false;
            bufferMessage.Clear();

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
        }

        public void StartServer()
        {
            this.listener = new TcpListener(IPAddress.Any, 1234);
            this.listener.Start();
            this.thread = new Thread(new ThreadStart(ListenToClient));
            this.thread.Start();

            listening = true;
        }

        private Package SendCommand(Package pck, int port)
        {
            TcpClient client = new TcpClient();
            IPEndPoint serverEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), port);
            client.Connect(serverEndPoint);
            NetworkStream clientStream = client.GetStream();
            ASCIIEncoding encoder = new ASCIIEncoding();

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

            return resp;
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

        private int GetUserByPublicKey(BigInteger n, BigInteger e)
        {
            for (int i = 0; i < users.Count; i++)
            {
                if (users[i].key.n == n && users[i].key.e == e)
                    return i;
            }

            return -1;
        }

        private Package ProcessMessage(Package pck)
        {
            // Console.WriteLine("#Get Message Client: ");
            // pck.Print();

            Package response = new Package();
            RSA certRSA = new RSA(certificateKey);
            string[] respCertf = certRSA.decrypt(pck.GetHeader("Certificate")).Split(';');
            BigInteger n = BigInteger.Parse(respCertf[2]);
            BigInteger e = BigInteger.Parse(respCertf[3]);
            int idUser = GetUserByPublicKey(n, e);

            switch(pck.GetHeader("Command"))
            {
                case "Connect":
                    UserData user = new UserData(n, e, pck.GetHeader("Username"));
                    users.Add(user);
                    response.SetHeader("Certificate", certificate);
                    break;

                case "Disconnect":
                    users.RemoveAt(idUser);
                    break;

                case "DES":
                    string desEnc = pck.GetHeader("DES Key");
                    users[idUser].desKey = rsa.decrypt(desEnc);

                    RSA resRSA = new RSA(users[idUser].key);
                    response.SetHeader("DES Key", resRSA.encrypt(ecbKey));
                    break;

                case "Message":
                    Message msg = new Message(users[idUser], ECB.decrypt(pck.GetContent(), users[idUser].desKey));
                    bufferMessage.Add(msg);

                    break;
                case "New Message":
                    string lastIndex = ECB.decrypt(pck.GetContent(), users[idUser].desKey);
                    int index = int.Parse(lastIndex);

                    if (index < bufferMessage.Count)
                        response.SetContent(bufferMessage[index].user.username + " = " + bufferMessage[index].message);

                    break;
            }

            if (response.GetContent() != "")
            {
                response.SetContent(ECB.encrypt(response.GetContent(), ecbKey));
            }

            return response;
        }

        public void StopServer()
        {
            listening = false;
        }
    }
}
