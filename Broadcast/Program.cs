using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Broadcast
{
    class Program
    {
        private const int ListenPort = 11000;
        static void Listener()
        {
            //UDP är ett protocol. Får inte svar på om vår data kommit fram
            UdpClient listener = new UdpClient(ListenPort);
            try
            {
                while (true)
                {
                    //IPAdress.Any - redo att lyssna på alla nätverksinterface
                    IPEndPoint groupEndpoint = new IPEndPoint(IPAddress.Any, ListenPort);
                    //ref - reference till värde från metod. Innebär att värdet av groupEndpoint kommer ändras                    
                    //se https://www.geeksforgeeks.org/ref-in-c-sharp/

                    //Lyssnar här. Stannar och väntar på svar
                    //Svaret fångas i bytes som sedan konverteras till sträng
                    byte[] bytes = listener.Receive(ref groupEndpoint);
                    Console.WriteLine("Received broadcast from {0} :\n {1}\n", groupEndpoint.ToString(),
                       Encoding.UTF8.GetString(bytes, 0, bytes.Length));
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
       
            }

            finally
            {
                listener.Close();
            }
        }
    
        static void Main()
        {
            //Ny tråd, alltså kan köras parallellt med andra trådar. 
            var listenThread = new Thread(Listener);
            listenThread.Start();
            //Socket - mjukvaruändpunkt i datanätverk för att skicka och ta emot data 
            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            socket.EnableBroadcast = true;
            //IPAdress.Broadcast - den ip-address för broadcast som nätverkskortet prioriterar
            //Kan hårdkoda in men koden kan behöva ändras vid byte av nätverk
            IPEndPoint endPoint = new IPEndPoint(IPAddress.Broadcast, ListenPort);
            Thread.Sleep(1000);

            while (true)
            {
                Console.Write(">");
                string message = Console.ReadLine();
                //meddelande koverteras till bytes
                byte[] sendBuffer = Encoding.UTF8.GetBytes(message);
                socket.SendTo(sendBuffer, endPoint);
                Thread.Sleep(200);
            }
        }
    }
}
