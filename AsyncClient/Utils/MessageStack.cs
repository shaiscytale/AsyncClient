using System;
using System.Collections.Generic;
using System.Text;

namespace AsyncClient.Utils
{
    internal class MessageStack
    {
        private List<string> _stack;
        private int _maxLines;
        private int _maxColumns;
        public MessageStack()
        {
            _stack = new List<string>();
            _maxLines = Config.MESSAGE_STACK_MAX_LIN;
            _maxColumns = Config.MESSAGE_STACK_MAX_COL;
            for( int i = 0; i < _maxLines; i++ )
            {
                _stack.Add( "" );
            }
        }

        internal void Add( string mess )
        {
            if( mess.Length > _maxColumns )
                _stack.Add( mess.Substring( 0, _maxColumns - 1 ) );
            else
                _stack.Add( mess );
        }

        internal List<String> Messages()
        {
            return ToDisplay();
        }

        internal void Clear()
        {
            _stack = new List<string>();
        }

        private List<String> ToDisplay()
        {
            List<String> messages = new List<string>();

            int firstLine = (_stack.Count < _maxLines) ? 0 : (_stack.Count - _maxLines) + 1;
            int lastLine = _stack.Count;

            for( int i = firstLine; i < lastLine; i++ )
            {
                messages.Add( _stack[i] );
            }

            return messages;
        }
    }
}
