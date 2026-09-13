IF DB_ID('PecDb') IS NULL EXEC('CREATE DATABASE [PecDb]');
GO
USE [PecDb];
GO
IF NOT EXISTS (SELECT 1 FROM sys.schemas WHERE name = 'pec') EXEC('CREATE SCHEMA [pec]');
GO
CREATE TABLE [pec].[PostOfficeData]
(
    [Id] INT IDENTITY(1,1) NOT NULL,
    [NID] VARCHAR(7) NULL,
    [PPNo] VARCHAR(50) NULL,
    [WorkPermitNo] VARCHAR(50) NULL,
    [PO_CustomerName] VARCHAR(255) NULL,
    [PO_TrackingNo] VARCHAR(100) NOT NULL,
    [PO_MobileNo] VARCHAR(30) NOT NULL,
    [PO_EmailAddress] VARCHAR(255) NULL,
    [PO_Weight] DECIMAL(18,3) NULL,
    [PO_CurrentDestination] VARCHAR(255) NULL,
    [PO_CurrentLocation] VARCHAR(255) NULL,
    [PO_CreatedAt] DATETIME2(7) NULL,
    [PO_MplUpdatedAt] DATETIME2(7) NULL,
    [PO_OriginCountryCode] VARCHAR(3) NULL,
    [PO_DestinationCountryCode] VARCHAR(3) NULL,
    [PO_ShippingAddress] VARCHAR(1000) NULL,
    [PO_ItemsDescription] VARCHAR(2000) NULL,
    [PO_Pieces] INT NULL,
    [PO_Value] DECIMAL(18,2) NULL,
    [PO_PackageNumber] VARCHAR(100) NULL,
    [PO_ServiceType] VARCHAR(100) NULL,
    [PO_PackageLastStatus] VARCHAR(100) NULL,
    [McsUpdateAt] DATETIME2(7) NULL,
    [McsUpdatedBy] INT NULL,
    [ValueCurrency] VARCHAR(3) NULL,
    [RecordStatus] INT NULL,
    [CurrentStatus] INT NULL,
    [ReleasingLocation] VARCHAR(255) NULL,
    [ReleasingOfficeCode] VARCHAR(4) NULL,
    [ReleasedBy] INT NULL,
    [ReleasedDateTime] DATETIME2(7) NULL,
    [ReceiverNid] VARCHAR(7) NULL,
    [ReceiverPPNo] VARCHAR(50) NULL,
    [ReceiverWorkPermitNo] VARCHAR(50) NULL,
    [ReceiverMobileNo] VARCHAR(30) NULL,
    [ReceiverAddress] VARCHAR(1000) NULL,
    [IsHit] BIT NULL,
    CONSTRAINT [PK_PostOfficeData] PRIMARY KEY CLUSTERED ([Id] ASC)
);
GO
