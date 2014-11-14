using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mono.Data.Sqlite;
using System.Data;
using PedestrianModel.Agents;
using System.Globalization;
using System.IO;

namespace PedestrianModel.Logging
{
    class AgentLogger
    {
        private SqliteConnection connection;

        public AgentLogger()
        {            
            connection = new SqliteConnection("URI=file:Database" + getDatabaseNumber() + ".sqlite");
            connection.Open();
            IDbCommand command = connection.CreateCommand();            
            command.CommandText = "PRAGMA journal_mode = off";
            command.ExecuteNonQuery();
            command.CommandText = "PRAGMA synchronous = off";
            command.ExecuteNonQuery();
            command.CommandText = "DROP TABLE IF EXISTS agent";
            command.ExecuteNonQuery();
            command.CommandText = "CREATE TABLE agent (id INTEGER, name TEXT, x REAL, y REAL, t INTEGER)";
            command.ExecuteNonQuery();            
            command.Dispose();
            command = null;
        }

        public void Log(List<Pedestrian> pedestrians)
        {
            SqliteTransaction transaction = connection.BeginTransaction();
            IDbCommand command = connection.CreateCommand();
            command.Transaction = transaction;
            foreach (Pedestrian ped in pedestrians) {
                string query = "INSERT INTO agent (id, name, x, y, t) VALUES ('" + ped.Id + "'," + "'" + ped.Name + "'" + "," + ped.GetPosition().X.ToString(CultureInfo.InvariantCulture) + "," + ped.GetPosition().Y.ToString(CultureInfo.InvariantCulture) + "," + ped.GetTick() + ");";
                command.CommandText = query;
                command.ExecuteNonQuery();
            }
            transaction.Commit();
            command.Dispose();
            command = null;            
        }

        public void Close()
        {
            connection.Close();
            connection = null;
        }

        private int getDatabaseNumber()
        {
            int i = 1;
            while (true)
            {
                bool exists = File.Exists("database" + i + ".sqlite");
                if (exists)
                {
                    i++;
                }
                else
                {
                    break;
                }
            }
            return i;
        }

    }
}
