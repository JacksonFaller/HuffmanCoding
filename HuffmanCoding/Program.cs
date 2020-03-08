using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using HuffmanCoding.Models;

namespace HuffmanCoding
{
    class Program
    {
        static void Main(string[] args)
        {
            var config = GetConfiguration(args, out string message);
            if (config.IsCorrupt)
            {
                Console.WriteLine(message);
                return;
            }

            if (!File.Exists(config.FilePath))
            {
                Console.WriteLine($"Input file is not found at a path {config.FilePath}");
                return;
            }

            if (new FileInfo(config.FilePath).Length == 0)
            {
                return;
            }

            if (config.IsEncode.Value)
            {
                Encode(config.FilePath);
                return;
            }

            if (!File.Exists(config.KeyPath))
            {
                Console.WriteLine($"Key file is not found at a path {config.KeyPath}");
                return;
            }
            Decode(config.FilePath, config.KeyPath);
        }

        private static void Decode(string filePath, string keyPath)
        {
            string input = ReadFileContent(filePath);
            var codeCharacterMap = KeyManager.ReadKey(keyPath);
            var result = HuffmanCodingOperations.Decode(input, codeCharacterMap);
            Console.WriteLine(result);
            WriteFileContent("DecodeOutput.txt", result);
        }

        private static void Encode(string filePath)
        {
            string input = ReadFileContent(filePath);
            var symbols = HuffmanCodingOperations.AnalyzeSymbols(input);
            var root = HuffmanCodingOperations.BuildSymbolTree(symbols);
            var codeCharacterMap = new Dictionary<char, List<bool>>();
            HuffmanCodingOperations.CreateCharacterMap(root, new List<bool>(), codeCharacterMap);
            KeyManager.SaveKey(codeCharacterMap, "key.txt");
            var result = HuffmanCodingOperations.Encode(input, codeCharacterMap);
            string resultFormatted = string.Join("", result.Select(x => x.ToChar()));
            Console.WriteLine(resultFormatted);
            WriteFileContent("EncodeOutput.txt", resultFormatted);
        }

        private static string ReadFileContent(string filePath)
        {
            using (var fileStream = File.OpenRead(filePath))
            using (var reader = new StreamReader(fileStream))
            {
                return reader.ReadToEnd();
            }
        }

        private static void WriteFileContent(string filePath, string content)
        {
            using var writer = File.CreateText(filePath);
            writer.Write(content);
        }

        private static Configuration GetConfiguration(string[] parameters, out string message)
        {
            var config = new Configuration();

            if (parameters.Length < 3)
            {
                message = "Mode parameter and file path are required";
                config.IsCorrupt = true;
                return config;
            }
            message = null;

            for (int i = 0; i < parameters.Length; i++)
            {
                string parameter = parameters[i];
                message = ProcessParameter(parameter, config, () => GetNextParameter(parameters, ++i));
                if (config.IsCorrupt)
                    return config;
            }

            if (!config.IsEncode.HasValue)
            {
                message = "Mode parameter is required";
                config.IsCorrupt = true;
                return config;
            }

            if (config.FilePath == null)
            {
                message = "File path parameter is required";
                config.IsCorrupt = true;
            }

            return config;
        }

        private static string GetNextParameter(string[] parameters, int index)
        {
            if (index >= parameters.Length)
                return null;
            return parameters[index];
        }

        private static string ProcessParameter(string parameter, Configuration config, Func<string> getNext)
        {
            switch (parameter)
            {
                case "-f":
                {
                    config.FilePath = getNext();
                    if (config.FilePath == null)
                    {
                        config.IsCorrupt = true;
                        return "File parameter is supplied, but not a path to it.";
                    }
                    break;
                }
                case "-e":
                {
                    config.IsEncode = true;
                    break;
                }
                case "-d":
                {
                    config.KeyPath = getNext();
                    if (config.KeyPath == null)
                    {
                        config.IsCorrupt = true;
                        return "Key is required for decoding, but not supplied";
                    }
                    config.IsEncode = false;
                    break;
                }
                default:
                {
                    config.IsCorrupt = true;
                    return $"Paramter {parameter} is not recognized";
                }
            }

            return null;
        }

    }

}
