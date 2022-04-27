using System;

namespace Swordfish.MMORPG.Enums
{
    [Flags]
    public enum AccountFlags
    {
        None = 0,
        UsernameIncorrect = 1,
        PasswordIncorrect = 2,
        EmailIncorrect = 4,
    }
}
