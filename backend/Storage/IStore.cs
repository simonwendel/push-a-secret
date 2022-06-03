namespace Storage;

public interface IStore
{
    PeekResponse Peek(PeekRequest request);
    CreateResponse Create(CreateRequest request);
    ReadResponse Read(ReadRequest request);
    DeleteResponse Delete(DeleteRequest request);
}
