namespace RobotCore.Library.Attributes;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public class RobotLibraryAttribute(
    string introduction = "",
    string initialization = "",
    bool explicitDiscovery = false) : Attribute
{
    public string Introduction { get; } = introduction;
    public string Initialization { get; } = initialization;
    public bool ExplicitDiscovery { get; } = explicitDiscovery;
}