﻿DECLARE
V_COUNT INTEGER;
BEGIN
SELECT COUNT(TABLE_NAME) INTO V_COUNT from USER_TABLES where TABLE_NAME = '__EFMigrationsHistory';
IF V_COUNT = 0 THEN
Begin
BEGIN 
EXECUTE IMMEDIATE 'CREATE TABLE 
"__EFMigrationsHistory" (
    "MigrationId" NVARCHAR2(150) NOT NULL,
    "ProductVersion" NVARCHAR2(32) NOT NULL,
    CONSTRAINT "PK___EFMigrationsHistory" PRIMARY KEY ("MigrationId")
)';
END;

End;

END IF;
EXCEPTION
WHEN OTHERS THEN
    IF(SQLCODE != -942)THEN
        RAISE;
    END IF;
END;
/

DECLARE
    v_Count INTEGER;
BEGIN
SELECT COUNT(*) INTO v_Count FROM "__EFMigrationsHistory" WHERE "MigrationId" = N'20210915064239_Migration_IdentityOracleDbContext_1';
IF v_Count = 0 THEN

    BEGIN 
    EXECUTE IMMEDIATE 'CREATE TABLE 
    "IdentityOrganization" (
        "Id" NUMBER(19) GENERATED BY DEFAULT ON NULL AS IDENTITY NOT NULL,
        "Name" NVARCHAR2(128) NOT NULL,
        "AddressLine1" NVARCHAR2(128) NOT NULL,
        "AddresssLine2" NVARCHAR2(128) NOT NULL,
        "Email" NVARCHAR2(128) NOT NULL,
        "Phone" NVARCHAR2(16) NOT NULL,
        "LogoUri" NVARCHAR2(256) NOT NULL,
        "Active" NUMBER(1) NOT NULL,
        "Enabled" NUMBER(1) NOT NULL,
        "ActiveUntil" TIMESTAMP(7) WITH TIME ZONE NOT NULL,
        "CreatedAt" TIMESTAMP(7) WITH TIME ZONE NOT NULL,
        CONSTRAINT "P_IdentityOrganization_Id_DBB7E" PRIMARY KEY ("Id")
    )';
    END;
 END IF;
END;

/

DECLARE
    v_Count INTEGER;
BEGIN
SELECT COUNT(*) INTO v_Count FROM "__EFMigrationsHistory" WHERE "MigrationId" = N'20210915064239_Migration_IdentityOracleDbContext_1';
IF v_Count = 0 THEN

    BEGIN 
    EXECUTE IMMEDIATE 'CREATE TABLE 
    "IdentityPlatform" (
        "Id" NUMBER(19) GENERATED BY DEFAULT ON NULL AS IDENTITY NOT NULL,
        "Name" NVARCHAR2(128) NOT NULL,
        "Description" NVARCHAR2(512) NOT NULL,
        "SecretKey" NVARCHAR2(128) NOT NULL,
        "LogoUri" NVARCHAR2(256) NOT NULL,
        "Active" NUMBER(1) NOT NULL,
        "Enabled" NUMBER(1) NOT NULL,
        "ActiveUntil" TIMESTAMP(7) WITH TIME ZONE NOT NULL,
        "CreatedAt" TIMESTAMP(7) WITH TIME ZONE NOT NULL,
        CONSTRAINT "P_IdentityPlatform_Id_A8230" PRIMARY KEY ("Id")
    )';
    END;
 END IF;
END;

/

DECLARE
    v_Count INTEGER;
