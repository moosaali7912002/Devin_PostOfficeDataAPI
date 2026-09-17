# PostOfficeApi (.NET 8 Web API)

Records post office data into the existing `pec.PostOfficeData` table in MS SQL Server.
Only the `PO_*` fields are accepted from clients; all other columns (NID, receiver info,
status columns, etc.) are left to the existing back-office processes.


Incoming Payload sample:
```
{
    "status": "success",
    "mail_item": {
        "customer_name": "Test Moosa 2",
        "tracking_no": "XX123456789XX",
        "mobile_no": "7778888",
        "email_address": "",
        "weight": "",
        "mail_class": "",
        "mail_sub_class": "",
        "current_destination": "",
        "current_location": "",
        "created_at": "02-09-2026 14:25:52",
        "updated_at": "08-09-2026 10:43:14",
        "origin_country": "",
        "destination_country": "",
        "shipping_address": "",
        "items": "",
        "pieces": 1,
        "value": 0,
        "package_number": "",
        "scan_time_zone": "UTC+05:00",
        "events": [
            {
                "details": "Item Delivered",
                "created_at": "08-09-2026 10:43:14"
            },
            {
                "details": "Item Information Created",
                "created_at": "02-09-2026 14:25:52"
            }
        ]
    }
}
```

## Structure

```
PostOfficeApi/
  Controllers/PostOfficeDataController.cs   HTTP endpoints
  Services/IPostOfficeDataService.cs        service contract
  Services/PostOfficeDataService.cs         business logic + mapping
  Models/PostOfficeData.cs                  EF Core entity mapped to pec.PostOfficeData
  Models/Dtos/PostOfficeDataRequest.cs      incoming payload (PO_* fields, validated)
  Models/Dtos/PostOfficeDataResponse.cs     outgoing payload
  Data/PostOfficeDbContext.cs               DbContext (table, columns, indexes)
  Program.cs                                DI registration
```

## Configuration

The connection string is **not** stored in `appsettings.json` — keep credentials out of
source control. Set it locally with user secrets (Visual Studio: right-click the project >
Manage User Secrets) or with an environment variable:

```
dotnet user-secrets set "ConnectionStrings:PostOfficeDb" "Server=YOURSERVER;Database=YourDatabase;User Id=YourUser;Password=YourPassword;TrustServerCertificate=True"
```

```
ConnectionStrings__PostOfficeDb=Server=YOURSERVER;Database=YourDatabase;...
```

The app fails fast at startup with a clear message if the connection string is missing.

For Windows authentication use:
`Server=YOURSERVER;Database=YourDatabase;Trusted_Connection=True;TrustServerCertificate=True`

The database table is expected to already exist (created by your DDL script). The API
does not create or migrate the schema.

## Authentication

Every request must be signed. Four headers are required:

| Header | Meaning |
| --- | --- |
| `X-App-Number` | `app_number` — the caller's public application id |
| `X-User-Token` | `user_token` — token issued to the calling user/system |
| `X-Call-DateTime` | ISO 8601 UTC timestamp of the call, e.g. `2026-01-01T10:00:00.000Z` |
| `X-Signature` | Base64 HMAC-SHA256 of the canonical string below, keyed with `app_secret` |

`app_secret` is never transmitted — it is only used to compute the signature.

Canonical string to sign (fields joined with `\n`):

```
app_number
user_token
call_datetime          (exactly the X-Call-DateTime header value)
HTTP_METHOD            (upper case, e.g. POST)
/api/post-office-data  (path + query string)
base64(sha256(body))   (empty string body for GET)
```

```
signature = base64(hmacsha256(app_secret, stringToSign))
```

Rejections return `401`:
- missing/invalid header, unknown `app_number`, or `user_token` not registered for that app;
- `X-Call-DateTime` more than `ApiClients:AllowedClockSkew` (default 5 minutes) away from server UTC time;
- signature mismatch (also catches a tampered body, since the body hash is signed);
- the same signature replayed a second time.

Register callers in configuration (put the secrets in user secrets / environment variables,
not in `appsettings.json`):

```json
"ApiClients": {
  "AllowedClockSkew": "00:05:00",
  "Clients": [
    {
      "AppNumber": "PEC-POST-001",
      "AppSecret": "a-long-random-secret",
      "DisplayName": "Maldives Post",
      "UserTokens": [ "token-issued-to-their-system" ]
    }
  ]
}
```

Leave `UserTokens` empty to accept any token value for that app.

Client-side signing (C#):

```csharp
var body = JsonSerializer.Serialize(payload);
var callDateTime = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ");
var bodyHash = Convert.ToBase64String(SHA256.HashData(Encoding.UTF8.GetBytes(body)));
var stringToSign = string.Join('\n', appNumber, userToken, callDateTime, "POST", "/api/post-office-data", bodyHash);

using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(appSecret));
var signature = Convert.ToBase64String(hmac.ComputeHash(Encoding.UTF8.GetBytes(stringToSign)));
```

`postman/PostOfficeApi.postman_collection.json` contains a pre-request script that does this
automatically — set the `appNumber`, `appSecret` and `userToken` collection variables.

## Endpoints

| Method | Route | Description |
| --- | --- | --- |
| POST | `/api/post-office-data` | Insert one record, returns 201 with the created record |
| POST | `/api/post-office-data/bulk` | Insert many records in one transaction |
| GET | `/api/post-office-data/{id}` | Fetch a record by identity |
| GET | `/api/post-office-data/by-tracking-no/{trackingNo}` | Fetch all records for a tracking number |

Sample POST body (see `PostOfficeApi.http`):

```json
{
  "PO_CustomerName": "John Doe",
  "PO_TrackingNo": "EE123456789MV",
  "PO_MobileNo": "9601234567",
  "PO_EmailAddress": "john.doe@example.com",
  "PO_Weight": 1.250,
  "PO_CurrentDestination": "Male",
  "PO_CurrentLocation": "Hulhumale Sorting Center",
  "PO_CreatedAt": "2026-01-01T10:00:00",
  "PO_MplUpdatedAt": "2026-01-02T08:30:00",
  "PO_OriginCountryCode": "SGP",
  "PO_DestinationCountryCode": "MDV",
  "PO_ShippingAddress": "H. Example, Male, Maldives",
  "PO_ItemsDescription": "Mobile phone accessories",
  "PO_Pieces": 2,
  "PO_Value": 150.00,
  "PO_PackageNumber": "PKG-0001",
  "PO_ServiceType": "EMS",
  "PO_PackageLastStatus": "In Transit"
}
```

`PO_TrackingNo` and `PO_MobileNo` are required (they are NOT NULL in the table); all
other fields are optional. String lengths are validated against the column sizes, so
over-long values return HTTP 400 instead of a SQL truncation error. `PO_CreatedAt`
defaults to `DateTime.UtcNow` when omitted.

JSON property names are kept exactly as declared (no camelCase conversion), so the
payload uses `PO_TrackingNo`, not `pO_TrackingNo`.

## Running

Open `PostOfficeApi.sln` in Visual Studio 2022 (17.8+) and press F5, or:

```
dotnet run --project PostOfficeApi/PostOfficeApi.csproj
```

Swagger UI is available at `/swagger` in the Development environment.
