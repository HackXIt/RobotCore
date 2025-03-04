namespace RobotCore.Library.Utility;

public class StringValueAttribute(string value) : Attribute
{
    public string Value => value;
}