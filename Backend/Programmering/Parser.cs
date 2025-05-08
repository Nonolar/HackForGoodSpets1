using System;
using System.Collections.Generic;

namespace NParser
{
    // Define the Token struct
    public struct Token
    {
        public string name { get; set; }
        public dynamic? value { get; set; }

        public Token(string name, dynamic? value)
        {
            this.name = name;
            this.value = value;
        }
    }

    // Define the statement types the parser can output
    public enum Stmt
    {
        Print,
        Start,
        End,
        Unknown
    }

    // Interface for the parser
    public interface IParser
    {
        Stmt[] Parse(Token[] tokens);
    }

    // The actual parser class
    public class Parser : IParser
    {
        public Stmt[] Parse(Token[] tokens)
        {
            List<Stmt> statements = new List<Stmt>();

            foreach (var token in tokens)
            {
                switch (token.name.ToLower())
                {
                    case "print":
                        statements.Add(Stmt.Print);
                        break;
                    case "start":
                        statements.Add(Stmt.Start);
                        break;
                    case "end":
                        statements.Add(Stmt.End);
                        break;
                    default:
                        statements.Add(Stmt.Unknown);
                        break;
                }
            }

            return statements.ToArray();
        }
    }
}
