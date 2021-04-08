BasicSharp
====
Simple BASIC interpreter written in C#. Language syntax is modernize version of BASIC, see example below.

MenMachine69
====

I have forked Timu5's BasicSharp project to add more functionality and especially a way to write graphics related BASIC applications for learning purposes. The goal is to create a BASIC-like programming application for beginners in computer programming that works a bit like the classic BASIC systems of the 80s (like Commodore, Atari, etc.).

I believe that such a system could give the audience the fun of programming that I had in the 80's and help much better than systems like Scratch to learn programming (in aspects of problem analysis, abstraction and simplicity).


Example
-------
print "Hello World"

let a = 10
print "Variable a: " + a

let b = 20
print "a+b=" + (a+b)

if a = 10 then
    print "True"
else
    print "False"
endif

for i = 1 to 10
    print i
next i

goto mylabel
print "False"

mylabel:
Print "True"


BASIC Reference
===============
List of all supported Commands and functions.

Keywords
--------
**LET** 
    
**PRINT** 
Output a expression, a string and/or a value without linebreak.

**PRINTL**
Output a expression, a string and/or a value with linebreak.

**CLS**
Clear console output and set position to top-left (row 1, col 1).

**POS**
Set output position to a specific row and column. In combination with PRINT this allows output on a specific position in the console window.

**SLEEP**
Execution interrupts for the given milliseconds.

**FREAD**
Read content of a file and save the contet into variable.

**FWRITE**
Write content to a file.

**IF THEN ELSE ENDIF***

**FOR TO STEP NEXT**

**GOTO**

**INPUT**

**INKEY**

**GOSUB RETURN**

**REM**

**ASSERT**

**END**

**OR** 

**AND** 

**NOT**

Build in functions
------------------




    
    
    
