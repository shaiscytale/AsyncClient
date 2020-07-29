using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Datalink.Models
{
    public class Bag
    {
        public Dictionary<string, string> Headers { get; private set; }
        public int CommandId { get; private set; }
        public object Load { get; private set; }

        public Bag() { Headers = new Dictionary<string, string>(); }

        public Bag( object load,
            int commandId = 0 )
        {
            Headers = new Dictionary<string, string>();
            CommandId = commandId;
            Load = load;
        }

        public Bag(byte[] message )
        {
            Bag obj = Deserialize(message);
            if(obj != null )
            {
                Headers = obj.Headers;
                CommandId = obj.CommandId;
                Load = obj.Load;
            }
        }

        public bool AddHeader( string key, string value )
        {
            if( Headers.ContainsKey( key ) )
                return false;
            Headers.Add( key, value );
            return true;
        }

        public void OverwriteHeader( string key, string value )
        {
            if( Headers.ContainsKey( key ) )
                Headers[key] = value;
            else
                Headers.Add( key, value );;
        }

        public byte[] Serialize()
        {
            using( var memoryStream = new MemoryStream() )
            {
                ( new BinaryFormatter() ).Serialize( memoryStream, this );
                return memoryStream.ToArray();
            }
        }

        public static Bag Deserialize( byte[] message )
        {
            using( var memoryStream = new MemoryStream( message ) )
            {
                Object obj = ( new BinaryFormatter() ).Deserialize( memoryStream );
                try
                {
                    return (Bag)obj;
                }
                catch( Exception )
                {
                    return null;
                }
            }
        }
    }
}
