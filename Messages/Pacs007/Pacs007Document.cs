using System.Xml.Serialization;
using FlexInt.ISOBridge.V1.Messages.Camt029;
using FlexInt.ISOBridge.V1.Messages.Pacs008;
using SupplementaryData = FlexInt.ISOBridge.V1.Messages.Pacs008.SupplementaryData;

namespace FlexInt.ISOBridge.V1.Messages.Pacs007
{
    [XmlRoot("Document", Namespace = "urn:iso:std:iso:20022:tech:xsd:pacs.007.001.05")]
    public class Pacs007Document
    {
        [XmlElement("FIToFIPmtRvsl")]
        public FiToFiPmtRvsl? PaymentReversal { get; set; }
    }

    public class FiToFiPmtRvsl
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

    public class OriginalGroupInformation
    {
        [XmlElement("OrgnlMsgId")]
        public string? OriginalMessageId { get; set; }

        [XmlElement("OrgnlMsgNmId")]
        public string? OriginalMessageNameId { get; set; }
    }

    public class TransactionInformation
    {
        [XmlElement("RvslId")]
        public string? ReversalId { get; set; }

        [XmlElement("OrgnlTxId")]
        public string? OriginalTransactionId { get; set; }

        [XmlElement("RvsdIntrBkSttlmAmt")]
        public Amount? RevisedInterbankSettlementAmount { get; set; }

        [XmlElement("RvslRsnInf")]
        public ReversalReasonInformation? ReversalReasonInformation { get; set; }
    }

    public class ReversalReasonInformation
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