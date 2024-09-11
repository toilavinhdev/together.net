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
    
    [GeneratedRegex("[\r|\n]")]
    private static partial Regex LineBreakGeneratedRegex();
    public static readonly Regex LineBreakRegex = LineBreakGeneratedRegex();
    
    [GeneratedRegex("\\s+")]
    private static partial Regex SpacesTabsGeneratedRegex();
    public static readonly Regex SpacesTabsRegex = SpacesTabsGeneratedRegex();
    
    [GeneratedRegex("[áàảãạâấầẩẫậăắằẳẵặ]")] 
    private static partial Regex CharacterVariantAGeneratedRegex();
    public static readonly Regex CharacterVariantARegex = CharacterVariantAGeneratedRegex();
    
    [GeneratedRegex("[éèẻẽẹêếềểễệ]")] 
    private static partial Regex CharacterVariantEGeneratedRegex();
    public static readonly Regex CharacterVariantERegex = CharacterVariantEGeneratedRegex();
    
    [GeneratedRegex("[iíìỉĩị]")] 
    private static partial Regex CharacterVariantIGeneratedRegex();
    public static readonly Regex CharacterVariantIRegex = CharacterVariantIGeneratedRegex();
    
    [GeneratedRegex("[óòỏõọơớờởỡợôốồổỗộ]")] 
    private static partial Regex CharacterVariantOGeneratedRegex();
    public static readonly Regex CharacterVariantORegex = CharacterVariantOGeneratedRegex();

    [GeneratedRegex("[úùủũụưứừửữự]")]
    private static partial Regex CharacterVariantUGeneratedRegex();
    public static readonly Regex CharacterVariantURegex = CharacterVariantUGeneratedRegex();
    
    [GeneratedRegex("[yýỳỷỹỵ]")] 
    private static partial Regex CharacterVariantYGeneratedRegex();
    public static readonly Regex CharacterVariantYRegex = CharacterVariantYGeneratedRegex();
    
    [GeneratedRegex("[đ]")]
    private static partial Regex CharacterVariantDGeneratedRegex();
    public static readonly Regex CharacterVariantDRegex = CharacterVariantDGeneratedRegex();
    
    [GeneratedRegex("[\"`~!@#$%^&*()\\-+=?/>.<,{}[]|]\\]")] 
    private static partial Regex SpecialCharactersGeneratedRegex();
    public static readonly Regex SpecialCharactersRegex = SpecialCharactersGeneratedRegex();
}