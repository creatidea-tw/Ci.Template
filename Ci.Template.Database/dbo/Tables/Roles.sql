CREATE TABLE [dbo].[Roles] (
    [Id]       UNIQUEIDENTIFIER CONSTRAINT [DF__Roles__Id__6319B466] DEFAULT (newid()) NOT NULL,
    [Name]     NVARCHAR (50)    NULL,
    [Sort]     INT              NULL,
    [IsDelete] BIT              CONSTRAINT [DF__Roles__IsDelete__640DD89F] DEFAULT ((0)) NOT NULL,
    CONSTRAINT [PK_Roles] PRIMARY KEY CLUSTERED ([Id] ASC)
);


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'權限角色', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Roles', @level2type = N'COLUMN', @level2name = N'Id';

