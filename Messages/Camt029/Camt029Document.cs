using System.Xml.Serialization;

namespace FlexInt.ISOBridge.V1.Messages.Camt029
{
    [XmlRoot("Document", Namespace = "urn:iso:std:iso:20022:tech:xsd:camt.029.001.05")]
    public class Camt029Document
    {
        [XmlElement("RsltnOfInvstgtn")]
        public ResolutionOfInvestigation? ResolutionOfInvestigation { get; set; }
    }

    public class ResolutionOfInvestigation
    {
        [XmlElement("Assgnmt")]
        public Assignment? Assignment { get; set; }

        [XmlElement("Sts")]
        public Status? Status { get; set; }

        [XmlElement("CxlDtls")]
        public CancellationDetails? CancellationDetails { get; set; }

        [XmlElement("SplmtryData")]
        public SupplementaryData? SupplementaryData { get; set; }
    }

    public class Assignment
    {
        [XmlElement("Id")]
        public string Id { get; set; }

        [XmlElement("Assgnr")]
        public FinancialInstitution? Assigner { get; set; }

        [XmlElement("Assgne")]
        public FinancialInstitution? Assignee { get; set; }

        [XmlElement("CreDtTm")]
        public DateTime CreationDateTime { get; set; }
    }

    public class Status
    {
        [XmlElement("AssgnmtCxlConf")]
        public bool AssignmentCancellationConfirmation { get; set; }
    }

    public class CancellationDetails
    {
        [XmlElement("OrgnlGrpInfAndSts")]
        public OriginalGroupInformationAndStatus? OriginalGroupInformationAndStatus { get; set; }

        [XmlElement("TxInfAndSts")]
        public TransactionInformationAndStatus[]? TransactionInformationAndStatus { get; set; }
    }

    public class OriginalGroupInformationAndStatus
    {
        [XmlElement("OrgnlMsgId")]
        public string? OriginalMessageId { get; set; }

        [XmlElement("OrgnlMsgNmId")]
        public string? OriginalMessageNameId { get; set; }

        [XmlElement("GrpCxlSts")]
        public string? GroupCancellationStatus { get; set; }
    }

    public class TransactionInformationAndStatus
    {
        [XmlElement("CxlStsId")]
        public string? CancellationStatusId { get; set; }

        [XmlElement("OrgnlTxId")]
        public string? OriginalTransactionId { get; set; }

        [XmlElement("TxCxlSts")]
        public string? TransactionCancellationStatus { get; set; }

        [XmlElement("CxlStsRsnInf")]
        public CancellationStatusReasonInformation? CancellationStatusReasonInformation { get; set; }
    }

    public class CancellationStatusReasonInformation
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

    public  class FinancialInstitution
    {
        [XmlElement("FinInstnId")]
        public FinancialInstitutionIdentification? FinancialInstitutionId { get; set; }
    }

    public class FinancialInstitutionIdentification
    {
        [XmlElement("BICFI")]
        public string? Bicfi { get; set; }
    }

    public class SupplementaryData
    {
        [XmlElement("Envlp")]
        public Envelope? Envelope { get; set; }
    }

    public class Envelope
    {
        [XmlElement("supplementaryData", Namespace = "http://www.Progressoft.com/ACH")]
        public SupplementaryDataContent? SupplementaryData { get; set; }
    }

    public class SupplementaryDataContent
    {
        [XmlElement("Items")]
        public Items? Items { get; set; }
    }

    public  class Items
    {
        [XmlElement("Item")]
        public Item[]? Item { get; set; }
    }

    public  class Item
    {
        [XmlAttribute("key")]
        public string? Key { get; set; }

        [XmlText]
        public string? Value { get; set; }
    }
}