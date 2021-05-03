CLS
Start:
PRINT "Hello, whats your name? "
INPUT name
PRINTL "Hello " + name 
PRINTL "Start again? (j/n)"
INKEY again
IF again = 74 THEN 
   GOTO Start
ELSE
   GOSUB Ende
   PRINT "Thats "
ENDIF
PRINT "it"
END

SUB Ende
   PRINTL "Bye Bye"
   PRINTL "Say something"
RETURN