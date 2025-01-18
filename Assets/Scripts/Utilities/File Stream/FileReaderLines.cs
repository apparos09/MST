using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

namespace util
{
    public class FileReaderLines : FileReader
    {
        // the lines from the file.
        public string[] lines;

        // Read from the file, and put the content into lines.
        public override void ReadFile()
        {
            // checks if the file exists.
            if (!FileExists())
            {
                Debug.LogError("File does not exist.");
                return;
            }

            // TODO: see if this works for clearing the array. C# garbage collection should take care of this?
            lines = null;

            // Gets all the lines from the file (@ specifies where the path is relative to - may not be needed).
            string f = filePath + fileName;
            lines = File.ReadAllLines(@f);
        }
    }
}