BEGIN
SELECT COUNT(*) INTO v_Count FROM "__EFMigrationsHistory" WHERE "MigrationId" = N'20210915064239_Migration_IdentityOracleDbContext_1';
IF v_Count = 0 THEN

    BEGIN 
    EXECUTE IMMEDIATE 'CREATE TABLE 
    "IdentityRole" (
        "Id" NUMBER(19) GENERATED BY DEFAULT ON NULL AS IDENTITY NOT NULL,
        "Name" NVARCHAR2(128) NOT NULL,
        "CreatedAt" TIMESTAMP(7) WITH TIME ZONE NOT NULL,
        "Active" NUMBER(1) NOT NULL,
        "Enabled" NUMBER(1) NOT NULL,
        "ActiveUntil" TIMESTAMP(7) WITH TIME ZONE NOT NULL,
        CONSTRAINT "P_IdentityRole_Id_5F896" PRIMARY KEY ("Id")
    )';
    END;
 END IF;
END;

/

DECLARE
    v_Count INTEGER;
BEGIN
SELECT COUNT(*) INTO v_Count FROM "__EFMigrationsHistory" WHERE "MigrationId" = N'20210915064239_Migration_IdentityOracleDbContext_1';
IF v_Count = 0 THEN

    BEGIN 
    EXECUTE IMMEDIATE 'CREATE TABLE 
    "IdentityUser" (
        "Id" NUMBER(19) GENERATED BY DEFAULT ON NULL AS IDENTITY NOT NULL,
        "UniqueId" NVARCHAR2(128) NOT NULL,
        "Username" NVARCHAR2(128) NOT NULL,
        "NormalizedUsername" NVARCHAR2(128) NOT NULL,
        "Password" NVARCHAR2(128) NOT NULL,
        "Email" NVARCHAR2(128) NOT NULL,
        "NormalizedEmail" NVARCHAR2(128) NOT NULL,
        "Phone" NVARCHAR2(16),
        "Mobile" NVARCHAR2(16) NOT NULL,
        "CreatedAt" TIMESTAMP(7) WITH TIME ZONE NOT NULL,
        "Enabled" NUMBER(1) NOT NULL,
        "Active" NUMBER(1) NOT NULL,
        "EmailConfirmed" NUMBER(1) NOT NULL,
        "EmailConfirmationCode" NVARCHAR2(2000) NOT NULL,
        "EmailConfirmationDeadline" TIMESTAMP(7) WITH TIME ZONE NOT NULL,
        "MobileConfirmed" NUMBER(1) NOT NULL,
        "MobileConfirmationCode" NVARCHAR2(2000) NOT NULL,
        "MobileConfirmationDeadline" TIMESTAMP(7) WITH TIME ZONE NOT NULL,
        "LoginFailCount" NUMBER(10) NOT NULL,
        "LockoutUntil" TIMESTAMP(7) WITH TIME ZONE NOT NULL,
        CONSTRAINT "P_IdentityUser_Id_D06A2" PRIMARY KEY ("Id")
    )';
    END;
 END IF;
END;

/

DECLARE
    v_Count INTEGER;
BEGIN
SELECT COUNT(*) INTO v_Count FROM "__EFMigrationsHistory" WHERE "MigrationId" = N'20210915064239_Migration_IdentityOracleDbContext_1';
IF v_Count = 0 THEN

    BEGIN 
    EXECUTE IMMEDIATE 'CREATE TABLE 
    "IdentityPermission" (
        "Id" NUMBER(19) GENERATED BY DEFAULT ON NULL AS IDENTITY NOT NULL,
        "UserId" NUMBER(19) NOT NULL,
        "RoleId" NUMBER(19) NOT NULL,
        "OrganizationId" NUMBER(19) NOT NULL,
        "Active" NUMBER(1) NOT NULL,
        "CreatedAt" TIMESTAMP(7) WITH TIME ZONE NOT NULL,
        "ActiveUntil" TIMESTAMP(7) WITH TIME ZONE NOT NULL,
        "Enabled" NUMBER(1) NOT NULL,
        CONSTRAINT "P_IdentityPermission_Id_AA4E4" PRIMARY KEY ("Id"),
        CONSTRAINT "F_IdentityPermission_OrganizationId_98097" FOREIGN KEY ("OrganizationId") REFERENCES "IdentityOrganization" ("Id"),
        CONSTRAINT "F_IdentityPermission_RoleId_63EA2" FOREIGN KEY ("RoleId") REFERENCES "IdentityRole" ("Id"),
        CONSTRAINT "F_IdentityPermission_UserId_C3DFE" FOREIGN KEY ("UserId") REFERENCES "IdentityUser" ("Id")
    )';
    END;
 END IF;
