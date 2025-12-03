using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BibliotekaAPI.Data;
using BibliotekaAPI.Models;
using BibliotekaAPI.DTOs;

namespace BibliotekaAPI.Controllers;

[ApiController]
[Route("copies")]
public class CopiesController : ControllerBase
{
    private readonly LibraryDbContext _context;

    public CopiesController(LibraryDbContext context)
    {
        _context = context;
    }

    // GET: copies
    [HttpGet]
    public async Task<ActionResult<IEnumerable<CopyDto>>> GetCopies()
    {
        var copies = await _context.Copies
            .Select(c => new CopyDto
            {
                Id = c.Id,
                BookId = c.BookId
            })
            .ToListAsync();

        return Ok(copies);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<CopyDto>> GetCopy(int id)
    {
        var copy = await _context.Copies.FindAsync(id);

        if (copy == null)
        {
            return NotFound();
        }

        var copyDto = new CopyDto
        {
            Id = copy.Id,
            BookId = copy.BookId
        };

        return Ok(copyDto);
    }

    // POST: copies
    [HttpPost]
    public async Task<ActionResult<CopyDto>> CreateCopy(CreateCopyDto createCopyDto)
    {
        // Sprawdzenie czy książka istnieje
        var book = await _context.Books.FindAsync(createCopyDto.BookId);
        if (book == null)
        {
            return BadRequest("Book with the specified BookId does not exist.");
        }

        var copy = new Copy
        {
            BookId = createCopyDto.BookId
        };

        _context.Copies.Add(copy);
        await _context.SaveChangesAsync();

        var copyDto = new CopyDto
        {
            Id = copy.Id,
            BookId = copy.BookId
        };

        return CreatedAtAction(nameof(GetCopy), new { id = copy.Id }, copyDto);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateCopy(int id, UpdateCopyDto updateCopyDto)
    {
        // Sprawdzenie czy książka istnieje
        var book = await _context.Books.FindAsync(updateCopyDto.BookId);
        if (book == null)
        {
            return BadRequest("Book with the specified BookId does not exist.");
        }

        var copy = await _context.Copies.FindAsync(id);

        if (copy == null)
        {
            return NotFound();
        }

        copy.BookId = updateCopyDto.BookId;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!CopyExists(id))
            {
                return NotFound();
            }
            throw;
        }

        return NoContent();
    }

    // DELETE: copies
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteCopy(int id)
    {
        var copy = await _context.Copies.FindAsync(id);
        if (copy == null)
        {
            return NotFound();
        }

        _context.Copies.Remove(copy);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool CopyExists(int id)
    {
        return _context.Copies.Any(e => e.Id == id);
    }
}

