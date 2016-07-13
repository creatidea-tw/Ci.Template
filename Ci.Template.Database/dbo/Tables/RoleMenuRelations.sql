CREATE TABLE [dbo].[RoleMenuRelations] (
    [RoleId] UNIQUEIDENTIFIER NOT NULL,
    [MenuId] UNIQUEIDENTIFIER NOT NULL,
    CONSTRAINT [PK_RoleMenuRelations_1] PRIMARY KEY CLUSTERED ([RoleId] ASC, [MenuId] ASC),
    CONSTRAINT [FK_RoleMenuRelations_Menus] FOREIGN KEY ([MenuId]) REFERENCES [dbo].[Menus] ([Id]),
    CONSTRAINT [FK_RoleMenuRelations_Roles] FOREIGN KEY ([RoleId]) REFERENCES [dbo].[Roles] ([Id])
);