END;

/

DECLARE
    v_Count INTEGER;
BEGIN
SELECT COUNT(*) INTO v_Count FROM "__EFMigrationsHistory" WHERE "MigrationId" = N'20210915064239_Migration_IdentityOracleDbContext_1';
IF v_Count = 0 THEN

    BEGIN 
    EXECUTE IMMEDIATE 'CREATE TABLE 
    "IdentitySession" (
        "Id" NUMBER(19) GENERATED BY DEFAULT ON NULL AS IDENTITY NOT NULL,
        "PlatformId" NUMBER(19) NOT NULL,
        "PermissionId" NUMBER(19) NOT NULL,
        "Jti" NVARCHAR2(128) NOT NULL,
        "TokenHash" NVARCHAR2(128) NOT NULL,
        "RefreshToken" NVARCHAR2(128) NOT NULL,
        "Key" NVARCHAR2(128) NOT NULL,
        "ClientUserAgent" NVARCHAR2(512) NOT NULL,
        "ClientFriendlyName" NVARCHAR2(128) NOT NULL,
        "ClientOS" NVARCHAR2(128) NOT NULL,
        "ClientIP" NVARCHAR2(64) NOT NULL,
        "ClientTimezone" NUMBER(10) NOT NULL,
        "ClientLatitude" DECIMAL(18, 2) NOT NULL,
        "ClientLongitude" DECIMAL(18, 2) NOT NULL,
        "CreatedAt" TIMESTAMP(7) WITH TIME ZONE NOT NULL,
        "ExpiresAt" TIMESTAMP(7) WITH TIME ZONE NOT NULL,
        "Revoked" NUMBER(1) NOT NULL,
        CONSTRAINT "P_IdentitySession_Id_ECB1D" PRIMARY KEY ("Id"),
        CONSTRAINT "F_IdentitySession_PermissionId_68ABC" FOREIGN KEY ("PermissionId") REFERENCES "IdentityPermission" ("Id"),
        CONSTRAINT "F_IdentitySession_PlatformId_83F1D" FOREIGN KEY ("PlatformId") REFERENCES "IdentityPlatform" ("Id")
    )';
    END;
 END IF;
END;

/

DECLARE
    v_Count INTEGER;
BEGIN
SELECT COUNT(*) INTO v_Count FROM "__EFMigrationsHistory" WHERE "MigrationId" = N'20210915064239_Migration_IdentityOracleDbContext_1';
IF v_Count = 0 THEN

    EXECUTE IMMEDIATE '
    CREATE UNIQUE INDEX "U_IdentityOrganization_Name_F13BC" ON "IdentityOrganization" ("Name")
    ';
 END IF;
END;

/

DECLARE
    v_Count INTEGER;
BEGIN
SELECT COUNT(*) INTO v_Count FROM "__EFMigrationsHistory" WHERE "MigrationId" = N'20210915064239_Migration_IdentityOracleDbContext_1';
IF v_Count = 0 THEN

    EXECUTE IMMEDIATE '
    CREATE INDEX "I_IdentityPermission_OrganizationId_EF38A" ON "IdentityPermission" ("OrganizationId")
    ';
 END IF;
END;

/

DECLARE
    v_Count INTEGER;
BEGIN
SELECT COUNT(*) INTO v_Count FROM "__EFMigrationsHistory" WHERE "MigrationId" = N'20210915064239_Migration_IdentityOracleDbContext_1';
IF v_Count = 0 THEN

    EXECUTE IMMEDIATE '
    CREATE INDEX "I_IdentityPermission_RoleId_C1B82" ON "IdentityPermission" ("RoleId")
    ';
 END IF;
