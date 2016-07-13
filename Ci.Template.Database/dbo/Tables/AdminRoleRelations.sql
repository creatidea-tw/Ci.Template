CREATE TABLE [dbo].[AdminRoleRelations] (
    [AdminId] UNIQUEIDENTIFIER NOT NULL,
    [RoleId]  UNIQUEIDENTIFIER NOT NULL,
    CONSTRAINT [PK_AdminRoleRelations] PRIMARY KEY CLUSTERED ([AdminId] ASC, [RoleId] ASC),
    CONSTRAINT [FK_AdminRoleRelations_Admins1] FOREIGN KEY ([AdminId]) REFERENCES [dbo].[Admins] ([Id]),
    CONSTRAINT [FK_AdminRoleRelations_Roles] FOREIGN KEY ([RoleId]) REFERENCES [dbo].[Roles] ([Id])
);

