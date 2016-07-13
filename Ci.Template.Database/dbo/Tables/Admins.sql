CREATE TABLE [dbo].[Admins] (
    [Id]            UNIQUEIDENTIFIER CONSTRAINT [DF_Admins_Id] DEFAULT (newid()) NOT NULL,
    [Account]       NVARCHAR (MAX)   NOT NULL,
    [Password]      NVARCHAR (MAX)   NOT NULL,
    [CreateTime]    DATETIME         CONSTRAINT [DF_Admins_CreateTime] DEFAULT (getdate()) NOT NULL,
    [IsDelete]      BIT              CONSTRAINT [DF_Admins_IsDelete] DEFAULT ((0)) NOT NULL,
    [DeleteTime]    DATETIME         NULL,
    [LastLoginTime] DATETIME         NULL,
    [LastLoginIp]   NVARCHAR (MAX)   NULL,
    [MuseumId]      UNIQUEIDENTIFIER NULL,
    CONSTRAINT [PK_Admins] PRIMARY KEY CLUSTERED ([Id] ASC)
);


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'管理者列表', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Admins', @level2type = N'COLUMN', @level2name = N'Id';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'帳號', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Admins', @level2type = N'COLUMN', @level2name = N'Account';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'密碼', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Admins', @level2type = N'COLUMN', @level2name = N'Password';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'建立時間', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Admins', @level2type = N'COLUMN', @level2name = N'CreateTime';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'是否刪除', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Admins', @level2type = N'COLUMN', @level2name = N'IsDelete';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'刪除時間', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Admins', @level2type = N'COLUMN', @level2name = N'DeleteTime';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'最後登入時間', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Admins', @level2type = N'COLUMN', @level2name = N'LastLoginTime';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'最後登入Ip', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Admins', @level2type = N'COLUMN', @level2name = N'LastLoginIp';

