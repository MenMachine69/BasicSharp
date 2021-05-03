using BasicConsole;
using System;
using System.Collections.Generic;
using System.Threading;

namespace BasicSharp
{
    public class Interpreter
    {
        public bool HasPrint { get; set; } = true;
        public bool HasInput { get; set; } = true;

        public IConsole Console { get; set; } = new TextModeConsole();
        public CancellationToken CancelationToken { get; set; }


        private Lexer lex;
        private Token prevToken; // token before last one
        private Token lastToken; // last seen token

        private Dictionary<string, Value> vars; // all variables are stored here
        private Dictionary<string, Marker> labels; // already seen labels 
        private Dictionary<string, Marker> loops; // for loop
        private Stack<Marker[]> whiles; // stack (lifo) for while loops
        private Dictionary<string, Marker> subs; // for subs
        private Dictionary<string, int> steps; // all variables are stored here

        public delegate Value BasicFunction(Interpreter interpreter, List<Value> args);
        private Dictionary<string, BasicFunction> funcs; // all maped functions

        private int ifcounter; // counter used for matching "if" with "else"
        private int whilecounter; // counter used for matching "while do" with "enddo"

        private Marker lineMarker; // current line marker

        private bool exit; // do we need to exit?

        public Interpreter(string input)
        {
            lex = new Lexer(input);
            vars = new Dictionary<string, Value>();
            steps = new Dictionary<string, int>();
            labels = new Dictionary<string, Marker>();
            loops = new Dictionary<string, Marker>();
            whiles = new Stack<Marker[]>();
            funcs = new Dictionary<string, BasicFunction>();
            subs = new Dictionary<string, Marker>();
            ifcounter = 0;
            whilecounter = 0;
            BuiltIns.InstallAll(this); // map all builtins functions
        }

        public Value GetVar(string name)
        {
            if (!vars.ContainsKey(name))
                throw new Exception("Variable with name " + name + " does not exist.");
            return vars[name];
        }

        public void SetVar(string name, Value val)
        {
            if (!vars.ContainsKey(name)) vars.Add(name, val);
            else vars[name] = val;
        }

        public string GetLine()
        {
            return lex.GetLine(lineMarker);
        }

        public void AddFunction(string name, BasicFunction function)
        {
            if (!funcs.ContainsKey(name)) funcs.Add(name, function);
            else funcs[name] = function;
        }

        void Error(string text)
        {
            if (text == "Canceled")
                Console.WriteLine(text + " at line " + lineMarker.Line + ": " + GetLine());
            else
            {
                Console.WriteLine("BAD");
                Console.WriteLine(text + " at line " + lineMarker.Line + ": " + GetLine());
            }

            throw new Exception(text + " at line " + lineMarker.Line + ": " + GetLine());
        }

        void Match(Token tok)
        {
            // check if current token is what we expect it to be
            if (lastToken != tok)
                Error("Expect " + tok.ToString() + " got " + lastToken.ToString());
        }

        public void Exec()
        {
            exit = false;
            GetNextToken();
            while (!exit) Line(); // do all lines
            Console.WriteLine("READY");
        }

        Token GetNextToken()
        {
            prevToken = lastToken;
            lastToken = lex.GetToken();

            if (lastToken == Token.EOF && prevToken == Token.EOF)
                Error("Unexpected end of file");

            return lastToken;
        }

        void Line()
        {
            // skip empty new lines
            while (lastToken == Token.NewLine) GetNextToken();

            if (lastToken == Token.EOF)
            {
                exit = true;
                return;
            }

            lineMarker = lex.TokenMarker; // save current line marker
            Statment(); // evaluate statment

            if (lastToken != Token.NewLine && lastToken != Token.EOF)
                Error("Expect new line got " + lastToken.ToString());
        }

