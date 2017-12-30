using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace PasswordGenerator
{
    class Program
    {
        static void Main(string[] args)
        {
            var options = new Options();
            if (CommandLine.Parser.Default.ParseArgumentsStrict(args, options))
            {
                var freq = LoadFrequencies("manzoni-cap1-frequencies.txt");
                for (int i = 0; i < options.NumPassword; i++)
                {
                    string pass = CreatePassword(options.PasswordLength, freq.Set, freq.Map);
                    Console.WriteLine(pass);
                }
            }
            else
            {
                // Display the default usage information
                Console.WriteLine(options.GetUsage());
            }
        }

        static (string Set, Dictionary<char, string> Map) LoadFrequencies(string filename)
        {
            var inset = new StringBuilder();
            var map = new Dictionary<char, string>();

            var lines = File.ReadAllLines(filename);
            var columns = lines[0].Split('\t');
            for (int i = 1; i < lines.Length; i++)
            {
                inset.Append(columns[i]);
            }
            string set = inset.ToString();

            for (int i = 1; i < lines.Length; i++)
            {
                char cur = columns[i][0];
                map[cur] = "";
                var fields = lines[i].Split('\t');
                for (int j = 1; j < columns.Length; j++)
                {
                    if (fields[j] != "0")
                    {
                        map[cur] += set[j - 1];
                    }
                }
            }
            return (set, map);
        }

        // see https://stackoverflow.com/questions/54991/generating-random-passwords
        static string CreatePassword(int length, string initialSet, Dictionary<char, string> map)
        {
            var res = new StringBuilder();

            // Use a 4-byte array to fill it with random bytes and convert it then
            // to an integer value.
            byte[] randomBytes = new byte[4];
            
            // Generate 4 random bytes.
            var rng = new RNGCryptoServiceProvider();
            rng.GetBytes(randomBytes);

            // Convert 4 bytes into a 32-bit integer value.
            int seed = BitConverter.ToInt32(randomBytes, 0);

            // Now, this is real randomization.
            Random random = new Random(seed);

            var lastThreeChars = new ThreeChars();
            char lastChar = initialSet[random.Next(initialSet.Length)];
            res.Append(lastChar);
            lastThreeChars.Push(lastChar);
            while (0 < length--)
            {
                bool good = true;
                while (good)
                {
                    string nextValidChars = map[lastChar];
                    lastChar = nextValidChars[random.Next(nextValidChars.Length)];
                    lastThreeChars.Push(lastChar);
                    var isSeq = IsSequence(lastThreeChars);
                    good = isSeq.allConsonants || isSeq.allVowels;
                    if (!good)
                        res.Append(lastChar);
                }
            }
            return res.ToString();
        }

        struct ThreeChars
        {
            char a;
            char b;
            char c;
            public void Push(char x)
            {
                c = b;
                b = a;
                a = x;
            }

            public static implicit operator char[] (ThreeChars tc)
            {
                return new char[] { tc.c, tc.b, tc.a };
            }
        }

        static readonly char[] Vowels = new char[] { 'A', 'E', 'I', 'O', 'U' };

        static public (bool allConsonants, bool allVowels) IsSequence(char[] chars)
        {
            bool allConsonants = true;
            bool allVowels = true;
            for (int i = 0; i < chars.Length; i++)
            {
                bool isVowel = Vowels.Contains(chars[i]);
                allConsonants = allConsonants && !isVowel;
                allVowels = allVowels && isVowel;
            }
            return (allConsonants, allVowels);
        }
    }
}
