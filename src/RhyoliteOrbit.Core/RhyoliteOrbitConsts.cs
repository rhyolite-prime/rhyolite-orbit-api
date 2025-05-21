using RhyoliteOrbit.Debugging;

namespace RhyoliteOrbit;

public class RhyoliteOrbitConsts
{
    public const string LocalizationSourceName = "RhyoliteOrbit";

    public const string ConnectionStringName = "Default";

    public const bool MultiTenancyEnabled = true;


    /// <summary>
    /// Default pass phrase for SimpleStringCipher decrypt/encrypt operations
    /// </summary>
    public static readonly string DefaultPassPhrase =
        DebugHelper.IsDebug ? "gsKxGZ012HLL3MI5" : "f9be3a4eed7441d1bb1da71898a30d95";
}
