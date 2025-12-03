namespace BibliotekaAPI.Models;

public class Book
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public int Year { get; set; }
    
    // Relacja: jedna książka → wiele egzemplarzy
    public ICollection<Copy> Copies { get; set; } = new List<Copy>();
    
    // Relacja: wiele książek → jeden autor
    public int AuthorId { get; set; }
    public Author Author { get; set; } = null!;
}

