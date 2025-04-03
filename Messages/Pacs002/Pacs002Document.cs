using System.Xml.Serialization;
using FlexInt.ISOBridge.V1.Messages.Pacs008;

namespace FlexInt.ISOBridge.V1.Messages.Pacs002
{
    [XmlRoot("Document", Namespace = "urn:iso:std:iso:20022:tech:xsd:pacs.002.001.06")]
    public class Pacs002Document
    {
        [XmlElement("FIToFIPmtStsRpt")]
        public FiToFiPmtStsRpt? PaymentStatusReport { get; set; }
    }

    public class FiToFiPmtStsRpt
    {
        [XmlElement("GrpHdr")]
        public GroupHeader? GroupHeader { get; set; }

        [XmlElement("OrgnlGrpInfAndSts")]
        public OriginalGroupInformationAndStatus? OriginalGroupInformationAndStatus { get; set; }

        [XmlElement("TxInfAndSts")]
        public TransactionInformationAndStatus[]? TransactionInformationAndStatus { get; set; }

        [XmlElement("SplmtryData")]
        public SupplementaryData? SupplementaryData { get; set; }
    }

    public class OriginalGroupInformationAndStatus
    {
        [XmlElement("OrgnlMsgId")]
        public string? OriginalMessageId { get; set; }

        [XmlElement("OrgnlMsgNmId")]
        public string? OriginalMessageNameId { get; set; }

        [XmlElement("GrpSts")]
        public string? GroupStatus { get; set; }

        [XmlElement("StsRsnInf")]
        public StatusReasonInformation? StatusReasonInformation { get; set; }
    }

    public class TransactionInformationAndStatus
    {
        [XmlElement("StsId")]
        public string? StatusId { get; set; }

        [XmlElement("OrgnlEndToEndId")]
        public string? OriginalEndToEndId { get; set; }

        [XmlElement("OrgnlTxId")]
        public string? OriginalTransactionId { get; set; }

        [XmlElement("TxSts")]
        public string? TransactionStatus { get; set; }

        [XmlElement("StsRsnInf")]
        public StatusReasonInformation? StatusReasonInformation { get; set; }
    }

    public class StatusReasonInformation
    {
        [XmlElement("Rsn")]
        public Reason? Reason { get; set; }

        [XmlElement("AddtlInf")]
        public string? AdditionalInformation { get; set; }
    }

    public partial class Reason
    {
        [XmlElement("Prtry")]
        public string? Proprietary { get; set; }
    }
}