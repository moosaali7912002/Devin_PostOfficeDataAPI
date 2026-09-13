# PostOfficeApi (.NET 8 Web API)

Records post office data into the existing `pec.PostOfficeData` table in MS SQL Server.
Only the `PO_*` fields are accepted from clients; all other columns (NID, receiver info,
status columns, etc.) are left to the existing back-office processes.

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

Set the connection string in `appsettings.json` (or via environment variable
`ConnectionStrings__PostOfficeDb`):

```json
"ConnectionStrings": {
  "PostOfficeDb": "Server=YOURSERVER;Database=YourDatabase;User Id=YourUser;Password=YourPassword;TrustServerCertificate=True"
}
```

For Windows authentication use:
`Server=YOURSERVER;Database=YourDatabase;Trusted_Connection=True;TrustServerCertificate=True`

The database table is expected to already exist (created by your DDL script). The API
does not create or migrate the schema.

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
