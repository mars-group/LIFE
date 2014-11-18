using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using Mono.Data.Sqlite;
using PedestrianModel.Agents;

namespace PedestrianModel.Logging {

    public class AgentLogger {
        private SqliteConnection _connection;

        public AgentLogger() {
            _connection = new SqliteConnection("URI=file:Database" + getDatabaseNumber() + ".sqlite");
            _connection.Open();
            IDbCommand command = _connection.CreateCommand();
            command.CommandText = "PRAGMA journal_mode = off";
            command.ExecuteNonQuery();
            command.CommandText = "PRAGMA synchronous = off";
            command.ExecuteNonQuery();
            command.CommandText = "DROP TABLE IF EXISTS agent";
            command.ExecuteNonQuery();
            command.CommandText = "CREATE TABLE agent (id INTEGER, name TEXT, x REAL, y REAL, t INTEGER)";
            command.ExecuteNonQuery();
            command.Dispose();
        }

        public void Log(List<Pedestrian> pedestrians) {
            SqliteTransaction transaction = _connection.BeginTransaction();
            IDbCommand command = _connection.CreateCommand();
            command.Transaction = transaction;
            foreach (Pedestrian ped in pedestrians) {
                string query = "INSERT INTO agent (id, name, x, y, t) VALUES ('" + ped.Id + "'," + "'" + ped.Name + "'"
                               + "," + ped.GetPosition().X.ToString(CultureInfo.InvariantCulture) + ","
                               + ped.GetPosition().Y.ToString(CultureInfo.InvariantCulture) + "," + ped.GetTick() + ");";
                command.CommandText = query;
                command.ExecuteNonQuery();
            }
            transaction.Commit();
            command.Dispose();
        }

        public void Close() {
            _connection.Close();
            _connection = null;
        }

        private int getDatabaseNumber() {
            int i = 1;
            while (true) {
                bool exists = File.Exists("database" + i + ".sqlite");
                if (exists) i++;
                else break;
            }
            return i;
        }
    }

}