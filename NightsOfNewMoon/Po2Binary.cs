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
using System.Globalization;
using System.Text;
using Yarhl.FileFormat;
using Yarhl.IO;
using Yarhl.Media.Text;

namespace NightsOfNewMoon
{
    public class Po2EBMBinary : IConverter<Po, BinaryFormat>
    {
        public byte Game { get; set; }

        public Dictionary<string, string> FontChara { get; set; }

        public Po2EBMBinary()
        {
            FontChara = new Dictionary<string, string>();
        }

        public BinaryFormat Convert(Po source)
        {
            GenerateFontMap(Game);
            BinaryFormat binary = new BinaryFormat();
            var writer = new DataWriter(binary.Stream);
            writer.Write((Int32)source.Entries.Count); //Write the number of blocks
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
                writer.Write((Int32)stringblock.Length + 1);

                //Write the text
                writer.Write(stringblock);
                writer.WriteTimes(0, 5);
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

        private String ParseString(string text)
        {
            string result = text.Replace("\n", "<CR>");
            foreach (var replace in FontChara)
                result = result.Replace(replace.Value, replace.Key);
            if (result == "<!empty>")
                result = string.Empty;
            return result;
        }

        private void GenerateFontMap(int game)
        {
            string file = "";
            switch (game)
            {
                case 0:
                    file = "FONT_NOA.map";
                    break;

                case 1:
                    file = "FONT_NOA2.map";
                    break;
            }
            try
            {
                string[] dictionary = System.IO.File.ReadAllLines(file);
                foreach (string line in dictionary)
                {
                    string[] lineFields = line.Split('=');
                    FontChara.Add(lineFields[0], lineFields[1]);
                }
            }
            catch
            {
                Console.WriteLine("The dictionary is wrong, please, check the readme and fix it.");
                System.Environment.Exit(-1);
            }
        }
    }
}