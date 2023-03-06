public enum TcpPacketType
{
    Answer = 0,
    Msg,
    Chat,
    SecretChat,
    DuplicationCheck = 100,
    SignUp = 200,
    SignIn,
    Quit = 999,
}

public enum Table{
    User
}

public enum UserTableColumn{
    IDpk,
    UserId,
    PassHash,
    NickName,
    Email
}