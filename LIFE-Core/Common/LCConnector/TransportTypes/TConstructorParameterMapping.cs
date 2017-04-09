namespace LCConnector.TransportTypes
{
    public enum MappingType
    {
        ColumnParameterMapping,
        ValueParameterMapping
    }

    public class TConstructorParameterMapping
    {
        public TConstructorParameterMapping(string type, string name, bool isAutoInitialized, string mappingType,
            string tableName = "", string columnName = "", string value = "")
        {
            Type = type;
            Name = name;
            IsAutoInitialized = isAutoInitialized;
            MappingType mT;
            MappingType.TryParse(mappingType, true, out mT);
            MappingType = mT;
            TableName = tableName;
            ColumnName = columnName;
            Value = value;
        }

        public string Type { get; private set; }
        public string Name { get; private set; }
        public bool IsAutoInitialized { get; private set; }
        public MappingType MappingType { get; private set; }
        public string TableName { get; private set; }
        public string ColumnName { get; private set; }
        public string Value { get; private set; }
    }
}