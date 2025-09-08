---
_layout: landing
---

# Senzing.Sdk for C# #

This is the Senzing SDK for C#.  After adding the `Senzing.Sdk` NuGet package to your project dependencies (see [instructions](https://github.com/senzing-garage/sz-sdk-csharp/blob/main/README.md#Usage)) you can leverage the Senzing SDK via the `SzCoreEnvironment` class:

```c#
using Senzing.Sdk;
using Senzing.Sdk.Core;
using System.Text.Json.Nodes;

. . .

SzEnvironment? env = null;

try {
    env = SzCoreEnvironment.NewBuilder().Settings(settingsJson).Build();

    SzEngine engine = env.GetEngine();

    engine.AddRecord("TEST", "ABC001", "{\"NAME_FULL\": \"Joe Schmoe\"}");

    string entityJson = engine.GetEntity("TEST", "ABC001");

    JsonObject? jsonObject  = JsonNode.Parse(entityJson)?.AsObject();
    JsonObject? entity      = jsonObject?["RESOLVED_ENTITY"]?.AsObject();
    long?       entityID    = entity?["ENTITY_ID"]?.GetValue<long>();

    Console.WriteLine("Record ABC001 has entity ID: " + entityID);

} finally {
    if (env != null) env.Destroy();
}
```

## See the "API" tab above for reference documentation ##

- A good starting place is the `Senzing.Sdk.SzEnvironment` interface and
  its "core" implementation `Senzing.Sdk.Core.SzCoreEnvironment`.

- The `SzEnvironment` breaks down the functionality into 5 interfaces:
  - `Senzing.Sdk.SzProduct`
  - `Senzing.Sdk.SzConfig`
  - `Senzing.Sdk.SzConfigManager`
  - `Senzing.Sdk.SzDiagnostic`
  - `Senzing.Sdk.SzEngine`

- The bulk of the Senzing functionality is contained in the `SzEngine` interface.
- The `SzProduct` interface provides basic functions pertaining to the Senzing product installation.
- The `SzConfig` and `SzConfigManager` interfaces provide functions that are typically used while setting up the configuration for the Senzing repository.
- The `SzDiagnostic` interface is primarily for internal diagnostic functionality.