        void Statment()
        {
            if (CancelationToken != null && CancelationToken.IsCancellationRequested)
                Error("Canceled");

            Token keyword = lastToken;

            if (keyword == Token.While)
            {
                Marker startMarker = new Marker(lex.TokenMarker.Pointer - 1, lex.TokenMarker.Line, lex.TokenMarker.Column - 1);

                if (whiles.Count == 0 || whiles.Peek()[0].Pointer != lineMarker.Pointer - 1)
                    whiles.Push(new Marker[] { startMarker, lineMarker });

            }

            GetNextToken();
            switch (keyword)
            {
                case Token.Sleep: Sleep(); break;
                case Token.Cls: Cls(); break;
                case Token.Locate: Locate(); break;
                case Token.Print: Print(); break;
                case Token.PrintL: PrintL(); break;
                case Token.Input: Input(); break;
                case Token.Inkey: Inkey(); break;
                case Token.Goto: Goto(); break;
                case Token.While: While(); break;
                case Token.EndWhile: EndDo(); break;
                case Token.If: If(); break;
                case Token.Else: Else(); break;
                case Token.EndIf: break;
                case Token.For: For(); break;
                case Token.Gosub: Gosub(); break;
                case Token.Return: Return(); break;
                case Token.Next: Next(); break;
                case Token.Let: Let(); break;
                case Token.End: End(); break;
                case Token.Assert: Assert(); break;
                case Token.FRead: FRead(); break;
                case Token.FWrite: FWrite(); break;
                case Token.SpriteLoad: SpriteLoad(); break;
                case Token.SpriteDraw: SpriteDraw(); break;
                case Token.SpriteRemove: SpriteRemove(); break;
                case Token.SetCanvas: SetCanvas(); break;

                case Token.SndPlay: SoundPlay(); break;
                case Token.SndSelect: SoundSelect(); break;
                case Token.SndStop: SoundStop(); break;
                case Token.SndVolume: SoundVolume(); break;

                case Token.SetColor: SetColor(); break;
                case Token.DrawElipse: DrawElipse(); break;
                case Token.DrawRect: DrawRect(); break;
                case Token.DrawLine: DrawLine(); break;
                case Token.FillElipse: FillElipse(); break;
                case Token.FillRect: FillRect(); break;
                case Token.SetPixel: SetPixel(); break;
                case Token.GetPixel: GetPixel(); break;

                case Token.Identifier:
                    if (lastToken == Token.Equal) Let();
                    else if (lastToken == Token.Colon) Label();
                    else goto default;
                    break;
                case Token.EOF:
                    exit = true;
                    break;
                default:
                    Error("Expect keyword got " + keyword.ToString());
                    break;
            }
            if (lastToken == Token.Colon)
            {
                // we can execute more statments in single line if we use ";"
                GetNextToken();
                Statment();
            }
        }


        #region command methods


        void SoundSelect()
        {
            object[] parameters = getParameters();

            if (parameters == null || parameters.Length != 2)
                Error("Wrong parameter count. Need 2 parameters.");

            try
            {
                Console.SoundSelect(Convert.ToInt32(parameters[0]), Convert.ToInt32(parameters[1]));
            }
            catch
            {
                Error("Error while SOUNDSELECT.");
            }
        }

        void SoundPlay()
        {
            object[] parameters = getParameters();

            if (parameters == null || parameters.Length != 5)
                Error("Wrong parameter count. Need 5 parameters.");

            try
            {
                Console.SoundPlay(Convert.ToInt32(parameters[0]), Convert.ToInt32(parameters[1]), Convert.ToInt32(parameters[2]), Convert.ToInt32(parameters[3]), Convert.ToInt32(parameters[4]));
            }
            catch
            {
                Error("Error while SOUNDPLAY.");
            }
        }

        void SoundStop()
        {
            object[] parameters = getParameters();

            if (parameters != null && parameters.Length > 0)
                Error("Wrong parameter count. Command requires no parameter.");

            try
            {
                Console.SoundStop();
            }
            catch
            {
                Error("Error while SOUNDSTOP.");
            }
        }

        void SoundVolume()
        {
            object[] parameters = getParameters();

            if (parameters == null || parameters.Length != 1)
                Error("Wrong parameter count. Need 1 parameters.");

            try
            {
                Console.SoundVolume(Convert.ToInt32(parameters[0]));
            }
            catch
            {
                Error("Error while SOUNDVOLUME.");
            }
        }

