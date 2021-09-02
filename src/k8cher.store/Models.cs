public class SetStoreRequest
{
    public string Name { get; set; } = String.Empty;
    public JsonDocument? Json { get; set; }
}

public class UpdateStorePropertyRequest
{
    public string Name { get; set; } = String.Empty;
    public string PropertyName { get; set; } = String.Empty;
    public string Json { get; set; } = "{}";
}