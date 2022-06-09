//
//     Webermania
//     © 2021 Christopher Weber
//     Apache-2.0 License
//     https://github.com/webermania/MicroExpressionEvaluator
//     https://www.nuget.org/packages/MicroExpressionEvaluator
//
//     This ("TestAndDemo") project is only for effective testing and demonstration of the MicroExpressionEvaluator
//     and was not meant to be beautiful
//

using System;
using System.Linq;
//using MicroExpressionEvaluator;
using MicroExpressionEvaluatorDotNetFramework;
using Microsoft.CodeAnalysis.CSharp.Scripting; // <-- We do NOT need the Microsoft CSharpScript Engine! We use it here only to collect expected results for later comparison.

namespace TestAndDemo
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            Console.WriteLine("-----------------------");
            Console.WriteLine("--- Testing started ---");
            Console.WriteLine("");
            Console.WriteLine("");
            Console.WriteLine("!!! For best performance, please make sure not to run this in debug-mode or from the IDE !!!");
            Console.WriteLine("");
            Console.WriteLine("");
            Console.WriteLine("--- Will now load the test sets: ");

            var testSet1 = new (string expression, string expectedResult, string actualResult, bool success)[]
            {
                ("true", "", "", false),
                ("false", "", "", false),
                ("true == true", "", "", false),
                ("true == false", "", "", false),
                ("false == false", "", "", false),
                ("false == true", "", "", false),
                ("true != true", "", "", false),
                ("true != false", "", "", false),
                ("false != false", "", "", false),
                ("false != true", "", "", false),
                ("!true == !true", "", "", false),
                ("!true != true", "", "", false),
                ("!true != !false", "", "", false),
                ("true == true == true", "", "", false),
                ("true == true != true", "", "", false),
                ("true != true == true", "", "", false),
                ("true == true == false", "", "", false),
                ("true == true != false", "", "", false),
                ("true != true == false", "", "", false),
                ("true == false == true", "", "", false),
                ("true == false != true", "", "", false),
                ("true != false == true", "", "", false),
                ("false == true == true", "", "", false),
                ("false == true != true", "", "", false),
                ("false != true == true", "", "", false),
                ("false == true == false", "", "", false),
                ("false == true != false", "", "", false),
                ("false != true == false", "", "", false),
                ("false == false == false", "", "", false),
                ("false == false != false", "", "", false),
                ("false != false == false", "", "", false),
                ("false != false != false", "", "", false),
                ("false == false == false", "", "", false),
                ("true != true != true", "", "", false),
                ("true == true == true", "", "", false),
                ("true == (true == false)", "", "", false),
                ("(true == true) == false", "", "", false),
                ("true == (true != false)", "", "", false),
                ("(true == true) != false", "", "", false),
                ("true != (true == false)", "", "", false),
                ("(true != true) == false", "", "", false),
                ("!true == !(true || false)", "", "", false),
                ("false == false == true", "", "", false),
                ("false == true == false", "", "", false),
                ("true == false == false", "", "", false),
                ("true && true", "", "", false),
                ("true && false", "", "", false),
                ("false && false", "", "", false),
                ("false && true", "", "", false),
                ("true || true", "", "", false),
                ("true || false", "", "", false),
                ("false || false", "", "", false),
                ("false || true", "", "", false),
                ("true || true && false", "", "", false),
                ("(true || true) && false", "", "", false),
                ("true || (true && false)", "", "", false),
                ("true && true || false", "", "", false),
                ("(true && true) || false", "", "", false),
                ("true && (true || false)", "", "", false),
                ("((((((true))))))", "", "", false),
                ("(false || (true == (false || (true == (false || true)))))", "", "", false),
                ("(true) && (true)", "", "", false),
                ("((true) && (true))", "", "", false),
                ("(false != true) && ((true || false) || (true == false))", "", "", false),
                ("-7 != 7", "", "", false),
                ("-7 == -7", "", "", false),
                ("7 <= 7", "", "", false),
                ("7 >= 7", "", "", false),
                ("-7 < 7", "", "", false),
                ("7 > -7", "", "", false),
                ("-7 <= -7", "", "", false),
                ("(10000000000,00000000000 < 10000000000,00000000001)", "", "", false),
                ("\"text123\" == \"text123\"", "", "", false),
                ("(\"text123\" == \"text123\") && (7 <= 8)", "", "", false)
            };

            // Expecting errors (exceptions) for the following tests:
            var testSet2 = new (string expression, string expectedResult, string actualResult, bool success)[]
            {
                ("-7 && 7", "", "", false),
                ("2 < 3 < 4", "", "", false),
                ("boom == boom", "", "", false),
                ("\"bla\" == true", "", "", false),
                ("\"bla\" == \"bla\" == \"bla\"", "", "", false),
                ("boom!", "", "", false),
                ("(true", "", "", false),
                ("true)", "", "", false),
                ("()", "", "", false),
                (")true(", "", "", false)
            };

            Console.WriteLine("Completed");
            Console.WriteLine("");
            Console.WriteLine("");
            Console.WriteLine("--- Will now perform the 1st test set with Microsoft Scripting engine to collect expected results: ");

            // Pre-heating. Makes a big difference when everything has been loaded once before!
            // We do NOT need the Microsoft CSharpScript Engine! We use it here only to collect expected results for later comparison.
            string preHeatTestExp = "true == (true != false)";
            var preHeat1Result = CSharpScript.EvaluateAsync(preHeatTestExp).Result;

            var fail1Count = 0;
            var fail2Count = 0;
            var start_tests1 = DateTime.Now;

            for (var i = 0; i < testSet1.Length; i++)
            {
                // We do NOT need the Microsoft CSharpScript Engine! We use it here only to collect expected results for later comparison.
                var result = CSharpScript.EvaluateAsync(testSet1[i].expression).Result;

                if (result is ValueTuple<long, bool, int>)
                    testSet1[i].expectedResult = ((ValueTuple<long, bool, int>) result).Item2.ToString();
                else
                    testSet1[i].expectedResult = result.ToString();
            }

            var diff1 = DateTime.Now - start_tests1;

            Console.WriteLine($"Completed all:({testSet1.Length}) tests in:({diff1.TotalMilliseconds}) milliseconds");
            Console.WriteLine("");
            Console.WriteLine("");
            Console.WriteLine("--- Will now perform the 1st test set with our MicroExpressionEvaluator to compare results: ");

            // Pre-heating. Makes a big difference when everything has been loaded once before!
            // This is where we call our MicroExpressionEvaluator
            var preHeat2Result = MicroEx.Evaluate(preHeatTestExp);

            var start_tests2 = DateTime.Now;

            for (var i = 0; i < testSet1.Length; i++)
            {
                // This is where we call our MicroExpressionEvaluator
                testSet1[i].actualResult = MicroEx.Evaluate(testSet1[i].expression).ToString();

                if (testSet1[i].expectedResult == testSet1[i].actualResult)
                {
                    // Test passed!
                    testSet1[i].success = true;
                    continue;
                }

                fail1Count++;
            }

            var diff2 = DateTime.Now - start_tests2;

            Console.WriteLine($"Completed all:({testSet1.Length}) tests in:({diff2.TotalMilliseconds}) milliseconds. FailedCount:({fail1Count})");

            foreach (var testX in testSet1.Where(n => !n.success))
            {
                Console.WriteLine("");
                Console.WriteLine($"    ---FAIL!---");
                Console.WriteLine($"    expression:{testX.expression}");
                Console.WriteLine($"    expectedResult:{TruncStr(testX.expectedResult)}");
                Console.WriteLine($"    actualResult:{TruncStr(testX.actualResult)}");
            }

            Console.WriteLine("");
            Console.WriteLine("");
            Console.WriteLine("!!! The second test set is much slower because we expect exceptions from faulty expressions.");
            Console.WriteLine("!!! Catching exceptions is very slow in .net");
            Console.WriteLine("");
            Console.WriteLine("");
            Console.WriteLine("--- Will now perform the 2nd test set with Microsoft Scripting engine to collect expected results: ");

            var start_tests3 = DateTime.Now;

            for (var i = 0; i < testSet2.Length; i++)
            {
                try
                {
                    testSet2[i].expectedResult = CSharpScript.EvaluateAsync(testSet2[i].expression).Result.ToString();
                }
                catch (Exception e)
                {
                    testSet2[i].expectedResult = $"Exception: {e.Message}";
                }
            }

            var diff3 = DateTime.Now - start_tests3;

            Console.WriteLine($"Completed all:({testSet2.Length}) tests in:({diff3.TotalMilliseconds}) milliseconds");
            Console.WriteLine("");
            Console.WriteLine("");
            Console.WriteLine("--- Will now perform the 2nd test set with our MicroExpressionEvaluator to compare results: ");

            var start_tests4 = DateTime.Now;

            for (var i = 0; i < testSet2.Length; i++)
            {
                try
                {
                    // This is where we call our MicroExpressionEvaluator
                    testSet2[i].actualResult = MicroEx.Evaluate(testSet2[i].expression).ToString();
                }
                catch (Exception e)
                {
                    testSet2[i].actualResult = $"Exception: {e.Message}";
                }

                if (testSet2[i].expectedResult == testSet2[i].actualResult)
                {
                    // Test passed!
                    testSet2[i].success = true;
                    continue;
                }

                if (testSet2[i].expectedResult.StartsWith("Exception:") && testSet2[i].actualResult.StartsWith("Exception:"))
                {
                    // Test passed!
                    //ToDo: Maybe check and compare exception messages to use similar wording...
                    testSet2[i].success = true;
                    continue;
                }

                fail2Count++;
            }

            var diff4 = DateTime.Now - start_tests4;

            Console.WriteLine($"Completed all:({testSet2.Length}) tests in:({diff4.TotalMilliseconds}) milliseconds. FailedCount:({fail2Count})");

            foreach (var testX in testSet2.Where(n => !n.success))
            {
                Console.WriteLine("");
                Console.WriteLine($"    ---FAIL!---");
                Console.WriteLine($"    expression:{testX.expression}");
                Console.WriteLine($"    expectedResult:{TruncStr(testX.expectedResult)}");
                Console.WriteLine($"    actualResult:{TruncStr(testX.actualResult)}");
            }

            Console.WriteLine("");
            Console.WriteLine("");
            Console.WriteLine("---Testing completed---");
            Console.WriteLine("-----------------------");

            Console.ReadLine();
        }

        public static string TruncStr(string val)
        {
            if (string.IsNullOrWhiteSpace(val))
                return val;

            int max = 60;
            return val.Length <= max ? val : val.Substring(0, max);
        }
    }
}