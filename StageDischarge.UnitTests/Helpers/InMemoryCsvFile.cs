using System.Collections.Generic;
using System.IO;
using FileHelpers;
using Server.Plugins.FieldVisit.StageDischarge.Parsers;

namespace Server.Plugins.FieldVisit.StageDischarge.UnitTests.Helpers
{
    class InMemoryCsvFile<TRecordType> where TRecordType : class, ISelfValidator
    {
        private readonly List<TRecordType> _records;

        public InMemoryCsvFile()
        {
            _records = new List<TRecordType>();
        }

        public void AddRecord(TRecordType record)
        {
            _records.Add(record);
        }

        public MemoryStream GetInMemoryCsvFileStream()
        {
            MemoryStream theMemStream = new MemoryStream();
            TextWriter writer = new StreamWriter(theMemStream);
            FileHelperAsyncEngine<TRecordType> engine = new FileHelperAsyncEngine<TRecordType>();
            engine.HeaderText = engine.GetFileHeader();
            engine.BeginWriteStream(writer);
            foreach (TRecordType record in _records)
            {
                engine.WriteNext(record);
            }
            engine.Flush();
            theMemStream.Seek(0, SeekOrigin.Begin);
            return theMemStream;
        }
    }
}
