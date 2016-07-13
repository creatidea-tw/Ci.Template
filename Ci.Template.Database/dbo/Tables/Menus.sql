CREATE TABLE [dbo].[Menus] (
    [Id]          UNIQUEIDENTIFIER CONSTRAINT [DF__Menus__Id__339FAB6E] DEFAULT (newid()) NOT NULL,
    [NatvieName]  NVARCHAR (50)    NOT NULL,
    [Controller]  NVARCHAR (50)    NULL,
    [Action]      NVARCHAR (50)    NULL,
    [Description] NVARCHAR (MAX)   NULL,
    [Url]         NVARCHAR (MAX)   NULL,
    [Type]        INT              NOT NULL,
    [ParentId]    UNIQUEIDENTIFIER NULL,
    [IsMenu]      BIT              CONSTRAINT [DF__Menus__IsMenu__3493CFA7] DEFAULT ((0)) NOT NULL,
    [Sort]        INT              NOT NULL,
    [IsDelete]    BIT              CONSTRAINT [DF__Menus__IsDelete__3587F3E0] DEFAULT ((0)) NOT NULL,
    [MuseumId]    UNIQUEIDENTIFIER NOT NULL,
    CONSTRAINT [PK_Menus] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Menus_Menus] FOREIGN KEY ([ParentId]) REFERENCES [dbo].[Menus] ([Id])
);


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'原生的名字', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Menus', @level2type = N'COLUMN', @level2name = N'NatvieName';

