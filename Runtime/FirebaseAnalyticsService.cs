using System;
using System.Collections.Generic;
using System.Text;
using Cysharp.Threading.Tasks;
using Firebase;
using Firebase.Analytics;
using Larje.Core;
using Larje.Core.Monetization;
using UnityEngine;

namespace Larje.Analytics.Firebase
{
    public sealed class FirebaseAnalyticsService : IAnalyticsService, IInitializableService
    {
        private const int MAX_NAME_LENGTH = 40;
        private const int MAX_PARAMETER_VALUE_LENGTH = 100;

        private bool _ready;
        private readonly FirebaseAnalyticsConfig _config;
        private readonly Queue<PendingEvent> _queue = new Queue<PendingEvent>();

        public bool Ready => _ready;

        public FirebaseAnalyticsService(FirebaseAnalyticsConfig config)
        {
            _config = config;
        }

        public void Initialize()
        {
            InitializeAsync().Forget();
        }

        public void Send(string eventName)
        {
            Send(eventName, null);
        }

        public void Send(string eventName, IReadOnlyDictionary<string, object> parameters)
        {
            string name = Sanitize(eventName);
            if (_config.LogEvents)
            {
                Debug.Log($"FirebaseAnalytics: {name}");
            }

            if (!_ready)
            {
                if (_queue.Count >= _config.MaxQueuedEvents)
                {
                    _queue.Dequeue();
                }

                _queue.Enqueue(new PendingEvent(name, parameters));
                return;
            }

            Log(name, parameters);
        }

        private async UniTaskVoid InitializeAsync()
        {
            DependencyStatus status;
            try
            {
                status = await FirebaseApp.CheckAndFixDependenciesAsync().AsUniTask();
            }
            catch (Exception exception)
            {
                Debug.LogException(exception);
                return;
            }

            await UniTask.SwitchToMainThread();
            if (status != DependencyStatus.Available)
            {
                Debug.LogError($"FirebaseAnalyticsService: dependencies are not available: {status}");
                return;
            }

            FirebaseAnalytics.SetAnalyticsCollectionEnabled(_config.CollectionEnabled);
            _ready = true;
            while (_queue.Count > 0)
            {
                PendingEvent pending = _queue.Dequeue();
                Log(pending.Name, pending.Parameters);
            }
        }

        private void Log(string name, IReadOnlyDictionary<string, object> parameters)
        {
            try
            {
                if (parameters == null || parameters.Count == 0)
                {
                    FirebaseAnalytics.LogEvent(name);
                }
                else
                {
                    FirebaseAnalytics.LogEvent(name, ToParameters(parameters));
                }
            }
            catch (Exception exception)
            {
                Debug.LogWarning($"FirebaseAnalyticsService: failed to log '{name}': {exception.Message}");
            }
        }

        private Parameter[] ToParameters(IReadOnlyDictionary<string, object> parameters)
        {
            List<Parameter> result = new List<Parameter>(parameters.Count);
            foreach (KeyValuePair<string, object> pair in parameters)
            {
                result.Add(ToParameter(Sanitize(pair.Key), pair.Value));
            }

            return result.ToArray();
        }

        private Parameter ToParameter(string key, object data)
        {
            switch (data)
            {
                case int number:
                    return new Parameter(key, number);
                case long number:
                    return new Parameter(key, number);
                case float number:
                    return new Parameter(key, number);
                case double number:
                    return new Parameter(key, number);
                case bool flag:
                    return new Parameter(key, flag ? 1L : 0L);
                default:
                    return new Parameter(key, Truncate(data?.ToString() ?? string.Empty, MAX_PARAMETER_VALUE_LENGTH));
            }
        }

        private string Sanitize(string name)
        {
            StringBuilder builder = new StringBuilder(name.Length);
            foreach (char symbol in name)
            {
                builder.Append(char.IsLetterOrDigit(symbol) && symbol < 128 ? symbol : '_');
            }

            if (builder.Length == 0 || !char.IsLetter(builder[0]))
            {
                builder.Insert(0, 'e');
            }

            return Truncate(builder.ToString(), MAX_NAME_LENGTH);
        }

        private string Truncate(string text, int length)
        {
            return text.Length <= length ? text : text.Substring(0, length);
        }

        private readonly struct PendingEvent
        {
            public readonly string Name;
            public readonly IReadOnlyDictionary<string, object> Parameters;

            public PendingEvent(string name, IReadOnlyDictionary<string, object> parameters)
            {
                Name = name;
                Parameters = parameters;
            }
        }
    }
}
