using Collaborate.Auth.Api.Authorization;
using Collaborate.Auth.Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Collaborate.Auth.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class DocumentsController : ControllerBase
{
    [Authorize(Policy = AuthorizationPolicies.ReadDocuments)]
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(DocumentDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public ActionResult<DocumentDto> GetDocument(string id)
    {
        return Ok(new DocumentDto(id, "Sample document content"));
    }
}
