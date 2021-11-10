using System;

public enum Packets
{
    Handshake = 0,
    KeepAlive = 1,

    RegisterAttempt = 2,
    RegisterSuccess = 3,
    RegisterFailed = 4,

    LoginAttempt = 5,
    LoginSuccess = 6,
    LoginFailed = 7,

    JoinAttempt = 8,
    JoinSuccess = 9,
    JoinFailed = 10,

    SpawnEntity = 11,
    RemoveEntity = 12,
    EntityPosition = 13,

    PlayerPosition = 14,
}