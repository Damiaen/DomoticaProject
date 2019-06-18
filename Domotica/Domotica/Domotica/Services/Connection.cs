using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace Domotica.Services
{
    // State object for receiving data from remote device.  
    public class StateObject
    {
        // Client socket.  
        public Socket workSocket = null;
        // Size of receive buffer.  
        public const int BufferSize = 1024;
        // Receive buffer.  
        public byte[] buffer = new byte[BufferSize];
        // Received data string.  
        public StringBuilder sb = new StringBuilder();
    }

    public class AsynchronousClient
    {
        // The port number for the remote device.  
        private const int port = 3300;

        // The response from the remote device.  
        private static String response = String.Empty;

        string[] sensors = new string[] { "a", "a", "a" };
        string[] sensorsValues = new string[3];
        int count = 0;


        public async Task<string> SingleAction(string argument)
        {
            // Connect to a remote device.
            try
            {
                // Establish the remote endpoint for the socket.  
                IPAddress ipAddress = IPAddress.Parse("192.168.0.101");
                IPEndPoint remoteEP = new IPEndPoint(ipAddress, port);

                // Create a TCP/IP socket.  
                Socket client = new Socket(ipAddress.AddressFamily,
                    SocketType.Stream, ProtocolType.Tcp);

                // Connect to the remote endpoint.  
                await client.ConnectAsync(remoteEP);

                Console.WriteLine("Socket connected to {0}",
                    client.RemoteEndPoint.ToString());

                await Send(client, argument);

                // Write the response to the console.  
                Console.WriteLine("Response received : {0}", response);
                // after all is done, we shut down the socket
                client.Shutdown(SocketShutdown.Both);
                client.Close();

                return response;

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                return null;
            }
        }

        public async Task<Array> GetSensorValues()
        {
            // Connect to a remote device.
            try
            {
                // Establish the remote endpoint for the socket.  
                IPAddress ipAddress = IPAddress.Parse("192.168.0.100");
                IPEndPoint remoteEP = new IPEndPoint(ipAddress, port);

                // Create a TCP/IP socket.  
                Socket client = new Socket(ipAddress.AddressFamily,
                    SocketType.Stream, ProtocolType.Tcp);

                // Connect to the remote endpoint.  
                await client.ConnectAsync(remoteEP);

                while (count < 3)
                {
                    Console.WriteLine("Socket connected to {0}",
                        client.RemoteEndPoint.ToString());

                    await Send(client, sensors[count]);

                    // Write the response to the console.  
                    Console.WriteLine("Response received : {0}", response);
                    sensorsValues[count] = response;
                    count++;

                }

                // after all is done, we shut down the socket
                client.Shutdown(SocketShutdown.Both);
                client.Close();
                count = 0;

                return sensorsValues;

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                return null;
            }
        }

        public void Receive(Socket client)
        {
            try
            {
                // Create the state object.  
                StateObject state = new StateObject();
                state.workSocket = client;

                // Begin receiving the data from the remote device.  
                client.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
                    new AsyncCallback(ReceiveCallback), state);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        public void ReceiveCallback(IAsyncResult ar)
        {
            try
            {
                // Retrieve the state object and the client socket   
                // from the asynchronous state object.  
                StateObject state = (StateObject)ar.AsyncState;
                Socket client = state.workSocket;

                // Read data from the remote device.  
                int bytesRead = client.EndReceive(ar);

                // There might be more data, so store the data received so far.  
                state.sb.Append(Encoding.ASCII.GetString(state.buffer, 0, bytesRead));

                response = state.sb.ToString();
                Console.WriteLine(response);



            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        public async Task Send(Socket client, String data)
        {
            // Convert the string data to byte data using ASCII encoding.  
            byte[] byteData = Encoding.ASCII.GetBytes(data);

            // Begin sending the data to the remote device.  
            client.BeginSend(byteData, 0, byteData.Length, 0,
                new AsyncCallback(SendCallback), client);

            await Task.Delay(2000);
        }

        public void SendCallback(IAsyncResult ar)
        {
            try
            {
                // Retrieve the socket from the state object.  
                Socket client = (Socket)ar.AsyncState;

                // Complete sending the data to the remote device.  
                int bytesSent = client.EndSend(ar);
                Console.WriteLine("Sent {0} bytes to server.", bytesSent);

                // We do it here now...
                Receive(client);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
    }
}

