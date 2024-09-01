using System.ComponentModel;
using System.Text.RegularExpressions;

namespace Infrastructure.SharedKernel.Utilities;

public static partial class RegexUtils
{
    [GeneratedRegex(@"^[\w-\.]+@([\w-]+\.)+[\w-]{2,4}$")] 
    private static partial Regex EmailGeneratedRegex();
    public static readonly Regex EmailRegex = EmailGeneratedRegex();
    
    
    [GeneratedRegex(@"^(?=.{6,24}$)[a-z0-9_]*$")] 
    [Description("6-24 characters long; Allow a-z 0-9 and _")]
    private static partial Regex UserNameGeneratedRegex();
    public static readonly Regex UserNameRegex = UserNameGeneratedRegex();
}