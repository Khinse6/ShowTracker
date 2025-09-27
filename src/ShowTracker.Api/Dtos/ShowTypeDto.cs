public record CreateShowTypeDto
{
    public required string Name { get; set; }
}

public record UpdateShowTypeDto
{
    public required string Name { get; set; }
}

public record ShowTypeDto
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
}



