using System.Xml.Serialization;

namespace FlexInt.ISOBridge.V1.Messages.Pacs008
{
    [XmlRoot("Document", Namespace = "urn:iso:std:iso:20022:tech:xsd:pacs.008.001.05")]
    public class Pacs008Document
    {
        [XmlElement("FIToFICstmrCdtTrf")]
        public FiToFiCstmrCdtTrf? CreditTransfer { get; set; }
    }

    public class FiToFiCstmrCdtTrf
    {
        [XmlElement("GrpHdr")]
        public GroupHeader? GroupHeader { get; set; }

        [XmlElement("CdtTrfTxInf")]
        public CreditTransferTransactionInformation[]? CreditTransferTransactions { get; set; }

        [XmlElement("SplmtryData")]
        public SupplementaryData? SupplementaryData { get; set; }
    }

    public class GroupHeader
    {
        [XmlElement("MsgId")]
        public string? MessageId { get; set; }

        [XmlElement("CreDtTm")]
        public DateTime CreationDateTime { get; set; }

        [XmlElement("NbOfTxs")]
        public int NumberOfTransactions { get; set; }

        [XmlElement("TtlIntrBkSttlmAmt")]
        public Amount? TotalInterbankSettlementAmount { get; set; }

        [XmlElement("IntrBkSttlmDt")]
        public DateTime InterbankSettlementDate { get; set; }

        [XmlElement("SttlmInf")]
        public SettlementInformation? SettlementInformation { get; set; }

        [XmlElement("PmtTpInf")]
        public PaymentTypeInformation? PaymentTypeInformation { get; set; }

        [XmlElement("InstgAgt")]
        public FinancialInstitution? InstructingAgent { get; set; }

        [XmlElement("InstdAgt")]
        public FinancialInstitution? InstructedAgent { get; set; }
    }

    public class CreditTransferTransactionInformation
    {
        [XmlElement("PmtId")]
        public PaymentIdentification? PaymentIdentification { get; set; }

        [XmlElement("IntrBkSttlmAmt")]
        public Amount? InterbankSettlementAmount { get; set; }

        [XmlElement("SttlmPrty")]
        public string? SettlementPriority { get; set; }

        [XmlElement("ChrgBr")]
        public string? ChargeBearer { get; set; }

        [XmlElement("Dbtr")]
        public Party? Debtor { get; set; }

        [XmlElement("DbtrAcct")]
        public Account? DebtorAccount { get; set; }

        [XmlElement("DbtrAgt")]
        public FinancialInstitution? DebtorAgent { get; set; }

        [XmlElement("CdtrAgt")]
        public FinancialInstitution? CreditorAgent { get; set; }

        [XmlElement("Cdtr")]
        public Party? Creditor { get; set; }

        [XmlElement("CdtrAcct")]
        public Account? CreditorAccount { get; set; }

        [XmlElement("Purp")]
        public Purpose? Purpose { get; set; }

        [XmlElement("RmtInf")]
        public RemittanceInformation? RemittanceInformation { get; set; }
    }

    public class PaymentIdentification
    {
        [XmlElement("InstrId")]
        public string? InstructionId { get; set; }

        [XmlElement("EndToEndId")]
        public string? EndToEndId { get; set; }

        [XmlElement("TxId")]
        public string? TransactionId { get; set; }
    }

    public class Amount
    {
        [XmlAttribute("Ccy")]
        public string? Currency { get; set; }

        [XmlText]
        public decimal Value { get; set; }
    }

    public class SettlementInformation
    {
        [XmlElement("SttlmMtd")]
        public string? SettlementMethod { get; set; }

        [XmlElement("ClrSys")]
        public ClearingSystem? ClearingSystem { get; set; }
    }

    public class ClearingSystem
    {
        [XmlElement("Prtry")]
        public string? Proprietary { get; set; }
    }

    public class PaymentTypeInformation
    {
        [XmlElement("CtgyPurp")]
        public CategoryPurpose? CategoryPurpose { get; set; }
    }

    public class CategoryPurpose
    {
        [XmlElement("Prtry")]
        public string? Proprietary { get; set; }
    }

    public class FinancialInstitution
    {
        [XmlElement("FinInstnId")]
        public FinancialInstitutionIdentification? FinancialInstitutionId { get; set; }

        [XmlElement("BrnchId")]
        public BranchIdentification? BranchId { get; set; }
    }

    public class FinancialInstitutionIdentification
    {
        [XmlElement("BICFI")]
        public string? Bicfi { get; set; }
    }

    public class BranchIdentification
    {
        [XmlElement("Id")]
        public string? Id { get; set; }
    }

    public class Party
    {
        [XmlElement("Nm")]
        public string? Name { get; set; }

        [XmlElement("Id")]
        public PartyIdentification? Id { get; set; }
    }

    public class PartyIdentification
    {
        [XmlElement("PrvtId")]
        public PrivateIdentification? PrivateId { get; set; }
    }

    public class PrivateIdentification
    {
        [XmlElement("Othr")]
        public OtherIdentification? Other { get; set; }
    }

    public class OtherIdentification
    {
        [XmlElement("Id")]
        public string? Id { get; set; }

        [XmlElement("SchmeNm")]
        public SchemeName? SchemeName { get; set; }
    }

    public class SchemeName
    {
        [XmlElement("Prtry")]
        public string? Proprietary { get; set; }
    }

    public class Account
    {
        [XmlElement("Id")]
        public AccountIdentification? Id { get; set; }
    }

    public class AccountIdentification
    {
        [XmlElement("Othr")]
        public OtherAccountIdentification? Other { get; set; }
    }

    public class OtherAccountIdentification
    {
        [XmlElement("Id")]
        public string? Id { get; set; }
    }

    public class Purpose
    {
        [XmlElement("Prtry")]
        public string? Proprietary { get; set; }
    }

    public class RemittanceInformation
    {
        [XmlElement("Ustrd")]
        public string? Unstructured { get; set; }
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

    public class Items
    {
        [XmlElement("Item")]
        public Item[]? Item { get; set; }
    }

    public class Item
    {
        [XmlAttribute("key")]
        public string? Key { get; set; }

        [XmlText]
        public string? Value { get; set; }
    }
}