# Agent Configuration

This document describes the configuration setup for the Agent Service Fabric service with proper environment-specific configuration loading.

## Configuration Files

The application uses environment-specific configuration files loaded in the following order:

1. `appsettings.json` - Base configuration (default/fallback values)
2. `appsettings.{Environment}.json` - Environment-specific overrides
3. Environment variables - Runtime overrides

### Supported Environments
- `Development` - Loads `appsettings.Development.json`
- `qa` - Loads `appsettings.qa.json`
- Any other value - Uses only `appsettings.json`

## Configuration Properties

### CacheServerUrl
- **Type**: `string`
- **Description**: The base URL for the cache server endpoint. This is also used for service registration endpoints.
- **Example**: `"https://cache-server.example.com"` or `"https://localhost:5001"`

### ApiKey
- **Type**: `string`
- **Description**: API key for authenticating with external services
- **Security**: This value should be kept secure and not logged

## Environment Setup

### Setting the Environment

Set the `ASPNETCORE_ENVIRONMENT` environment variable:
# For Development
set ASPNETCORE_ENVIRONMENT=Development

# For QA
set ASPNETCORE_ENVIRONMENT=qa

# For Production (or any other environment)
set ASPNETCORE_ENVIRONMENT=Production
### Configuration Validation

The application includes built-in configuration validation that:
- Logs the current environment on startup
- Validates required configuration properties
- Lists all loaded configuration sources
- Verifies environment-specific files are found and loaded

## API Endpoints for Configuration Testing

### Configuration Endpoints

#### Get Current Configuration
- **GET** `/api/configuration/info`
- **Description**: Shows current configuration with environment information
- **Response**: Configuration values with sensitive data masked

#### Get Cache Server URL
- **GET** `/api/configuration/cache-server-url`
- **Description**: Returns the current cache server URL and environment

#### Get Live Configuration
- **GET** `/api/configuration/current`
- **Description**: Gets current configuration using IOptionsMonitor (supports live updates)

### Environment Testing Endpoints

#### Get Environment Information
- **GET** `/api/environment/info`
- **Description**: Shows current environment, configuration files loaded, and validation status

#### Validate Configuration
- **POST** `/api/environment/validate`
- **Description**: Runs configuration validation and returns detailed results

#### Get All Configuration Keys
- **GET** `/api/environment/config`
- **Description**: Lists all configuration keys with sensitive data masked

#### Test Configuration Files
- **GET** `/api/environment/test-files`
- **Description**: Tests if configuration files exist and shows file information

## Service Registration

The agent can register services with the cache server using the configured API key. Service registration requests are sent to the same server specified in `CacheServerUrl`.

### API Endpoints

#### Register Single Service
- **POST** `/api/serviceregistration/{serviceName}/register`
- **Description**: Registers a single service with the cache server
- **Authentication**: Uses the configured API key automatically

#### Register Multiple Services
- **POST** `/api/serviceregistration/register-multiple`
- **Body**: Array of service names
- **Description**: Registers multiple services in a single request
- **Authentication**: Uses the configured API key automatically

### Usage Examples

#### Register a servicecurl -X POST https://localhost:5000/api/serviceregistration/MyService/register
#### Register multiple servicescurl -X POST https://localhost:5000/api/serviceregistration/register-multiple \
  -H "Content-Type: application/json" \
  -d '["Service1", "Service2", "Service3"]'
## Code Usage Examples

### Constructor Injection (Recommended)public class MyService
{
    private readonly AgentConfiguration _config;

    public MyService(AgentConfiguration config)
    {
        _config = config;
        // Use _config.CacheServerUrl and _config.ApiKey
        // Configuration will be automatically loaded per environment
    }
}
### IOptions Pattern (For live updates)public class MyService
{
    private readonly IOptionsMonitor<AgentConfiguration> _options;

    public MyService(IOptionsMonitor<AgentConfiguration> options)
    {
        _options = options;
        var currentConfig = _options.CurrentValue;
        // Use currentConfig.CacheServerUrl and currentConfig.ApiKey
        // Supports configuration changes without restart
    }
}
### Service Registration Servicepublic class MyBackgroundService
{
    private readonly IServiceRegistrationService _registrationService;

    public MyBackgroundService(IServiceRegistrationService registrationService)
    {
        _registrationService = registrationService;
    }

    public async Task RegisterAsync()
    {
        // API key and URL are automatically loaded from environment-specific config
        await _registrationService.RegisterServiceAsync("MyService");
    }
}
### Configuration Validation Servicepublic class MyService
{
    private readonly IConfigurationValidationService _validationService;

    public MyService(IConfigurationValidationService validationService)
    {
        _validationService = validationService;
    }

    public async Task CheckConfigAsync()
    {
        var isValid = await _validationService.ValidateConfigurationAsync();
        var info = _validationService.GetEnvironmentInfo();
    }
}
## Troubleshooting Configuration

### Debugging Configuration Loading

1. **Check Environment Variable**: Ensure `ASPNETCORE_ENVIRONMENT` is set correctly
2. **Verify Files Exist**: Use `/api/environment/test-files` to check configuration files
3. **Validate Configuration**: Use `/api/environment/validate` to run validation
4. **Check Logs**: Look for configuration validation logs in the application startup logs
5. **View All Config**: Use `/api/environment/config` to see all loaded configuration

### Common Issues

- **Environment file not found**: Ensure `appsettings.{Environment}.json` exists in the content root
- **Configuration not overriding**: Check that environment-specific values are properly formatted
- **API Key not loading**: Verify the API key exists in the environment-specific file

## Service Fabric Deployment

When deploying to Service Fabric:

1. **Set Environment Variable**: Configure `ASPNETCORE_ENVIRONMENT` in the service manifest
2. **Include Configuration Files**: Ensure all appsettings files are included in the deployment package
3. **Secure Sensitive Data**: Use Azure Key Vault or secure configuration providers for production
4. **Test Configuration**: Use the validation endpoints to verify configuration after deployment
5. **Monitor Logs**: Check application logs for configuration validation results

### Service Manifest Example<ServiceManifest>
  <ConfigOverrides>
    <ConfigOverride Name="Config">
      <Settings>
        <Section Name="Environment">
          <Parameter Name="ASPNETCORE_ENVIRONMENT" Value="Production" />
        </Section>
      </Settings>
    </ConfigOverride>
  </ConfigOverrides>
</ServiceManifest>