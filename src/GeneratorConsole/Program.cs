using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Data;
using System.Linq;
using DatabaseWrapper;
using DatabaseWrapper.Core;
using DatabaseWrapper.Mysql;
using ExpressionTree;
using Generator;
using GetSomeInput;
using Timestamps;
using Watson.ORM;

namespace Test
{
    public static class Program
    {
        private static bool _RunForever = true;
        private static int _MinimumEditDistance = 10;
        private static KeyGenerator _Generator = new KeyGenerator();
        private static RecordManager _Records = new RecordManager();
        private static KeyValidator _Validator = new KeyValidator();
        private static WatsonORM _ORM = null;

        private static Func<List<string>> _RetrieveVendors = () =>
        {
            Expr eVendor = new Expr
            {
                Left = _ORM.GetColumnName<VendorMetadata>(nameof(VendorMetadata.Id)),
                Operator = OperatorEnum.GreaterThan,
                Right = 0
            };

            List<VendorMetadata> vendors = _ORM.SelectMany<VendorMetadata>(eVendor);
            return vendors.Select(v => v.Key).Distinct().ToList();
        };

        private static Func<List<string>> _RetrieveCodecs = () =>
        {
            Expr eCodec = new Expr
            {
                Left = _ORM.GetColumnName<CodecMetadata>(nameof(CodecMetadata.Id)),
                Operator = OperatorEnum.GreaterThan,
                Right = 0
            };

            List<CodecMetadata> codecs = _ORM.SelectMany<CodecMetadata>(eCodec);
            return codecs.Select(v => v.Key).Distinct().ToList();
        };

        public static void Main(string[] args)
        {
            InitializeOrm();
            InitializeRecordsManager();

            while (_RunForever)
            {
                string userInput = Inputty.GetString("Command [?/help]:", null, false);

                switch (userInput)
                {
                    case "c":
                    case "C":
                    case "cls":
                        Console.Clear();
                        break;

                    case "q":
                    case "Q":
                        _RunForever = false;
                        break;

                    case "?":
                        Menu();
                        break;

                    case "debug":
                        if (_Generator.Logger == null) _Generator.Logger = Console.WriteLine;
                        else _Generator.Logger = null;
                        break;

                    case "distance":
                        _MinimumEditDistance = Inputty.GetInteger("Edit distance:", 10, true, false);
                        break;

                    case "gen vendor":
                        GenerateVendor();
                        break;

                    case "gen codec":
                        GenerateCodec();
                        break;
                }
            }
        }

        private static void Menu()
        {
            Console.WriteLine("");
            Console.WriteLine("Available commands");
            Console.WriteLine("  c             clear the screen");
            Console.WriteLine("  q             quit the program");
            Console.WriteLine("  ?             help, e.g. this menu");
            Console.WriteLine("  debug         enable or disable debug logging (currently: " + (_Generator.Logger != null ? "true" : "false") + ")");
            Console.WriteLine("  distance      set minimum edit distance (currently: " + _MinimumEditDistance + ")");
            Console.WriteLine("  gen vendor    generate a vendor key sequences");
            Console.WriteLine("  gen codec     generate a CODEC key sequences");
            Console.WriteLine("");
        }

        private static void InitializeOrm()
        {
            string sqliteFilename = Inputty.GetString("Sqlite filename   :", null, true);

            if (!String.IsNullOrEmpty(sqliteFilename))
            {
                _ORM = new WatsonORM(new DatabaseSettings(sqliteFilename));
            }
            else
            {
                DbTypeEnum dbType = (DatabaseWrapper.Core.DbTypeEnum)Enum.Parse(typeof(DbTypeEnum), Inputty.GetString("Database type      :", "Mysql", false));
                DatabaseWrapper.Core.DatabaseSettings settings = new DatabaseWrapper.Core.DatabaseSettings(
                    dbType,
                    Inputty.GetString("Hostname          :", "rosetta-stone-database.cdgksvvrmx4w.us-west-1.rds.amazonaws.com", false),
                    Inputty.GetInteger("Port              :", 3306, true, false),
                    Inputty.GetString("User              :", "admin", false),
                    Inputty.GetString("Password          :", "password", false),
                    Inputty.GetString("Database name     :", "rosettastone", false)
                    );

                _ORM = new WatsonORM(settings);
            }

            _ORM.InitializeDatabase();
            _ORM.InitializeTables(new List<Type> 
                { 
                    typeof(CodecMetadata),
                    typeof(VendorMetadata)
                }
            );
        }

        private static void InitializeRecordsManager()
        {
            _Records = new RecordManager();

        }

        private static (string, int) Generate(
            int len,
            int maxAttempts,
            bool allowHomopolymers,
            double maxGcContent,
            int maxComplementaryBases,
            int minRepeatedSequenceLength,
            int maxAllowedRepetitions)
        {
            return _Generator.Generate(
                len,
                maxAttempts,
                allowHomopolymers,
                maxGcContent,
                maxComplementaryBases,
                minRepeatedSequenceLength,
                maxAllowedRepetitions);
        }

