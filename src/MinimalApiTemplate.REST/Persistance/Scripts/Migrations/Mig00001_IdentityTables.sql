IF NOT EXISTS (
	SELECT 1 FROM INFORMATION_SCHEMA.TABLES
	WHERE TABLE_SCHEMA = 'dbo'
	AND TABLE_NAME = 'Users'
)
BEGIN
	CREATE TABLE [dbo].[Users](
		[Id] INT IDENTITY(1, 1) NOT NULL,
        [UserName] NVARCHAR (100) NULL,
        [NormalizedUserName] NVARCHAR (100) NULL,
        [Email] NVARCHAR (100) NULL,
        [NormalizedEmail] NVARCHAR (100) NULL,
        [EmailConfirmed] BIT NOT NULL,
        [PasswordHash] NVARCHAR (MAX) NULL,
        [SecurityStamp] NVARCHAR (MAX) NULL,
        [ConcurrencyStamp] NVARCHAR (MAX) NULL,
        [PhoneNumber] NVARCHAR (30) NULL,
        [PhoneNumberConfirmed] BIT NOT NULL,
        [TwoFactorEnabled] BIT NOT NULL,
        [LockoutEnd] DATETIMEOFFSET (7) NULL,
        [LockoutEnabled] BIT NOT NULL,
        [AccessFailedCount] INT NOT NULL,
        [RefreshToken] NVARCHAR (MAX) NULL,
        [RefreshTokenExpiryTime] DATETIME NULL,
        CONSTRAINT [PK_Users] PRIMARY KEY CLUSTERED ([Id] ASC)
	);

    CREATE UNIQUE NONCLUSTERED INDEX [IX_Users_UserName]
    ON [dbo].[Users] ([NormalizedUserName] ASC) WHERE [NormalizedUserName] IS NOT NULL

    CREATE UNIQUE NONCLUSTERED INDEX [IX_Users_Email]
    ON [dbo].[Users] ([NormalizedEmail] ASC)
END


IF NOT EXISTS (
	SELECT 1 FROM INFORMATION_SCHEMA.TABLES
	WHERE TABLE_SCHEMA = 'dbo'
	AND TABLE_NAME = 'Roles'
)
BEGIN
	CREATE TABLE [dbo].[Roles](
		[Id] INT IDENTITY(1, 1) NOT NULL,
        [Name] NVARCHAR (100) NULL,
        [NormalizedName] NVARCHAR (100) NULL,
        [ConcurrencyStamp] NVARCHAR (100) NULL,
        CONSTRAINT [PK_Roles] PRIMARY KEY CLUSTERED ([Id] ASC)
	);

    CREATE UNIQUE NONCLUSTERED INDEX [IX_Roles_Name]
    ON [dbo].[Roles] ([NormalizedName] ASC) WHERE [NormalizedName] IS NOT NULL;
END


IF NOT EXISTS (
	SELECT 1 FROM INFORMATION_SCHEMA.TABLES
	WHERE TABLE_SCHEMA = 'dbo'
	AND TABLE_NAME = 'UserRoles'
)
BEGIN
	CREATE TABLE [dbo].[UserRoles](
		[UserId] INT NOT NULL,
        [RoleId] INT NOT NULL,
        CONSTRAINT [PK_UserRoles] PRIMARY KEY CLUSTERED ([UserId] ASC, [RoleId] ASC),
        CONSTRAINT [FK_UserRoles_Roles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [dbo].[Roles] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_UserRoles_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [dbo].[Users] ([Id]) ON DELETE CASCADE
	);

    CREATE NONCLUSTERED INDEX [IX_UserRoles_UserId]
    ON [dbo].[UserRoles]([UserId] ASC);
END


IF NOT EXISTS (
	SELECT 1 FROM INFORMATION_SCHEMA.TABLES
	WHERE TABLE_SCHEMA = 'dbo'
	AND TABLE_NAME = 'UserClaims'
)
BEGIN
	CREATE TABLE [dbo].[UserClaims](
        [Id] INT IDENTITY(1, 1) NOT NULL,           
		[UserId] INT NOT NULL,
        [ClaimType] NVARCHAR(100) NULL,
        [ClaimValue] NVARCHAR(100) NULL,
        CONSTRAINT [PK_UserClaims] PRIMARY KEY CLUSTERED ([Id] ASC),
        CONSTRAINT [FK_UserClaims_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [dbo].[Users] ([Id]) ON DELETE CASCADE
	);

    CREATE NONCLUSTERED INDEX [IX_UserClaims_UserId]
    ON [dbo].[UserClaims]([UserId] ASC);
END


IF NOT EXISTS (
	SELECT 1 FROM INFORMATION_SCHEMA.TABLES
	WHERE TABLE_SCHEMA = 'dbo'
	AND TABLE_NAME = 'RoleClaims'
)
BEGIN
	CREATE TABLE [dbo].[RoleClaims](
        [Id] INT IDENTITY(1, 1) NOT NULL,           
		[RoleId] INT NOT NULL,
        [ClaimType] NVARCHAR(100) NULL,
        [ClaimValue] NVARCHAR(100) NULL,
        CONSTRAINT [PK_RoleClaims] PRIMARY KEY CLUSTERED ([Id] ASC),
        CONSTRAINT [FK_RoleClaims_Roles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [dbo].[Roles] ([Id]) ON DELETE CASCADE
	);

    CREATE NONCLUSTERED INDEX [IX_RoleClaims_RoleId]
    ON [dbo].[RoleClaims]([RoleId] ASC);
END


IF NOT EXISTS (
	SELECT 1 FROM INFORMATION_SCHEMA.TABLES
	WHERE TABLE_SCHEMA = 'dbo'
	AND TABLE_NAME = 'UserLogins'
)
BEGIN
	CREATE TABLE [dbo].[UserLogins](
        [LoginProvider] NVARCHAR(128) NOT NULL,        
		[ProviderKey] NVARCHAR(128) NOT NULL,
        [ProviderDisplayName] NVARCHAR(128) NULL,
        [UserId] INT NOT NULL,
        CONSTRAINT [PK_UserLogins] PRIMARY KEY CLUSTERED ([LoginProvider] ASC, [ProviderKey] ASC),
        CONSTRAINT [FK_UserLogins_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [dbo].[Users] ([Id]) ON DELETE CASCADE
	);

    CREATE NONCLUSTERED INDEX [IX_UserLogins_UserId]
    ON [dbo].[UserLogins]([UserId] ASC);
END


IF NOT EXISTS (
	SELECT 1 FROM INFORMATION_SCHEMA.TABLES
	WHERE TABLE_SCHEMA = 'dbo'
	AND TABLE_NAME = 'UserTokens'
)
BEGIN
	CREATE TABLE [dbo].[UserTokens](
        [UserId] INT NOT NULL,
        [LoginProvider] NVARCHAR(128) NOT NULL,
        [Name] NVARCHAR(128) NOT NULL,
        [Value] NVARCHAR(MAX) NULL,
        CONSTRAINT [PK_UserTokens] PRIMARY KEY CLUSTERED ([UserId] ASC, [LoginProvider] ASC, [Name] ASC),
        CONSTRAINT [FK_UserTokens_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [dbo].[Users] ([Id]) ON DELETE CASCADE
	);
END
