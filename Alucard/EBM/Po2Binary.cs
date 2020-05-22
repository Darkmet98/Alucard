// Copyright (C) 2019 Pedro Garau Martínez
//
// This file is part of Alucard.
//
// Alucard is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// Alucard is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with Alucard. If not, see <http://www.gnu.org/licenses/>.
//
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Yarhl.FileFormat;
using Yarhl.IO;
using Yarhl.Media.Text;

namespace Alucard
{
    public class Po2EBMBinary : IConverter<Po, BinaryFormat>
    {
        public byte Game { get; set; }
        private bool DictionaryEnabled { get; set; }
        private Dictionary<string, string> FontChara { get; }
        public Po2EBMBinary()
        {
            FontChara = new Dictionary<string, string>();
        }

        public BinaryFormat Convert(Po source)
        {
            GenerateFontMap(Game);

            BinaryFormat binary = new BinaryFormat();
            var writer = new DataWriter(binary.Stream);
            writer.Write((int)source.Entries.Count); //Write the number of blocks
            foreach (var entry in source.Entries)
            {
                //Generate the block
                writer.Write(GenerateEBMBlock(entry.Reference));

                //Generate the text to insert
                string potext = string.IsNullOrEmpty(entry.Translated) ?
                entry.Original : entry.Translated;
                potext = ParseString(potext);

                //Get the text array
                byte[] stringblock = Encoding.UTF8.GetBytes(potext);

                //Write the size
                writer.Stream.Position = writer.Stream.Position - 4;
                writer.Write((int)stringblock.Length + 1);

                //Write the text
                writer.Write(stringblock);
                if (Game == 0 || Game == 1) //Nights of Azure/Nigths of Azure 2
                    writer.WriteTimes(0, 5);
                else if (Game == 2 || Game == 3) //Blue Reflection/Atelier Sophie: The Alchemist of the Mysterious Book
                    writer.WriteTimes(0, 1);
            }
            return new BinaryFormat(binary.Stream);
        }

        private byte[] GenerateEBMBlock(string array)
        {
            string[] blockstring = array.Remove(array.Length - 1).Split('|');
            byte[] result = new byte[0x24];
            int i = 0;
            foreach (string bytes in blockstring)
            {
                result[i] = Byte.Parse(bytes.Substring(2), NumberStyles.HexNumber);
                i++;
            }

            return result;
        }

        public string ParseString(string text, bool isXml=false)
        {
            var result = text;
            if (!isXml)
                result = result.Replace("\n", "<CR>");

            if (DictionaryEnabled)
            {
                foreach (var replace in FontChara)
                    result = result.Replace(replace.Value, replace.Key);
            }
            if (result == "<!empty>")
                result = string.Empty;
            return result;
        }

        public void GenerateFontMap(int game)
        {
            string file = "";
            switch (game)
            {
                case 0: //Nights Of Azure
                    file = "FONT_NOA.map";
                    break;

                case 1: //Nights Of Azure 2
                    file = "FONT_NOA2.map";
                    break;
                case 2: //Blue Reflection
                    file = "FONT_BR.map";
                    break;
                case 3: //Atelier Sophie: The Alchemist of the Mysterious Book
                    file = "FONT_ATSO.map";
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
                        FontChara.Add(lineFields[0], lineFields[1]);
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