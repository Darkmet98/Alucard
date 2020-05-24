using System;
using System.Collections.Generic;
using System.Text;
using Yarhl.FileFormat;
using Yarhl.IO;
using Yarhl.Media.Text;

namespace Alucard.ELF
{
    public class ExportElf_common : IConverter<BinaryFormat, Po>
    {
        private List<long> positions;
        private List<string> text;
        private DataReader reader;
        private Po po;

        public Po Convert(BinaryFormat source)
        {
            // Make sure that the shift-jis encoding is initialized in
            // .NET Core
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            reader = new DataReader(source.Stream);
            text = new List<string>();
            positions = new List<long>();

            po = new Po
            {
                Header = new PoHeader("Nights of azure", "dummy@dummy.com", "es-ES")
            };

            SearchPositions(0x903880, 0xB00); // Names
            SearchPositions(0xA35B80, 0x1230); // Extras
            SearchPositions(0x9B7E40, 0x18F08); // Conversation 1
            SearchPositions(0x9E7FB8, 0xA4A8); // Conversation 2
            GeneratePo();

            return po;
        }

        private Encoding CheckEncoding(byte text)
        {
            if(text == 0x81 || text == 0x82 || text == 0x83)
                return Encoding.GetEncoding("shift_jis");

            return Encoding.UTF8;
        }

        private void DumpText(long position, int size = 0)
        {
            reader.Stream.PushCurrentPosition();

            reader.Stream.Position = position - 0x140001C00;
            var encoding = CheckEncoding(reader.ReadByte());
            reader.Stream.Position--;

            text.Add((size == 0 ? reader.ReadString(encoding)
                : reader.ReadString(size, encoding)).Replace("<CR>", "\n"));

            reader.Stream.PopPosition();
        }

        private void GeneratePo()
        {
            for (var i = 0; i < text.Count; i++)
            {
                var entry = new PoEntry
                {
                    Original = string.IsNullOrEmpty(text[i])?"<!empty>":text[i],
                    Context = i.ToString(),
                    Reference = positions[i].ToString()
                };

                po.Add(entry);
            }
        }

        private bool SanitizeData(long data)
        {
            //Negative or null value
            if (data <= 0)
                return false;
            //Low value
            if (data < 10000000)
                return false;
            //Big value
            if (data > 0x200000000)
                return false;
            //Null pointer
            if (data == 0x00000001ffffffff)
                return false;
            //Negative pointer
            if ((data - 0x140001C00) < 0)
                return false;
            //Null string - NOA
            if (data == 5378075960)
                return false;

            return true;
        }

        private void SearchPositions(long position, int size)
        {
            reader.Stream.Position = position;
            var sizeSplit = size / 8;

            for (int i = 0; i < sizeSplit; i++)
            {
                //read the data
                var data = reader.ReadInt64();

                //Check the data
                if(!SanitizeData(data))
                    continue;

                positions.Add(reader.Stream.Position - 0x8);
                DumpText(data);
            }

        }
    }
}
