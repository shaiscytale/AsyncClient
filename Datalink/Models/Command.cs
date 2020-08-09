using System;
using System.Collections.Generic;
using System.Linq;

namespace Datalink.Models
{
    public class Command
    {
        public const string EXIT         = "$exit";
        public const string NICKNAME     = "$nick";
        public const string USER         = "$user";
        public const string RANDOM       = "$rdm";
        public const string HELP_COMMAND = "$help";
        private static List<Command> _list;

        public int Id { get; set; }
        public string Name { get; set; }
        public string Caller { get; set; }
        public string Description { get; set; }
        public int AuthLevel { get; set; }

        public Command() { }

        public Command( int id, string name, string caller, string desc = "", int authLevel = 9 )
        {
            Id = id;
            Name = name;
            Caller = caller;
            Description = desc;
            AuthLevel = authLevel;
        }

        public override string ToString()
        {
            return String.Format( "{0} - {1}: {2} (auth:{3})", Caller, Name, Description, AuthLevel );
        }


        public static List<Command> List()
        {
            if( _list != null )
                return _list;

            _list = new List<Command>();
            _list.Add( new Command( 1, "Exit", "$exit", "Close your connection.", 0 ) );
            _list.Add( new Command( 3, "Nickname", "$nick", "Set a new nickname.", 0 ) );
            _list.Add( new Command( 2, "Help", "$help", "Return the list of available commands.", 0 ) );
            _list.Add( new Command( 4, "User info", "$user", "Get your user profile.", 0 ) );
            _list.Add( new Command( 5, "RandomPeople", "$rdm", "Return a random generated person. API test.", 0 ) );

            return _list;
        }

        public static Command GetById( int id )
        {
            return _list.Where( cmd => cmd.Id == id ).FirstOrDefault();
        }
    }
}
