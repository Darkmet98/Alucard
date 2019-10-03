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
using NightsOfNewMoon.ELF.NOA;
using Yarhl.FileFormat;
using Yarhl.FileSystem;
using Yarhl.IO;
using Yarhl.Media.Text;

namespace NightsOfNewMoon
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            Console.WriteLine("NightsOfNewMoon 1.1 - A simple tool to translate Gust games by Darkmet98.");
            Console.WriteLine("Thanks to Pleonex for Yarhl libraries and xml.e decryption (SiA), Nex for the Yarhl node implementation and Kaplas80 for disable the xml decryption on NOA executable.");
            Console.WriteLine("This program is licensed with a GPL V3 license.");
            if (args.Length != 3 && args.Length != 2 && args.Length != 1)
            {
                Console.WriteLine("USAGE: NightsOfNewMoon <-export/-export_folder/-import/-import_folder/-credits> file/folder <NOA/NOA2/BR/ATSO>");
                Console.WriteLine("Export to POT example: NightsOfNewMoon,exe -export EVENT_MESSAGE_MM00_OP1_010.ebm NOA");
                Console.WriteLine("Export folder to POT example: NightsOfNewMoon.exe -export_folder MM02_CP02 NOA2");
                Console.WriteLine("Import PO example: NightsOfNewMoon.exe -import EVENT_MESSAGE_MM00_OP1_010.po NOA");
                Console.WriteLine("Import folder to Po example: NightsOfNewMoon.exe -import_folder MM02_CP02 NOA2");
                Environment.Exit(-1);
            }
            Byte game = 0;
            String gameName = "";
            switch (args[0])
            {
                case "-export":
                    if (File.Exists(args[1])) {
                        // 1
                        Node nodo = NodeFactory.FromFile(args[1]); // BinaryFormat


                        // 2
                        if (args.Length != 3)
                        {
                            Console.WriteLine("Error, the game doesn't exist.");
                            Console.Beep();
                            Environment.Exit(-1);
                        }
                        switch(args[2].ToUpper())
                        {
                            case "NOA": //Nigts of Azure
                                game = 0;
                                gameName = "Nights Of Azure";
                                break;

                            case "NOA2": //Nigths Of Azure 2
                                game = 1;
                                gameName = "Nights Of Azure 2";
                                break;
                            case "BR": //Blue Reflection
                                game = 2;
                                gameName = "Blue Reflection";
                                break;
                            case "ATSO": //Atelier Sophie: The Alchemist of the Mysterious Book
                                game = 3;
                                gameName = "Atelier Sophie: The Alchemist of the Mysterious Book";
                                break;
                            default:
                                Console.WriteLine("Error, the game doesn't exists.");
                                Console.Beep();
                                Environment.Exit(-1);
                                break;
                        }

                        Binary2Po converter = new Binary2Po
                        {
                            Game = game,
                            GameName = gameName
                        };

                        Node nodoPo = nodo.Transform<BinaryFormat, Po>(converter);
                        //3
                        Console.WriteLine("Exporting " + args[1] + "...");

                        string file = args[1].Remove(args[1].Length - 4);

                        nodoPo.Transform<Po2Binary, Po, BinaryFormat>()
                        .Stream.WriteTo(file + ".pot");
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
                            if (args.Length != 3)
                            {
                                Console.WriteLine("Error, the game doesn't exist.");
                                Console.Beep();
                                Environment.Exit(-1);
                            }
                            switch (args[2].ToUpper())
                            {
                                case "NOA": //Nigts of Azure
                                    game = 0;
                                    gameName = "Nights Of Azure";
                                    break;

                                case "NOA2": //Nigths Of Azure 2
                                    game = 1;
                                    gameName = "Nights Of Azure 2";
                                    break;
                                case "BR": //Blue Reflection
                                    game = 2;
                                    gameName = "Blue Reflection";
                                    break;
                                case "ATSO": //Atelier Sophie: The Alchemist of the Mysterious Book
                                    game = 3;
                                    gameName = "Atelier Sophie: The Alchemist of the Mysterious Book";
                                    break;
                                default:
                                    Console.WriteLine("Error, the game doesn't exists.");
                                    Console.Beep();
                                    Environment.Exit(-1);
                                    break;
                            }

                            Binary2Po converter = new Binary2Po
                            {
                                Game = game,
                                GameName = gameName
                            };
                            Node nodePo = child.Transform<BinaryFormat, Po>(converter);
                            //3
                            Console.WriteLine("Exporting " + child.Name + "...");
                            nodePo.Transform<Po2Binary, Po, BinaryFormat>()
                            .Stream.WriteTo(Path.Combine(args[1], child.Name.Remove(child.Name.Length - 4) + ".pot"));
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
                        if (args.Length != 3)
                        {
                            Console.WriteLine("Error, the game doesn't exist.");
                            Console.Beep();
                            Environment.Exit(-1);
                        }
                        switch (args[2].ToUpper())
                        {
                            case "NOA": //Nigts of Azure
                                game = 0;
                                break;
                            case "NOA2": //Nigths Of Azure 2
                                game = 1;
                                break;
                            case "BR": //Blue Reflection
                                game = 2;
                                break;
                            case "ATSO": //Atelier Sophie: The Alchemist of the Mysterious Book
                                game = 3;
                                break;
                            default:
                                Console.WriteLine("Error, the game doesn't exists.");
                                Console.Beep();
                                Environment.Exit(-1);
                                break;
                        }
                        Po2EBMBinary pogenerator = new Po2EBMBinary
                        {
                            Game = game
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

                case "-import_folder":
                    if (Directory.Exists(args[1]))
                    {
                        // 1

                        Node folder = NodeFactory.FromDirectory(args[1], "*.po"); // Pos

                        foreach (Node child in folder.Children)
                        {
                            // 2
                            if (args.Length != 3)
                            {
                                Console.WriteLine("Error, the game doesn't exist.");
                                Console.Beep();
                                Environment.Exit(-1);
                            }
                            switch (args[2].ToUpper())
                            {
                                case "NOA": //Nigts of Azure
                                    game = 0;
                                    break;
                                case "NOA2": //Nigths Of Azure 2
                                    game = 1;
                                    break;
                                case "BR": //Blue Reflection
                                    game = 2;
                                    break;
                                case "ATSO": //Atelier Sophie: The Alchemist of the Mysterious Book
                                    game = 3;
                                    break;
                                default:
                                    Console.WriteLine("Error, the game doesn't exists.");
                                    Console.Beep();
                                    Environment.Exit(-1);
                                    break;
                            }
                            Po2EBMBinary pogenerator = new Po2EBMBinary
                            {
                                Game = game
                            };

                            Node pofile = child.Transform<Po2Binary, BinaryFormat, Po>();
                            Node nodoEbm = pofile.Transform<Po, BinaryFormat>(pogenerator);

                            //3
                            Console.WriteLine("Exporting " + child.Name + "...");
                            nodoEbm.Stream.WriteTo(Path.Combine(args[1], child.Name.Remove(child.Name.Length - 3) + ".ebm"));
                        }
                    }
                    else
                    {
                        Console.WriteLine("Error, the folder doesn't exist.");
                        Console.Beep();
                    }
                    break;

                case "-PatchExe":
                    if (File.Exists(args[1]))
                    {
                        DataStream fileStream = new DataStream(args[1], FileOpenMode.ReadWrite);
                        ELF_DisableXmlEncryption patch = new ELF_DisableXmlEncryption(fileStream);
                        var result = patch.Patch();
                        result.WriteTo(args[1].Remove(args[1].Length-4)+"_new.exe");
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
 