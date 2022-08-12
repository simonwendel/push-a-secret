namespace Storage;

internal interface IMongoDbRepositoryFactory
{
    MongoDbRepository Build();
}
