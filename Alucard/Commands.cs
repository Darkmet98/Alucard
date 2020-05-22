// Copyright (C) 2020 Pedro Garau Martínez
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
using System.IO;
using Alucard.ELF.NOA;
using Alucard.XML;
using SiA.Library;
using Yarhl.FileSystem;
using Yarhl.IO;
using Yarhl.Media.Text;

namespace Alucard
{
    class Commands
    {
        byte gameCode;
        string gameName;

        public Commands(string game)
        {
            switch (game.ToUpper())
            {
                case "NOA": //Nigts of Azure
                case "DEFAULT": //Import mode
                    gameCode = 0;
                    gameName = "Nights Of Azure";
                    break;
                case "NOA2": //Nigths Of Azure 2
                    gameCode = 1;
                    gameName = "Nights Of Azure 2";
                    break;
                case "BR": //Blue Reflection
                    gameCode = 2;
                    gameName = "Blue Reflection";
                    break;
                case "ATSO": //Atelier Sophie: The Alchemist of the Mysterious Book
                    gameCode = 3;
                    gameName = "Atelier Sophie: The Alchemist of the Mysterious Book";
                    break;
                default:
                    Console.WriteLine("Error, the game doesn't exists.");
                    Console.Beep();
                    Environment.Exit(-1);
                    break;
            }
        }
        public void ExportEbm(string file)
        {
            Console.WriteLine($"Exporting {file}...");

            var nodoPo = NodeFactory.FromFile(file).
                TransformWith(Binary2Po());

            nodoPo.TransformWith<Po2Binary>()
                .Stream.WriteTo(Path.GetFileNameWithoutExtension(file) + ".po");
        }

        public void ExportEbmFolder(string folderPath)
        {
            var folder = NodeFactory.FromDirectory(folderPath, "*.ebm", "out", true);

            foreach (var folderChild in folder.Children)
            {
                foreach (var child in folderChild.Children)
                {
                    Console.WriteLine($"Exporting {child.Name}...");

                    var nodoPo = child.TransformWith(Binary2Po());

                    var path = AppDomain.CurrentDomain.BaseDirectory + folderChild.Path + "/" +
                               Path.GetFileNameWithoutExtension(child.Name) + ".po";

                    nodoPo.TransformWith<Po2Binary>()
                        .Stream.WriteTo(path);
                }
            }
        }

        public void DecryptXml(string path)
        {
            Console.WriteLine($"Decrypting {path}...");

            var file = NodeFactory.FromFile(path).TransformWith<Decrypter>();
            var name = Path.GetFileNameWithoutExtension(path) + "_dec.e";

            file.Stream.WriteTo(name);
        }

        public void DecryptXmlFolder(string folderPath)
        {
            var folder = NodeFactory.CreateContainer("XML");


            string[] folderXml = Directory.GetFiles(folderPath,
                "*.xml.e",
                SearchOption.AllDirectories);


            foreach (string file in folderXml)
            {
                var nodo = NodeFactory.FromFile(file);
                folder.Add(nodo);
            }

            if (!Directory.Exists("Decrypted"))
                Directory.CreateDirectory("Decrypted");

            foreach (Node child in folder.Children)
            {
                Console.WriteLine($"Decrypting {child.Name}...");
                
                child.TransformWith<Decrypter>().
                    Stream.WriteTo("Decrypted/" + SearchFile(folderXml, child.Name));
            }
        }

        public void ImportEbm(string file)
        {
            Console.WriteLine($"Importing {file}...");

            var nodo = NodeFactory.FromFile(file).
                TransformWith<Po2Binary>();

            nodo.TransformWith(Po2EbmBinary())
                .Stream.WriteTo(Path.GetFileNameWithoutExtension(file)
                                + "_new.ebm");
        }

        public void ImportEbmFolder(string folderPath)
        {
            var folder = NodeFactory.FromDirectory(folderPath, "*.po", "out", true);

            foreach (var folderChild in folder.Children)
            {
                foreach (var child in folderChild.Children)
                {
                    Console.WriteLine($"Importing {child.Name}...");

                    var nodoEbm = child.TransformWith<Po2Binary>().
                        TransformWith(Po2EbmBinary());

                    var path = AppDomain.CurrentDomain.BaseDirectory + folderChild.Path + "/" +
                               Path.GetFileNameWithoutExtension(child.Name) + ".ebm";

                    nodoEbm.Stream.WriteTo(path);
                }

            }
        }

        public void PathExe(string path)
        {
            if(gameCode != 0) throw new NotSupportedException("This function only works with Nights Of Azure for now.");

            Console.WriteLine($"Patching {path}...");
            var fileStream = DataStreamFactory.FromFile(path, FileOpenMode.ReadWrite);
            var patch = new ELF_DisableXmlEncryption(fileStream);
            var result = patch.Patch();
            var fileName = Path.GetFileNameWithoutExtension(path) + "_new.exe";
            result.WriteTo(fileName);
        }

        public void PathXml(string path)
        {
            Console.WriteLine($"Patching {path}...");
            var xml = new XmlReplacer(gameCode);
            xml.Convert(path);
        }

        private Binary2Po Binary2Po()
        {
            return new Binary2Po
            {
                Game = gameCode,
                GameName = gameName
            };
        }

        private Po2EBMBinary Po2EbmBinary()
        {
            return new Po2EBMBinary
            {
                Game = gameCode
            };
        }

        private string SearchFile(string[] array, string file)
        {
            foreach (var result in array)
            {
                if (result.Contains(file))
                    return result;
            }

            return "";
        }
    }
}