        void Print()
        {
            if (!HasPrint)
                Error("Print command not allowed");

            while (true)
            {
                Console.Write(Expr().ToString());

                if (lastToken == Token.NewLine || lastToken == Token.Semicolon || lastToken == Token.EOF)
                    break;

                GetNextToken();
            }
        }

        void PrintL()
        {
            if (!HasPrint)
                Error("Print command not allowed");

            while (true)
            {
                Console.WriteLine(Expr().ToString());

                if (lastToken == Token.NewLine || lastToken == Token.Semicolon || lastToken == Token.EOF)
                    break;

                GetNextToken();
            }
        }

        void FRead()
        {
            Match(Token.Identifier);

            if (!vars.ContainsKey(lex.Identifier)) vars.Add(lex.Identifier, new Value());

            GetNextToken();

            string file = "";

            if (lastToken == Token.Comma)
            {
                GetNextToken();
                file = Expr().String;
            }
            else
                GetNextToken();

            if (string.IsNullOrEmpty(file))
                Error("Missing filename (second paramneter).");

            if (System.IO.File.Exists(file) == false)
                Error("File not found.");

            try
            {
                string content = System.IO.File.ReadAllText(file, System.Text.Encoding.Default);
                vars[lex.Identifier] = new Value(content);
            }
            catch
            {
                Error("Error while reading from file.");
            }
        }

        void FWrite()
        {
            object[] parameters = getParameters();

            if (parameters == null || parameters.Length != 2)
                Error("Wrong parameter count. Need 2 parameters.");

            if (!(parameters[1] is string))
                Error("Wrong parameter type (2). Need string parameter.");

            try
            {
                System.IO.File.WriteAllText(parameters[0].ToString(), parameters[1].ToString(), System.Text.Encoding.Default);
            }
            catch
            {
                Error("Error while writing to file.");
            }
        }

        void SpriteLoad()
        {
            object[] parameters = getParameters();

            if (parameters == null || parameters.Length != 2)
                Error("Wrong parameter count. Need 2 parameters.");

            if (!(parameters[1] is string))
                Error("Wrong parameter type (2). Need string parameter.");

            try
            {
                Console.LoadSprite(parameters[0].ToString(), parameters[1].ToString());
            }
            catch
            {
                Error("Error while loading sprite from file.");
            }
        }

        void SetColor()
        {
            object[] parameters = getParameters();

            if (parameters == null || parameters.Length != 2)
                Error("Wrong parameter count. Need 2 parameter.");

            try
            {
                Console.SetColor(Convert.ToInt32(parameters[0]), Convert.ToInt32(parameters[1]));
            }
            catch
            {
                Error("Error while set colors.");
            }
        }

        void DrawElipse()
        {
            object[] parameters = getParameters();

            if (parameters == null || parameters.Length < 5)
                Error("Wrong parameter count. Need min 5 parameters.");

            try
            {
                Console.DrawElipse(
                    Convert.ToInt32(parameters[0]),
                    Convert.ToInt32(parameters[1]),
                    Convert.ToInt32(parameters[2]),
                    Convert.ToInt32(parameters[3]),
                    Convert.ToInt32(parameters[4]),
                    (parameters.Length > 5 ? Convert.ToInt32(parameters[5]) : 1),
                    (parameters.Length > 6 ? Convert.ToInt32(parameters[6]) : 0));
            }
            catch
            {
                Error("Error in DRAWELIPSE.");
            }
        }

        void DrawRect()
        {
            object[] parameters = getParameters();

            if (parameters == null || parameters.Length < 5)
                Error("Wrong parameter count. Need min 5 parameters.");

            try
            {
                Console.DrawRect(
                    Convert.ToInt32(parameters[0]),
                    Convert.ToInt32(parameters[1]),
                    Convert.ToInt32(parameters[2]),
                    Convert.ToInt32(parameters[3]),
                    Convert.ToInt32(parameters[4]),
                    (parameters.Length > 5 ? Convert.ToInt32(parameters[5]) : 1),
                    (parameters.Length > 6 ? Convert.ToInt32(parameters[6]) : 0));
            }
            catch
            {
                Error("Error in DRAWRECT.");
            }
        }

