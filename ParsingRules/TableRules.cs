namespace FlexInt.ISOBridge.ParsingRules;

using System.Collections.Generic;

public class TableRules
{
    public Dictionary<string, TableRule> Rules { get; set; } = new Dictionary<string, TableRule>();
}

public class TableRule
{
    public string TableName { get; set; } = string.Empty;
    public Dictionary<string, ColumnRule> ColumnRules { get; set; } = new Dictionary<string, ColumnRule>();
}

public class ColumnRule
{
    public string ColumnName { get; set; } = string.Empty;
}