END;

/

DECLARE
    v_Count INTEGER;
BEGIN
SELECT COUNT(*) INTO v_Count FROM "__EFMigrationsHistory" WHERE "MigrationId" = N'20210915064239_Migration_IdentityOracleDbContext_1';
IF v_Count = 0 THEN

    EXECUTE IMMEDIATE '
    CREATE INDEX "I_IdentityPermission_UserId_A10AB" ON "IdentityPermission" ("UserId")
    ';
 END IF;
END;

/

DECLARE
    v_Count INTEGER;
BEGIN
SELECT COUNT(*) INTO v_Count FROM "__EFMigrationsHistory" WHERE "MigrationId" = N'20210915064239_Migration_IdentityOracleDbContext_1';
IF v_Count = 0 THEN

    EXECUTE IMMEDIATE '
    CREATE UNIQUE INDEX "U_IdentityPermission_UserId_RoleId_OrganizationId_800D6" ON "IdentityPermission" ("UserId", "RoleId", "OrganizationId")
    ';
 END IF;
END;

/

DECLARE
    v_Count INTEGER;
BEGIN
SELECT COUNT(*) INTO v_Count FROM "__EFMigrationsHistory" WHERE "MigrationId" = N'20210915064239_Migration_IdentityOracleDbContext_1';
IF v_Count = 0 THEN

    EXECUTE IMMEDIATE '
    CREATE UNIQUE INDEX "U_IdentityPlatform_Name_423B5" ON "IdentityPlatform" ("Name")
    ';
 END IF;
END;

/

DECLARE
    v_Count INTEGER;
BEGIN
SELECT COUNT(*) INTO v_Count FROM "__EFMigrationsHistory" WHERE "MigrationId" = N'20210915064239_Migration_IdentityOracleDbContext_1';
IF v_Count = 0 THEN

    EXECUTE IMMEDIATE '
    CREATE UNIQUE INDEX "U_IdentityPlatform_SecretKey_E10BA" ON "IdentityPlatform" ("SecretKey")
    ';
 END IF;
END;

/

DECLARE
    v_Count INTEGER;
BEGIN
SELECT COUNT(*) INTO v_Count FROM "__EFMigrationsHistory" WHERE "MigrationId" = N'20210915064239_Migration_IdentityOracleDbContext_1';
IF v_Count = 0 THEN

    EXECUTE IMMEDIATE '
    CREATE UNIQUE INDEX "U_IdentityRole_Name_36689" ON "IdentityRole" ("Name")
    ';
 END IF;
END;

/

DECLARE
    v_Count INTEGER;
BEGIN
SELECT COUNT(*) INTO v_Count FROM "__EFMigrationsHistory" WHERE "MigrationId" = N'20210915064239_Migration_IdentityOracleDbContext_1';
IF v_Count = 0 THEN

    EXECUTE IMMEDIATE '
    CREATE INDEX "I_IdentitySession_PermissionId_EB569" ON "IdentitySession" ("PermissionId")
    ';
 END IF;
END;

/

DECLARE
    v_Count INTEGER;
BEGIN
SELECT COUNT(*) INTO v_Count FROM "__EFMigrationsHistory" WHERE "MigrationId" = N'20210915064239_Migration_IdentityOracleDbContext_1';
IF v_Count = 0 THEN

    EXECUTE IMMEDIATE '
    CREATE INDEX "I_IdentitySession_PlatformId_0A8EA" ON "IdentitySession" ("PlatformId")
    ';
 END IF;
END;

/

DECLARE
    v_Count INTEGER;
BEGIN
SELECT COUNT(*) INTO v_Count FROM "__EFMigrationsHistory" WHERE "MigrationId" = N'20210915064239_Migration_IdentityOracleDbContext_1';
IF v_Count = 0 THEN

    EXECUTE IMMEDIATE '
    CREATE UNIQUE INDEX "U_IdentitySession_Jti_B9210" ON "IdentitySession" ("Jti")
    ';
 END IF;
