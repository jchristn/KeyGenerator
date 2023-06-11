using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Generator
{
    public class KeyGenerator
    {
        #region Public-Members

        /// <summary>
        /// Function to invoke to send log messages.
        /// </summary>
        public Action<string> Logger { get; set; } = null;

        #endregion

        #region Private-Members

        private string _Header = "[KeyGenerator] ";
        private Random _Random = new Random();
        private string _Characters = "ACTG";

        #endregion

        #region Constructors-and-Factories

        /// <summary>
        /// Instantiate.
        /// </summary>
        public KeyGenerator()
        {

        }

        #endregion

        #region Public-Methods
        public enum SequenceStatus
        {
            Valid,
            Homopolymer,
            GCContent,
            Folding,
            Repeats
        }

        /// <summary>
        /// Generate a key.
        /// </summary>
        /// <param name="len">Number of bases.</param>
        /// <param name="allowHomopolymers">Enable or disable allowing homopolymers.</param>
        /// <param name="maxGcContent">Max GC content, between 0 and 1.</param>
        /// <param name="maxComplementaryBases">Maximum number of complementary bases to allow.</param>
        /// <param name="minRepetitiveSequenceLength"></param>
        /// <param name="maxAllowedRepetitions"></param>
        /// <returns>Tuple containing the key and the number of iterations required.</returns>
        public (string, int) Generate(
            int len,
            int maxAttempts = 1000,
            bool allowHomopolymers = false,
            double maxGcContent = 0.5,
            int maxComplementaryBases = 8,
            int minRepetitiveSequenceLength = 3,
            int maxAllowedRepetitions = 1
            )
        {
            if (len < 1) throw new ArgumentOutOfRangeException(nameof(len));
            if (maxAttempts < 1) throw new ArgumentOutOfRangeException(nameof(maxAttempts));
            if (maxGcContent < 0.01 || maxGcContent > 0.99) throw new ArgumentOutOfRangeException(nameof(maxGcContent));
            if (maxComplementaryBases < 1 || maxComplementaryBases > len) throw new ArgumentOutOfRangeException(nameof(maxComplementaryBases));
            if (minRepetitiveSequenceLength < 2 || minRepetitiveSequenceLength > len) throw new ArgumentOutOfRangeException(nameof(minRepetitiveSequenceLength));
            if (maxAllowedRepetitions < 0) throw new ArgumentOutOfRangeException(nameof(maxAllowedRepetitions));

            int attempt = -1;
            while (attempt < maxAttempts)
            {
                attempt++;
                Logger?.Invoke(_Header + "starting attempt " + attempt);
                string logPrefix = _Header + attempt + "/" + maxAttempts + ": ";

                // generate random string
                string str = null;
                if (allowHomopolymers)
                {
                    str = RandomString(len);
                    Logger?.Invoke(logPrefix + "generated random sequence: " + str);
                }
                else
                {
                    str = RandomStringWithoutHomopolymers(len);
                    Logger?.Invoke(logPrefix + "generated random sequence without homopolymers: " + str);
                }

                SequenceStatus status = Validate(str, logPrefix, maxGcContent, maxComplementaryBases, minRepetitiveSequenceLength, maxAllowedRepetitions);
                if (status != SequenceStatus.Valid)
                    continue;

                Logger?.Invoke(logPrefix + "sequence " + str + " passes all checks");
                return (str, attempt);
            }

            Logger?.Invoke(_Header + "exceeded maximum attempts, could not generate a candidate sequence");
            return (null, attempt);
        }
        
        public SequenceStatus Validate(
            string str,
            string logPrefix,
            double maxGcContent = 0.5,
            int maxComplementaryBases = 8,
            int minRepetitiveSequenceLength = 3,
            int maxAllowedRepetitions = 1
            )
        {
            // check for homopolymers
            if (HomopolymersExist(str))
            {
                Logger?.Invoke(logPrefix + "homopolymers detected");
                return SequenceStatus.Homopolymer;
            }
            else
            {
                Logger?.Invoke(logPrefix + "no homopolymers detected");
            }

            // check for GC content
            double gcContent = CalculateGcContent(str);
            if (gcContent > maxGcContent)
            {
                Logger?.Invoke(logPrefix + "GC content threshold exceeded: " + gcContent + "/" + maxGcContent);
                return SequenceStatus.GCContent;
            }
            else
            {
                Logger?.Invoke(logPrefix + "GC content threshold within tolerance: " + gcContent + "/" + maxGcContent);
            }

            // check for complementary bases that could lead to folding
            bool foldPossible = FoldingPotentialExists(str, maxComplementaryBases);
            if (foldPossible)
            {
                Logger?.Invoke(logPrefix + "folding possible with " + maxComplementaryBases + " bases");
                return SequenceStatus.Folding;
            }
            else
            {
                Logger?.Invoke(logPrefix + "folding not possible with " + maxComplementaryBases + " bases");
            }

            // check for repeated patterns
            bool repeats = IsSequenceRepeated(str, minRepetitiveSequenceLength, maxAllowedRepetitions);
            if (repeats)
            {
                Logger?.Invoke(logPrefix + "excessive repeats detected");
                return SequenceStatus.Repeats;
            }
            else
            {
                Logger?.Invoke(logPrefix + "excessive repeats not detected");
            }
            return SequenceStatus.Valid;
        }


        #endregion

        #region Private-Methods

        private string RandomBase()
        {
            int next = _Random.Next(_Characters.Length);
            return _Characters.Substring(next, 1);
        }

        private string RandomString(int len)
        {
            string ret = "";

            for (int i = 0; i < len; i++)
            {
                ret += RandomBase();
            }

            return ret;
        }

        public string RandomStringWithoutHomopolymers(int len)
        {
            string ret = "";

            for (int i = 0; i < len; i++)
            {
                if (i == 0)
                {
                    ret += RandomBase();
                }
                else
                {
                    string randomBase = null;
                    while (true)
                    {
                        randomBase = RandomBase();
                        if (randomBase.Equals(ret.Substring(i - 1, 1))) continue;
                        ret += randomBase;
                        break;
                    }
                }
            }

            return ret;
        }

        private bool HomopolymersExist(string str)
        {
            for (int i = 1; i < str.Length; i++)
            {
                if (str[i] == str[i - 1]) return true;
            }

            return false;
        }

        private double CalculateGcContent(string str)
        {
            int len = str.Length;
            int atContent = 0;
            int gcContent = 0;

            foreach (char c in str)
            {
                if (c == 'A' || c == 'T') atContent++;
                else if (c == 'C' || c == 'G') gcContent++;
            }

            return (double)gcContent / (double)len;
        }

        private bool FoldingPotentialExists(string str, int maxComplementaryBases)
        {
            for (int i = 0; i < (str.Length - maxComplementaryBases); i++)
            {
                string sequenceToCheck = str.Substring(i, maxComplementaryBases);
                string complementaryBases = GetComplementaryBases(sequenceToCheck);

                if (str.IndexOf(complementaryBases, i + 1) != -1) return true;
            }

            return false;
        }

        private char GetComplementaryBase(char dnaBase)
        {
            switch (dnaBase)
            {
                case 'A':
                    return 'T';
                case 'T':
                    return 'A';
                case 'C':
                    return 'G';
                case 'G':
                    return 'C';
                default:
                    throw new ArgumentException("Invalid base: " + dnaBase);
            }
        }

        private string GetComplementaryBases(string str)
        {
            string ret = "";
            foreach (char c in str) ret += GetComplementaryBase(c);
            return ret;
        }
        
        private bool IsSequenceRepeated(string str, int minRepetitiveSequenceLength, int maxAllowedRepetitions)
        {
            for (int i = 0; i <= str.Length - minRepetitiveSequenceLength * maxAllowedRepetitions; i++)
            {
                string sequenceToCheck = str.Substring(i, minRepetitiveSequenceLength);
                int count = 0;

                int pos = str.IndexOf(sequenceToCheck, i + minRepetitiveSequenceLength);
                while (pos != -1)
                {
                    count++;
                    if (count > maxAllowedRepetitions)
                    {
                        Logger?.Invoke($"Sequence {sequenceToCheck} repeats at least {count} times");
                        return true;
                    }

                    pos = str.IndexOf(sequenceToCheck, pos + 1);
                }

            }

            return false;
        }

    }

    #endregion 
}