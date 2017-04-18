using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Cassandra;
using LIFE.API.Results;
using MongoDB.Bson;

namespace ResultAdapter.Implementation.DataOutput
{
    /// <summary>
    ///   Adapter for the Cassandra storage pipeline developed by Janus.
    /// </summary>
    public class CassandraWriter : IResultWriter
    {
        private readonly string _simId; // Simulation identifier.
        // Use Geo2D spatial index for agent positions
        // ID Type Mapping for each active Agent
        //                   ID     Type
        private ConcurrentDictionary<string, string> _agentIdTypeMapping =
            new ConcurrentDictionary<string, string>();

        private ConcurrentDictionary<string, PreparedStatement> _preparedFrameStatements =
            new ConcurrentDictionary<string, PreparedStatement>();

        private readonly ISession _session;
        private readonly string _keyspace;

        private readonly ConcurrentDictionary<string, string> _objectTypeMapping =
            new ConcurrentDictionary<string, string>();

        //                         tablename   hasKFPosition    property
        private ConcurrentDictionary<string, Tuple<bool,    List<string>>> _propertiesTableMapping =
            new ConcurrentDictionary<string, Tuple<bool,    List<string>>>();

        private bool _verboseOutput = false;
        /// <summary>
        ///   Create a new Cassandra adapter.
        /// </summary>
        /// <param name="initParams">Initialization settings.</param>
        public CassandraWriter(IDictionary<string, string> initParams,
                               IEnumerable<LoggerConfig> loggerConfigs) {

            _objectTypeMapping.TryAdd("int32", "INT");
            _objectTypeMapping.TryAdd("string", "TEXT");
            _objectTypeMapping.TryAdd("double", "DOUBLE");
            _objectTypeMapping.TryAdd("boolean", "BOOLEAN");

            _simId = initParams["SimulationId"];

            string cassandraIPfirst = "cassandra";

            if( loggerConfigs == null || !loggerConfigs.Any() )
                throw new ArgumentNullException("CassandraWriter needs at least one LoggerConfig for initialization.");

            Cluster cluster = Cluster.Builder().AddContactPoint(cassandraIPfirst).Build();
            _session = cluster.Connect();

            // create keyspace if not exists
            _keyspace = _simId.Replace("-","_").Insert(0,"XXX"); // https://docs.datastax.com/en/cql/3.1/cql/cql_reference/ref-lexical-valid-chars.html
            try
            {
                string keyspaceStmt = string.Format(@"
                  CREATE KEYSPACE IF NOT EXISTS {0} WITH replication = {{'class': 'SimpleStrategy', 'replication_factor': 1}};
                  ", _keyspace);
                if(_verboseOutput)
                    Console.WriteLine(keyspaceStmt);

                _session.Execute(keyspaceStmt);
            } catch (AlreadyExistsException)
            {
            }
            List<string> typeTableNames = new List<string>();

            // iterate over loggerConfigs, for each logger init cassandra tables
            foreach (var loggerDef in loggerConfigs)
            {
                bool useGeoSpatialIndex = loggerDef.SpatialType == "GPS";
                bool hasKFPosition = ! loggerDef.IsStationary;
                string agentType = loggerDef.TypeName;
                string tableName = agentType;
                typeTableNames.Add(tableName);

                // create table with dynamic propoerties
                // TODO:maybe use UUID for AgentID?
                // TODO: STATIC PROPS
                string tablesPropertiesList = "";
                var propType = "TEXT";
                foreach (var property in loggerDef.Properties) {
                    if (! property.Value.Item1) // is not static
                    {
                        _objectTypeMapping.TryGetValue(property.Value.Item2, out propType);
                        tablesPropertiesList += string.Format(
                            "{0} {1},\n", property.Key, propType
                        );
                    }
                }

                string positionKF = "";
                if (hasKFPosition)
                    positionKF += loggerDef.SpatialType == "Grid" ? "latitude INT, longitude INT," :
                                                           "latitude DOUBLE, longitude DOUBLE,";

                string createTableStmt = string.Format(@"
                    CREATE TABLE IF NOT EXISTS {0}.{1} (
                       agentid TEXT,
                       tick BIGINT,
                       {2}
                       {3}
                       PRIMARY KEY(agentid,tick)
                    ) WITH CLUSTERING ORDER BY (tick ASC);
                ", _keyspace, tableName, positionKF, tablesPropertiesList);
                _session.Execute(createTableStmt);
                Console.WriteLine("Cassandra: Created Table: " + createTableStmt);

                // TODO: create table with static propoerties


                // create lucence index
                if (useGeoSpatialIndex)
                {
                    string geospatialIndexStmt = string.Format(@"
                        CREATE CUSTOM INDEX {0}_{1}_index ON {0}.{1} ()
                        USING 'com.stratio.cassandra.lucene.Index'
                        WITH OPTIONS = {{
                           'refresh_seconds': '1',
                           'schema': '{{
                              fields: {{
                                 agentid: {{type: ""string""}},
                                 tick: {{type: ""integer""}},
                                 place: {{type: ""geo_point"", latitude: ""latitude"", longitude: ""longitude""}}
                                }}
                            }}'
                        }};
                    ", _keyspace, tableName);
                    _session.Execute(geospatialIndexStmt);
                    Console.WriteLine("Cassandra: Create GeoIndex: " + geospatialIndexStmt);
                }

                // create prepared statements for later use
                int propCount = loggerDef.Properties.Count;
                string stmtsPropertiesList = "";
                string stmtsValuePlaceholder = "";
                List<string> sorting = new List<string>();
                for (var i = 0; i < propCount; i++)
                {
                    if (!loggerDef.Properties.ElementAt(i).Value.Item1) // is not static
                    {
                        string prop = loggerDef.Properties.ElementAt(i).Key;
                        stmtsPropertiesList += string.Format(
                            ", {0}",prop);
                        stmtsValuePlaceholder += ",?";
                        sorting.Add(prop);
                    }
                }

                _propertiesTableMapping.TryAdd(tableName,
                                               new Tuple<bool, List<string>>(hasKFPosition,
                                                                             sorting));

                string prepareStmt = "";
                if (hasKFPosition)
                {
                    prepareStmt =
                        string.Format(@"INSERT INTO {0}.{1} (agentid, tick, latitude, longitude{2})
                                        VALUES (?,?,?,?{3})",
                            _keyspace, tableName, stmtsPropertiesList, stmtsValuePlaceholder);
                }
                else
                {
                    prepareStmt =
                        string.Format(@"INSERT INTO {0}.{1} (agentid, tick{2})
                                        VALUES (?,?{3})",
                            _keyspace, tableName, stmtsPropertiesList, stmtsValuePlaceholder);
                }

                if(_verboseOutput)
                    Console.WriteLine(prepareStmt);

                PreparedStatement insertIntoStmt = _session.Prepare(prepareStmt);

                _preparedFrameStatements.TryAdd(tableName,insertIntoStmt);
            }

            WriteCassandraMetaData(typeTableNames);
        }

        private void WriteCassandraMetaData(List<string> typeTableNames)
        {
            string metaCreateStmt = string.Format(@"
                    CREATE TABLE IF NOT EXISTS {0}.metadata (
                       id INT,
                       typeTableName TEXT,
                       PRIMARY KEY(typeTableName)
                    );
                ", _keyspace);
            if(_verboseOutput)
                Console.WriteLine(metaCreateStmt);
            _session.Execute(metaCreateStmt);

            for(var i = 1; i < typeTableNames.Count; i++)
            {
                string insertTableToMetaDataStmt = string.Format(
                    @"INSERT INTO {0}.metadata (id, typeTableName) VALUES ({1},'{2}');",
                    _keyspace, i, typeTableNames.ElementAt(i));
                if(_verboseOutput)
                    Console.WriteLine(insertTableToMetaDataStmt);
                _session.Execute(insertTableToMetaDataStmt);
                Console.WriteLine("Cassandra: Created Table: " + insertTableToMetaDataStmt);
            }
        }

        public void WriteLegacyResults(IEnumerable<AgentSimResult> results)
        {
            throw new NotImplementedException("CassandraWriter does not support Legacy Mode.");
        }

        public void AddMetadataEntries(IEnumerable<AgentMetadataEntry> metadataEntries)
        {
            //TODO: ADD TO CASSANDRA TABLE STATIC PROPS

            foreach (var metadata in metadataEntries) {
                _agentIdTypeMapping.TryAdd(metadata.AgentId,
                                           metadata.AgentType);
            }
        }

        public void SetAgentDeletionFlags(IEnumerable<string> agentIds, int delTick)
        {
            //TODO: UPDATE CASSANDRA TABLE STATIC PROPS

            foreach (var agentId in agentIds)
            {
                try
                {
                    string ignored;
                    _agentIdTypeMapping.TryRemove(agentId, out ignored);
                } catch (ArgumentNullException e) {
                    Console.WriteLine(e);
                    throw;
                }
            }
        }

        public void WriteAgentFrames(IEnumerable<AgentFrame> results, bool isKeyframe)
        {
            Console.WriteLine("Cassandra started keyframe output");
            var swParallel = Stopwatch.StartNew();

            var insertStmts = new ConcurrentBag<BoundStatement>();
            Parallel.ForEach(results, result =>
            {
                if(_verboseOutput)
                    Console.WriteLine(result.ToJson().ToString());
                if(!result.IsKeyframe) throw new NotImplementedException("CassandraWriter does not support delta frames.");
                string tableName;
                PreparedStatement preparedStmt;

                _agentIdTypeMapping.TryGetValue(result.AgentId, out tableName);
                _preparedFrameStatements.TryGetValue(tableName, out preparedStmt);

                Tuple<bool,List<string>> propPreparedStmtSorting;
                _propertiesTableMapping.TryGetValue(tableName, out propPreparedStmtSorting);

                List<object> bindValues = new List<object>();
                bindValues.Add(result.AgentId);
                bindValues.Add(result.Tick);

                if(propPreparedStmtSorting.Item1) {
                    bindValues.Add(result.Position.ElementAt(0));
                    bindValues.Add(result.Position.ElementAt(1));
                }

                foreach (var property in propPreparedStmtSorting.Item2)
                {
                    object propertyOut = result.Properties[property];
                    bindValues.Add(propertyOut);
                }
                if (preparedStmt != null)
                    insertStmts.Add(preparedStmt.Bind(bindValues.ToArray()));
            });

            var tasks = new ConcurrentBag<Task<RowSet>>();
            var options = new ParallelOptions {MaxDegreeOfParallelism = Environment.ProcessorCount * 4};
            Parallel.ForEach(insertStmts, options, stmt =>
            {
               if(_verboseOutput)
                    Console.WriteLine(stmt.PreparedStatement.Variables.ToJson().ToString() + stmt.QueryValues.ToJson().ToString());
                tasks.Add(_session.ExecuteAsync(stmt));
            });
            try {
                Task.WaitAll(tasks.ToArray());
            } catch (AggregateException e) {
                Console.WriteLine("\nThe following exceptions have been thrown by WaitAll() \n");
                foreach (var exception in e.InnerExceptions)
                {
                    Console.WriteLine(exception.ToString());
                }
            }
            Console.WriteLine("Cassandra finished writting results in " + swParallel.ElapsedMilliseconds + " ms");
        }
    }
}