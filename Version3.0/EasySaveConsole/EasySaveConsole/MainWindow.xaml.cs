using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;


namespace EasySaveConsole
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static NetworkStream networkStream;
        public static TcpClient socketForServer;
        public static string clientmessage = "test";

        // The port number for the remote device.  
        private const int port = 66;

        // ManualResetEvent instances signal completion.  
        private static ManualResetEvent connectDone = new ManualResetEvent(false);
        private static ManualResetEvent sendDone = new ManualResetEvent(false);
        private static ManualResetEvent receiveDone = new ManualResetEvent(false);

        // The response from the remote device.  
        private static String response = String.Empty;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void GridMenu_MouseDown(object sender, RoutedEventArgs e)//Function that allows you to move the software window.
        {
            DragMove();//Function that allows movement
        }

        private void Button_exit(object sender, RoutedEventArgs e)//Function of the button to close the software
        {
            Process.GetCurrentProcess().Kill();
        }

        private void Ip_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void StartClient(object sender, RoutedEventArgs e)//Function to start the function to start the connection with the server.
        {
            try
            {
                StartClient(text_ipserevr.Text);

            }
            catch (Exception ex)
            {
                MessageBox.Show("Server not found " + ex.ToString());
            }

        }

        private void SelectionChanged(object sender, SelectionChangedEventArgs e)//Method to display the name of the backup in the content.
        {
            name_backup.Content = "Name : " + Save_work.SelectedItem;

        }

        private void PlayBackup(object sender, RoutedEventArgs e)//Method for the play button, to start the save process.
        {
            try
            {
                IPHostEntry ipHostInfo = Dns.GetHostEntry(text_ipserevr.Text);
                IPAddress ipAddress = ipHostInfo.AddressList[0];
                IPEndPoint remoteEP = new IPEndPoint(ipAddress, port);

                // Create a TCP/IP socket.  
                Socket client = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

                // Connect to the remote endpoint.  
                client.BeginConnect(remoteEP, new AsyncCallback(ConnectCallback), client);
                connectDone.WaitOne();

                // Send test data to the remote device.  
                Send(client, "PLAY" + Save_work.SelectedItem);
                sendDone.WaitOne();

            }
            catch
            {

            }
        }

        private void PauseBackup(object sender, RoutedEventArgs e)//Method for the pause button, to pause the backup.
        {
            try
            {
                IPHostEntry ipHostInfo = Dns.GetHostEntry(text_ipserevr.Text);
                IPAddress ipAddress = ipHostInfo.AddressList[0];
                IPEndPoint remoteEP = new IPEndPoint(ipAddress, port);

                // Create a TCP/IP socket.  
                Socket client = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

                // Connect to the remote endpoint.  
                client.BeginConnect(remoteEP, new AsyncCallback(ConnectCallback), client);
                connectDone.WaitOne();

                // Send test data to the remote device.  
                Send(client, "PAUSE" + Save_work.SelectedItem);
                sendDone.WaitOne();

            }
            catch
            {

            }

        }

        private void StopBackup(object sender, RoutedEventArgs e)//Method for the stop button, to stop backup
        {
            try
            {
                IPHostEntry ipHostInfo = Dns.GetHostEntry(text_ipserevr.Text);
                IPAddress ipAddress = ipHostInfo.AddressList[0];
                IPEndPoint remoteEP = new IPEndPoint(ipAddress, port);

                // Create a TCP/IP socket.  
                Socket client = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

                // Connect to the remote endpoint.  
                client.BeginConnect(remoteEP, new AsyncCallback(ConnectCallback), client);
                connectDone.WaitOne();

                // Send test data to the remote device.  
                Send(client, "STOP" + Save_work.SelectedItem);
                sendDone.WaitOne();

            }
            catch
            {

            }

        }

        public void  StartClient(string ip)//Function that allows to start the client with the server and view the backups.
        {
            // Connect to a remote device.  
            try
            {
                // Establish the local endpoint for the socket.
                IPHostEntry ipHostInfo = Dns.GetHostEntry(ip);
                IPAddress ipAddress = ipHostInfo.AddressList[0];
                IPEndPoint remoteEP = new IPEndPoint(ipAddress, port);

                // Create a TCP/IP socket.  
                Socket client = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

                // Connect to the remote endpoint.  
                client.BeginConnect(remoteEP, new AsyncCallback(ConnectCallback), client);
                connectDone.WaitOne();

                // Send test data to the remote device.  
                Send(client, "getdata");
                sendDone.WaitOne();

                // Receive the response from the remote device.  
                Receive(client);
                receiveDone.WaitOne();

                Save_work.Items.Clear(); //Cleaning the list table
                string request = response;
                string[] array = request.Split(Environment.NewLine);
                foreach (var obj in array)
                {
                    Save_work.Items.Add(obj); //Displaying backups in the listbox
                }
                Save_work.Items.RemoveAt(array.GetUpperBound(0));

            }
            catch (Exception e)
            {
               // MessageBox.Show(e.ToString());
            }

        }

        private void ConnectCallback(IAsyncResult ar)//Receive the message and call ReceiveCallBack
        {
            try
            {
                // Retrieve the socket from the state object.  
                Socket client = (Socket)ar.AsyncState;

                // Complete the connection.  
                client.EndConnect(ar);

                // Signal that the connection has been made.  
                connectDone.Set();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
        }

        private void Receive(Socket client) //Reads and assigns message to response
        {
            try
            {
                // Create the state object.  
                StateObject state = new StateObject();
                state.workSocket = client;

                // Begin receiving the data from the remote device.  
                client.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(ReceiveCallback), state);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
        }

        private void ReceiveCallback(IAsyncResult ar)//Function to receive the message
        {
            try
            {
                // Retrieve the state object and the client socket
                // from the asynchronous state object.  
                StateObject state = (StateObject)ar.AsyncState;
                Socket client = state.workSocket;

                // Read data from the remote device.  
                int bytesRead = client.EndReceive(ar);

                if (bytesRead > 0)
                {
                    // There might be more data, so store the data received so far.  
                    state.sb.Append(Encoding.ASCII.GetString(state.buffer, 0, bytesRead));

                    // Get the rest of the data.  
                    client.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(ReceiveCallback), state);
                }
                else
                {
                    // All the data has arrived; put it in response.  
                    if (state.sb.Length > 1)
                    {
                        response = state.sb.ToString();
                    }
                    // Signal that all bytes have been received.  
                    receiveDone.Set();
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
        }

        private void Send(Socket client, String data)//Function to send a message
        {
            // Convert the string data to byte data using ASCII encoding.  
            byte[] byteData = Encoding.ASCII.GetBytes(data);

            // Begin sending the data to the remote device.  
            client.BeginSend(byteData, 0, byteData.Length, 0, new AsyncCallback(SendCallback), client);
        }

        private void SendCallback(IAsyncResult ar)//Function that allows you to send a message asynchronously. Method linked with the send function
        {
            try
            {
                // Retrieve the socket from the state object.  
                Socket client = (Socket)ar.AsyncState;

                // Complete sending the data to the remote device.  
                int bytesSent = client.EndSend(ar);
       
                // Signal that all bytes have been sent.  
                sendDone.Set();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
        }

        private void LoadProgress(object sender, RoutedEventArgs e)//Function to retrieve progress information.
        {
            try
            {
                IPHostEntry ipHostInfo = Dns.GetHostEntry(text_ipserevr.Text);
                IPAddress ipAddress = ipHostInfo.AddressList[0];
                IPEndPoint remoteEP = new IPEndPoint(ipAddress, port);

                // Create a TCP/IP socket.  
                Socket client = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

                // Connect to the remote endpoint.  
                client.BeginConnect(remoteEP, new AsyncCallback(ConnectCallback), client);
                connectDone.WaitOne();
                
                // Send test data to the remote device.  
                Send(client, "getprogressing" + Save_work.SelectedItem);
                sendDone.WaitOne();
                
                Receive(client);
                receiveDone.WaitOne();
                MessageBox.Show("GET PRGRESSING....");
                progression.Content = "Progressing : " + response;
            }
            catch
            {

            }
        }
    }
}
