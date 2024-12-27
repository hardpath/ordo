# M365 Authentication Method

## Overview
This application uses the **Application Permissions (Non-Interactive)** authentication model to access Microsoft 365 resources (Microsoft ToDo and Calendar).

## Why Application Permissions?
- The application is designed to run as a **background service** (Windows service) without user interaction.
- **Application Permissions** allow the app to authenticate as itself using the `ClientId`, `ClientSecret`, and `TenantId`, without requiring user login.

## Benefits of Application Permissions
- **No User Interaction**: Suitable for non-interactive environments like background services.
- **Organisational Access**: Enables the app to access resources across the tenant.
- **Scalable**: The app can operate autonomously without user involvement.

## Considerations
- The `/me` endpoint is not supported with Application Permissions. Instead, queries must specify the target user (e.g., `Users[userId]`).
- Admin consent is required to grant Application Permissions.

## Future Enhancements
If needed, **Delegated Permissions** can be added later for scenarios requiring user context (e.g., `/me` endpoint or interactive login).

## Azure Configuration
1. **App Registration**:
   - The app is registered in Azure AD with the following details:
     - **ClientId**: (your app's client ID)
     - **TenantId**: (your tenant ID)
   - Permissions granted:
     - `Tasks.ReadWrite`
     - `Calendars.ReadWrite`

2. **Client Secret**:
   - The app authenticates using a client secret stored securely in `config.json`.

## References
- [Microsoft Graph API Documentation](https://learn.microsoft.com/en-us/graph/)
- [Azure Identity and Authentication](https://learn.microsoft.com/en-us/azure/active-directory/develop/)
