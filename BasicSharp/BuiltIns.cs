using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Threading.Tasks;

namespace BasicSharp
{
    class BuiltIns
    {
        static Random _randomizer;

        public static void InstallAll(Interpreter interpreter)
        {
            interpreter.AddFunction("str", Str);
            interpreter.AddFunction("num", Num);
            interpreter.AddFunction("abs", Abs);
            interpreter.AddFunction("min", Min);
            interpreter.AddFunction("max", Max);
            interpreter.AddFunction("not", Not);
            interpreter.AddFunction("sin", Sin);
            interpreter.AddFunction("cos", Cos);
            interpreter.AddFunction("tan", Tan);
            interpreter.AddFunction("atan", ATan);
            interpreter.AddFunction("chr", Chr);
            interpreter.AddFunction("asc", Asc);
            interpreter.AddFunction("color", Color);
            interpreter.AddFunction("rand", Rand);

            _randomizer = new Random(DateTime.Now.TimeOfDay.Milliseconds);
        }

        public static Value Str(Interpreter interpreter, List<Value> args)
        {
            if (args.Count < 1)
                throw new ArgumentException();

            return args[0].Convert(ValueType.String);
        }

        public static Value Num(Interpreter interpreter, List<Value> args)
        {
            if (args.Count < 1)
                throw new ArgumentException();

            return args[0].Convert(ValueType.Real);
        }

        public static Value Abs(Interpreter interpreter, List<Value> args)
        {
            if (args.Count < 1)
                throw new ArgumentException();

            return new Value(Math.Abs(args[0].Real));
        }

        public static Value Cos(Interpreter interpreter, List<Value> args)
        {
            if (args.Count < 1)
                throw new ArgumentException();

            return new Value(Math.Cos(args[0].Real));
        }

        public static Value Sin(Interpreter interpreter, List<Value> args)
        {
            if (args.Count < 1)
                throw new ArgumentException();

            return new Value(Math.Sin(args[0].Real));
        }

        public static Value Tan(Interpreter interpreter, List<Value> args)
        {
            if (args.Count < 1)
                throw new ArgumentException();

            return new Value(Math.Tan(args[0].Real));
        }

        public static Value ATan(Interpreter interpreter, List<Value> args)
        {
            if (args.Count < 1)
                throw new ArgumentException();

            return new Value(Math.Atan(args[0].Real));
        }

        public static Value Asc(Interpreter interpreter, List<Value> args)
        {
            if (args.Count < 1)
                throw new ArgumentException();

            return new Value(Convert.ToDouble(Encoding.ASCII.GetBytes(new char[] { args[0].String[0] })[0]));
        }

        public static Value Chr(Interpreter interpreter, List<Value> args)
        {
            if (args.Count < 1)
                throw new ArgumentException();

            return new Value(Convert.ToString(Encoding.ASCII.GetChars(new byte[] { Convert.ToByte(args[0].Real) })[0]));
        }

        public static Value Color(Interpreter interpreter, List<Value> args)
        {
            if (args.Count < 3)
                throw new ArgumentException();

            return new Value(Convert.ToDouble(System.Drawing.Color.FromArgb(Convert.ToInt32(args[0].Real), Convert.ToInt32(args[1].Real), Convert.ToInt32(args[2].Real)).ToArgb()));
        }

        public static Value Min(Interpreter interpreter, List<Value> args)
        {
            if (args.Count < 2)
                throw new ArgumentException();

            return new Value(Math.Min(args[0].Real, args[1].Real));
        }

        public static Value Max(Interpreter interpreter, List<Value> args)
        {
            if (args.Count < 2)
                throw new ArgumentException();

            return new Value(Math.Max(args[0].Real, args[1].Real));
        }

        public static Value Rand(Interpreter interpreter, List<Value> args)
        {
            if (args.Count < 1)
                throw new ArgumentException();

            return new Value(_randomizer.Next(Convert.ToInt32(args[0].Real), Convert.ToInt32(args[1].Real)));
        }

        public static Value Not(Interpreter interpreter, List<Value> args)
        {
            if (args.Count < 1)
                throw new ArgumentException();

            return new Value(args[0].Real == 0 ? 1 : 0);
        }
    }
}
