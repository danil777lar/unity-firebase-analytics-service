# Larje Analytics Firebase

`com.larje.analytics-firebase`: Firebase provider for the `IAnalyticsService` from `com.larje.core` and an optional Firebase Remote Config service. Events sent before Firebase is ready are queued; parameter values are converted to Firebase types and names are sanitized to Firebase rules.

## Install

UPM cannot resolve git dependencies, so add core, UniTask and this package to `Packages/manifest.json`:

```json
"com.cysharp.unitask": "https://github.com/Cysharp/UniTask.git?path=src/UniTask/Assets/Plugins/UniTask#2.5.10",
"com.larje.core": "https://github.com/danil777lar/unity-gameplay-core-services.git",
"com.larje.zenject": "https://github.com/danil777lar/unity-zenject-upm.git",
"com.larje.analytics-firebase": "https://github.com/danil777lar/unity-firebase-analytics-service.git"
```

Runtime code compiles only when `LARJE_FIREBASE_ANALYTICS` is defined, so the package is safe to install before the SDK. The Zenject installer additionally needs `com.larje.zenject` (`LARJE_ZENJECT`).

### SDK

Firebase Unity SDK 11+: `FirebaseAnalytics` and, for remote config, `FirebaseRemoteConfig`. Import the `.unitypackage` files from the Firebase Unity SDK archive or add the `com.google.firebase.*` tarballs in Package Manager. Put `google-services.json` / `GoogleService-Info.plist` into `Assets`.

`LARJE_FIREBASE_ANALYTICS` and `LARJE_FIREBASE_REMOTE_CONFIG` are added automatically when the `Firebase.Analytics` / `Firebase.RemoteConfig` assemblies are present (DLL import or UPM tarball).

## Setup

1. Create `FirebaseAnalyticsConfig` (`Create > Larje > Analytics > Firebase Analytics Config`).
2. Create `FirebaseAnalyticsInstaller`, assign the config and add it to the `ProjectContext` next to `LarjeCoreInstaller`. It replaces `LogAnalyticsService`.
3. Optional remote config: create `FirebaseRemoteConfigSettings` with default values and a `FirebaseRemoteConfigInstaller`; inject `IRemoteConfigService`.

## Config

| Group | Fields |
|---|---|
| Events | `maxQueuedEvents` kept until Firebase is ready (oldest dropped), `logEvents` mirrors events to the console. |
| Collection | `collectionEnabled` for `SetAnalyticsCollectionEnabled`. |
| Remote config | `cacheExpirationHours` for `FetchAsync`, `defaults` as key/text pairs. |

## Behavior

- `int`/`long` parameters are sent as integers, `float`/`double` as doubles, `bool` as 0/1, anything else as a string trimmed to 100 characters.
- Event and parameter names keep ASCII letters, digits and `_`, start with a letter and are trimmed to 40 characters.
- `IRemoteConfigService` getters return the fallback until the config is ready and for keys that are not defined; `Updated` fires after defaults are set and again after fetch and activate.
- Cloud Messaging from the previous module (it only logged tokens) was dropped.

## Status

Not compiled against the Firebase SDK yet (it is not available from a package registry). Verify after importing Firebase.
