namespace Back.Domain.Helpers;

public enum DatabaseEventType
{
    RowCreated,
    RowDeleted,
    RowUpdated,
    RowNotFound
}