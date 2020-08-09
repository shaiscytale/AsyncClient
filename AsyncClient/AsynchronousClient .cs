using AsyncClient.UI;
using AsyncClient.Utils;
using Datalink.Models;
using System;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;

namespace AsyncClient
{

    public class AsynchronousClient
    {
        #region system windows

        private const int MF_BYCOMMAND = 0x00000000;
        public const int SC_CLOSE = 0xF060;
        public const int SC_MINIMIZE = 0xF020;
        public const int SC_MAXIMIZE = 0xF030;
        public const int SC_SIZE = 0xF000;

        [DllImport( "user32.dll" )]
        public static extern int DeleteMenu( IntPtr hMenu, int nPosition, int wFlags );

        [DllImport( "user32.dll" )]
        private static extern IntPtr GetSystemMenu( IntPtr hWnd, bool bRevert );

        [DllImport( "kernel32.dll", ExactSpelling = true )]
        private static extern IntPtr GetConsoleWindow();

        #endregion

        #region fuckin' var

        private static Socket _socket;

        private static IPAddress _ip;
        private static int _port;
        private static bool _keepGoing = true;

        private static string _sysmess = "";
        private static string _connectedLabel = "";

        #endregion

        #region Main
        static void Main()
        {
            Console.Title = "Client";
            _keepGoing = true;

            Startup();

            while( _keepGoing )
            {
                if( TargetRemote( out _ip, out _port ) )
                {
                    ConnectToServer( _ip, _port );
                    SendAndReceiveLoop();
                    Disconnect();
                }

                string keepGoingResponse = CustomConsole.Ask( "Connect to another server ? (yes/no) " ).ToLower();
                _keepGoing = ( keepGoingResponse == "yes" );
            }

            CustomConsole.Ask( "End of program, press a key to close..." );

            Exit();
        }
        #endregion

        #region startup & console custom config

        private static void DisableWeirdActions()
        {
            IntPtr handle = GetConsoleWindow();
            IntPtr sysMenu = GetSystemMenu(handle, false);

            if( handle != IntPtr.Zero )
            {
                //DeleteMenu( sysMenu, SC_CLOSE, MF_BYCOMMAND );
                DeleteMenu( sysMenu, SC_MINIMIZE, MF_BYCOMMAND );
                DeleteMenu( sysMenu, SC_MAXIMIZE, MF_BYCOMMAND );
                DeleteMenu( sysMenu, SC_SIZE, MF_BYCOMMAND );
            }
        }

        private static void Startup()
        {
            DisableWeirdActions();

            Console.SetWindowSize( Config.WIN_WIDTH, Config.WIN_HEIGHT );
            Console.SetBufferSize( Config.WIN_WIDTH, Config.WIN_HEIGHT );
            Console.BackgroundColor = Config.MESSBOX_COLOR_BACK;
            Console.ForegroundColor = Config.MESSBOX_COLOR_FRONT;

            CustomConsole.SystemMessage( "Startup..." );
        }

        #endregion

        #region connection

        private static bool TargetRemote( out IPAddress ip, out int port )
        {
            if( Config.DEBUG )
            {
                IPAddress.TryParse( Config.DEFAULT_IP, out ip );
                port = Config.DEFAULT_PORT;
            }
            else
            {
                string ipResponse;
                bool ipOk = false;
                ip = IPAddress.Loopback;
                port = -1;
                string portResponse;
                bool portOk = false;

                CustomConsole.SystemMessage( "Setting remote connection infos..." );

                while( !ipOk )
                {
                    ipResponse = CustomConsole.Ask( "ip => " );
                    ipOk = IPAddress.TryParse( ipResponse, out ip );
                    if( !ipOk ) CustomConsole.SystemMessage( "invalid ip" );
                }
                CustomConsole.DrawTopRightLabelBox( String.Format( "no-port@{0}", ip ) );

                while( !portOk )
                {
                    portResponse = CustomConsole.Ask( "port => " );
                    portOk = int.TryParse( portResponse, out port ) && ( 0 < port && port < 9999 );
                    if( !portOk ) CustomConsole.SystemMessage( "invalid port" );
                }
            }
            CustomConsole.DrawTopRightLabelBox( String.Format( "{0}@{1}", port, ip ) );

            CustomConsole.SystemMessage( "Remote connection ready to test..." );

            return true;
        }

        private static void ConnectToServer( IPAddress address, int port = Config.DEFAULT_PORT )
        {
            _socket = new Socket( AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp );
            int attempts = 0;

            while( !_socket.Connected )
            {
                try
                {
                    attempts++;
                    // ya pas grand chose d'autre à faire que de se frotter la cuisse
                    CustomConsole.SystemMessage( String.Format("{0} attempt ", attempts) );
                    _socket.Connect( address, port );
                }
                catch( SocketException )
                {
                    Console.Clear();
                }
            }

            if( _socket != null
                && _socket.Connected
                && _ip != null
                && _port != -1 )
                CustomConsole.DrawTopRightLabelBox( String.Format( "{0}@{1}", _port, _ip ) );
            else
                CustomConsole.DrawTopRightLabelBox( "Not connected" );
            
            CustomConsole.SystemMessage( "Connected" );
        }

        #endregion

        #region messagin' the server
        private static void SendAndReceiveLoop()
        {
            bool connection = (_socket != null && _socket.Connected);
            bool exit = false;
            while( connection && !exit )
            {
                SendRequest( out exit );
                if( _socket != null && _socket.Connected && !exit )
                    ReceiveResponse();
                connection = ( _socket != null && _socket.Connected );
            }
        }

        private static void SendRequest( out bool exit )
        {

            string request = CustomConsole.Ask( "send => " );
            // compute wich commands the user wants to send

            // build a bag with the corresponding command
            Bag bag = new Bag("test", 1);
            // send the bad
            SendBag( bag );

            exit = ( request.ToLower().StartsWith( Commands.EXIT ) );

            //// temporary, change this to send Bags
            //SendString( request );
        }

        private static void SendString( string text )
        {
            if( _socket != null && _socket.Connected )
            {
                byte[] buffer = Encoding.ASCII.GetBytes(text);
                _socket.Send( buffer, 0, buffer.Length, SocketFlags.None );
            }
        }

        private static void SendBag( Bag bag )
        {
            if( _socket != null && _socket.Connected )
            {
                byte[] buffer = bag.JSerialize();
                _socket.Send( buffer, 0, buffer.Length, SocketFlags.None );
            }
        }

        private static void ReceiveResponse()
        {
            if( _socket != null && _socket.Connected )
            {
                var buffer = new byte[2048];
                int received = _socket.Receive(buffer, SocketFlags.None);
                if( received == 0 ) return;
                var data = new byte[received];
                Array.Copy( buffer, data, received );
                string text = Encoding.ASCII.GetString(data);
                CustomConsole.UpdateMessageBox( text );
            }
        }
        #endregion

        #region Disconnect from the server

        private static void Exit()
        {
            _socket.Close();
            Environment.Exit( 0 );
        }

        private static void Disconnect()
        {
            _socket.Shutdown( SocketShutdown.Both );
        }

        #endregion
    }
}
