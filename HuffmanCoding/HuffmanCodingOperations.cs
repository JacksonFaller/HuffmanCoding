using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using HuffmanCoding.Models;

namespace HuffmanCoding
{
    public static class HuffmanCodingOperations
    {
        public static List<bool> Encode(string input, Dictionary<char, List<bool>> codeCharacterMap)
        {
            int maxLenght = (int)Math.Ceiling(Math.Log2(codeCharacterMap.Count));
            var result = new List<bool>(maxLenght * input.Length);
            for (int i = 0; i < input.Length; i++)
            {
                var character = input[i];
                if (!codeCharacterMap.ContainsKey(character))
                    throw new Exception($"Character {character} is not found in a code map");

                result.AddRange(codeCharacterMap[character]);
            }

            return result;
        }

        public static string Decode(string input, IEnumerable<KeyValuePair<char, List<bool>>> characterCodeMap)
        {
            var codeCharacterMap = new Dictionary<List<bool>, char>(new ListComparer<bool>());
            foreach (var mapPair in characterCodeMap)
            {
                codeCharacterMap.Add(mapPair.Value, mapPair.Key);
            }

            var stringBuilder = new StringBuilder();
            var code = new List<bool>();
            for (int i = 0; i < input.Length; i++)
            {
                code.Add(input[i].ToBool());
                if (codeCharacterMap.ContainsKey(code))
                {
                    stringBuilder.Append(codeCharacterMap[code]);
                    code.Clear();
                }
            }

            return stringBuilder.ToString();
        }

        public static List<Symbol> AnalyzeSymbols(Stream stream)
        {
            var reader = new StreamReader(stream);
            var symbolFrequencies = new Dictionary<char, SymbolFrequency>();
            int symbolsRead = 0;
            while (!reader.EndOfStream)
            {
                var character = Convert.ToChar(reader.Read());
                if (symbolFrequencies.ContainsKey(character))
                    symbolFrequencies[character].Count++;
                else
                    symbolFrequencies.Add(character, new SymbolFrequency(character));

                symbolsRead++;
            }
            reader.Close();

            return symbolFrequencies.Values
                .Select(x => new Symbol((float)x.Count / symbolsRead, x.Character))
                .ToList();
        }

        public static List<Symbol> AnalyzeSymbols(string input)
        {
            var symbolFrequencies = new Dictionary<char, SymbolFrequency>();
            for (int i = 0; i < input.Length; i++)
            {
                var character = input[i];
                if (symbolFrequencies.ContainsKey(character))
                    symbolFrequencies[character].Count++;
                else
                    symbolFrequencies.Add(character, new SymbolFrequency(character));
            }

            return symbolFrequencies.Values
                .Select(x => new Symbol((float)x.Count / input.Length, x.Character))
                .ToList();
        }

        /// <summary>
        /// Builds a tree of nodes with symbols
        /// </summary>
        /// <returns>Tree's root</returns>
        public static Node BuildSymbolTree(List<Symbol> symbols)
        {
            if (symbols.Count == 0)
                throw new Exception("Symbols array is empty");

            var nodes = symbols.OrderBy(x => x.Probability).Select(x => new Node(x)).ToList();

            while (nodes.Count > 1)
            {
                var first = nodes.First();
                nodes.RemoveAt(0);
                var second = nodes.First();
                nodes.RemoveAt(0);

                float newProbability = first.Symbol.Probability + second.Symbol.Probability;
                var internalNode = new Node(new Symbol(newProbability))
                {
                    LeftChild = first,
                    RightChild = second,
                    IsInternal = true
                };

                nodes.AddSorted(internalNode);
            }

            return nodes.First();
        }

        public static void CreateCharacterMap(Node node, List<bool> code, Dictionary<char, List<bool>> codeCharacterMap)
        {
            if (!node.IsInternal)
            {
                codeCharacterMap.Add(node.Symbol.Character, code);
                //_codeCharacterMap.Add(code, node.Symbol.Character);
            }

            if (node.LeftChild != null)
            {
                var newCode = new List<bool>(code);
                newCode.Add(false);
                CreateCharacterMap(node.LeftChild, newCode, codeCharacterMap);
            }

            if (node.RightChild != null)
            {
                var newCode = new List<bool>(code);
                newCode.Add(true);
                CreateCharacterMap(node.RightChild, newCode, codeCharacterMap);
            }
        }
    }
}
