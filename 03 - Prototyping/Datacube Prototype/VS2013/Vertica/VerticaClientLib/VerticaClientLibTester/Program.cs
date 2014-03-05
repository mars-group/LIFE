using ComLib.CsvParse;
using de.haw.mars.rock.abstraction.vertica;
using GlobalMercatorLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vertica.Data.VerticaClient;

namespace VerticaClientLibTester
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                CsvDoc doc = Csv.Load(@"d:\data.csv", true, true, ';');
                ClientConfig conf = new ClientConfig("localhost", "mars_rock", "mars", "rock");
                Client client = new Client(conf);

                for (int i = 0; i < doc.Data.Count; i++)
                {
                    Console.Write("[#" + i + "]: ");

                    double lat = doc.Get<double>(i, "X");
                    double lon = doc.Get<double>(i, "Y");
                    double alt = doc.Get<double>(i, "Z");

                    string family = doc.Get<string>(i, "Family");
                    string species = doc.Get<string>(i, "Species");
                    double height = doc.Get<double>(i, "Height");
                    double circumferen = doc.Get<double>(i, "Circumferen");
                    double scope_sn = -1;
                    double scope_ew = -1;
                    try
                    {
                        scope_sn = doc.Get<double>(i, "Houpier_sn");
                    }
                    catch { }
                    try
                    {
                        scope_ew = doc.Get<double>(i, "Houpier_ew");
                    }
                    catch { }

                    try
                    {
                        using (VerticaConnection conn = client.CreateAndOpen())
                        {
                            conn.InfoMessage += conn_InfoMessage;
                            VerticaTransaction txn = conn.BeginTransaction();
                            try
                            {
                                int id = i + 1;
                                InsertTree(client, id, family, species, height, circumferen, scope_ew, scope_sn, conn);
                                InsertSpace(client, id, lat, lon, alt, conn);
                                InsertFact(client, conn, id, id, id);
                                txn.Commit();
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex.ToString());
                                txn.Rollback();
                                break;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            Console.WriteLine("End.");
            Console.ReadLine();
        }

        static void conn_InfoMessage(object sender, VerticaInfoMessageEventArgs e)
        {
            Console.WriteLine(e.SqlState + ": " + e.Message);
        }

        private static void InsertFact(Client client, VerticaConnection conn, int fId, int tId, int sId)
        {
            using (VerticaCommand command = client.CreateCommand(conn, "INSERT INTO facts VALUES (@id, @space, @tree);"))
            {
                command.Parameters.Add(new VerticaParameter("id", VerticaType.BigInt, fId));
                command.Parameters.Add(new VerticaParameter("space", VerticaType.BigInt, sId));
                command.Parameters.Add(new VerticaParameter("tree", VerticaType.BigInt, tId));

                if (command.ExecuteNonQuery() != 1)
                {
                    throw new Exception("");
                }
            }
        }

        private static void InsertTree(Client client, int id, string family, string species, double height, double circumference, double scope_ew, double scope_sn, VerticaConnection conn)
        {
            using (VerticaCommand command = client.CreateCommand(conn, "INSERT INTO d_tree VALUES (@id, @species, @family, @height, @circumference, @scope_sn, @scope_ew);"))
            {
                command.Parameters.Add(new VerticaParameter("id", VerticaType.BigInt, id));

                command.Parameters.Add(new VerticaParameter("species", VerticaType.VarChar, species));
                command.Parameters.Add(new VerticaParameter("family", VerticaType.VarChar, family));
                command.Parameters.Add(new VerticaParameter("height", VerticaType.Numeric, height));
                command.Parameters.Add(new VerticaParameter("circumference", VerticaType.Numeric, circumference));
                command.Parameters.Add(new VerticaParameter("scope_sn", VerticaType.Numeric, scope_sn));
                command.Parameters.Add(new VerticaParameter("scope_ew", VerticaType.Numeric, scope_ew));

                if (command.ExecuteNonQuery() != 1)
                {
                    throw new Exception("");
                }
            }
        }

        private static void InsertSpace(Client client, int id, double lat, double lon, double alt, VerticaConnection conn)
        {
            using (VerticaCommand command = conn.CreateCommand())
            {
                command.CommandText = "INSERT INTO d_space VALUES (@id, @lat, @lon, @alt{0});";

                command.Parameters.Add(new VerticaParameter("id", VerticaType.BigInt, id));

                command.Parameters.Add(new VerticaParameter("lat", VerticaType.Double, lat));
                command.Parameters.Add(new VerticaParameter("lon", VerticaType.Double, lon));
                command.Parameters.Add(new VerticaParameter("alt", VerticaType.Numeric, alt));

                Point latLon = new Point(lat, lon);
                string paramNames = "";
                //for (int zoom = 1; zoom <= 21; zoom++)
                //{
                //    command.Parameters.Add(new VerticaParameter("z" + zoom, VerticaType.Char, GlobalMercator.LatLonToQuadTree(latLon, zoom)));
                //    paramNames += ", @z" + zoom;
                //}
                command.CommandText = string.Format(command.CommandText, paramNames);

                if (command.ExecuteNonQuery() != 1)
                {
                    throw new Exception("");
                }
            }
        }

        private static void ConnectVertica()
        {
            ClientConfig conf = new ClientConfig("localhost", "mars_rock", "mars", "rock");
            Client client = new Client(conf);

            try
            {
                using (VerticaConnection conn = client.CreateAndOpen())
                {
                    Console.WriteLine(conn.State.ToString());
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
    }
}

