using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace TcpServerTest
{
    class TCPClient
    {
        public static void connect(string filename)
        {
            try
            {
                // Set the IP address and port number of the server to connect
                string serverIP = "172.29.28.63";
                int serverPort = 12345;
                long offset = 0;
                byte[] receivedData = new byte[32768];
                byte[] nullbuffer = new byte[3];
                

                nullbuffer[0] = 0x4b;
                nullbuffer[1] = 0x4d;
                nullbuffer[2] = 0x54;

                Console.WriteLine("TCPclient started.");



                // Create a TcpClient to connect to the server
                using (TcpClient tcpClient = new TcpClient(serverIP, serverPort))
                {
                    Console.WriteLine($"Connected to {serverIP}:{serverPort}");

                    var start = DateTime.Now;

                    // Get the network stream for reading and writing data
                    using (NetworkStream clientStream = tcpClient.GetStream())
                    {
                        // Example: Send data to the server

                        

                        using (FileStream fileStream = new FileStream(filename, FileMode.Open, FileAccess.Read))
                        {
                            var bits = new byte[32768];
                            int bytesRead;

                            // Loop to read the file in chunks
                            while ((bytesRead = fileStream.Read(bits, 0, 32768)) > 0)
                            {
                                // Process the chunk of data (you can modify this part)
                                clientStream.Write(bits, 0, bytesRead);
                                offset = offset + bytesRead;
                            }
                        }

                        Console.Write("Write END");
                        clientStream.Write(nullbuffer, 0, 3);


                        // wait for answer from Server
                        while (true)
                        {
                            Console.Write(".");
                            int bytesRead = clientStream.Read(receivedData, 0, 32768);
                            if (bytesRead != 0) break;
                        }
                         
                       
                        
                    }
                    var end = DateTime.Now;
                    float speed = (float)((offset / (1024 * (end - start).TotalMilliseconds)) * 1000);
                    Console.WriteLine();
                    Console.WriteLine("done in " + ((end - start).TotalMilliseconds).ToString());
                    Console.WriteLine("Transferred with " + speed.ToString() + " kB/s");

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }

        }

        
        

    }
}
