# MicroExpressionEvaluator
Parse, interpret and evaluate nested conditional logic expressions and operators represented as string (very quickly) and return result or error. Sample input: false != true && !(-0,2 <= 0,1 || "a" == "b") 

## My problem
I needed something **`really fast (and low on CPU)`** at solving rules with dynamic conditions.  
In my case, for dynamic and configurable rules that need to be constantly re-checked at (near) real-time.  
Source of my expressions can be local or remote, contained in a json, yaml, xml, hcl or any format.

## My solution
I wrote this .Net class library (in [one simple class], in under 250 lines of code, with help of recursive functions) to evaluate logic expressions.  
  
It can currently deal with types: `{ decimal, boolean, string }`.  
It understands the following operators: `{ "||", "&&", "!=", "==", "<=", ">=", "<", ">" }`  
and logical NOT `{ "!" }`.  

It ((can **simplify** and solve (multiple (**nested**))) and (separate (**logic groups**))) from the ((((**inside**)))) **out**.  
It splits the problem respecting the correct operator precedence (tested this implementation against C# implementation).  
  
Runs this [test class] and evaluates all **74** test expressions in total of **~0,2 milliseconds!** Even on my low-end Intel i5.  
It uses **no Regular expressions** *(~100x)* and **no scripting engines** *(~1000x)* as these are **way too slow**!  
It's no C++ but I find it pretty good performance wise.  

The [test class] evaluates and compares our results and performance of all 74 tests against the ".NET Compiler Platform" (aka "Roslyn") ("Microsoft.CodeAnalysis.CSharp.Scripting.EvaluateAsync()")

## Example **In-** *--and-->* **Output**
It is **very low on CPU and super-fast** at solving expressions such as:
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

## Usage
as simple as:
```sh
bool result = MicroEx.Evaluate("(true || false) && false");
```

## Real life examples
For example, I would have rules such as the following and replace the placeholders `%xxx%` with live values from variables then evaluate:
```sh
%CXB% == true && (%OOP% == "ACTIVE" || %OOP% == "STANDBY")
%PRE% >= 29,25 && %PRE% <= 78,5
```

## Info
It is purpose build and therefore also safer than using scripting engines. Scripting engines potentially allow scripts to cause harm and to break-out. This could be used to hijack your machine by injecting malicious payloads. At least in my use-cases this is something I needed to worry about.  
  
It required **no extra libs, or 3rd party packages**.

It can easily be adapted to other .Net versions or languages (TBD).  
  
For now, it only does logic and cannot yet solve math problems (TBD).  
Adding other operators, as for example: `{ "+", "-", "*", "/", "^", "%" }` to solve simple (or more complex) math would be easy using this as a base framework.

## Alternatives
These really cool projects did not work for me, or were just **too slow**:
-   NCalc
-   Jint
-   NLua
-   Dynamic Expresso
-   Flee
-   CS-Script
-   Roslyn (Microsoft)
-   MathParser
-   Eval Expression.NET
-   ...

## Closing words
I created this a while ago, but finally managed to make the repo public.  
**I hope this can help you!!**  
Enjoy and let me know what you like, dislike or what you would do differently  

   [one simple class]: <https://github.com/webermania/MicroExpressionEvaluator/blob/master/MicroExpressionEvaluatorDotNetFramework/MicroEx.cs>
   [test class]: <https://github.com/webermania/MicroExpressionEvaluator/blob/master/TestAndDemo/Program.cs>
