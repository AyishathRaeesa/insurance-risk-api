using InsuranceRiskApi.Application.Dtos;
using InsuranceRiskApi.Application.Interfaces;
using InsuranceRiskApi.Domain;
using Microsoft.AspNetCore.Mvc;

namespace InsuranceRiskApi.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class QuotesController : ControllerBase
{
    private readonly IQuoteService _quoteService;

    public QuotesController(IQuoteService quoteService)
    {
        _quoteService = quoteService;
    }

    /// <summary>
    /// Submits applicant details and returns a computed risk score + premium quote.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(QuoteResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<QuoteResponse>> CreateQuote([FromBody] CreateQuoteRequest request)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        var result = await _quoteService.CreateQuoteAsync(request);
        return CreatedAtAction(nameof(GetQuote), new { id = result.Id }, result);
    }

    /// <summary>
    /// Retrieves a single quote by id.
    /// </summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(QuoteResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<QuoteResponse>> GetQuote(int id)
    {
        var result = await _quoteService.GetQuoteByIdAsync(id);
        return result is null ? NotFound() : Ok(result);
    }

    /// <summary>
    /// Lists all quotes, optionally filtered by coverage type.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<QuoteResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<QuoteResponse>>> GetQuotes([FromQuery] CoverageType? coverageType)
    {
        var results = await _quoteService.GetAllQuotesAsync(coverageType);
        return Ok(results);
    }
}
