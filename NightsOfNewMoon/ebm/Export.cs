// Copyright (C) 2019 Pedro Garau Martínez
//
// This file is part of NightsOfNewMoon.
//
// NightsOfNewMoon is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// NightsOfNewMoon is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with NightsOfNewMoon. If not, see <http://www.gnu.org/licenses/>.
//

using System;
using Yarhl;
using Yarhl.IO;
using Yarhl.FileFormat;
using Yarhl.FileSystem;
using Yarhl.Media;
using Yarhl.Media.Text;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


/*
         * --Header
         * Int32 = Nº entries
         * 
         * --Block
         * Int32 - Unknown
         * Int32 - Unknown
         * Int32 - Unknown
         * Int32 - Unknown
         * Int32 - Unknown
         * Int32 - Character
         * Int32 - Unknown
         * Int32 - Unknown
         * Int32 - Size
         * String = Size-1
         * Padding Int32
*/

namespace NightsOfNewMoon.ebm
{
    public class GameSentences : Format
    {
        public Dictionary<byte, string> Characters { get; set; }
        public byte[] BlockSentence { get; set; }
        public Int32 Count { get; set; }
        public byte SentenceCharacterName { get; set; }
        public String Sentence { get; set; }
        public Int32 SentenceSize { get; set; }
        //Thanks Pleonex
        public Dictionary<byte, string> GenerateCharacterName(int game)
        {
            string file = "";
            switch (game) {
                case 0:
                    file = "NOA.map";
                    break;
                case 1:
                    file = "NOA2.map";
                    break;
            }
            try {
                Characters = new Dictionary<byte, string>();
                string[] dictionary = System.IO.File.ReadAllLines(file);
                foreach (string line in dictionary)
                {
                    string[] lineFields = line.Split('=');
                    Characters.Add(Convert.ToByte(lineFields[0], 16), lineFields[1]);
                }
            }
            catch {
                Console.WriteLine("The dictionary is wrong, please, check the readme and fix it.");
                System.Environment.Exit(-1);
            }
            return Characters;
        }

        //Thanks Pleonex
        //Encoding for the string
        public byte[] Encode(string text, Dictionary<char, byte[]> chararemap)
        {
            List<byte> encoded = new List<byte>();
            foreach (var chr in text) {
                // If chr is in the dict, append to the list the bytes from the dict
                if (chararemap.ContainsKey(chr)) {
                    encoded.AddRange(chararemap[chr]);
                }
                // Else, append to the list the bytes from encoding the char with utf8
                else {
                    byte[] stringtext = Encoding.UTF8.GetBytes(new char[] { chr });
                    encoded.AddRange(stringtext);
                }
            }
            return encoded.ToArray();
        }

    }

    class Export
    {
        private GameSentences Game = new GameSentences();
        public void GeneratePo(string file, int game)
        {
            Game.Characters = Game.GenerateCharacterName(game); //Generate the name dictionary

            Po po = new Po
            {
                Header = new PoHeader("Nights Of Azure", "mail@sample.es", "es")
                {
                    LanguageTeam = "",
                }
            };

            using (var stream = new DataStream(file, FileOpenMode.Read)) {
                var reader = new DataReader(stream) {
                    DefaultEncoding = new UTF8Encoding(),
                    Endianness = EndiannessMode.LittleEndian,
                };

                Game.Count = reader.ReadInt32(); //Read the number of strings on the file

                for (int i = 0; i < Game.Count; i++) {
                    PoEntry entry = new PoEntry(); //Generate the entry on the po file
                    Game.BlockSentence = reader.ReadBytes(0x24); //Read the block
                    Game.SentenceSize = BitConverter.ToInt32(Game.BlockSentence, 0x20); //Get the sentence size
                    entry.ExtractedComments = CheckName(Game.BlockSentence[20]); //Get the speaking character
                    entry.Original = GenerateString(Encoding.UTF8.GetString(reader.ReadBytes(Game.SentenceSize - 1))); //Add the string block
                    entry.Context = i.ToString();
                    reader.Stream.Position = reader.Stream.Position + 0x5; //Skip padding
                    po.Add(entry);
                }
                po.ConvertTo<BinaryFormat>().Stream.WriteTo(file + ".po");
            }
        }

        private string CheckName(byte chara)
        {
            var sb = new StringBuilder();
            if (Game.Characters.TryGetValue(chara, out string result))
            {
                return result;
            }
            else
            {
                return sb.Append($"{{{chara:X2}") + "}";
            }
        }

        private string GenerateString(string Sentence)
        {
            string result = Sentence;
            if (string.IsNullOrEmpty(result))
                result = "<!empty>";
            result = result.Replace("<CR>", "\n");
            return result;
        }
    }
}
