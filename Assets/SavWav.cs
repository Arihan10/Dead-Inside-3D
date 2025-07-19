//	Copyright (c) 2012 Calvin Rien
//        http://the.darktable.com
//
//	This software is provided 'as-is', without any express or implied warranty. In
//	no event will the authors be held liable for any damages arising from the use
//	of this software.
//
//	Permission is granted to anyone to use this software for any purpose,
//	including commercial applications, and to alter it and redistribute it freely,
//	subject to the following restrictions:
//
//	1. The origin of this software must not be misrepresented; you must not claim
//	that you wrote the original software. If you use this software in a product,
//	an acknowledgment in the product documentation would be appreciated but is not
//	required.
//
//	2. Altered source versions must be plainly marked as such, and must not be
//	misrepresented as being the original software.
//
//	3. This notice may not be removed or altered from any source distribution.
//
//  =============================================================================
//
//  derived from Gregorio Zanon's script
//  http://forum.unity3d.com/threads/119295-Writing-AudioListener.GetOutputData-to-wav-problem?p=806734&viewfull=1#post806734

using System;
using System.IO;
using UnityEngine;
using System.Collections.Generic;

public static class SavWav {

    const int HEADER_SIZE = 44;

    public static bool Save(string filepath, float[] samples, int sampleRate, int channels)
    {

        using (var fileStream = new FileStream(filepath, FileMode.Create))
        {
            // Reserve space for header
            byte[] emptyHeader = new byte[HEADER_SIZE];
            fileStream.Write(emptyHeader, 0, HEADER_SIZE);

            // Convert float samples to bytes
            Int16[] intData = new Int16[samples.Length];
            byte[] byteData = new byte[samples.Length * 2];
            int rescaleFactor = 32767;

            for (int i = 0; i < samples.Length; i++)
            {
                intData[i] = (short)(Mathf.Clamp(samples[i], -1f, 1f) * rescaleFactor);
                BitConverter.GetBytes(intData[i]).CopyTo(byteData, i * 2);
            }

            fileStream.Write(byteData, 0, byteData.Length);

            // Write header
            fileStream.Seek(0, SeekOrigin.Begin);
            WriteHeader(fileStream, sampleRate, channels, samples.Length);
        }

        return true;
    }

    private static void WriteHeader(FileStream fileStream, int sampleRate, int channels, int sampleCount)
    {
        fileStream.Write(System.Text.Encoding.ASCII.GetBytes("RIFF"), 0, 4);
        fileStream.Write(BitConverter.GetBytes(36 + sampleCount * 2), 0, 4);
        fileStream.Write(System.Text.Encoding.ASCII.GetBytes("WAVE"), 0, 4);
        fileStream.Write(System.Text.Encoding.ASCII.GetBytes("fmt "), 0, 4);
        fileStream.Write(BitConverter.GetBytes(16), 0, 4);
        fileStream.Write(BitConverter.GetBytes((ushort)1), 0, 2);
        fileStream.Write(BitConverter.GetBytes((ushort)channels), 0, 2);
        fileStream.Write(BitConverter.GetBytes(sampleRate), 0, 4);
        fileStream.Write(BitConverter.GetBytes(sampleRate * channels * 2), 0, 4);
        fileStream.Write(BitConverter.GetBytes((ushort)(channels * 2)), 0, 2);
        fileStream.Write(BitConverter.GetBytes((ushort)16), 0, 2);
        fileStream.Write(System.Text.Encoding.ASCII.GetBytes("data"), 0, 4);
        fileStream.Write(BitConverter.GetBytes(sampleCount * 2), 0, 4);
    }

}