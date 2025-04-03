using System.Data.SqlClient;
using FlexInt.ISOBridge.Data;
using FlexInt.ISOBridge.V1.Messages.Pacs003;
using FlexInt.ISOBridge.V1.Messages.Pacs007;
using FlexInt.ISOBridge.V1.Messages.Pacs008;

namespace FlexInt.ISOBridge.V1.Services
{
    public class MappingService
    {
        private readonly DatabaseManager _databaseManager;

        public MappingService(DatabaseManager databaseManager)
        {
            _databaseManager = databaseManager ?? throw new ArgumentNullException(nameof(databaseManager));
        }



         public List<Pacs008Document> MapPacs008Data()
        {
            var documents = new List<Pacs008Document>();

            using (var connection = _databaseManager.CreateConnection())
            {
                connection.Open();
                var query = @"
                    SELECT 
                        MsgId, CreDtTm, NbOfTxs, TtlIntrBkSttlmAmt, IntrBkSttlmDt, 
                        SttlmMtd, ClrSys, InstrId, EndToEndId, DbtrNm, CdtTrfTxInf, 
                        CdtrNm, DbtrAcctId, CdtrAcctId
                    FROM 
                        fexint_clearing_dbs.dbo.pacs8valid;";

                using (var command = new SqlCommand(query, connection))

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var document = new Pacs008Document
                        {
                            CreditTransfer = new FiToFiCstmrCdtTrf
                            {
                                GroupHeader = new GroupHeader
                                {
                                    MessageId = reader["MsgId"].ToString(),
                                    CreationDateTime = Convert.ToDateTime(reader["CreDtTm"]),
                                    NumberOfTransactions = Convert.ToInt32(reader["NbOfTxs"]),
                                    TotalInterbankSettlementAmount = new Amount
                                    {
                                        Currency = "EUR",
                                        Value = Convert.ToDecimal(reader["TtlIntrBkSttlmAmt"])
                                    },
                                    InterbankSettlementDate = Convert.ToDateTime(reader["IntrBkSttlmDt"]),
                                    SettlementInformation = new SettlementInformation
                                    {
                                        SettlementMethod = reader["SttlmMtd"].ToString(),
                                        ClearingSystem = new ClearingSystem
                                        {
                                            Proprietary = reader["ClrSys"].ToString()
                                        }
                                    }
                                },
                                CreditTransferTransactions = new[]
                                {
                                    new CreditTransferTransactionInformation
                                    {
                                        PaymentIdentification = new PaymentIdentification
                                        {
                                            InstructionId = reader["InstrId"].ToString(),
                                            EndToEndId = reader["EndToEndId"].ToString()
                                        },
                                        InterbankSettlementAmount = new Amount
                                        {
                                            Currency = "EUR",
                                            Value = Convert.ToDecimal(reader["TtlIntrBkSttlmAmt"])
                                        },
                                        Debtor = new Party
                                        {
                                            Name = reader["DbtrNm"].ToString(),
                                            Id = new PartyIdentification
                                            {
                                                PrivateId = new PrivateIdentification
                                                {
                                                    Other = new OtherIdentification
                                                    {
                                                        Id = reader["DbtrAcctId"].ToString(),
                                                        SchemeName = new SchemeName
                                                        {
                                                            Proprietary = "AccountId"
                                                        }
                                                    }
                                                }
                                            }
                                        },
                                        Creditor = new Party
                                        {
                                            Name = reader["CdtrNm"].ToString(),
                                            Id = new PartyIdentification
                                            {
                                                PrivateId = new PrivateIdentification
                                                {
                                                    Other = new OtherIdentification
                                                    {
                                                        Id = reader["CdtrAcctId"].ToString(),
                                                        SchemeName = new SchemeName
                                                        {
                                                            Proprietary = "AccountId"
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        };

                        documents.Add(document);
                    }
                }
            }

            return documents;
        }

        public List<Pacs007Document> MapPacs007Data()
        {
            var documents = new List<Pacs007Document>();

            using (var connection = _databaseManager.CreateConnection())
            {
                connection.Open();
                var query = @"
                    SELECT 
                        MsgId, CreDtTm, NbOfTxs, IntrBkSttlmDt, SttlmInf, SttlmMtd, 
                        ClrSys, Prtry, InstgAgt, OrgnlMsgId, OrgnlMsgNmId, 
                        RvslId, OrgnlTxId, RvsdIntrBkSttlmAmt, Rsn, AddtlInf
                    FROM 
                        fexint_clearing_dbs.dbo.valid_pacs7_withAdditionalInfo;";

                using (var command = new SqlCommand(query, connection))

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var document = new Pacs007Document
                        {
                            PaymentReversal = new FiToFiPmtRvsl
                            {
                                GroupHeader = new GroupHeader
                                {
                                    MessageId = reader["MsgId"].ToString(),
                                    CreationDateTime = Convert.ToDateTime(reader["CreDtTm"]),
                                    NumberOfTransactions = Convert.ToInt32(reader["NbOfTxs"]),
                                    InterbankSettlementDate = Convert.ToDateTime(reader["IntrBkSttlmDt"]),
                                    SettlementInformation = new SettlementInformation
                                    {
                                        SettlementMethod = reader["SttlmMtd"].ToString(),
                                        ClearingSystem = new ClearingSystem
                                        {
                                            Proprietary = reader["ClrSys"].ToString()
                                        },
                                        /*
                                        InstructingAgent = new FinancialInstitution
                                        {
                                            FinancialInstitutionId = new FinancialInstitutionIdentification
                                            {
                                                Bicfi = reader["BICFI"].ToString()
                                            },
                                            BranchId = new BranchIdentification
                                            {
                                                Id = reader["BrnchId"].ToString()
                                            }
                                        },
                                        InstructedAgent = new FinancialInstitution
                                        {
                                            FinancialInstitutionId = new FinancialInstitutionIdentification
                                            {
                                                Bicfi = reader["InstdAgtBICFI"].ToString() // Example for instructed agent
                                            }
                                        } */
                                    }
                                },
                                OriginalGroupInformation = new OriginalGroupInformation
                                {
                                    OriginalMessageId = reader["OrgnlMsgId"].ToString(),
                                    OriginalMessageNameId = reader["OrgnlMsgNmId"].ToString()
                                },
                                TransactionInformation = new[]
                                {
                                    new TransactionInformation
                                    {
                                        ReversalId = reader["RvslId"].ToString(),
                                        OriginalTransactionId = reader["OrgnlTxId"].ToString(),
                                        RevisedInterbankSettlementAmount = new Amount
                                        {
                                            Currency = "EUR", // Example currency
                                            Value = Convert.ToDecimal(reader["RvsdIntrBkSttlmAmt"])
                                        },
                                        ReversalReasonInformation = new ReversalReasonInformation
                                        {
                                            Reason = new Reason
                                            {
                                                Proprietary = reader["Rsn"].ToString()
                                            },
                                            AdditionalInformation = reader["AddtlInf"].ToString()
                                        }
                                    }
                                },
                                SupplementaryData = new SupplementaryData
                                {
                                    Envelope = new Envelope
                                    {
                                        SupplementaryData = new SupplementaryDataContent
                                        {
                                            Items = new Items
                                            {
                                                Item = new[]
                                                {
                                                    new Item { Key = "ExampleKey", Value = "ExampleValue" }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        };

                        documents.Add(document);
                    }
                }
            }

            return documents;
        }

        public List<Pacs003Document> MapPacs003Data()
        {
            var documents = new List<Pacs003Document>();

            using (var connection = _databaseManager.CreateConnection())
            {
                connection.Open();
                var query = @"
                    SELECT 
                        MsgId, CreDtTm, NbOfTxs, TtlIntrBkSttlmAmt, IntrBkSttlmDt, 
                        SttlmInf, SttlmMtd, ClrSys, Prtry, PmtTpInf, CtgyPurp, 
                        InstgAgt, FinInstnId, BICFI, BrnchId, InstdAgt, 
                        PmtId, EndToEndId, TxId, IntrBkSttlmAmt, 
                        SttlmPrty, ChrgBr, DrctDbtTx, MndtId, 
                        FrstColltnDt, Cdtr, Dbtr, DbtrAcct 
                    FROM 
                        fexint_clearing_dbs.dbo.pacs3valid;";

                using (var command = new SqlCommand(query, connection))
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var document = new Pacs003Document
                        {
                            DirectDebit = new FiToFiCstmrDrctDbt
                            {
                                GroupHeader = new GroupHeader
                                {
                                    MessageId = reader["MsgId"].ToString(),
                                    CreationDateTime = Convert.ToDateTime(reader["CreDtTm"]),
                                    NumberOfTransactions = Convert.ToInt32(reader["NbOfTxs"]),
                                    TotalInterbankSettlementAmount = new Amount
                                    {
                                        Currency = "EUR", // Example currency
                                        Value = Convert.ToDecimal(reader["TtlIntrBkSttlmAmt"])
                                    },
                                    InterbankSettlementDate = Convert.ToDateTime(reader["IntrBkSttlmDt"]),
                                    SettlementInformation = new SettlementInformation
                                    {
                                        SettlementMethod = reader["SttlmMtd"].ToString(),
                                        ClearingSystem = new ClearingSystem
                                        {
                                            Proprietary = reader["ClrSys"].ToString()
                                        }
                                    },
                                    InstructingAgent = new FinancialInstitution
                                    {
                                        FinancialInstitutionId = new FinancialInstitutionIdentification
                                        {
                                            Bicfi = reader["BICFI"].ToString()
                                        },
                                        BranchId = new BranchIdentification
                                        {
                                            Id = reader["BrnchId"].ToString()
                                        }
                                    },
                                    InstructedAgent = new FinancialInstitution
                                    {
                                        FinancialInstitutionId = new FinancialInstitutionIdentification
                                        {
                                            Bicfi = reader["InstdAgtBICFI"].ToString() // Example for instructed agent
                                        }
                                    }
                                },
                                DirectDebitTransactions = new[]
                                {
                                    new DirectDebitTransactionInformation
                                    {
                                        PaymentIdentification = new PaymentIdentification
                                        {
                                            InstructionId = reader["PmtId"].ToString(),
                                            EndToEndId = reader["EndToEndId"].ToString()
                                        },
                                        InterbankSettlementAmount = new Amount
                                        {
                                            Currency = "EUR",
                                            Value = Convert.ToDecimal(reader["IntrBkSttlmAmt"])
                                        },
                                        SettlementPriority = reader["SttlmPrty"].ToString(),
                                        ChargeBearer = reader["ChrgBr"].ToString(),
                                        DirectDebitTransaction = new DirectDebitTransaction
                                        {
                                            MandateRelatedInformation = new MandateRelatedInformation
                                            {
                                                MandateId = reader["MndtId"].ToString(),
                                                FirstCollectionDate = Convert.ToDateTime(reader["FrstColltnDt"])
                                            }
                                        },
                                        Creditor = new Party
                                        {
                                            Name = reader["Cdtr"].ToString()
                                        },
                                        Debtor = new Party
                                        {
                                            Name = reader["Dbtr"].ToString()
                                        },
                                        DebtorAccount = new Account
                                        {
                                            Id = new AccountIdentification
                                            {
                                                Other = new OtherAccountIdentification
                                                {
                                                    Id = reader["DbtrAcct"].ToString()
                                                }
                                            }
                                        },
                                        Purpose = new Purpose
                                        {
                                            Proprietary = reader["CtgyPurp"].ToString()
                                        },
                                        RemittanceInformation = new RemittanceInformation
                                        {
                                            Unstructured = reader["RmtInf"].ToString()
                                        },
                                        SupplementaryData = new SupplementaryData
                                        {
                                            Envelope = new Envelope
                                            {
                                                SupplementaryData = new SupplementaryDataContent
                                                {
                                                    Items = new Items
                                                    {
                                                        Item = new[]
                                                        {
                                                            new Item { Key = "ExampleKey", Value = "ExampleValue" }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        };

                        documents.Add(document);
                    }
                }
            }

            return documents;
        }

    }
}