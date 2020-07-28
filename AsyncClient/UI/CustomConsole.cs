using AsyncClient.Utils;
using System;
using System.Collections.Generic;

namespace AsyncClient.UI
{
    public static class CustomConsole
    {
        private static MessageStack _mess = new MessageStack();
        private static string _sysmess = "";
        private static string _topLabel = "";

        #region draw console custom

        internal static void Redraw( 
            MessageStack mess,
            string sysmess,
            string topInfos
            )
        {
            Console.Clear();
            DrawSystemMessage( sysmess );
            DrawTopRightLabelBox( topInfos );
            DrawTheFuckinLine();
            DrawMessageBox( mess );
            ResetCursorToUser();
        }

        internal static void ResetCursorToUser()
        {
            Console.SetCursorPosition( Config.ASK_W, Config.ASK_H );
        }

        internal static void DrawTopRightLabelBox(string text = "")
        {
            _topLabel = text;
            Console.SetCursorPosition( Config.IP_W, Config.IP_H );
            Console.Write( _topLabel );
        }

        internal static void DrawSystemMessage( string text )
        {
            _sysmess = text;
            Console.SetCursorPosition( 0, 0 );
            Console.Write( _sysmess );
        }

        internal static void DrawTheFuckinLine()
        {
            Console.SetCursorPosition( 0, Config.ASK_H - 1 );
            Console.Write( "________________________________________________________________" );
        }

        internal static void DrawMessageBox( MessageStack mess)
        {
            _mess = mess;

            Console.SetCursorPosition( 0, 40 );
            List<String> lines = _mess.Messages();
            for( int i = 0; ( i < lines.Count && Config.MESSBOX_START_H + i < Config.MESSBOX_END_W ); i++ )
            {
                Console.SetCursorPosition( Config.MESSBOX_START_W, Config.MESSBOX_START_H + i );
                Console.Write( lines[i] );
            }
        }

        internal static void SystemMessage( string sysmess )
        {
            _sysmess = sysmess;
            Redraw( _mess, _sysmess, _topLabel );
        }

        internal static void UpdateMessageBox( string mess = "" )
        {
            if( mess != "" )
                _mess.Add( mess );
            Redraw( _mess, _sysmess, _topLabel );
        }

        internal static string Ask( string question = "" )
        {
            if( question != "" )
                SystemMessage( question );
            ResetCursorToUser();
            string response = Console.ReadLine();
            //UpdateMessageBox( response );
            return response;
        }

        #endregion
    }
}
