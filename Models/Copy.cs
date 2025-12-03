namespace BibliotekaAPI.Models;

public class Copy
{
    public int Id { get; set; }
    
    // Relacja: wiele egzemplarzy → jedna książka
    public int BookId { get; set; }
    public Book Book { get; set; } = null!;
}

