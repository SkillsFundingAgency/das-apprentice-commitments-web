# Apprentice Commitments UI

This is the user-facing website for the Apprentice Portal's Electronic Commitment Statement microsite.

## Contents

## Introduction

This is the main cmad website which allows the apprentice to confirm his apprenticeship.  The SFA.DAS.ApprenticeCommitments.Web project should be run in kestrel to ensure that the 7070 port is used.

## Developer Setup

Create your local `appsettings.Development.json` with:

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft": "Warning",
      "Microsoft.Hosting.Lifetime": "Information"
    }
  },
  "ConnectionStrings": {
    "RedisConnectionString": "localhost",
    "DataProtectionKeysDatabase": "DefaultDatabase=3"
  },
  "cdn": {
    "url": "https://das-at-frnt-end.azureedge.net"
  },
  "Authentication": {
    "MetadataAddress": "https://localhost:5001"
  },
  "ApprenticeCommitmentsApi": {
    //Endpoints
    "ApiBaseUrl": "https://localhost:5121"
    //API
    //"ApiBaseUrl": "https://localhost:5501"    
  },
  "Hashing": {
    "AllowedHashstringCharacters": "abcdefgh12345678",
    "Hashstring": "testing"
  },
  "GoogleAnalytics": {
    "GoogleTagManagerId": "xxx"
  },
  "ApplicationUrls": {
    "ApprenticeHomeUrl": "https://localhost:44398",
    "ApprenticeCommitmentsUrl": "https://localhost:7070",
    "ApprenticeLoginUrl": "https://localhost:5001/",
    "ApprenticeAccountsUrl": "https://localhost:7080"
  },
  "ApprenticeCommitmentsBaseUrl": "https://localhost:44398/"
}
```

### Requirements

### Setup

This API uses the standard Apprenticeship Service configuration. All configuration can be found in the [das-employer-config repository](https://github.com/SkillsFundingAgency/das-employer-config).

### Config
