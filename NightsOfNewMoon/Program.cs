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
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NightsOfNewMoon
{
    class Program
    {
        static void Main(string[] args)
        {
            var Exp = new ebm.Export();
            
            Console.WriteLine("NightsOfNewMoon 1.0 - A simple ebm to po converter from the game Nights of Azure and Nights of Azure 2 by Darkmet98.");
            Console.WriteLine("Thanks to Pleonex for the help and Yarhl libraries.");
            if (args.Length != 3 && args.Length != 2)
            {
                Console.WriteLine("USAGE: [mono] TheNewMoon.exe -export/-import file ");
                Console.WriteLine("Export to Po example: TheNewMoon.exe -export EVENT_MESSAGE_MM00_OP1_010.ebm ");
                Console.WriteLine("Import Po example: TheNewMoon.exe -import EVENT_MESSAGE_MM00_OP1_010.po ");
                Console.WriteLine("Read first the readme file before use this program!");
                Environment.Exit(-1);
            }

            switch(args[0])
            {
                case "-export":
                    Exp.GeneratePo(args[1], 0);
                    break;

                case "-import":

                    break;

                case "-credits":
                    Console.WriteLine("Pleonex: Yarhl Libraries.");
                    break;
                default:
                    Console.WriteLine("Error, the option has you entered is incorrrect.");
                    break;
            }
        }
    }
}