        private static List<string> GenerateVendor()
        {
            Console.WriteLine("Specify a negative number to run indefinitely");

            int count =
                Inputty.GetInteger("Number of sequences to generate  :", 5, false, false);
            if (count == 0) return new List<string>();

            int len =
                Inputty.GetInteger("Length                           :", 35, true, false);
            int maxAttempts =
                Inputty.GetInteger("Maximum attempts                 :", 1000, true, false);
            bool allowHomopolymers =
                Inputty.GetBoolean("Allow homopolymers               :", false);
            double maxGcContent =
                Inputty.GetDouble("Max GC content                   :", 0.5, true, false);
            int maxComplementaryBases =
                Inputty.GetInteger("Max complementary bases          :", 8, true, false);
            int minRepeatedSequenceLength =
                Inputty.GetInteger("Minimum repeated sequence length :", 3, true, false);
            int maxAllowedRepetitions =
                Inputty.GetInteger("Maximum allowed repetitions      :", 1, true, false);

            List<string> generated = new List<string>();

            while (true)
            {
                Timestamp ts = new Timestamp();

                bool isValid = false;
                (string, int) ret = new();

                while (!isValid)
                {
                    ret = Generate(
                        len,
                        maxAttempts,
                        allowHomopolymers,
                        maxGcContent,
                        maxComplementaryBases,
                        minRepeatedSequenceLength,
                        maxAllowedRepetitions);

                    if (!String.IsNullOrEmpty(ret.Item1))
                    {
                        isValid = _Validator.Validate(ret.Item1, _Records.VendorKeys, _MinimumEditDistance);
                    }

                    ts.End = DateTime.UtcNow;
                }

                if (isValid && !String.IsNullOrEmpty(ret.Item1))
                {
                    generated.Add(ret.Item1);
                    Console.WriteLine(generated.Count + ": " + ret.Item1 + " (" + ret.Item2 + " attempts, " + ts.TotalMs + "ms)");

                    VendorMetadata vendor = new VendorMetadata
                    {
                        Key = ret.Item1,
                        Name = "Unassigned Vendor ID",
                        ContactInformation = "Unassigned Vendor ID"
                    };

                    vendor = _ORM.Insert<VendorMetadata>(vendor);

                    _Records.RetrieveVendorKeys(_RetrieveVendors);

                    if (count > 0 && generated.Count >= count)
                    {
                        break;
                    }
                }
            }

            return generated;
        }

        private static List<string> GenerateCodec()
        {
            Console.WriteLine("Specify a negative number to run indefinitely");

            int count =
                Inputty.GetInteger("Number of sequences to generate  :", 5, false, false);
            if (count == 0) return new List<string>();

            int len =
                Inputty.GetInteger("Length                           :", 35, true, false);
            int maxAttempts =
                Inputty.GetInteger("Maximum attempts                 :", 1000, true, false);
            bool allowHomopolymers =
                Inputty.GetBoolean("Allow homopolymers               :", false);
            double maxGcContent =
                Inputty.GetDouble("Max GC content                   :", 0.5, true, false);
            int maxComplementaryBases =
                Inputty.GetInteger("Max complementary bases          :", 8, true, false);
            int minRepeatedSequenceLength =
                Inputty.GetInteger("Minimum repeated sequence length :", 3, true, false);
            int maxAllowedRepetitions =
                Inputty.GetInteger("Maximum allowed repetitions      :", 1, true, false);

            List<string> generated = new List<string>();

            while (true)
            {
                Timestamp ts = new Timestamp();

                bool isValid = false;
                (string, int) ret = new();

                while (!isValid)
                {
                    ret = Generate(
                        len,
                        maxAttempts,
                        allowHomopolymers,
                        maxGcContent,
                        maxComplementaryBases,
                        minRepeatedSequenceLength,
                        maxAllowedRepetitions);

                    if (!String.IsNullOrEmpty(ret.Item1))
                    {
                        isValid = _Validator.Validate(ret.Item1, _Records.CodecKeys, _MinimumEditDistance);
                    }

                    ts.End = DateTime.UtcNow;
                }

                if (isValid && !String.IsNullOrEmpty(ret.Item1))
                {
                    generated.Add(ret.Item1);
                    Console.WriteLine(generated.Count + ": " + ret.Item1 + " (" + ret.Item2 + " attempts, " + ts.TotalMs + "ms)");

                    CodecMetadata codec = new CodecMetadata
                    {
                        Key = ret.Item1,
                        Name = "Unassigned CODEC ID",
                        Version = "Unassigned CODEC ID",
                        Uri = "Unassigned CODEC ID"
                    };

                    codec = _ORM.Insert<CodecMetadata>(codec);

                    _Records.RetrieveCodecKeys(_RetrieveVendors);

                    if (count > 0 && generated.Count >= count)
                    {
                        break;
                    }
                }
            }

            return generated;
        }
    }
}