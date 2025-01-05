namespace SharedModels.Requests;

// Model class for the incoming request
public class ModelRequest
{
    public string Input { get; set; }
    public Guid UserId { get; set; }
}
