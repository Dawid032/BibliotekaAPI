using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BibliotekaAPI.Data;
using BibliotekaAPI.Models;
using BibliotekaAPI.DTOs;

namespace BibliotekaAPI.Controllers;

[ApiController]
[Route("authors")]
public class AuthorsController : ControllerBase
{
    private readonly LibraryDbContext _context;

    public AuthorsController(LibraryDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<AuthorDto>>> GetAuthors()
    {
        var authors = await _context.Authors
            .Select(a => new AuthorDto
            {
                Id = a.Id,
                FirstName = a.FirstName,
                LastName = a.LastName
            })
            .ToListAsync();

        return Ok(authors);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<AuthorDto>> GetAuthor(double id)
    {
        if (id > int.MaxValue || id < int.MinValue)
        {
            return NotFound();
        }

        var author = await _context.Authors.FindAsync((int)id);

        if (author == null)
        {
            return NotFound();
        }

        var authorDto = new AuthorDto
        {
            Id = author.Id,
            FirstName = author.FirstName,
            LastName = author.LastName
        };

        return Ok(authorDto);
    }

    [HttpPost]
    public async Task<ActionResult<AuthorDto>> CreateAuthor(CreateAuthorDto createAuthorDto)
    {
        if (string.IsNullOrWhiteSpace(createAuthorDto.FirstName))
        {
            return BadRequest("First name cannot be empty.");
        }

        if (string.IsNullOrWhiteSpace(createAuthorDto.LastName))
        {
            return BadRequest("Last name cannot be empty.");
        }

        var author = new Author
        {
            FirstName = createAuthorDto.FirstName,
            LastName = createAuthorDto.LastName
        };

        _context.Authors.Add(author);
        await _context.SaveChangesAsync();

        var authorDto = new AuthorDto
        {
            Id = author.Id,
            FirstName = author.FirstName,
            LastName = author.LastName
        };

        return CreatedAtAction(nameof(GetAuthor), new { id = author.Id }, authorDto);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateAuthor(double id, UpdateAuthorDto updateAuthorDto)
    {
        if (id > int.MaxValue || id < int.MinValue)
        {
            return NotFound();
        }

        if (string.IsNullOrWhiteSpace(updateAuthorDto.FirstName))
        {
            return BadRequest("First name cannot be empty.");
        }

        if (string.IsNullOrWhiteSpace(updateAuthorDto.LastName))
        {
            return BadRequest("Last name cannot be empty.");
        }

        var author = await _context.Authors.FindAsync((int)id);

        if (author == null)
        {
            return NotFound();
        }

        author.FirstName = updateAuthorDto.FirstName;
        author.LastName = updateAuthorDto.LastName;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!AuthorExists((int)id))
            {
                return NotFound();
            }
            throw;
        }

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAuthor(double id)
    {
        if (id > int.MaxValue || id < int.MinValue)
        {
            return NotFound();
        }

        var author = await _context.Authors.FindAsync((int)id);
        if (author == null)
        {
            return NotFound();
        }

        _context.Authors.Remove(author);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool AuthorExists(int id)
    {
        return _context.Authors.Any(e => e.Id == id);
    }
}