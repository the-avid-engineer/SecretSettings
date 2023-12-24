# SecretSettings

A NuGet package with an extension method for `IConfigurationBuilder` that loads a secrets file into memory, rewrites the secret refs with the secret returned by the secret provider, and loads the rewritten file as an appsettings file.

## Setup

In the consuming application, create files with a name of `secrets.json` and `secrets.{Environment}.json`. See Supported Secret Providers for examples on how to references secrets for that provider. Note that you can have multiple secrets in a single secrets file; you do not need one file per secret.

To make the this actually work, open `Program.cs` in the consuming application, and in the `ConfigureAppConfiguration` method, add the following lines:

```
configurationBuilder.AddJsonSecretsFile("secrets.json");
configurationBuilder.AddJsonSecretsFile($"secrets.{builderContext.HostingEnvironment.EnvironmentName}.json");
```

## Supported Formats

### JSON

You may use any secret [that can be represented by JsonValueKind](https://docs.microsoft.com/en-us/dotnet/api/system.text.json.jsonvaluekind?view=net-5.0). This includes simple strings, numbers, booleans, and arrays. You do not need to use an object.

## Supported Secret Providers

### [AWS Secrets Manager](https://aws.amazon.com/secrets-manager/) (AWS SM)

For AWS SM secrets, your code should be running on an EC2 that has an instance role which can reach AWS SM. This is typical when settings up EC2s so you shouldn't have much of an issue there. For local development, however, you will need to configure a default profile with the AWS CLI (v2).

Here's how to install the AWS CLI:

- [Installation Instructions for macOS](https://docs.aws.amazon.com/cli/latest/userguide/install-cliv2-mac.html)  
- [Installation Instructions for Windows](https://docs.aws.amazon.com/cli/latest/userguide/install-cliv2-windows.html)

After you have installed the CLI, you can configure a profile by running run the following command. This will summon a series of prompts, asking for the Access Token to use.

```
aws configure --profile default
```

The JSON rewriter will scan for any object with exactly one property with a name of `SecretRef::AWS` and a value type of object. If the rewriter finds such an object, the value of the property will be deserialized into an AWS SM provider model with the following properties:

1. `SecretId` - Required String; equivalent of the `--secret-id` option for [aws get-secret-value](https://docs.aws.amazon.com/cli/latest/reference/secretsmanager/get-secret-value.html)
2. `VersionStage` - Optional String; equivalent of the `--version-stage` option for [aws get-secret-value](https://docs.aws.amazon.com/cli/latest/reference/secretsmanager/get-secret-value.html) (Default is `null`)
3. `VersionId` - Optional String; equivalent of the `--version-id` option for [aws get-secret-value](https://docs.aws.amazon.com/cli/latest/reference/secretsmanager/get-secret-value.html) (Default is `null`)
4. `Default` - Optional JSON - If the secret cannot be loaded, use this value (if provided) or throw an exception (if not provided). (Default is `null`)

An example file might look like this:

```
{
    "EncryptionKey": {
        "SecretRef::AWS": {
            "SecretId": "MyEncryptionKey"
        }
    }
}
```

If there is a secret stored in AWS SM with a Secret ID of `MyEncryptionKey` and a Value of `"24c37af5-1d62-4e44-b2d6-13d141b66312"` (note the quotation marks make this valid JSON), the equivalent appsettings file would look like this:

```
{
    "EncryptionKey": "24c37af5-1d62-4e44-b2d6-13d141b66312"
}
```

If there is no such secret stored in AWS SM, the application will throw an exception at startup because the default value of `ThrowOnError` is `true`.
