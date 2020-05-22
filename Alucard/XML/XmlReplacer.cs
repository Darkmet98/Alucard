using System.IO;
using System.Text;
using Yarhl.IO;
using TextReader = Yarhl.IO.TextReader;

namespace Alucard.XML
{
    class XmlReplacer
    {
        private byte game;

        public XmlReplacer(byte gameVal)
        {
            game = gameVal;
        }
        public void Convert(string file)
        {
            // Make sure that the shift-jis encoding is initialized in
            // .NET Core
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            //Check the dictionary
            var po2Binary = new Po2EBMBinary();
            po2Binary.GenerateFontMap(game);

            var reader = new TextReader(DataStreamFactory.FromFile(file, FileOpenMode.Read), GetEncoding(file))
            {
                NewLine = "\r\n"
            };

            var text = po2Binary.ParseString(reader.ReadToEnd(), true);
            reader.Stream.Dispose();

            var writer = new Yarhl.IO.TextWriter(DataStreamFactory.FromFile(
                Path.GetFileNameWithoutExtension(file) + "_exp.e",
                    FileOpenMode.Write), Encoding.GetEncoding("shift_jis"))
            {
                NewLine = "\r\n"
            };

            writer.Write(text);
            writer.Stream.Dispose();
        }


        //https://stackoverflow.com/questions/3825390/effective-way-to-find-any-files-encoding
        /// <summary>
        /// Determines a text file's encoding by analyzing its byte order mark (BOM).
        /// Defaults to ASCII when detection of the text file's endianness fails.
        /// </summary>
        /// <param name="filename">The text file to analyze.</param>
        /// <returns>The detected encoding.</returns>
        public static Encoding GetEncoding(string filename)
        {

            // Read the BOM
            var bom = new byte[4];
            using (var file = new FileStream(filename, FileMode.Open, FileAccess.Read))
            {
                file.Read(bom, 0, 4);
            }

            // Analyze the BOM
            if (bom[0] == 0x2b && bom[1] == 0x2f && bom[2] == 0x76) return Encoding.UTF7;
            if (bom[0] == 0xef && bom[1] == 0xbb && bom[2] == 0xbf) return Encoding.UTF8;
            if (bom[0] == 0xff && bom[1] == 0xfe && bom[2] == 0 && bom[3] == 0) return Encoding.UTF32; //UTF-32LE
            if (bom[0] == 0xff && bom[1] == 0xfe) return Encoding.Unicode; //UTF-16LE
            if (bom[0] == 0xfe && bom[1] == 0xff) return Encoding.BigEndianUnicode; //UTF-16BE
            if (bom[0] == 0 && bom[1] == 0 && bom[2] == 0xfe && bom[3] == 0xff) return new UTF32Encoding(true, true);  //UTF-32BE

            // We actually have no idea what the encoding is if we reach this point, so
            // you may wish to return null instead of defaulting to SJIS (only for NOA)
            return Encoding.GetEncoding("shift_jis");
        }
    }
}
