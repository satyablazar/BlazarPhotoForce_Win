using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PhotoForce.Helpers
{
    public static class StringSwap
    {
        public static Dictionary<TValue, TKey>
SwapKeysValues<TKey, TValue>(this Dictionary<TKey, TValue> input)
        {
            var result = new Dictionary<TValue, TKey>();
            input.ToList().ForEach((keyValuePair) =>
            {
                result.Add(keyValuePair.Value, keyValuePair.Key);
            });
            return result;
        }

        public static string Swap(
            this string input,
            string alphabet,
            string move)
        {
            Dictionary<char, int>
                alphabetDictionary = new Dictionary<char, int>();

            for (int i = 0; i < alphabet.Length; i++)
            {
                alphabetDictionary.Add(alphabet[i], i);
            }

            var swapedAlphabet = alphabetDictionary.SwapKeysValues();
            return Enumerable
                .Range(0, (int)Math.Ceiling(input.Length / (move.Length * 1M)))
                .ToList()
                .Aggregate<int, string>("", (s, i) =>
                {
                    var l = i * move.Length + move.Length;
                    var cInput = input.Substring(i * move.Length,
                        (l > input.Length)
                            ? input.Length - i * move.Length : move.Length);
                    return s + cInput
                .Select((c, index) =>
                {
                    int intCandidate;
                    if (!Int32.TryParse(c.ToString(), out intCandidate))
                    {
                        var length = (alphabetDictionary[c] +
                            Int32.Parse(move[index].ToString()));
                        //return
                        //    swapedAlphabet[(alphabet.Length > length)
                        //        ? length : length % alphabet.Length];

                        #region Bharat

                        if (alphabet.Length > length)
                        {
                            if (length == 8 || length == 9 || length == 11 || length == 14 || length == 16 || length == 18)
                            {
                                return swapedAlphabet[length + 11];
                            }
                            else
                            {
                                return swapedAlphabet[length];
                            }
                        }
                        else
                        {
                            if (length == 44 || length == 45 || length == 47 || length == 50 || length == 52 || length == 54)
                            {
                                return swapedAlphabet[(length % alphabet.Length) + 11];
                            }
                            else
                            {
                                return swapedAlphabet[length % alphabet.Length];
                            }
                        }
                        #endregion
                    }
                    else
                    {
                        var moveInt = Int32.Parse(move[index].ToString());
                        return Char.Parse(((intCandidate + moveInt) % 10)
                            .ToString());
                    }
                })
                .Aggregate<char, string>("", (a, b) => a + b);
                });
        }
    }
}
