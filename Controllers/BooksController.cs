using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BibliotekaAPI.Data;
using BibliotekaAPI.Models;
using BibliotekaAPI.DTOs;

namespace BibliotekaAPI.Controllers;

[ApiController]
[Route("books")]
public class BooksController : ControllerBase
{
    private readonly LibraryDbContext _context;

    public BooksController(LibraryDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<BookDto>>> GetBooks([FromQuery] int? authorId)
    {
        var query = _context.Books.Include(b => b.Author).AsQueryable();

        if (authorId.HasValue)
        {
            query = query.Where(b => b.AuthorId == authorId.Value);
        }

        var books = await query
            .Select(b => new BookDto
            {
                Id = b.Id,
                Title = b.Title,
                Year = b.Year,
                Author = new AuthorDto
                {
                    Id = b.Author.Id,
                    FirstName = b.Author.FirstName,
                    LastName = b.Author.LastName
                }
            })
            .ToListAsync();

        return Ok(books);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<BookDto>> GetBook(double id)
    {
        if (id > int.MaxValue || id < int.MinValue)
        {
            return NotFound();
        }

        var book = await _context.Books
            .Include(b => b.Author)
            .FirstOrDefaultAsync(b => b.Id == (int)id);

        if (book == null)
        {
            return NotFound();
        }

        var bookDto = new BookDto
        {
            Id = book.Id,
            Title = book.Title,
            Year = book.Year,
            Author = new AuthorDto
            {
                Id = book.Author.Id,
                FirstName = book.Author.FirstName,
                LastName = book.Author.LastName
            }
        };

        return Ok(bookDto);
    }

    [HttpPost]
    public async Task<ActionResult<BookDto>> CreateBook(CreateBookDto createBookDto)
    {
        if (string.IsNullOrWhiteSpace(createBookDto.Title))
        {
            return BadRequest("Title cannot be empty.");
        }

        if (createBookDto.Year < 0)
        {
            return BadRequest("Year cannot be negative.");
        }

        var author = await _context.Authors.FindAsync(createBookDto.AuthorId);
        if (author == null)
        {
            return BadRequest("Author with the specified AuthorId does not exist.");
        }

        var book = new Book
        {
            Title = createBookDto.Title,
            Year = createBookDto.Year,
            AuthorId = createBookDto.AuthorId
        };

        _context.Books.Add(book);
        await _context.SaveChangesAsync();

        await _context.Entry(book).Reference(b => b.Author).LoadAsync();

        var bookDto = new BookDto
        {
            Id = book.Id,
            Title = book.Title,
            Year = book.Year,
            Author = new AuthorDto
            {
                Id = book.Author.Id,
                FirstName = book.Author.FirstName,
                LastName = book.Author.LastName
            }
        };

        return CreatedAtAction(nameof(GetBook), new { id = book.Id }, bookDto);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateBook(double id, UpdateBookDto updateBookDto)
    {
        if (id > int.MaxValue || id < int.MinValue)
        {
            return NotFound();
        }

        if (string.IsNullOrWhiteSpace(updateBookDto.Title))
        {
            return BadRequest("Title cannot be empty.");
        }

        if (updateBookDto.Year < 0)
        {
            return BadRequest("Year cannot be negative.");
        }

        var author = await _context.Authors.FindAsync(updateBookDto.AuthorId);
        if (author == null)
        {
            return BadRequest("Author with the specified AuthorId does not exist.");
        }

        var book = await _context.Books.FindAsync((int)id);

        if (book == null)
        {
            return NotFound();
        }

        book.Title = updateBookDto.Title;
        book.Year = updateBookDto.Year;
        book.AuthorId = updateBookDto.AuthorId;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!BookExists((int)id))
            {
                return NotFound();
            }
            throw;
        }

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteBook(double id)
    {
        if (id > int.MaxValue || id < int.MinValue)
        {
            return NotFound();
        }

        var book = await _context.Books.FindAsync((int)id);
        if (book == null)
        {
            return NotFound();
        }

        _context.Books.Remove(book);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool BookExists(int id)
    {
        return _context.Books.Any(e => e.Id == id);
    }
}