# MicroExpressionEvaluator

## High-Performance C# Logical Expression Parser and Evaluator

**MicroExpressionEvaluator** is a blazing-fast, lightweight .NET library for parsing and evaluating complex nested logical expressions in C#. Designed for real-time applications, it delivers unparalleled performance with minimal CPU usage, making it perfect for dynamic rule evaluation in financial modeling, gaming logic, automation systems, and more.

---

## Table of Contents

- [Features](#features)
- [Why MicroExpressionEvaluator?](#why-microexpressionevaluator)
- [Usage](#usage)
- [Examples](#examples)
- [Real-World Applications](#real-world-applications)
- [Performance Comparison](#performance-comparison)
- [Roadmap](#roadmap)
- [Contributing](#contributing)

---

## Features

- **Blazing Fast Performance**: Executes complex logical expressions with minimal CPU overhead.
- **Multi-Type Support**: Handles `decimal`, `bool`, and `string` data types.
- **Comprehensive Operator Support**: Includes logical (`||`, `&&`, `!`) and comparison (`==`, `!=`, `<`, `>`, `<=`, `>=`) operators.
- **Nested and Grouped Expressions**: Efficiently evaluates expressions with multiple nesting levels and logical groupings.
- **Secure and Lightweight**: No reliance on regular expressions or scripting engines, reducing security risks and external dependencies.

## Why MicroExpressionEvaluator?

MicroExpressionEvaluator significantly outperforms traditional methods:

- **Up to 100x Faster Than Regular Expressions**: Avoids the overhead of regex-based parsing.
- **Up to 1000x Faster Than Scripting Engines**: Eliminates latency from external scripting engines like Roslyn or NLua.
- **Optimized for Real-Time Applications**: Ideal for scenarios requiring instantaneous logic evaluation without compromising accuracy.

## Usage

Using MicroExpressionEvaluator is straightforward. Pass your logical expression as a string to the `Evaluate` method:

```csharp
using MicroExpressionEvaluator;

bool result = MicroEx.Evaluate("your_logical_expression");
```

### Example

```csharp
bool result = MicroEx.Evaluate("false != true && !(-0.2 <= 0.1 || \"a\" == \"b\")");
// result will be true
```

## Examples

### Valid Expressions

Expression | Output
--- | ---
`7 > -7` | `true`
`true == true != false` | `true`
`(true \|\| true) && false` | `false`
`!true == !(true \|\| false)` | `true`
`("text123" == "text123") && (7.00000 < 7.00001)` | `true`
`10000000000.00000000000 < 10000000000.00000000001` | `true`
`(false != true) && ((true \|\| false) \|\| (true == false))` | `true`
`(false \|\| (true == (false \|\| (true == (false \|\| true)))))` | `true`

### Invalid Expressions

Invalid expressions throw descriptive exceptions:

Expression | Exception Message
--- | ---
`(true` | `Invalid input! ')' expected.`
`-7 && 7` | `Invalid input! Operator '&&' can only be applied to operands of type 'bool'.`
`"text123" == true` | `Invalid input! Operator '==' can only be applied to operands of type 'string' or 'bool'.`

## Real-World Applications

- **Financial Modeling**: Real-time evaluation of complex financial conditions and business rules.
- **Game Development**: Efficient processing of game logic and AI decision-making.
- **Automation Systems**: Dynamic decision-making in automated workflows and processes.
- **System Monitoring**: Rapid validation and enforcement of system configurations and policies.

## Performance Comparison

Compared to other tools like **NCalc**, **Jint**, **NLua**, and **Roslyn**, MicroExpressionEvaluator offers:

- **Superior Speed**: Outperforms in execution time, making it ideal for performance-critical applications.
- **Enhanced Security**: Reduces risks by not relying on external scripting engines or regular expressions.
- **Lightweight Footprint**: Minimal dependencies ensure easy integration and deployment.

## Roadmap

Upcoming enhancements:

- **Mathematical Operations**: Support for arithmetic operators (`+`, `-`, `*`, `/`, `^`, `%`).
- **Extended Data Types**: Enhanced handling of various numeric types and collections.
- **Custom Functions and Variables**: Ability to define custom functions and variables within expressions.

## Contributing

We welcome contributions! If you have ideas, suggestions, or encounter any issues, please open an [issue](https://github.com/webermania/MicroExpressionEvaluator/issues) or submit a pull request.


---

**We appreciate your feedback and hope MicroExpressionEvaluator becomes an invaluable tool in your projects!**
