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

namespace Alucard
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            Console.WriteLine("Alucard 1.2 - A simple tool to translate Gust games by Darkmet98.");
            Console.WriteLine("Thanks to Pleonex for Yarhl libraries and xml.e decryption (SiA)," +
                              " Nex for the Yarhl node implementation " +
                              "and Kaplas80 for disable the xml encryption on NOA executable.");
            Console.WriteLine("This program is licensed with a GPL V3 license.");
            
            if (args.Length != 3 && args.Length != 2 && args.Length != 1) 
                Info();

            Check(args[1]);
            var command = new Commands((args.Length == 3)? args[2]: "DEFAULT");

            switch (args[0].ToUpper())
            {
                case "-EXPORT":
                    command.ExportEbm(args[1]);
                    break;
                case "-EXPORT_FOLDER":
                    command.ExportEbmFolder(args[1]);
                    break;
                case "-IMPORT":
                    command.ImportEbm(args[1]);
                    break;
                case "-IMPORT_FOLDER":
                    command.ImportEbmFolder(args[1]);
                    break;
                case "-EXPORT_XML":
                   command.DecryptXml(args[1]);
                    break;
                case "-EXPORT_XMLFOLDER":
                   command.DecryptXmlFolder(args[1]);
                    break;
                case "-PATCHEXE":
                   command.PathExe(args[1]);
                    break;
                default:
                    Console.WriteLine("Error, the option has you entered is incorrect.");
                    break;
            }
        }

        private static void Check(string path)
        {
            if (!string.IsNullOrWhiteSpace(path))
            {
                if (File.Exists(path)) 
                    return;
                
                var attr = File.GetAttributes(path);

                if (attr.HasFlag(FileAttributes.Directory)) 
                    return;

                Console.WriteLine("Error, the file doesn't exist.");
                Console.Beep();
                Environment.Exit(-1);
            }
            else
            {
                Console.WriteLine("Error, the path is empty.");
                Console.Beep();
                Environment.Exit(-1);
            }
           
        }

        private static void Info()
        {
            Console.WriteLine("USAGE: Alucard <-export/-export_folder/-import/-import_folder/-export_XML/-export_XMLFolder/-PatchExe> file/folder <NOA/NOA2/BR/ATSO>");
            Console.WriteLine("Export to PO example: Alucard -export EVENT_MESSAGE_MM00_OP1_010.ebm NOA");
            Console.WriteLine("Export folder to PO example: Alucard -export_folder MM02_CP02 NOA2");
            Console.WriteLine("Import PO example: Alucard -import EVENT_MESSAGE_MM00_OP1_010.po NOA");
            Console.WriteLine("Import folder to Po example: Alucard -import_folder MM02_CP02 NOA2");
            Console.WriteLine("Export XML.e example: Alucard -export_XML AbilityData.xml.e");
            Console.WriteLine("Export XML.e folder example: Alucard -export_XMLFolder Saves");
            Console.WriteLine("Patch Nights Of Azure Executable to load decrypted xml.e: Alucard -PatchExe CNN.exe NOA");
            Environment.Exit(-1);
        }
    }
}
 