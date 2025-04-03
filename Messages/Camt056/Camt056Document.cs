using System.Xml.Serialization;

namespace FlexInt.ISOBridge.V1.Messages.Camt056
{
 [XmlRoot("Document", Namespace = "urn:iso:std:iso:20022:tech:xsd:camt.056.001.04")]
    public class Camt056Document
    {
        [XmlElement("FIToFIPmtCxlReq")]
        public FiToFiPmtCxlReq? PaymentCancellationRequest { get; set; }
    }

    public class FiToFiPmtCxlReq
    {
        [XmlElement("Assgnmt")]
        public Assignment? Assignment { get; set; }

        [XmlElement("Undrlyg")]
        public Underlying? Underlying { get; set; }

        [XmlElement("SplmtryData")]
        public SupplementaryData? SupplementaryData { get; set; }
    }

    public class Assignment
    {
        [XmlElement("Id")]
        public string? Id { get; set; }

        [XmlElement("Assgnr")]
        public FinancialInstitution? Assigner { get; set; }

        [XmlElement("Assgne")]
        public FinancialInstitution? Assignee { get; set; }

        [XmlElement("CreDtTm")]
        public DateTime CreationDateTime { get; set; }
    }

    public class Underlying
    {
        [XmlElement("OrgnlGrpInfAndCxl")]
        public OriginalGroupInformationAndCancellation? OriginalGroupInformationAndCancellation { get; set; }

        [XmlElement("TxInf")]
        public TransactionInformation[]? TransactionInformation { get; set; }
    }

    public class OriginalGroupInformationAndCancellation
    {
        [XmlElement("GrpCxlId")]
        public string? GroupCancellationId { get; set; }

        [XmlElement("OrgnlMsgId")]
        public string? OriginalMessageId { get; set; }

        [XmlElement("OrgnlMsgNmId")]
        public string? OriginalMessageNameId { get; set; }

        [XmlElement("NbOfTxs")]
        public int NumberOfTransactions { get; set; }

        [XmlElement("GrpCxl")]
        public bool GroupCancellation { get; set; }
    }

    public class TransactionInformation
    {
        [XmlElement("CxlId")]
        public string? CancellationId { get; set; }

        [XmlElement("OrgnlTxId")]
        public string? OriginalTransactionId { get; set; }

        [XmlElement("CxlRsnInf")]
        public CancellationReasonInformation? CancellationReasonInformation { get; set; }
    }

    public class CancellationReasonInformation
    {
        [XmlElement("Rsn")]
        public Reason? Reason { get; set; }

        [XmlElement("AddtlInf")]
        public string? AdditionalInformation { get; set; }
    }

    public  class Reason
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