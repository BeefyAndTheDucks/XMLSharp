public abstract class Token { }

public class IdentifierToken(string name) : Token
{
    public string Name = name;
}

public class ImmediateToken(int value) : Token
{
    public int Value = value;
}

public class AssignmentToken : Token { }
public class AddToken : Token { }
public class SemicolonToken : Token { }
public class VariableDefinitionToken(XMLSType type) : Token
{
    public XMLSType Type = type;
}