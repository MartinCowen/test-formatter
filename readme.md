# TestFormatter
This utility converts between different formats for unit tests. It uses a simple text substitution method rather than a code parser so you need to be aware of the limitations and check the results, but it can still save a lot of time when converting many tests.

## Conversions
 - MSTest C++ to minunit C
 - minunit C to MSTest C++

## Limitations
 - can only deal with Assert::AreEqual or Assert::AreNotEqual
 - expects single spaces between keywords
 - only works with numeric comparisons, not strings, nulls etc


## Usage
Conversion is run on every character change in the left pane or on change of selected types, so there is no Run button.
 - Paste your source code snippet into the left pane. It is automatically formatted as per the option selected into the right pane. 
 - Click Copy button and paste into your code. This uses your clipboard.
 - Clear button clears both panes.
 - Class text box is added to function names and mu_assert messages when converting to minuint, or is removed from test method names when converting to MSTest.

The window is resizable and the pane splitter is movable.

## Installation
No installer is provided because the program consists of a single executable with one dependent dll. It assumes the standard .NET 5 library is available. The executable file is at

https://github.com/MartinCowen/

## Source Code
Written in VB.NET as a WinForms application using .NET Framework 5 in Visual Studio 2019
No third party components are used.