        void DrawLine()
        {
            object[] parameters = getParameters();

            if (parameters == null || parameters.Length < 5)
                Error("Wrong parameter count. Need min 5 parameters.");

            try
            {
                Console.DrawLine(
                    Convert.ToInt32(parameters[0]),
                    Convert.ToInt32(parameters[1]),
                    Convert.ToInt32(parameters[2]),
                    Convert.ToInt32(parameters[3]),
                    Convert.ToInt32(parameters[4]),
                    (parameters.Length > 5 ? Convert.ToInt32(parameters[5]) : 1));
            }
            catch
            {
                Error("Error in DRAWLINE.");
            }
        }

        void FillElipse()
        {
            object[] parameters = getParameters();

            if (parameters == null || parameters.Length < 5)
                Error("Wrong parameter count. Need min 5 parameters.");

            try
            {
                Console.FillElipse(
                    Convert.ToInt32(parameters[0]),
                    Convert.ToInt32(parameters[1]),
                    Convert.ToInt32(parameters[2]),
                    Convert.ToInt32(parameters[3]),
                    Convert.ToInt32(parameters[4]),
                    (parameters.Length > 5 ? Convert.ToInt32(parameters[5]) : 0));
            }
            catch
            {
                Error("Error in FILLELIPSE.");
            }

        }

        void FillRect()
        {
            object[] parameters = getParameters();

            if (parameters == null || parameters.Length < 5)
                Error("Wrong parameter count. Need min 5 parameters.");

            try
            {
                Console.FillRect(
                    Convert.ToInt32(parameters[0]),
                    Convert.ToInt32(parameters[1]),
                    Convert.ToInt32(parameters[2]),
                    Convert.ToInt32(parameters[3]),
                    Convert.ToInt32(parameters[4]),
                    (parameters.Length > 5 ? Convert.ToInt32(parameters[5]) : 0));
            }
            catch
            {
                Error("Error in FILLRECT.");
            }
        }

        void SetPixel()
        {
            object[] parameters = getParameters();

            if (parameters == null || parameters.Length < 3)
                Error("Wrong parameter count. Need 3 parameters.");

            try
            {
                Console.SetPixel(
                    Convert.ToInt32(parameters[0]),
                    Convert.ToInt32(parameters[1]),
                    Convert.ToInt32(parameters[2]));
            }
            catch
            {
                Error("Error in SETPIXEL.");
            }
        }

        void GetPixel()
        {
            object[] parameters = getParameters();

            if (parameters == null || parameters.Length != 1)
                Error("Wrong parameter count. Need 1 parameter.");

            //if (!(parameters[1] is string))
            //    Error("Wrong parameter type (2). Need string parameter.");

            try
            {
                Console.SetCanvas(Convert.ToInt32(parameters[0]));
            }
            catch
            {
                Error("Error while selecting canvas.");
            }
        }

        void SetCanvas()
        {
            object[] parameters = getParameters();

            if (parameters == null || parameters.Length != 1)
                Error("Wrong parameter count. Need 1 parameter.");

            //if (!(parameters[1] is string))
            //    Error("Wrong parameter type (2). Need string parameter.");

            try
            {
                Console.SetCanvas(Convert.ToInt32(parameters[0]));
            }
            catch
            {
                Error("Error while selecting canvas.");
            }
        }

        void SpriteDraw()
        {
            object[] parameters = getParameters();

            if (parameters == null || (parameters.Length != 3 && parameters.Length != 5))
                Error("Wrong parameter count. Need 3 or 5 parameters.");

            //if (!(parameters[1] is string))
            //    Error("Wrong parameter type (2). Need string parameter.");

            try
            {
                if (parameters.Length == 3)
                    Console.DrawSprite(parameters[0].ToString(), Convert.ToInt32(parameters[1]), Convert.ToInt32(parameters[2]));
                else if (parameters.Length == 5)
                    Console.DrawSprite(parameters[0].ToString(), Convert.ToInt32(parameters[1]), Convert.ToInt32(parameters[2]), Convert.ToInt32(parameters[3]), Convert.ToInt32(parameters[4]));
            }
            catch
            {
                Error("Error while drawing sprite.");
            }
        }

