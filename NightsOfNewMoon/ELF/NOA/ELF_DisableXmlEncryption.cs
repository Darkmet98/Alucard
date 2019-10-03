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


/*
 * Thanks to Kaplas80 for disable the encryption from the NOA executable
 */
using Yarhl.IO;

namespace NightsOfNewMoon.ELF.NOA
{
    class ELF_DisableXmlEncryption
    {
        private DataStream Executable { get; }
        private DataWriter Writer { get; set; }
        public ELF_DisableXmlEncryption(DataStream executable)
        {
            Executable = new DataStream();
            executable.WriteTo(Executable);
        }

        public DataStream Patch()
        {
            //Initialize the writer

            Writer = new DataWriter(Executable);

            //Write the patch
            Writer.Stream.Position = 0x3A51F9;
            Writer.Write((byte)0x8B);

            Writer.Stream.Position = 0x3A524D;
            byte[] patchBytes = {0x48, 0x89, 0x7D, 0x38, 0xB0, 0x01, 0x90, 0x90, 0x90, 0x90, 0x90};
            Writer.Write(patchBytes);

            return Writer.Stream;
        }

    }
}
