
namespace BasicSharp
{
    public enum Token
    {
        Unknown,

        Identifier,
        Value,

        //Keywords
        Print,
        PrintL,
        If,
        EndIf,
        Then,
        Else,
        For,
        To,
        Step,
        Next,
        Goto,
        Input,
        Inkey,
        Let,
        Gosub,
        Return,
        Rem,
        End,
        Assert,
        Clear,
        Sleep,
        SetPos,

        FRead,
        FWrite,

        NewLine,
        Colon,
        Semicolon,
        Comma,

        Plus,
        Minus,
        Slash,
        Asterisk,
        Caret,
        Equal,
        Less,
        More,
        NotEqual,
        LessEqual,
        MoreEqual,
        Or,
        And,
        Not,

        LParen,
        RParen,

        Test,

        EOF = -1   //End Of File
    }
}