        void SpriteRemove()
        {
            object[] parameters = getParameters();

            if (parameters == null || parameters.Length != 3)
                Error("Wrong parameter count. Need 3 parameters.");

            //if (!(parameters[1] is string))
            //    Error("Wrong parameter type (2). Need string parameter.");

            try
            {
                Console.RemoveSprite(parameters[0].ToString(), Convert.ToInt32(parameters[1]), Convert.ToInt32(parameters[2]));
            }
            catch
            {
                Error("Error while removing sprite.");
            }
        }

        void Sleep()
        {
            object[] parameters = getParameters();

            if (parameters == null || parameters.Length != 1)
                Error("Wrong parameter count. Need 1 parameter.");

            if (!(parameters[0] is double))
                Error("Wrong parameter type (1). Need numeric parameter.");

            Console.Sleep(Convert.ToInt32(parameters[0]));
        }

        void Locate()
        {
            object[] parameters = getParameters();

            if (parameters == null || parameters.Length != 2)
                Error("Wrong parameter count. Need 2 parameter.");

            if (!(parameters[0] is double))
                Error("Wrong parameter type (1). Need numeric parameter.");

            if (!(parameters[1] is double))
                Error("Wrong parameter type (2). Need numeric parameter.");

            Console.SetPos(Convert.ToInt32(parameters[0]), Convert.ToInt32(parameters[1]));
        }

        void Cls()
        {
            object[] parameters = getParameters();

            if (parameters == null || parameters.Length < 1)
                Console.Cls(0);
            else
                Console.Cls(Convert.ToInt32(parameters[0]));
        }



        void Input()
        {
            if (!HasInput)
                Error("Input command not allowed");

            while (true)
            {
                Match(Token.Identifier);

                if (!vars.ContainsKey(lex.Identifier)) vars.Add(lex.Identifier, new Value());

                string input = Console.ReadLine();
                double d;
                // try to parse as double, if failed read value as string
                if (double.TryParse(input, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out d))
                    vars[lex.Identifier] = new Value(d);
                else
                    vars[lex.Identifier] = new Value(input);

                GetNextToken();
                if (lastToken != Token.Comma) break;
                GetNextToken();
            }
        }

        void Inkey()
        {
            if (!HasInput)
                Error("Inkey command not allowed");

            Match(Token.Identifier);

            if (!vars.ContainsKey(lex.Identifier)) vars.Add(lex.Identifier, new Value());

            int wait = 0;

            GetNextToken();

            if (lastToken == Token.Comma)
            {
                GetNextToken();
                wait = Convert.ToInt32(Expr().Real);
            }

            int input = Console.Read(wait);

            vars[lex.Identifier] = new Value(input);
        }

        void Goto()
        {
            Match(Token.Identifier);
            string name = lex.Identifier;

            if (!labels.ContainsKey(name))
            {
                // if we didn't encaunter required label yet, start to search for it
                while (true)
                {
                    if (GetNextToken() == Token.Colon && prevToken == Token.Identifier)
                    {
                        if (!labels.ContainsKey(lex.Identifier))
                            labels.Add(lex.Identifier, lex.TokenMarker);
                        if (lex.Identifier == name)
                            break;
                    }
                    if (lastToken == Token.EOF)
                    {
                        Error("Cannot find label named " + name);
                    }
                }
            }
            lex.GoTo(labels[name]);
            lastToken = Token.NewLine;
        }

        void If()
        {
            // check if argument is equal to 0
            bool result = (Expr().BinOp(new Value(0), Token.Equal).Real == 1);

            Match(Token.Then);
            GetNextToken();

            if (result)
            {
                // in case "if" evaulate to zero skip to matching else or endif
                int i = ifcounter;
                while (true)
                {
                    if (lastToken == Token.If)
                    {
                        i++;
                    }
                    else if (lastToken == Token.Else)
                    {
                        if (i == ifcounter)
                        {
                            GetNextToken();
                            return;
                        }
                    }
                    else if (lastToken == Token.EndIf)
                    {
                        if (i == ifcounter)
                        {
                            GetNextToken();
                            return;
                        }
                        i--;
                    }
                    GetNextToken();
                }
            }
        }

