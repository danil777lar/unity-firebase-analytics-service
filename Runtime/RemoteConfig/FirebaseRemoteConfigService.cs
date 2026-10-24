using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Firebase;
using Firebase.RemoteConfig;
using Larje.Core;
using UnityEngine;

namespace Larje.Analytics.Firebase.RemoteConfig
{
    public sealed class FirebaseRemoteConfigService : IRemoteConfigService, IInitializableService
    {
        private bool _ready;
        private readonly FirebaseRemoteConfigSettings _settings;

        public bool Ready => _ready;

        public event Action Updated;

        public FirebaseRemoteConfigService(FirebaseRemoteConfigSettings settings)
        {
            _settings = settings;
        }

        public void Initialize()
        {
            InitializeAsync().Forget();
        }

        public bool Has(string key)
        {
            return _ready && FirebaseRemoteConfig.DefaultInstance.GetValue(key).Source != ValueSource.StaticValue;
        }

        public string GetString(string key, string fallback = "")
        {
            return Has(key) ? FirebaseRemoteConfig.DefaultInstance.GetValue(key).StringValue : fallback;
        }

        public long GetLong(string key, long fallback = 0)
        {
            return Has(key) ? FirebaseRemoteConfig.DefaultInstance.GetValue(key).LongValue : fallback;
        }

        public double GetDouble(string key, double fallback = 0d)
        {
            return Has(key) ? FirebaseRemoteConfig.DefaultInstance.GetValue(key).DoubleValue : fallback;
        }

        public bool GetBool(string key, bool fallback = false)
        {
            return Has(key) ? FirebaseRemoteConfig.DefaultInstance.GetValue(key).BooleanValue : fallback;
        }

        private async UniTaskVoid InitializeAsync()
        {
            try
            {
                DependencyStatus status = await FirebaseApp.CheckAndFixDependenciesAsync().AsUniTask();
                if (status != DependencyStatus.Available)
                {
                    await UniTask.SwitchToMainThread();
                    Debug.LogError($"FirebaseRemoteConfigService: dependencies are not available: {status}");
                    return;
                }

                FirebaseRemoteConfig remoteConfig = FirebaseRemoteConfig.DefaultInstance;
                await remoteConfig.SetDefaultsAsync(BuildDefaults()).AsUniTask();
                await UniTask.SwitchToMainThread();
                _ready = true;
                Updated?.Invoke();

                await remoteConfig.FetchAsync(_settings.CacheExpiration).AsUniTask();
                await remoteConfig.ActivateAsync().AsUniTask();
                await UniTask.SwitchToMainThread();
                Updated?.Invoke();
            }
            catch (Exception exception)
            {
                await UniTask.SwitchToMainThread();
                Debug.LogWarning($"FirebaseRemoteConfigService: {exception.Message}");
            }
        }

        private Dictionary<string, object> BuildDefaults()
        {
            Dictionary<string, object> result = new Dictionary<string, object>();
            foreach (FirebaseRemoteConfigSettings.DefaultValue entry in _settings.Defaults)
            {
                if (!string.IsNullOrEmpty(entry.Key))
                {
                    result[entry.Key] = entry.Text;
                }
            }

            return result;
        }
    }
}
