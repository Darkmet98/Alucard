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
using System.IO;
using Yarhl.FileFormat;
using Yarhl.FileSystem;
using Yarhl.Media.Text;

namespace NightsOfNewMoon
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            Console.WriteLine("NightsOfNewMoon 1.0 - A simple ebm to po converter from the game Nights of Azure by Darkmet98.");
            if (args.Length != 3 && args.Length != 2 && args.Length != 1)
            {
                Console.WriteLine("USAGE: [mono] TheNewMoon.exe <-export/-export_folder/-import/-import_folder/-credits> file/folder ");
                Console.WriteLine("Export to Po example: TheNewMoon.exe -export EVENT_MESSAGE_MM00_OP1_010.ebm ");
                Console.WriteLine("Export folder to Po example: TheNewMoon.exe -export_folder MM02_CP02");
                Console.WriteLine("Import Po example: TheNewMoon.exe -import EVENT_MESSAGE_MM00_OP1_010.po ");
                Console.WriteLine("Import folder to Po example: TheNewMoon.exe -import_folder MM02_CP02");
                Environment.Exit(-1);
            }

            switch (args[0])
            {
                case "-export":
                    if (File.Exists(args[1])) {
                        // 1
                        Node nodo = NodeFactory.FromFile(args[1]); // BinaryFormat

                        // 2
                        Binary2Po converter = new Binary2Po
                        {
                            Game = 0
                        };

                        Node nodoPo = nodo.Transform<BinaryFormat, Po>(converter);
                        //3
                        Console.WriteLine("Exporting " + args[1] + "...");

                        string file = args[1].Remove(args[1].Length - 4);

                        nodoPo.Transform<Po2Binary, Po, BinaryFormat>()
                        .Stream.WriteTo(file + ".po");
                    }
                    else
                    {
                        Console.WriteLine("Error, the file doesn't exist.");
                        Console.Beep();
                    }
                    break;

                case "-export_folder":
                    if (Directory.Exists(args[1]))
                    {
                        // 1
                        Node folder = NodeFactory.FromDirectory(args[1], "*.ebm"); // BinaryFormat

                        foreach (Node child in folder.Children)
                        {
                            // 2
                            Binary2Po converter = new Binary2Po
                            {
                                Game = 0
                            };
                            Node nodePo = child.Transform<BinaryFormat, Po>(converter);
                            //3
                            Console.WriteLine("Exporting " + child.Name + "...");
                            nodePo.Transform<Po2Binary, Po, BinaryFormat>()
                            .Stream.WriteTo(Path.Combine(args[1], child.Name.Remove(child.Name.Length - 4) + ".po"));
                        }
                    }
                    else
                    {
                        Console.WriteLine("Error, the folder doesn't exist.");
                        Console.Beep();
                    }
                    break;

                case "-import":
                    if (File.Exists(args[1]))
                    {
                        // 1
                        Node nodo = NodeFactory.FromFile(args[1]); // Po

                        // 2
                        Po2EBMBinary pogenerator = new Po2EBMBinary
                        {
                            Game = 0
                        };

                        nodo.Transform<Po2Binary, BinaryFormat, Po>();
                        Node nodoEbm = nodo.Transform<Po, BinaryFormat>(pogenerator);
                        //3
                        Console.WriteLine("Importing " + args[1] + "...");
                        string file = args[1].Remove(args[1].Length - 3);
                        nodoEbm.Stream.WriteTo(file + ".ebm");
                    }
                    else
                    {
                        Console.WriteLine("Error, the file doesn't exist.");
                        Console.Beep();
                    }
                    break;

                case "-credits":
                    Console.WriteLine("Pleonex: Yarhl Libraries.");
                    Console.WriteLine("Nex: Yarhl node implementation.");
                    break;

                default:
                    Console.WriteLine("Error, the option has you entered is incorrrect.");
                    break;
            }
        }
    }
}