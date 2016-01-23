using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vertica.Data.VerticaClient;

namespace de.haw.mars.rock.abstraction.vertica
{
    public class ClientConfig
    {
        public VerticaConnectionStringBuilder VerticaConnectionBuilder { get; private set; }

        public ClientConfig(string host, string database)
        {
            VerticaConnectionBuilder = new VerticaConnectionStringBuilder();
            VerticaConnectionBuilder.Host = host;
            VerticaConnectionBuilder.Database = database;
            VerticaConnectionBuilder.User = "dbadmin";
        }

        public ClientConfig(string host, int port, string database)
            : this(host, database)
        {
            VerticaConnectionBuilder.Port = port;
        }

        public ClientConfig(string host, string database, string user, string password)
            : this(host, database)
        {
            VerticaConnectionBuilder.User = user;
            VerticaConnectionBuilder.Password = password;
        }
    }
}
