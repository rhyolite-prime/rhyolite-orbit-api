using System;

namespace RhyoliteOrbit.Utils;

public class Otp
{
    public string GenerateOtp()
    {
        var random = new Random();
        return random.Next(100000, 999999).ToString();
    }
}