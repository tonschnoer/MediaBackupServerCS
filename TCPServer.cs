using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

class TCPServer
{

    static byte[] receivedData = new byte[32768];
    static byte[] nullbuffer = new byte[3];



    public static void StartServer()
    {
        nullbuffer[0] = 0x45;
        nullbuffer[1] = 0x4e;
        nullbuffer[2] = 0x44;

        TcpListener tcpListener = null;
        try
        {
            // Set the IP address and port number on which the server will listen
            IPAddress ipAddress = IPAddress.Parse("192.168.178.20");
            int port = 12345;

            // Create the TcpListener
            tcpListener = new TcpListener(ipAddress, port);

            // Start listening for client requests
            tcpListener.Start();

            Console.WriteLine($"Server is listening on {ipAddress}:{port}");

            while (true)
            {
                // Accept a pending connection request from a client
                TcpClient tcpClient = tcpListener.AcceptTcpClient();

                // Create a separate thread to handle the client
                Thread clientThread = new Thread(HandleClient);
                clientThread.Start(tcpClient);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
        finally
        {
            // Stop listening for new clients if an exception occurs
            tcpListener?.Stop();
        }
    }

// *****************************************************************************
// Write file to Client
// *****************************************************************************
    static void write_file(Stream clientStream, string filename)
    {
        int bytesRead;
        long offset = 0;
        var start = DateTime.Now;
        var bits = new byte[32768];

        using (FileStream fileStream = new FileStream(AppDomain.CurrentDomain.BaseDirectory + "\\" + filename, FileMode.Open, FileAccess.Read))
        {

            // Loop to read the file in chunks
            bytesRead = 1;
            while (bytesRead > 0)
            {
                bytesRead = fileStream.Read(bits, 0, 32768);
                clientStream.Write(bits, 0, bytesRead);
                offset = offset + bytesRead;
                //Console.WriteLine($"{offset}-{bytesRead}");
            }
        }

        Console.Write("Write END to client");
        clientStream.Write(nullbuffer, 0, 3);
        clientStream.Flush();

        // wait for answer from Client
        while (true)
        {
            Console.Write("::");
            bytesRead = clientStream.Read(receivedData, 0, 32768);
            if (bytesRead == 0) break;
        }


        var end = DateTime.Now;
        float speed = (float)((offset / (1024 * (end - start).TotalMilliseconds)) * 1000);
        Console.WriteLine();
        Console.WriteLine("done in " + ((end - start).TotalMilliseconds).ToString() + "ms");
        Console.WriteLine("Transferred with " + speed.ToString() + " kB/s");
    }

    // *****************************************************************************
    // Client Thread
    // *****************************************************************************
    static void HandleClient(object obj)
    {
        TcpClient tcpClient = (TcpClient)obj;
        NetworkStream clientStream = tcpClient.GetStream();
        string command = "";


        byte[] message = new byte[32768];
        byte[] response = new byte[1];
        response[0] = 0x41;
      

        int bytesRead;

        try
        {

            bytesRead = clientStream.Read(message, 0, 32768);
            if (bytesRead>4)
            {
                command = Encoding.UTF8.GetString(message, 0, 5);
                Console.WriteLine(Encoding.UTF8.GetString(message,0,bytesRead));
            }

            if (command=="GET |")
            {
                string filename = Encoding.UTF8.GetString(message, 5, bytesRead - 5);
                Console.WriteLine($"writing {filename} to client");
                write_file(clientStream, filename);
            }

            if (command=="UPDA|")
            {
                Console.WriteLine("Writing updated files in response to UPDA command.");
            }

            if (command=="CHKS|")
            {
                Console.WriteLine("Getting MD5 checksum of file...");
            }
            


        } catch (Exception ex) { Console.WriteLine(ex.ToString()); } finally
        {
            tcpClient.Close();
        }

        Console.WriteLine("close stream");
        
    }  
}
    
