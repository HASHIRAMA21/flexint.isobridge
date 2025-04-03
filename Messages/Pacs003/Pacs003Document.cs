using System;
using System.Xml.Serialization;
using FlexInt.ISOBridge.V1.Messages.Pacs008;


namespace FlexInt.ISOBridge.V1.Messages.Pacs003
{
    [XmlRoot("Document", Namespace = "urn:iso:std:iso:20022:tech:xsd:pacs.003.001.05")]
    public class Pacs003Document
    {
        [XmlElement("FIToFICstmrDrctDbt")]
        public FiToFiCstmrDrctDbt? DirectDebit { get; set; }
    }

    public class FiToFiCstmrDrctDbt
    {
        [XmlElement("GrpHdr")]
        public GroupHeader? GroupHeader { get; set; }

        [XmlElement("DrctDbtTxInf")]
        public DirectDebitTransactionInformation[]? DirectDebitTransactions { get; set; }

        [XmlElement("SplmtryData")]
        public SupplementaryData? SupplementaryData { get; set; }
    }

    public class DirectDebitTransactionInformation
    {
        [XmlElement("PmtId")]
        public PaymentIdentification? PaymentIdentification { get; set; }

        [XmlElement("IntrBkSttlmAmt")]
        public Amount? InterbankSettlementAmount { get; set; }

        [XmlElement("SttlmPrty")]
        public string? SettlementPriority { get; set; }

        [XmlElement("ChrgBr")]
        public string? ChargeBearer { get; set; }

        [XmlElement("DrctDbtTx")]
        public DirectDebitTransaction? DirectDebitTransaction { get; set; }

        [XmlElement("Cdtr")]
        public Party? Creditor { get; set; }

        [XmlElement("CdtrAcct")]
        public Account? CreditorAccount { get; set; }

        [XmlElement("CdtrAgt")]
        public FinancialInstitution? CreditorAgent { get; set; }

        [XmlElement("Dbtr")]
        public Party? Debtor { get; set; }

        [XmlElement("DbtrAcct")]
        public Account? DebtorAccount { get; set; }

        [XmlElement("DbtrAgt")]
        public FinancialInstitution? DebtorAgent { get; set; }

        [XmlElement("Purp")]
        public Purpose? Purpose { get; set; }

        [XmlElement("RmtInf")]
        public RemittanceInformation? RemittanceInformation { get; set; }

        [XmlElement("SplmtryData")]
        public SupplementaryData? SupplementaryData { get; set; }
    }

    public class DirectDebitTransaction
    {
        [XmlElement("MndtRltdInf")]
        public MandateRelatedInformation? MandateRelatedInformation { get; set; }
    }

    public class MandateRelatedInformation
    {
        [XmlElement("MndtId")]
        public string? MandateId { get; set; }

        [XmlElement("FrstColltnDt")]
        public DateTime FirstCollectionDate { get; set; }
    }
    
}