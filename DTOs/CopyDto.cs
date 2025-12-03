namespace BibliotekaAPI.DTOs;

public class CopyDto
{
    public int Id { get; set; }
    public int BookId { get; set; }
}

public class CreateCopyDto
{
    public int BookId { get; set; }
}

public class UpdateCopyDto
{
    public int BookId { get; set; }
}

