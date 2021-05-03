
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
        Sub,
        Return,
        Rem,
        End,
        Assert,
        Cls,
        Sleep,
        Locate,

        FRead,
        FWrite,

        SetCanvas,
        SpriteLoad,
        SpriteDraw,
        SpriteRemove,

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

        EOF = -1,   //End Of File
        SetColor = 53,
        DrawLine = 54,
        DrawRect = 55,
        DrawElipse = 56,
        FillRect = 57,
        FillElipse = 58,
        SetPixel = 59,
        GetPixel = 60,
        SndPlay = 61,
        SndStop = 62,
        SndSelect = 63,
        SndVolume = 64,
        While = 65,
        EndWhile = 66,
        Do = 67
    }
}
