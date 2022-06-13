//
//     Webermania
//     © 2021 Christopher Weber
//     Apache-2.0 License
//     https://github.com/webermania/MicroExpressionEvaluator
//     https://www.nuget.org/packages/MicroExpressionEvaluator
//
//     This ("MicroExpressionEvaluator") interprets and evaluates logic expressions represented as
//     string (very quickly) and returns evaluation success or throws Exception with clear Error message.
//

using System;
using System.Linq;

namespace MicroExpressionEvaluatorDotNetFramework
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
            if (string.IsNullOrWhiteSpace(expression))
                throw new Exception("Invalid input! Empty expression.");

            return SimplifyAndSolveExpression(expression);
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
            var containsOpenBracket = expr.Contains('(');
            var containsCloseBracket = expr.Contains(')');

            if (containsOpenBracket && !containsCloseBracket)
                throw new Exception($"Invalid input:'{expr}'! ) expected.");

            if (!containsOpenBracket && containsCloseBracket)
                throw new Exception($"Invalid input:'{expr}'! ( expected.");

            if (containsOpenBracket)
            {
                var potentialGroups = expr.Split(new string[] { "(" }, StringSplitOptions.RemoveEmptyEntries);

                foreach (var potentialGroupX in potentialGroups)
                {
                    if (potentialGroupX.StartsWith(")") && potentialGroupX.Length > 1)
                        throw new Exception($"Invalid input:'{potentialGroupX}'! ( expected.");

                    if (potentialGroupX.StartsWith(")"))
                        throw new Exception("Invalid input! Group has no value ().");

                    var nextOpenBracket = potentialGroupX.IndexOf('(');
                    var nextCloseBracket = potentialGroupX.IndexOf(')');

                    if (nextCloseBracket == -1 || nextOpenBracket != -1 && nextOpenBracket < nextCloseBracket)
                        continue;

                    // found a (group) that has (no (deeper nested) group)
                    var subExpr = potentialGroupX.Substring(0, nextCloseBracket);
                    var subResult = SplitAndValidateLogicalOperators(subExpr);

                    var simplifiedExpr = expr.Replace($"({subExpr})", subResult.ToString().ToLower());
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
            //return source.Split(separator, 2, StringSplitOptions.RemoveEmptyEntries);

            if (string.IsNullOrWhiteSpace(source))
                return new string[] { };

            source = source.Trim();

            if (string.IsNullOrWhiteSpace(separator))
                return new string[] { source };

            if (!source.Contains(separator))
                return new string[] { source };

            var position = source.IndexOf(separator, 0, StringComparison);
            var before = source.Substring(0, position).Trim();
            var after = source.Substring(position + separator.Length).Trim();

            if (string.IsNullOrWhiteSpace(after))
                return new string[] { before };

            return new string[] { before, after };
        }

        /// <summary>
        ///     Splits the problem respecting the correct operator precedence.
        ///     (Testsed against C# implementation.)
        /// </summary>
        private static bool SplitAndValidateLogicalOperators(string expr)
        {
            // ||
            var op1 = SplitOnce(expr, "||");
            if (op1.Length > 1) return op1.Any(SimplifyAndSolveExpression);

            // &&
            var op2 = SplitOnce(expr, "&&");
            if (op2.Length > 1) return op2.All(SimplifyAndSolveExpression);

            // !=
            var op3 = SplitOnce(expr, "!=");
            if (op3.Length > 1)
            {
                var part1GoesDeeper = ContainsAnyOperators(op3[0]);
                var part2GoesDeeper = ContainsAnyOperators(op3[1]);

                if (part1GoesDeeper || part2GoesDeeper)
                {
                    var resultPart1 = part1GoesDeeper ? SimplifyAndSolveExpression(op3[0]) : ValidateBool(op3[0]);
                    var resultPart2 = part2GoesDeeper ? SimplifyAndSolveExpression(op3[1]) : ValidateBool(op3[1]);

                    return resultPart1 != resultPart2;
                }

                return !ValidateEquality(op3[0], op3[1]);
            }

            // ==
            var op4 = SplitOnce(expr, "==");
            if (op4.Length > 1)
            {
                var part1GoesDeeper = ContainsAnyOperators(op4[0]);
                var part2GoesDeeper = ContainsAnyOperators(op4[1]);

                if (part1GoesDeeper || part2GoesDeeper)
                {
                    var resultPart1 = part1GoesDeeper ? SimplifyAndSolveExpression(op4[0]) : ValidateBool(op4[0]);
                    var resultPart2 = part2GoesDeeper ? SimplifyAndSolveExpression(op4[1]) : ValidateBool(op4[1]);

                    return resultPart1 == resultPart2;
                }

                return ValidateEquality(op4[0], op4[1]);
            }

            // >=
            var op5 = SplitOnce(expr, ">=");
            if (op5.Length > 1)
            {
                var part1GoesDeeper = ContainsAnyOperators(op5[0]);
                var part2GoesDeeper = ContainsAnyOperators(op5[1]);

                if (part1GoesDeeper || part2GoesDeeper)
                    throw new Exception(
                        $"Invalid input:'{expr}'! Operator '>=' cannot be applied to operands of type 'bool' and 'object'.");

                return Convert.ToDecimal(op5[0]) >= Convert.ToDecimal(op5[1]);
            }

            // <=
            var op6 = SplitOnce(expr, "<=");
            if (op6.Length > 1)
            {
                var part1GoesDeeper = ContainsAnyOperators(op6[0]);
                var part2GoesDeeper = ContainsAnyOperators(op6[1]);

                if (part1GoesDeeper || part2GoesDeeper)
                    throw new Exception(
                        $"Invalid input:'{expr}'! Operator '<=' cannot be applied to operands of type 'bool' and 'unknown object'.");

                return Convert.ToDecimal(op6[0]) <= Convert.ToDecimal(op6[1]);
            }

            // >
            var op7 = SplitOnce(expr, ">");
            if (op7.Length > 1)
            {
                var part1GoesDeeper = ContainsAnyOperators(op7[0]);
                var part2GoesDeeper = ContainsAnyOperators(op7[1]);

                if (part1GoesDeeper || part2GoesDeeper)
                    throw new Exception(
                        $"Invalid input:'{expr}'! Operator '>' cannot be applied to operands of type 'bool' and 'unknown object'.");

                return Convert.ToDecimal(op7[0]) > Convert.ToDecimal(op7[1]);
            }

            // <
            var op8 = SplitOnce(expr, "<");
            if (op8.Length > 1)
            {
                var part1GoesDeeper = ContainsAnyOperators(op8[0]);
                var part2GoesDeeper = ContainsAnyOperators(op8[1]);

                if (part1GoesDeeper || part2GoesDeeper)
                    throw new Exception(
                        $"Invalid input:'{expr}'! Operator '<' cannot be applied to operands of type 'bool' and 'unknown object'.");

                return Convert.ToDecimal(op8[0]) < Convert.ToDecimal(op8[1]);
            }

            return ValidateBool(expr);
        }

        private static bool ValidateBool(string val)
        {
            val = val.Trim();

            if (val.StartsWith("!"))
                return !ValidateBool(val.Substring(1, val.Length - 1));

            // .net System.Convert.ToBoolean no good here
            if (val.Equals("true", StringComparison))
                return true;

            if (val.Equals("false", StringComparison))
                return false;

            throw new Exception($"String '{val}' was not recognized as a valid Boolean.");
        }

        private static bool ValidateEquality(string val1, string val2)
        {
            val1 = val1.Trim();
            val2 = val2.Trim();

            var val1HasStringFlag = val1.StartsWith("\"") && val1.EndsWith("\"");
            var val2HasStringFlag = val2.StartsWith("\"") && val2.EndsWith("\"");

            if (val1HasStringFlag != val2HasStringFlag)
                throw new Exception(
                    $"Invalid input i1:'{val1}' i2:'{val1}'! Operator cannot be applied to operands of type 'string' and 'unknown object'.");

            if (val1HasStringFlag)
                return val1.Equals(val2, StringComparison);

            var val1IsDec = decimal.TryParse(val1, out var val1Dec);
            var val2IsDec = decimal.TryParse(val2, out var val2Dec);

            if (val1IsDec != val2IsDec)
                throw new Exception(
                    $"Invalid input i1:'{val1}' i2:'{val1}'! Operator cannot be applied to operands of type 'decimal' and 'unknown object'.");

            if (val1IsDec)
                return val1Dec == val2Dec;

            return ValidateBool(val1) == ValidateBool(val2);
        }
    }
}
