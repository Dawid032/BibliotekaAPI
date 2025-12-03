namespace BibliotekaAPI.Models;

public class Author
{
    public int Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    
    // Relacja: jeden autor → wiele książek
    public ICollection<Book> Books { get; set; } = new List<Book>();
}