        void EndDo()
        {
            whiles.Peek()[1] = lex.TokenMarker;
                        
            lineMarker = whiles.Peek()[0];
            lex.Identifier = "";
            lex.Rewind(whiles.Peek()[0]);
            lastToken = Token.NewLine;
        }

        void While()
        {
            // check if argument is equal to 0
            bool result = (Expr().BinOp(new Value(0), Token.Equal).Real == 1);

            Match(Token.Do);
            GetNextToken();


            if (result)
            {
                lineMarker = whiles.Pop()[1];
                lex.Rewind(lineMarker);
                lastToken = Token.NewLine;
                return;
            }
        }

        void Else()
        {
            // skip to matching endif
            int i = ifcounter;
            while (true)
            {
                if (lastToken == Token.If)
                {
                    i++;
                }
                else if (lastToken == Token.EndIf)
                {
                    if (i == ifcounter)
                    {
                        GetNextToken();
                        return;
                    }
                    i--;
                }
                GetNextToken();
            }
        }

        void Label()
        {
            string name = lex.Identifier;
            if (!labels.ContainsKey(name)) labels.Add(name, lex.TokenMarker);

            GetNextToken();
            Match(Token.NewLine);
        }

        void End()
        {
            exit = true;
        }

        void Let()
        {
            if (lastToken != Token.Equal)
            {
                Match(Token.Identifier);
                GetNextToken();
                Match(Token.Equal);
            }

            string id = lex.Identifier;

            GetNextToken();

            SetVar(id, Expr());
        }

        void Gosub()
        {
            lex.SaveReturnMarker();
            Match(Token.Identifier);
            string name = lex.Identifier;

            if (!subs.ContainsKey(name))
            {
                // if we didn't encaunter required sub yet, start to search for it
                while (true)
                {
                    if (GetNextToken() == Token.Identifier && prevToken == Token.Sub)
                    {
                        if (!subs.ContainsKey(lex.Identifier))
                            subs.Add(lex.Identifier, lex.TokenMarker);
                        if (lex.Identifier == name)
                            break;
                    }
                    if (lastToken == Token.EOF)
                    {
                        Error("Cannot find sub named " + name);
                    }
                }
            }
            lex.GoSub(subs[name], name);
            lastToken = Token.NewLine;
        }

        void Return()
        {
            lex.Return();
            lastToken = Token.NewLine;
            //GetNextToken();
        }

        void For()
        {
            Match(Token.Identifier);
            string var = lex.Identifier;
            Double start = 1;

            GetNextToken();
            Match(Token.Equal);

            GetNextToken();
            Value v = Expr();

            start = v.Real;

            // save for loop marker
            if (loops.ContainsKey(var))
            {
                loops[var] = lineMarker;
            }
            else
            {
                SetVar(var, v);
                loops.Add(var, lineMarker);
            }

            Match(Token.To);

            GetNextToken();
            v = Expr();

            // save steps for loop
            if (!steps.ContainsKey(var))
                steps.Add(var, (start > v.Real ? -1 : 1));
            else
                steps[var] = (start > v.Real ? -1 : 1);

            if (lastToken == Token.Step)
            {
                GetNextToken();
                steps[var] = Convert.ToInt32(Expr().Real);
            }

            if (start > v.Real)
            {
                if (vars[var].BinOp(v, Token.Less).Real == 1)
                {
                    while (true)
                    {
                        while (!(GetNextToken() == Token.Identifier && prevToken == Token.Next)) ;
                        if (lex.Identifier == var)
                        {
                            loops.Remove(var);
                            GetNextToken();
                            Match(Token.NewLine);
                            break;
                        }
                    }
                }
            }
            else
            {
                if (vars[var].BinOp(v, Token.More).Real == 1)
                {
                    while (true)
                    {
                        while (!(GetNextToken() == Token.Identifier && prevToken == Token.Next)) ;
                        if (lex.Identifier == var)
                        {
                            loops.Remove(var);
                            GetNextToken();
                            Match(Token.NewLine);
                            break;
                        }
                    }
                }
            }
        }

