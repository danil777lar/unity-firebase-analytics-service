using System;

namespace Larje.Analytics.Firebase.RemoteConfig
{
    public interface IRemoteConfigService
    {
        bool Ready { get; }

        event Action Updated;

        bool Has(string key);
        string GetString(string key, string fallback = "");
        long GetLong(string key, long fallback = 0);
        double GetDouble(string key, double fallback = 0d);
        bool GetBool(string key, bool fallback = false);
    }
}
