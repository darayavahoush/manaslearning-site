SET QUOTED_IDENTIFIER ON;

IF OBJECT_ID('dbo.ConsultService') IS NULL
CREATE TABLE dbo.ConsultService (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Code NVARCHAR(40) NOT NULL UNIQUE,
    Name NVARCHAR(120) NOT NULL,
    Description NVARCHAR(500) NULL,
    DurationMinutes INT NOT NULL,
    SlotStepMinutes INT NOT NULL,
    BufferMinutes INT NOT NULL DEFAULT 0,
    MinNoticeHours INT NOT NULL DEFAULT 4,
    MaxAdvanceDays INT NOT NULL DEFAULT 30,
    PriceInr DECIMAL(10,2) NOT NULL DEFAULT 0,
    IsActive BIT NOT NULL DEFAULT 1,
    SortOrder INT NOT NULL DEFAULT 0
);

IF OBJECT_ID('dbo.ConsultStaff') IS NULL
CREATE TABLE dbo.ConsultStaff (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    DisplayName NVARCHAR(120) NOT NULL,
    Email NVARCHAR(256) NOT NULL,
    Phone NVARCHAR(30) NULL,
    IsActive BIT NOT NULL DEFAULT 1,
    SortOrder INT NOT NULL DEFAULT 0,
    CreatedUtc DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME()
);

IF OBJECT_ID('dbo.ConsultStaffService') IS NULL
CREATE TABLE dbo.ConsultStaffService (
    StaffId INT NOT NULL REFERENCES dbo.ConsultStaff(Id),
    ServiceId INT NOT NULL REFERENCES dbo.ConsultService(Id),
    PRIMARY KEY (StaffId, ServiceId)
);

IF OBJECT_ID('dbo.ConsultWeeklyHour') IS NULL
CREATE TABLE dbo.ConsultWeeklyHour (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    StaffId INT NOT NULL REFERENCES dbo.ConsultStaff(Id),
    DayOfWeek TINYINT NOT NULL,          -- 0 = Sunday ... 6 = Saturday
    StartMinute SMALLINT NOT NULL,       -- minutes after midnight IST, e.g. 600 = 10:00
    EndMinute SMALLINT NOT NULL,
    CONSTRAINT CK_ConsultWeeklyHour CHECK (DayOfWeek BETWEEN 0 AND 6 AND StartMinute >= 0 AND EndMinute <= 1440 AND StartMinute < EndMinute)
);

IF OBJECT_ID('dbo.ConsultTimeOff') IS NULL
CREATE TABLE dbo.ConsultTimeOff (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    StaffId INT NOT NULL REFERENCES dbo.ConsultStaff(Id),
    StartUtc DATETIME2 NOT NULL,
    EndUtc DATETIME2 NOT NULL,
    Reason NVARCHAR(200) NULL,
    CONSTRAINT CK_ConsultTimeOff CHECK (StartUtc < EndUtc)
);

IF OBJECT_ID('dbo.ConsultBooking') IS NULL
CREATE TABLE dbo.ConsultBooking (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    ServiceId INT NOT NULL REFERENCES dbo.ConsultService(Id),
    StaffId INT NOT NULL REFERENCES dbo.ConsultStaff(Id),
    StartUtc DATETIME2 NOT NULL,
    EndUtc DATETIME2 NOT NULL,
    Status TINYINT NOT NULL,             -- 0 PendingPayment, 1 Confirmed, 2 Cancelled, 3 Completed, 4 NoShow
    ParentName NVARCHAR(120) NOT NULL,
    Email NVARCHAR(256) NOT NULL,
    Phone NVARCHAR(30) NOT NULL,
    ChildName NVARCHAR(120) NULL,
    Notes NVARCHAR(1000) NULL,
    PaymentOrderId NVARCHAR(100) NULL,
    HoldExpiresUtc DATETIME2 NULL,
    ManageToken UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
    CreatedUtc DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME()
);

-- Safety net: one live booking per staff member per start time.
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'UX_ConsultBooking_StaffStart')
CREATE UNIQUE INDEX UX_ConsultBooking_StaffStart ON dbo.ConsultBooking (StaffId, StartUtc) WHERE Status <= 1;

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_ConsultBooking_Start')
CREATE INDEX IX_ConsultBooking_Start ON dbo.ConsultBooking (StartUtc, StaffId);

-- Starter services (editable later from the team portal). Paid duration is an assumption.
IF NOT EXISTS (SELECT 1 FROM dbo.ConsultService)
INSERT INTO dbo.ConsultService (Code, Name, Description, DurationMinutes, SlotStepMinutes, BufferMinutes, MinNoticeHours, MaxAdvanceDays, PriceInr, SortOrder) VALUES
 ('free-15', 'Free Introductory Call', 'A quick 15-minute chat to understand your concerns and how we can help.', 15, 15, 5, 4, 30, 0, 1),
 ('iep-30',  'IEP Consultation', 'A 30-minute conversation about an Individualised Education Plan for your child.', 30, 30, 10, 12, 30, 0, 2),
 ('paid-45', 'Detailed Consultation', 'An in-depth 45-minute session with a specialist.', 45, 45, 15, 12, 45, 999, 3);
