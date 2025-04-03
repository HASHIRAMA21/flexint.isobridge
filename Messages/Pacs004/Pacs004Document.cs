using System.Xml.Serialization;
using FlexInt.ISOBridge.V1.Messages.Camt056;
using FlexInt.ISOBridge.V1.Messages.Pacs007;
using FlexInt.ISOBridge.V1.Messages.Pacs008;
using SupplementaryData = FlexInt.ISOBridge.V1.Messages.Pacs008.SupplementaryData;

namespace FlexInt.ISOBridge.V1.Messages.Pacs004
{
    [XmlRoot("Document", Namespace = "urn:iso:std:iso:20022:tech:xsd:pacs.004.001.05")]
    public class Pacs004Document
    {
        [XmlElement("PmtRtr")]
        public PaymentReturn? PaymentReturn { get; set; }
    }

    public class PaymentReturn
    {
        [XmlElement("GrpHdr")]
        public GroupHeader? GroupHeader { get; set; }

        [XmlElement("OrgnlGrpInf")]
        public OriginalGroupInformation? OriginalGroupInformation { get; set; }

        [XmlElement("TxInf")]
        public TransactionInformation[]? TransactionInformation { get; set; }

        [XmlElement("SplmtryData")]
        public SupplementaryData? SupplementaryData { get; set; }
    }

    public class TransactionInformation
    {
        [XmlElement("RtrId")]
        public string? ReturnId { get; set; }

        [XmlElement("OrgnlTxId")]
        public string? OriginalTransactionId { get; set; }

        [XmlElement("RtrdIntrBkSttlmAmt")]
        public Amount? ReturnedInterbankSettlementAmount { get; set; }

        [XmlElement("RtrRsnInf")]
        public ReturnReasonInformation? ReturnReasonInformation { get; set; }
    }

    public class ReturnReasonInformation
    {
        [XmlElement("Rsn")]
        public Reason? Reason { get; set; }

        [XmlElement("AddtlInf")]
        public string? AdditionalInformation { get; set; }
    }

    public class Reason
    {
        [XmlElement("Prtry")]
        public string? Proprietary { get; set; }
    }
}