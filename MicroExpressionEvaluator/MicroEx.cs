//
//     Webermania
//     © 2024 Christopher Weber
//     Apache-2.0 License
//     https://github.com/webermania/MicroExpressionEvaluator
//     https://www.nuget.org/packages/MicroExpressionEvaluator
//
//     This ("MicroExpressionEvaluator") interprets and evaluates logic expressions represented as
//     string (very quickly) and returns evaluation success or throws Exception with clear Error message.
//

using System;
using System.Linq;

namespace MicroExpressionEvaluator
{
    public static class MicroEx
    {
        public static StringComparison StringComparison { get; set; } = StringComparison.Ordinal;

        /// <summary>
        /// interprets and evaluates logic expressions represented as string
        /// </summary>
        /// <param name="expression">Input expression such as "(\"text123\" == \"text123\") && (7 <= 8)"</param>
        /// <returns>returns evaluation success as bool or throws Exception with clear a Error message
        public static bool Evaluate(string expression)
        {
            return string.IsNullOrWhiteSpace(expression)
                ? throw new Exception("Invalid input! Empty expression.")
                : SimplifyAndSolveExpression(expression);
        }

        private static bool ContainsAnyOperators(string expr)
        {
            return new[] { "||", "&&", "!=", "==", "<=", ">=", "<", ">" }.Any(expr.Contains);
        }

        /// <summary>
        ///     Solves nested groups from the ((((inside)))) out
        /// </summary>
        private static bool SimplifyAndSolveExpression(string expr)
        {
            bool containsOpenBracket = expr.Contains('(');
            bool containsCloseBracket = expr.Contains(')');

            if (containsOpenBracket && !containsCloseBracket)
            {
                throw new Exception($"Invalid input:'{expr}'! ) expected.");
            }

            if (!containsOpenBracket && containsCloseBracket)
            {
                throw new Exception($"Invalid input:'{expr}'! ( expected.");
            }

            if (containsOpenBracket)
            {
                string[] potentialGroups = expr.Split(new string[] { "(" }, StringSplitOptions.RemoveEmptyEntries);

                foreach (string potentialGroupX in potentialGroups)
                {
                    if (potentialGroupX.StartsWith(")") && potentialGroupX.Length > 1)
                    {
                        throw new Exception($"Invalid input:'{potentialGroupX}'! ( expected.");
                    }

                    if (potentialGroupX.StartsWith(")"))
                    {
                        throw new Exception("Invalid input! Group has no value ().");
                    }

                    int nextOpenBracket = potentialGroupX.IndexOf('(');
                    int nextCloseBracket = potentialGroupX.IndexOf(')');

                    if (nextCloseBracket == -1 || (nextOpenBracket != -1 && nextOpenBracket < nextCloseBracket))
                    {
                        continue;
                    }

                    // found a (group) that has (no (deeper nested) group)
                    string subExpr = potentialGroupX.Substring(0, nextCloseBracket);
                    bool subResult = SplitAndValidateLogicalOperators(subExpr);

                    string simplifiedExpr = expr.Replace($"({subExpr})", subResult.ToString().ToLower());
                    return SimplifyAndSolveExpression(simplifiedExpr);
                }
            }

            return SplitAndValidateLogicalOperators(expr);
        }

        /// <summary>
        ///     Fast and simple way to split into only two pieces
        /// </summary>
        /// <param name="source">string to be split</param>
        /// <param name="separator">what to split by</param>
        /// <returns>string array of result(s)</returns>
        private static string[] SplitOnce(string source, string separator)
        {
            if (string.IsNullOrEmpty(source))
            {
                return Array.Empty<string>();
            }

            if (string.IsNullOrEmpty(separator))
            {
                return new string[] { source.Trim() };
            }

            int position = source.IndexOf(separator, StringComparison.Ordinal);
            if (position == -1)
            {
                return new string[] { source.Trim() };
            }

            string before = source.Substring(0, position);
            string after = source.Substring(position + separator.Length);

            return new string[] { before.Trim(), after.Trim() };
        }

