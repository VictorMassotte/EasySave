using System;
using System.IO;
using System.Diagnostics;
using Newtonsoft.Json;
using EasySaveApp.model;
using EasySaveApp.view;
using System.Collections.Generic;
using System.Windows;
using MessageBox = System.Windows.MessageBox;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using System.Text;
using System.ComponentModel;

namespace EasySaveApp.viewmodel
{
    public class ViewModel : INotifyPropertyChanged
    {

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        private Model model;

        string[] blacklisted_app = Model.getBlackList();
        public string[] blacklitapp { get => blacklisted_app; set=>blacklisted_app = value; }

        // Thread signal.  
        public static ManualResetEvent allDone = new ManualResetEvent(false);

        private List<string> nameslist;


        public ViewModel()
        {
            model = new Model();
            Thread ServerThread = new Thread(StartServer);
            ServerThread.Start();

        }


        public void AddSaveModel(int type, string saveName, string sourceDir, string targetDir, string mirrorDir)//Function that allows you to add a backup
        {
            model.SaveName = saveName;
            Backup backup = new Backup(model.SaveName, sourceDir, targetDir, type, mirrorDir);
            model.AddSave(backup); // Calling the function to add a backup job
        }

        public List<String> NamesList
        {
            get
            {
                return nameslist;
            }
            set
            {
                nameslist = value;

                OnPropertyChanged("NamesList");
            }
        }

        public void ListBackup1()//Function that lets you know the lists of the names of the backups.
        {

            nameslist = new List<string>();
            foreach (var obj in model.NameList())
            {
                nameslist.Add(obj.SaveName);
            }

        }

        public List<string> ListBackup()//Function that lets you know the lists of the names of the backups.
        {
            List<string> nameslist = new List<string>();
            foreach (var obj in model.NameList())
            {
                nameslist.Add(obj.SaveName);
            }
            return nameslist;
        }

        public void LoadBackup(string backupname, string langue)//Function that allows you to load the backups that were selected by the user.
        {
            if (Model.checkSoftware(blacklitapp))//If a program is in the blacklist we do not start the backup.
            {
                if (langue == "fr")
                {
                    MessageBox.Show("ECHEC DE SAUVEGARDE ❎\n" +
                        "ERREUR N°1 : LOGICIEL BLACKLIST \n" +
                        "EN COURS D'EXECUTION");
                    
                }
                else
                {
                    MessageBox.Show("BACKUP FAILURE ❎\n" +
                        "ERROR N°1 : BLACKLIST SOFTWARE\n" +
                        "IN PROGRESS");
                }
            }
            else
            {
                model.LoadSave(backupname);//Function that launches backups

                if (langue == "fr")
                {
                    MessageBox.Show("SAUVEGARDE REUSSIE ✅");
                }
                else
                {
                    MessageBox.Show("SUCCESSFUL BACKUP ✅");
                }
            }
        }

        public void DontSave() //Function that prevent EasySave from saving while a third party app is running
        {
            List<string> BL = new List<string>();

            foreach (string bl in blacklisted_app)
            {
                BL.Add(bl);

                Process[] i = Process.GetProcessesByName(bl);

                if (i.Length > 0 == true)
                {
                    foreach(Process proc in i)
                    {
                        proc.WaitForExit();

                        if (proc.HasExited)
                        {
                            proc.CloseMainWindow();
                            proc.Close();
                        }
                    }               
                }
            }         
        }

        public void StartServer()//Function to start the server
        {
            // Establish the local endpoint for the socket.    
            IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
            IPAddress ipAddress = ipHostInfo.AddressList[0];
            IPEndPoint localEndPoint = new IPEndPoint(ipAddress, 66);

            // Create a TCP/IP socket.  
            Socket listener = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            // Bind the socket to the local endpoint and listen for incoming connections.  
            try
            {
                listener.Bind(localEndPoint);
                listener.Listen(100);

                while (true)
                {
                    allDone.Reset();// Set the event to nonsignaled state.  

                    // Start an asynchronous socket to listen for connections. 
                    listener.BeginAccept(new AsyncCallback(AcceptCallback), listener);

                    allDone.WaitOne();// Wait until a connection is made before continuing.  
                }

            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
        }

        public void AcceptCallback(IAsyncResult ar)
        {
            allDone.Set();// Signal the main thread to continue.  

            // Get the socket that handles the client request.  
            Socket listener = (Socket)ar.AsyncState;
            Socket handler = listener.EndAccept(ar);

            // Create the state object.  
            StateObject state = new StateObject();
            state.workSocket = handler;
            handler.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(ReadCallback), state);
        }

        public void ReadCallback(IAsyncResult ar)
        {
            String content = String.Empty;

            try
            {
                // Retrieve the state object and the handler socket  
                // from the asynchronous state object.  
                StateObject state = (StateObject)ar.AsyncState;
                Socket handler = state.workSocket;

                // Read data from the client socket.
                int bytesRead = handler.EndReceive(ar);

                if (bytesRead >= 0)
                {
                    // There  might be more data, so store the data received so far.  
                    state.sb.Append(Encoding.ASCII.GetString(state.buffer, 0, bytesRead));

                    // Check for end-of-file tag. If it is not there, read
                    // more data.  
                    content = state.sb.ToString();

                    List<string> names = ListBackup();
                    foreach (var name in names)//Loop that allows you to manage the names in the list.
                    {
                        if (content.IndexOf("getdata") > -1)
                        {
                            Send(handler, name + Environment.NewLine); //Function that allows you to insert the names of the backups in the list.
                        }
                        else if (content.IndexOf("PLAY" + name) > -1)
                        {
                           // MessageBox.Show("PLAY" + name);
                            LoadBackup(name, "en");

                        }else if(content.IndexOf("PAUSE" + name) > -1)
                        {
                            MessageBox.Show("PAUSE" + name);

                        }
                        else if(content.IndexOf("STOP" + name) > -1)
                        {
                            MessageBox.Show("STOP" + name);
                        }else if(content.IndexOf("getprogressing" + name) > -1)
                        {
                            string prog = "Progressions de la Save";
                            Send(handler, prog);
                        }
                        else
                        {
                            // Not all data received. Get more.  
                            handler.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
                            new AsyncCallback(ReadCallback), state);
                        }

                    }
                 }
                handler.Shutdown(SocketShutdown.Both);
                handler.Close();
            }
            catch
            {

            }
        }

        private void Send(Socket handler, String data)//Function to send a message
        {
            try
            {
                byte[] byteData = Encoding.ASCII.GetBytes(data);// Convert the string data to byte data using ASCII encoding.  

                handler.BeginSend(byteData, 0, byteData.Length, 0, new AsyncCallback(SendCallback), handler); // Begin sending the data to the remote device. 

            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }

        }
        private void SendCallback(IAsyncResult ar)//Function to send a message a asynchronous
        {
            try
            {
                Socket handler = (Socket)ar.AsyncState;// Retrieve the socket from the state object.  

                int bytesSent = handler.EndSend(ar); // Complete sending the data to the remote device.  
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
        }

        public string Check_buttonStatus()
        {

            return model.StatusButton;
        }

        public void PlayButton_click()
        {
            model.Play_click();
        }
        public void PauseButton_click()
        {
            model.Pause_click();
        }
        public void StopButton_click()
        {
            model.Stop_click();
        }

    }
}
