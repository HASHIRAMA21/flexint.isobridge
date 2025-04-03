namespace FlexInt.ISOBridge;

public class Pacs008Message
{
    public int Id { get; set; }
    public string Document { get; set; }
    public string FIToFICstmrCdtTrf { get; set; }
    public string GrpHdr { get; set; }
    public string MsgId { get; set; }
    public DateTime CreDtTm { get; set; }
    public int NbOfTxs { get; set; }
    public decimal TtlIntrBkSttlmAmt { get; set; }
    public DateTime IntrBkSttlmDt { get; set; }
    public string SttlmInf { get; set; }
    public string SttlmMtd { get; set; }
    public string ClrSys { get; set; }
    public string Prtry { get; set; }
    public string PmtTpInf { get; set; }
    public string CtgyPurp { get; set; }
    public string InstgAgt { get; set; }
    public string FinInstnId { get; set; }
    public string BICFI { get; set; }
    public string BrnchId { get; set; }
    public string InstdAgt { get; set; }
    public string CdtTrfTxInf { get; set; }
    public string PmtId { get; set; }
    public string InstrId { get; set; }
    public string EndToEndId { get; set; }
    public string TxId { get; set; }
    public decimal IntrBkSttlmAmt { get; set; }
    public string SttlmPrty { get; set; }
    public string ChrgBr { get; set; }
    public string Dbtr { get; set; }
    public string Nm { get; set; }
    public string PrvtId { get; set; }
    public string Othr { get; set; }
    public string SchmeNm { get; set; }
    public string DbtrAcct { get; set; }
    public string DbtrAgt { get; set; }
    public string CdtrAgt { get; set; }
    public string Cdtr { get; set; }
    public string CdtrAcct { get; set; }
    public string Purp { get; set; }
    public string RmtInf { get; set; }
    public string Ustrd { get; set; }
    public string SplmtryData { get; set; }
    public string Envlp { get; set; }
    public string supplementaryData { get; set; }
    public string Items { get; set; }
    public string Item { get; set; }
}
