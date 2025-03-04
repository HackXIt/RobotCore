using System.Text;

namespace RobotCore.Library.Utility;

public class SanitizedXmlEncoding : UTF8Encoding
{
    public SanitizedXmlEncoding()
    {
    }

    public SanitizedXmlEncoding(bool encoderShouldEmitUtf8Identifier) : base(encoderShouldEmitUtf8Identifier)
    {
    }

    public SanitizedXmlEncoding(bool encoderShouldEmitUtf8Identifier, bool throwOnInvalidBytes) : base(encoderShouldEmitUtf8Identifier, throwOnInvalidBytes)
    {
    }

    public static bool IsLegalXmlChar(int character)
    {
        return 
            character is 0x9 
                or 0xA
                or 0xD 
                or >= 0x20
                and <= 0xD7FF 
                or >= 0xE000 
                and <= 0xFFFD 
                or >= 0x10000 
                and <= 0x10FFFF;
    }

    public override byte[] GetBytes(string s)
    {
        var filtered = new string(s.Where(c => IsLegalXmlChar(c)).ToArray());
        return base.GetBytes(filtered);
    }

    public override string GetString(byte[] bytes)
    {
        var filtered = bytes.Where(b => IsLegalXmlChar(b)).ToArray();
        return base.GetString(filtered);
    }
}