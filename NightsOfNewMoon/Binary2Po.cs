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
using System.Collections.Generic;
using System.Text;
using Yarhl.FileFormat;
using Yarhl.IO;
using Yarhl.Media.Text;

namespace NightsOfNewMoon
{
    public class Binary2Po : IConverter<BinaryFormat, Po>
    {
        public byte Game { get; set; }
        public string GameName { get; set; }
        private bool DictionaryEnabled { get; set; }
        public Dictionary<byte, string> Characters { get; set; }

        public Binary2Po()
        {
            Characters = new Dictionary<byte, string>();
        }

        public Po Convert(BinaryFormat source)
        {
            GenerateCharacterName(Game); //Generate the name dictionary

            Po po = new Po
            {
                Header = new PoHeader(GameName, "dummy@dummy.com", "en-US")
            };

            var reader = new DataReader(source.Stream)
            {
                DefaultEncoding = new UTF8Encoding(),
                Endianness = EndiannessMode.LittleEndian,
            };

            int stringsNumber = reader.ReadInt32(); //Read the number of strings on the file

            for (int i = 0; i < stringsNumber; i++)
            {
                PoEntry entry = new PoEntry(); //Generate the entry on the po file
                byte[] blockSentence = reader.ReadBytes(0x24); //Read the block
                int sentenceSize = BitConverter.ToInt32(blockSentence, 0x20); //Get the sentence size
                entry.ExtractedComments = CheckName(blockSentence[12]); //Get the speaking character
                entry.Original = GenerateString(Encoding.UTF8.GetString(reader.ReadBytes(sentenceSize - 1))); //Add the string block
                entry.Context = i.ToString(); //Context
                entry.Reference = GenerateHeaderString(blockSentence); //Export the game block on a string
                if (Game == 0 || Game == 1) //Nights of Azure/Nigths of Azure 2
                    reader.Stream.Position += 0x5; //Skip padding
                else if (Game == 2 || Game == 3) //Blue Reflection/Atelier Sophie: The Alchemist of the Mysterious Book
                    reader.Stream.Position += 0x1; //Skip padding
                po.Add(entry);
            }

            return po;
        }

        private string CheckName(byte chara)
        {
            var sb = new StringBuilder();
            if (DictionaryEnabled)
            {
                if (Characters.TryGetValue(chara, out string result))
                {
                    return result;
                }
                else
                {
                    return sb.Append($"{{{chara:X2}") + "}";
                }
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

        private string GenerateHeaderString(byte[] array)
        {
            string result = "";
            var sb = new StringBuilder();
            foreach (byte bytes in array)
            {
                result += "0x" + sb.Append($"{bytes:X1}") + "|";
                sb.Clear();
            }
            return result;
        }

        private void GenerateCharacterName(int game)
        {
            string file = "";
            switch (game)
            {
                case 0:
                    file = "NOA.map";
                    break;

                case 1:
                    file = "BF.map";
                    break;
            }
            if (System.IO.File.Exists(file))
            {
                try
                {
                    string[] dictionary = System.IO.File.ReadAllLines(file);
                    foreach (string line in dictionary)
                    {
                        string[] lineFields = line.Split('=');
                        Characters.Add(System.Convert.ToByte(lineFields[0], 16), lineFields[1]);
                    }
                }
                catch (Exception e)
                {
                    Console.Beep();
                    Console.WriteLine("The dictionary is wrong, please, check the readme and fix it.");
                    Console.WriteLine(e);
                    System.Environment.Exit(-1);
                }
                DictionaryEnabled = true;
            }
            else
            {
                DictionaryEnabled = false;
            }
        }
    }
}