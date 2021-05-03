using System;
using System.Collections;

namespace BasicSharp
{
    public class Lexer
    {
        private readonly string source;
        private Marker sourceMarker; // current position in source string
        private char lastChar;
        private Stack subReturnMarker = new Stack(); 

        public Marker TokenMarker { get; set; }

        public string Identifier { get; set; } // Last encountered identifier
        public Value Value { get; set; } // Last number or string

        public Lexer(string input)
        {
            source = input;
            sourceMarker = new Marker(0, 1, 1);
            lastChar = source[0];
        }

        public void GoTo(Marker marker)
        {
            if (subReturnMarker.Count > 0)
                throw new Exception("Goto isn't allowed from inside a subroutine.");

            sourceMarker = marker;
        }

        public void Rewind(Marker marker)
        {
            sourceMarker = marker;
            lastChar = ' ';
        }

        public void GoSub(Marker marker, string name)
        {
            marker.Pointer += name.Length;
            sourceMarker = marker;
        }

        public void Return()
        {
            sourceMarker = (Marker)subReturnMarker.Pop();
            lastChar = '\n';
        }

        public void SaveReturnMarker()
        {
            subReturnMarker.Push(sourceMarker);
        }

        public string GetLine(Marker marker)
        {
            Marker oldMarker = sourceMarker;
            marker.Pointer--;
            GoTo(marker);

            string line = "";
            do
            {
                line += GetChar();
            } while (lastChar != '\n' && lastChar != (char)0);

            line.Remove(line.Length - 1);

            GoTo(oldMarker);

            return line;
        }

        char GetChar()
        {
            sourceMarker.Column++;
            sourceMarker.Pointer++;

            if (sourceMarker.Pointer >= source.Length)
                return lastChar = (char)0;

            if ((lastChar = source[sourceMarker.Pointer]) == '\n')
            {
                sourceMarker.Column = 1;
                sourceMarker.Line++;
            }
            return lastChar;
        }

        public Token GetToken()
        {
            // skip white chars
            while (lastChar == ' ' || lastChar == '\t' || lastChar == '\r')
                GetChar();

            if (lastChar == '#')
            {
                while (lastChar != '\n')
                    GetChar();
            }

            TokenMarker = sourceMarker;

            if (char.IsLetter(lastChar))
            {
                Identifier = lastChar.ToString();
                while (char.IsLetterOrDigit(GetChar()))
                    Identifier += lastChar;

                switch (Identifier.ToUpper())
                {
                    case "SETCOLOR": return Token.SetColor;
                    // Draw on canvas
                    case "SETCANVAS": return Token.SetCanvas;
                    case "LOADSPRITE": return Token.SpriteLoad;
                    case "DRAWSPRITE": return Token.SpriteDraw;
                    case "REMOVESPRITE": return Token.SpriteRemove;
                    case "DRAWLINE": return Token.DrawLine;
                    case "DRAWRECT": return Token.DrawRect;
                    case "DRAWELIPSE": return Token.DrawElipse;
                    case "FILLRECT": return Token.FillRect;
                    case "FILLELIPSE": return Token.FillElipse;
                    case "SETPIXEL": return Token.SetPixel;
                    case "GETPIXEL": return Token.GetPixel;

                    // sound via midi
                    case "SNDPLAY": return Token.SndPlay; // play one note
                    case "SNDSTOP": return Token.SndStop; // stop all sounds
                    case "SNDSELECT": return Token.SndSelect; // Select instrument for a channel
                    case "SNDVOLUME": return Token.SndVolume; // set sound volume

                    case "FREAD": return Token.FRead;
                    case "FWRITE": return Token.FWrite;
                    
                    case "LOCATE": return Token.Locate;
                    case "CLS": return Token.Cls;
                    case "SLEEP": return Token.Sleep;
                    case "PRINT": return Token.Print;
                    case "PRINTL": return Token.PrintL;
                    case "IF": return Token.If;
                    case "ENDIF": return Token.EndIf;
                    case "THEN": return Token.Then;
                    case "ELSE": return Token.Else;
                    case "WHILE": return Token.While;
                    case "DO": return Token.Do;
                    case "ENDDO": return Token.EndWhile;
                    case "FOR": return Token.For;
                    case "TO": return Token.To;
                    case "STEP": return Token.Step;
                    case "NEXT": return Token.Next;
                    case "GOTO": return Token.Goto;
                    case "INPUT": return Token.Input;
                    case "INKEY": return Token.Inkey;
                    case "LET": return Token.Let;
                    case "GOSUB": return Token.Gosub;
                    case "SUB": return Token.Sub;
                    case "RETURN": return Token.Return;
                    case "END": return Token.End;
                    case "OR": return Token.Or;
                    case "AND": return Token.And;
                    case "NOT": return Token.Not;
                    case "ASSERT": return Token.Assert;
                    case "REM":
                        while (lastChar != '\n') GetChar();
                        GetChar();
                        return GetToken();
                    default:
                        return Token.Identifier;
                }
            }

            if (char.IsDigit(lastChar))
            {
                string num = "";
                do { num += lastChar; } while (char.IsDigit(GetChar()) || lastChar == '.');

                double real;
                if (!double.TryParse(num, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out real))
                    throw new Exception("ERROR while parsing number");
                Value = new Value(real);
                return Token.Value;
            }

            Token tok = Token.Unknown;
            switch (lastChar)
            {
                case '\n': tok = Token.NewLine; break;
                case ':': tok = Token.Colon; break;
                case ';': tok = Token.Semicolon; break;
                case ',': tok = Token.Comma; break;
                case '=': tok = Token.Equal; break;
                case '+': tok = Token.Plus; break;
                case '-': tok = Token.Minus; break;
                case '/': tok = Token.Slash; break;
                case '*': tok = Token.Asterisk; break;
                case '^': tok = Token.Caret; break;
                case '(': tok = Token.LParen; break;
                case ')': tok = Token.RParen; break;
                case '<':
                    GetChar();
                    if (lastChar == '>') tok = Token.NotEqual;
                    else if (lastChar == '=') tok = Token.LessEqual;
                    else return Token.Less;
                    break;
                case '>':
                    GetChar();
                    if (lastChar == '=') tok = Token.MoreEqual;
                    else return Token.More;
                    break;
                case '\'':
                    Value = new Value(readStr('\''));
                    tok = Token.Value;
                    break;
                case '"':
                    Value = new Value(readStr('"'));
                    tok = Token.Value;
                    break;
                case (char)0:
                    return Token.EOF;
            }

            GetChar();
            return tok;
        }

        string readStr(char limiter)
        {
            string str = "";
            while (GetChar() != limiter)
            {
                if (lastChar == '\\')
                {
                    // parse \n, \t, \\, \", \'
                    switch (char.ToLower(GetChar()))
                    {
                        case 'n': str += '\n'; break;
                        case 't': str += '\t'; break;
                        case '\\': str += '\\'; break;
                        case '"': str += '"'; break;
                        case '\'': str += '\''; break;
                    }
                }
                else
                {
                    str += lastChar;
                }
            }

            return str;
        }
    }
}