        void Next()
        {
            // jump to begining of the "for" loop
            Match(Token.Identifier);
            string var = lex.Identifier;
            vars[var] = vars[var].BinOp(new Value(steps[var]), Token.Plus);
            lex.GoTo(new Marker(loops[var].Pointer - 1, loops[var].Line, loops[var].Column - 1));
            lastToken = Token.NewLine;
        }

        void Assert()
        {
            bool result = (Expr().BinOp(new Value(0), Token.Equal).Real == 1);

            if (result)
            {
                Error("Assertion fault"); // if out assert evaluate to false, throw error with souce code line
            }
        }
        #endregion

        #region helping methods
        object[] getParameters()
        {
            if (lastToken == Token.NewLine || lastToken == Token.Semicolon || lastToken == Token.EOF)
                return null;

            List<object> param = new List<object>();

            while (true)
            {
                Value val = Expr();
                param.Add(val.Type == ValueType.Real ? (object)val.Real : (object)val.String);

                if (lastToken == Token.NewLine || lastToken == Token.Semicolon || lastToken == Token.EOF)
                    break;

                GetNextToken();
            }

            return (param.Count > 0 ? param.ToArray() : null);
        }

        Value Expr(int min = 0)
        {
            // originally we were using shunting-yard algorithm, but now we parse it recursively 
            Dictionary<Token, int> precedens = new Dictionary<Token, int>()
            {
                { Token.Or, 0 }, { Token.And, 0 },
                { Token.Equal, 1 }, { Token.NotEqual, 1 },
                { Token.Less, 1 }, { Token.More, 1 },
                { Token.LessEqual, 1 },  { Token.MoreEqual, 1 },
                { Token.Plus, 2 }, { Token.Minus, 2 },
                { Token.Asterisk, 3 }, {Token.Slash, 3 },
                { Token.Caret, 4 }
            };

            Value lhs = Primary();

            while (true)
            {
                if (lastToken < Token.Plus || lastToken > Token.And || precedens[lastToken] < min)
                    break;

                Token op = lastToken;
                int prec = precedens[lastToken]; // Operator Precedence
                int assoc = 0; // 0 left, 1 right; Operator associativity
                int nextmin = assoc == 0 ? prec : prec + 1;
                GetNextToken();
                Value rhs = Expr(nextmin);
                lhs = lhs.BinOp(rhs, op);
            }

            return lhs;
        }

        Value Primary()
        {
            Value prim = Value.Zero;

            if (lastToken == Token.Value)
            {
                // number | string
                prim = lex.Value;
                GetNextToken();
            }
            else if (lastToken == Token.Identifier)
            {
                // ident | ident '(' args ')'
                if (vars.ContainsKey(lex.Identifier))
                {
                    prim = vars[lex.Identifier];
                }
                else if (funcs.ContainsKey(lex.Identifier))
                {
                    string name = lex.Identifier;
                    List<Value> args = new List<Value>();
                    GetNextToken();
                    Match(Token.LParen);

                start:
                    if (GetNextToken() != Token.RParen)
                    {
                        args.Add(Expr());
                        if (lastToken == Token.Comma)
                            goto start;
                    }

                    prim = funcs[name](null, args);
                }
                else
                {
                    Error("Undeclared variable " + lex.Identifier);
                }
                GetNextToken();
            }
            else if (lastToken == Token.LParen)
            {
                // '(' expr ')'
                GetNextToken();
                prim = Expr();
                Match(Token.RParen);
                GetNextToken();
            }
            else if (lastToken == Token.Plus || lastToken == Token.Minus || lastToken == Token.Not)
            {
                // unary operator
                // '-' | '+' primary
                Token op = lastToken;
                GetNextToken();
                prim = Primary().UnaryOp(op);
            }
            else
            {
                Error("Unexpexted token in primary!");
            }

            return prim;
        }
        #endregion
    }
}
