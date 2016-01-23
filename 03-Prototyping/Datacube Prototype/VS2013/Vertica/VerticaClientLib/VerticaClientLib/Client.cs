using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vertica.Data.VerticaClient;

namespace de.haw.mars.rock.abstraction.vertica
{
    public class Client
    {
        public ClientConfig Config { get; set; }

        public Client(ClientConfig config)
        {
            this.Config = config;
        }

        public VerticaConnection CreateConnection()
        {
            return new VerticaConnection(Config.VerticaConnectionBuilder.ToString());
        }

        public VerticaConnection CreateAndOpen()
        {
            VerticaConnection res = new VerticaConnection(Config.VerticaConnectionBuilder.ToString());
            res.Open();
            return res;
        }

        public VerticaCommand CreateCommand(VerticaConnection conn, string query)
        {
            VerticaCommand command = conn.CreateCommand();
            command.CommandText = query;
            return command;
        }
    }
}
