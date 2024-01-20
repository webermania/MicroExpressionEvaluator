
# C# MicroExpressionEvaluator: High-Performance Logic Parsing for Real-Time Applications

## Overview

The MicroExpressionEvaluator is a highly efficient .NET class library, optimized for parsing and evaluating complex nested conditional logic expressions in C#. Tailored for real-time scenarios, it offers unparalleled performance with minimal CPU usage, ideal for dynamic rule evaluation in various applications like financial modeling, gaming logic, and more.

## Key Features

- **Rapid Evaluation**: Executes complex logic expressions extremely fast.
- **Type Support**: Handles expressions involving `decimal`, `boolean`, and `string`.
- **Operator Versatility**: Supports logical (`||`, `&&`, `!`, etc.) and comparison (`==`, `!=`, `<`, `>`, etc.) operators.
- **Nested and Grouped Expressions**: Efficiently simplifies and evaluates expressions with multiple nested and logical groups.
- **Security and Independence**: Operates without regular expressions or scripting engines, reducing security risks and dependencies.

## Performance Advantage

Benchmarked to outperform traditional methods and tools, the MicroExpressionEvaluator avoids slow methods like regular expressions (about **100x** faster) and scripting engines (around **1000x** faster), making it a superior choice for applications requiring high-speed logic processing.

## Usage

Simple and straightforward: 

```csharp
bool result = MicroEx.Evaluate("your_expression_here");
```

Replace `your_expression_here` with your logic expression.

## Examples
**Input** *-->* **Output**
```sh
7 > -7                                                      -->   Returns: true
true == true != false                                       -->   Returns: true
(true || true) && false                                     -->   Returns: false
!true == !(true || false)                                   -->   Returns: true
("text123" == "text123") && (7,00000 < 7,00001)             -->   Returns: true
10000000000,00000000000 < 10000000000,00000000001           -->   Returns: true
(false != true) && ((true || false) || (true == false))     -->   Returns: true
(false || (true == (false || (true == (false || true)))))   -->   Returns: true
(true                                                       -->   Throws (expected) exception: "Invalid input! ) expected."
-7 && 7                                                     -->   Throws (expected) exception: "Invalid input! Operator can only be applied to operands of type 'bool'."
"text123" == true                                           -->   Throws (expected) exception: "Invalid input! Operator can only be applied to operands of type 'bool'."
```

## Real-World Applications

Perfect for systems requiring constant rule evaluation under varying conditions, such as in automated decision-making, system monitoring, or configuration validation.

## Alternatives Considered

Other projects like NCalc, Jint, NLua, and Roslyn were evaluated but found to be slower or less suitable for the specific needs.

## Future Directions

Plans to expand capabilities include support for mathematical operations and additional operators (`+`, `-`, `*`, `/`, `^`, `%`).

## Contributions

Your contributions, issues, and suggestions are welcome to enhance this project further.

## Licensing

Please refer to the provided license information for usage guidelines.

**We welcome your feedback and hope you find the MicroExpressionEvaluator invaluable in your projects!**
