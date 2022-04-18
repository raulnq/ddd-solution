GO

CREATE TABLE $schema$.[TaskLists] (
    [TaskListId] int NOT NULL,
    [Name] nvarchar(255) NOT NULL,
    [Description] nvarchar(255) NULL,
    CONSTRAINT [PK_TaskLists] PRIMARY KEY ([TaskListId]),
);

GO

CREATE SEQUENCE $schema$.[TaskList_Sequence]
AS [INT]
START WITH 1
INCREMENT BY 1
CACHE

GO
