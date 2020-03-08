using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace HuffmanCoding
{
    public static class KeyManager
    {
        public static void SaveKey(IEnumerable<KeyValuePair<char, List<bool>>> symbols, string path)
        {
            using (var fileStream = File.Create(path))
            using (var writer = new StreamWriter(fileStream))
            {

                foreach (var symbol in symbols)
                {
                    string code = string.Join("", symbol.Value.Select(x => x.ToChar()));
                    writer.WriteLine($"{symbol.Key}||{code}");
                }
            }
        }

        public static IEnumerable<KeyValuePair<char, List<bool>>> ReadKey(string path)
        {
            if (!File.Exists(path))
                throw new FileNotFoundException($"File not found at path {path}");

            using (var fileStream = File.OpenRead(path))
            using (var reader = new StreamReader(fileStream))
            {
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    string[] keyValue = line.Split("||");
                    if (keyValue.Length != 2)
                        throw new Exception("Key file is corrupted");

                    if (keyValue[0].Length != 1)
                        throw new Exception("Single character expected");

                    char character = keyValue[0][0];
                    var code = keyValue[1].Select(x => x.ToBool()).ToList();

                    yield return new KeyValuePair<char, List<bool>>(character, code);
                }
            }
        }
    }
}
