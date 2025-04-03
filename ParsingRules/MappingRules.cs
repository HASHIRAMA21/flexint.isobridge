namespace FlexInt.ISOBridge;

public class MappingRules
{
    public string Namespace { get; set; }
    public List<MappingTable> Mappings { get; set; }
}

public class MappingTable
{
    public string NodeName { get; set; }
    public string XPath { get; set; }
    public IList<MappingColumn> Columns { get; set;}
}

public class MappingColumn
{
    public int OrderNumber { get; set; }
    public string NodeName { get; set; }
    public string XPath { get; set; }
    public string DataType { get; set; }
    public int MinLength { get; set; }
    public int MaxLength { get; set; }
    public int fractionDigits { get; set; }
    public int totalDigits { get; set; }
}