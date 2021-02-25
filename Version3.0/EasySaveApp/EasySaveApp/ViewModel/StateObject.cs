using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;

namespace EasySaveApp.viewmodel
{
    public class StateObject// State object for reading client data asynchronously  
    {
        // Client socket.  
        public Socket workSocket = null;
        // Size of receive buffer.  
        public const int BufferSize = 4096;
        // Receive buffer.  
        public byte[] buffer = new byte[4096];
        // Received data string.  
        public StringBuilder sb = new StringBuilder();
    }
}