        /// <summary>
        ///     Splits the problem respecting the correct operator precedence.
        ///     (Testsed against C# implementation.)
        /// </summary>
        private static bool SplitAndValidateLogicalOperators(string expr)
        {
            // ||
            string[] op1 = SplitOnce(expr, "||");
            if (op1.Length > 1)
            {
                return op1.Any(SimplifyAndSolveExpression);
            }

            // &&
            string[] op2 = SplitOnce(expr, "&&");
            if (op2.Length > 1)
            {
                return op2.All(SimplifyAndSolveExpression);
            }

            // !=
            string[] op3 = SplitOnce(expr, "!=");
            if (op3.Length > 1)
            {
                bool part1GoesDeeper = ContainsAnyOperators(op3[0]);
                bool part2GoesDeeper = ContainsAnyOperators(op3[1]);

                if (part1GoesDeeper || part2GoesDeeper)
                {
                    bool resultPart1 = part1GoesDeeper ? SimplifyAndSolveExpression(op3[0]) : ValidateBool(op3[0]);
                    bool resultPart2 = part2GoesDeeper ? SimplifyAndSolveExpression(op3[1]) : ValidateBool(op3[1]);

                    return resultPart1 != resultPart2;
                }

                return !ValidateEquality(op3[0], op3[1]);
            }

            // ==
            string[] op4 = SplitOnce(expr, "==");
            if (op4.Length > 1)
            {
                bool part1GoesDeeper = ContainsAnyOperators(op4[0]);
                bool part2GoesDeeper = ContainsAnyOperators(op4[1]);

                if (part1GoesDeeper || part2GoesDeeper)
                {
                    bool resultPart1 = part1GoesDeeper ? SimplifyAndSolveExpression(op4[0]) : ValidateBool(op4[0]);
                    bool resultPart2 = part2GoesDeeper ? SimplifyAndSolveExpression(op4[1]) : ValidateBool(op4[1]);

                    return resultPart1 == resultPart2;
                }

                return ValidateEquality(op4[0], op4[1]);
            }

            // >=
            string[] op5 = SplitOnce(expr, ">=");
            if (op5.Length > 1)
            {
                bool part1GoesDeeper = ContainsAnyOperators(op5[0]);
                bool part2GoesDeeper = ContainsAnyOperators(op5[1]);

                return part1GoesDeeper || part2GoesDeeper
                    ? throw new Exception(
                        $"Invalid input:'{expr}'! Operator '>=' cannot be applied to operands of type 'bool' and 'object'.")
                    : Convert.ToDecimal(op5[0]) >= Convert.ToDecimal(op5[1]);
            }

            // <=
            string[] op6 = SplitOnce(expr, "<=");
            if (op6.Length > 1)
            {
                bool part1GoesDeeper = ContainsAnyOperators(op6[0]);
                bool part2GoesDeeper = ContainsAnyOperators(op6[1]);

                return part1GoesDeeper || part2GoesDeeper
                    ? throw new Exception(
                        $"Invalid input:'{expr}'! Operator '<=' cannot be applied to operands of type 'bool' and 'unknown object'.")
                    : Convert.ToDecimal(op6[0]) <= Convert.ToDecimal(op6[1]);
            }

            // >
            string[] op7 = SplitOnce(expr, ">");
            if (op7.Length > 1)
            {
                bool part1GoesDeeper = ContainsAnyOperators(op7[0]);
                bool part2GoesDeeper = ContainsAnyOperators(op7[1]);

                return part1GoesDeeper || part2GoesDeeper
                    ? throw new Exception(
                        $"Invalid input:'{expr}'! Operator '>' cannot be applied to operands of type 'bool' and 'unknown object'.")
                    : Convert.ToDecimal(op7[0]) > Convert.ToDecimal(op7[1]);
            }

            // <
            string[] op8 = SplitOnce(expr, "<");
            if (op8.Length > 1)
            {
                bool part1GoesDeeper = ContainsAnyOperators(op8[0]);
                bool part2GoesDeeper = ContainsAnyOperators(op8[1]);

                return part1GoesDeeper || part2GoesDeeper
                    ? throw new Exception(
                        $"Invalid input:'{expr}'! Operator '<' cannot be applied to operands of type 'bool' and 'unknown object'.")
                    : Convert.ToDecimal(op8[0]) < Convert.ToDecimal(op8[1]);
            }

            return ValidateBool(expr);
        }

        private static bool ValidateBool(string val)
        {
            if (string.IsNullOrEmpty(val))
            {
                throw new ArgumentException("Input string is null or empty.");
            }

            val = val.Trim();

            if (val.Length == 0)
            {
                throw new ArgumentException("Input string is only whitespace.");
            }

            if (val[0] == '!')
            {
                return !ValidateBool(val.Substring(1));
            }

            // Using StringComparison.OrdinalIgnoreCase for case-insensitive comparison
            return val.Equals("true", StringComparison.OrdinalIgnoreCase) || (val.Equals("false", StringComparison.OrdinalIgnoreCase)
                    ? false
                    : throw new ArgumentException($"String '{val}' was not recognized as a valid Boolean."));
        }

        private static bool ValidateEquality(string val1, string val2)
        {
            val1 = val1.Trim();
            val2 = val2.Trim();

            bool val1HasStringFlag = val1.StartsWith("\"") && val1.EndsWith("\"");
            bool val2HasStringFlag = val2.StartsWith("\"") && val2.EndsWith("\"");

            if (val1HasStringFlag != val2HasStringFlag)
            {
                throw new Exception(
                    $"Invalid input i1:'{val1}' i2:'{val1}'! Operator cannot be applied to operands of type 'string' and 'unknown object'.");
            }

            if (val1HasStringFlag)
            {
                return val1.Equals(val2, StringComparison);
            }

            bool val1IsDec = decimal.TryParse(val1, out decimal val1Dec);
            bool val2IsDec = decimal.TryParse(val2, out decimal val2Dec);

            return val1IsDec != val2IsDec
                ? throw new Exception(
                    $"Invalid input i1:'{val1}' i2:'{val1}'! Operator cannot be applied to operands of type 'decimal' and 'unknown object'.")
                : val1IsDec ? val1Dec == val2Dec : ValidateBool(val1) == ValidateBool(val2);
        }
    }
}
