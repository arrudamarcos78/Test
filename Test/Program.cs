using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Test
{
    internal class Program
    {
        private const string MulticastIP = "239.255.0.0";
        private const int MulticastPort = 31337;

        private static void TestMulticastSender()
        {
            try
            {
                UdpClient udpClient = new UdpClient();
                IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse(MulticastIP), MulticastPort);

                Console.WriteLine("Enter the message to send (type 'exit' to quit):");

                while (true)
                {
                    string message = Console.ReadLine();
                    if (message.ToLower() == "exit")
                    {
                        break;
                    }

                    byte[] buffer = Encoding.UTF8.GetBytes(message);
                    udpClient.Send(buffer, buffer.Length, endPoint);
                    Console.WriteLine("Message sent: " + message);

                    // Optional delay to control message frequency
                    Thread.Sleep(1000);
                }

                udpClient.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
        }

        private static void TestMulticastReceiver()
        {
            UdpClient udpClient = new UdpClient(MulticastPort);
            udpClient.JoinMulticastGroup(IPAddress.Parse(MulticastIP));

            IPEndPoint endPoint = new IPEndPoint(IPAddress.Any, MulticastPort);

            Console.WriteLine("Listening for multicast messages...");

            try
            {
                while (true)
                {
                    byte[] buffer = udpClient.Receive(ref endPoint);
                    string message = Encoding.UTF8.GetString(buffer);
                    Console.WriteLine("Received message from {0}: {1}", endPoint.Address, message);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
            finally
            {
                udpClient.DropMulticastGroup(IPAddress.Parse(MulticastIP));
                udpClient.Close();
            }
        }

        private static void TestMulticast()
        {
            try
            {
                Console.WriteLine("Multicast Sender (1) or Receiver (2):");
                string userOpt = Console.ReadLine();

                if (userOpt == "1")
                    TestMulticastSender();
                else if (userOpt == "2")
                    TestMulticastReceiver();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
        }

        static void Main(string[] args)
        {
            //AppDomain.CurrentDomain.UnhandledException +=
            //    (sender, e) => AsyncMessageBox.Show(e.ExceptionObject.ToString());

            AppDomain.CurrentDomain.UnhandledException +=
                (sender, e) => Console.WriteLine(e.ExceptionObject.ToString());

            string methodName = null;

            if (args != null && args.Length > 0)
            {
                for (int i = 0; i < args.Length; i++)
                {
                    if (args[i] == "-method" && args.Length > i + 1)
                    {
                        i++;
                        methodName = args[i];
                    }
                }
            }

            if (string.IsNullOrEmpty(methodName))
            {
                /************************************************************************************************************/
                // quando se quiser chamar um método, basta setar o valor da variável methodName abaixo para o nome do método
                methodName = "TestMulticast";
            }

            if (string.IsNullOrEmpty(methodName))
                return;

            string type = "Test.Program";
            Type t = Type.GetType(type);
            System.Reflection.MethodInfo theMethod = t.GetMethod(methodName, System.Reflection.BindingFlags.Static |
                                                                 System.Reflection.BindingFlags.NonPublic);
            object[] paramArray = null;

            // Para usar quando for testar o método TestCompileProjects            
            theMethod.Invoke(null, paramArray);
            //Task.Run(() => theMethod.Invoke(null, paramArray));
            Console.WriteLine("\nDigite <ENTER> para sair do programa:");
            Console.ReadLine();
            return;
        }
    }
}
