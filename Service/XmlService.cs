using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using FlexInt.ISOBridge.Data;

namespace FlexInt.ISOBridge.Services
{
    public class XmlGenerationService
    {
        private readonly DatabaseManager _databaseManager;

        public XmlGenerationService(DatabaseManager databaseManager)
        {
            _databaseManager = databaseManager;
        }

        public void GenerateXmlFiles<T>(List<T> documents, string outputDirectory, string messageType)
        {
            var serializer = new XmlSerializer(typeof(T));

            if (!Directory.Exists(outputDirectory))
            {
                Directory.CreateDirectory(outputDirectory);
            }

            foreach (var document in documents)
            {
                var fileName = Path.Combine(outputDirectory, $"{messageType}_{Guid.NewGuid()}.xml");
                try
                {
                    using (var writer = new StreamWriter(fileName))
                    {
                        serializer.Serialize(writer, document);
                    }

                    Console.WriteLine($"Fichier XML généré : {fileName}");
                    //_databaseManager.LogProcessedFileAsync(fileName).Wait();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Erreur lors de la génération du fichier XML : {ex.Message}");
                   // _databaseManager.LogFailedFileAsync(fileName, ex.Message).Wait();
                }
            }
        }
    }
}

/*
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace FlexInt.ISOBridge.V1.Services
{
    public class XmlService
    {
        public void GenerateXmlFiles<T>(List<T> documents, string outputDirectory, string messageType)
        {
            var serializer = new XmlSerializer(typeof(T));

            if (!Directory.Exists(outputDirectory))
            {
                Directory.CreateDirectory(outputDirectory);
            }

            foreach (var document in documents)
            {
                var fileName = Path.Combine(outputDirectory, $"{messageType}_{Guid.NewGuid()}.xml");
                try
                {
                    using (var writer = new StreamWriter(fileName))
                    {
                        serializer.Serialize(writer, document);
                    }

                    Console.WriteLine($"Fichier XML généré : {fileName}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Erreur lors de la génération du fichier XML : {ex.Message}");
                }
            }
        }
    }
}
*/