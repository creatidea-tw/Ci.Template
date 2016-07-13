CREATE TABLE [dbo].[MenuLangs] (
    [MenuId]   UNIQUEIDENTIFIER NOT NULL,
    [Lang]     INT              NOT NULL,
    [Name]     NVARCHAR (MAX)   NOT NULL,
    [Contents] NVARCHAR (MAX)   NULL,
    [IsShow]   BIT              CONSTRAINT [DF_MenuLangs_IsShow] DEFAULT ((0)) NOT NULL,
    CONSTRAINT [PK_MenuLangs_1] PRIMARY KEY CLUSTERED ([MenuId] ASC, [Lang] ASC),
    CONSTRAINT [FK_MenuLangs_Menus] FOREIGN KEY ([MenuId]) REFERENCES [dbo].[Menus] ([Id])
);