END;

/

DECLARE
    v_Count INTEGER;
BEGIN
SELECT COUNT(*) INTO v_Count FROM "__EFMigrationsHistory" WHERE "MigrationId" = N'20210915064239_Migration_IdentityOracleDbContext_1';
IF v_Count = 0 THEN

    EXECUTE IMMEDIATE '
    CREATE UNIQUE INDEX "U_IdentitySession_Key_45F50" ON "IdentitySession" ("Key")
    ';
 END IF;
END;

/

DECLARE
    v_Count INTEGER;
BEGIN
SELECT COUNT(*) INTO v_Count FROM "__EFMigrationsHistory" WHERE "MigrationId" = N'20210915064239_Migration_IdentityOracleDbContext_1';
IF v_Count = 0 THEN

    EXECUTE IMMEDIATE '
    CREATE UNIQUE INDEX "U_IdentitySession_RefreshToken_BC11C" ON "IdentitySession" ("RefreshToken")
    ';
 END IF;
END;

/

DECLARE
    v_Count INTEGER;
BEGIN
SELECT COUNT(*) INTO v_Count FROM "__EFMigrationsHistory" WHERE "MigrationId" = N'20210915064239_Migration_IdentityOracleDbContext_1';
IF v_Count = 0 THEN

    EXECUTE IMMEDIATE '
    CREATE UNIQUE INDEX "U_IdentitySession_TokenHash_15499" ON "IdentitySession" ("TokenHash")
    ';
 END IF;
END;

/

DECLARE
    v_Count INTEGER;
BEGIN
SELECT COUNT(*) INTO v_Count FROM "__EFMigrationsHistory" WHERE "MigrationId" = N'20210915064239_Migration_IdentityOracleDbContext_1';
IF v_Count = 0 THEN

    EXECUTE IMMEDIATE '
    CREATE UNIQUE INDEX "U_IdentityUser_Email_A5252" ON "IdentityUser" ("Email")
    ';
 END IF;
END;

/

DECLARE
    v_Count INTEGER;
BEGIN
SELECT COUNT(*) INTO v_Count FROM "__EFMigrationsHistory" WHERE "MigrationId" = N'20210915064239_Migration_IdentityOracleDbContext_1';
IF v_Count = 0 THEN

    EXECUTE IMMEDIATE '
    CREATE UNIQUE INDEX "U_IdentityUser_Mobile_B52FB" ON "IdentityUser" ("Mobile")
    ';
 END IF;
END;

/

DECLARE
    v_Count INTEGER;
BEGIN
SELECT COUNT(*) INTO v_Count FROM "__EFMigrationsHistory" WHERE "MigrationId" = N'20210915064239_Migration_IdentityOracleDbContext_1';
IF v_Count = 0 THEN

    EXECUTE IMMEDIATE '
    CREATE UNIQUE INDEX "U_IdentityUser_UniqueId_E083B" ON "IdentityUser" ("UniqueId")
    ';
 END IF;
END;

/

DECLARE
    v_Count INTEGER;
BEGIN
SELECT COUNT(*) INTO v_Count FROM "__EFMigrationsHistory" WHERE "MigrationId" = N'20210915064239_Migration_IdentityOracleDbContext_1';
IF v_Count = 0 THEN

    EXECUTE IMMEDIATE '
    CREATE UNIQUE INDEX "U_IdentityUser_Username_85643" ON "IdentityUser" ("Username")
    ';
 END IF;
END;

/

DECLARE
    v_Count INTEGER;
BEGIN
SELECT COUNT(*) INTO v_Count FROM "__EFMigrationsHistory" WHERE "MigrationId" = N'20210915064239_Migration_IdentityOracleDbContext_1';
IF v_Count = 0 THEN

    EXECUTE IMMEDIATE '
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES (N''20210915064239_Migration_IdentityOracleDbContext_1'', N''5.0.10'')
    ';
 END IF;
END;

/

