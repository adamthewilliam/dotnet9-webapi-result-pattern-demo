using Dotnet9.WebApi.ResultPattern.Demo.Contracts.Responses;
using Dotnet9.WebApi.ResultPattern.Demo.Domain;
using Dotnet9.WebApi.ResultPattern.Demo.FluentResults;
using FakeItEasy;
using FluentAssertions;
using FluentResults;
using FluentResults.Extensions.AspNetCore;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Dotnet9.WebApi.ResultPattern.Demo.UnitTests;

public class FluentResultsEndpointProfileTests
{
    private readonly FluentResultsEndpointProfile _sut;
    private readonly HttpContext _httpContext;

    public FluentResultsEndpointProfileTests()
    {
        var httpContextAccessor = A.Fake<IHttpContextAccessor>();
        _httpContext = new DefaultHttpContext();
        A.CallTo(() => httpContextAccessor.HttpContext).Returns(_httpContext);

        _sut = new FluentResultsEndpointProfile();
        _sut.SetHttpContextProvider(() => _httpContext);
    }

    [Fact]
    public void TransformFailedResult_WithValidationError_ReturnsBadRequest()
    {
        // Arrange
        var error = new ValidationError("Name", "Name is required");
        var result = Result.Fail(error);
        var context = new FailedResultToActionResultTransformationContext(result);

        // Act
        var actionResult = _sut.TransformFailedResultToActionResult(context);

        // Assert
        actionResult.Should().BeOfType<BadRequestObjectResult>();
        var badRequestResult = (BadRequestObjectResult)actionResult;
        badRequestResult.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
    }

    [Fact]
    public void TransformFailedResult_WithNotFoundError_ReturnsNotFound()
    {
        // Arrange
        var error = new NotFoundError("Penguin", Guid.NewGuid());
        var result = Result.Fail(error);
        var context = new FailedResultToActionResultTransformationContext(result);

        // Act
        var actionResult = _sut.TransformFailedResultToActionResult(context);

        // Assert
        actionResult.Should().BeOfType<NotFoundObjectResult>();
        var notFoundResult = (NotFoundObjectResult)actionResult;
        notFoundResult.StatusCode.Should().Be(StatusCodes.Status404NotFound);
    }

    [Fact]
    public void TransformFailedResult_WithConflictError_ReturnsConflict()
    {
        // Arrange
        var error = new ConflictError("Penguin", "Penguin with this name already exists");
        var result = Result.Fail(error);
        var context = new FailedResultToActionResultTransformationContext(result);

        // Act
        var actionResult = _sut.TransformFailedResultToActionResult(context);

        // Assert
        actionResult.Should().BeOfType<ConflictObjectResult>();
        var conflictResult = (ConflictObjectResult)actionResult;
        conflictResult.StatusCode.Should().Be(StatusCodes.Status409Conflict);
    }

    [Fact]
    public void TransformFailedResult_WithUnauthorizedError_ReturnsUnauthorized()
    {
        // Arrange
        var error = new UnauthorizedError("User is not authenticated");
        var result = Result.Fail(error);
        var context = new FailedResultToActionResultTransformationContext(result);

        // Act
        var actionResult = _sut.TransformFailedResultToActionResult(context);

        // Assert
        actionResult.Should().BeOfType<UnauthorizedObjectResult>();
        var unauthorizedResult = (UnauthorizedObjectResult)actionResult;
        unauthorizedResult.StatusCode.Should().Be(StatusCodes.Status401Unauthorized);
    }

    [Fact]
    public void TransformFailedResult_WithForbiddenError_ReturnsForbidden()
    {
        // Arrange
        var error = new ForbiddenError("Penguin", "User does not have access");
        var result = Result.Fail(error);
        var context = new FailedResultToActionResultTransformationContext(result);

        // Act
        var actionResult = _sut.TransformFailedResultToActionResult(context);

        // Assert
        actionResult.Should().BeOfType<ObjectResult>();
        var forbiddenResult = (ObjectResult)actionResult;
        forbiddenResult.StatusCode.Should().Be(StatusCodes.Status403Forbidden);
    }

    [Fact]
    public void TransformFailedResult_WithThrottlingError_ReturnsThrottled()
    {
        // Arrange
        var error = new ThrottlingError(DateTime.UtcNow.AddMinutes(5));
        var result = Result.Fail(error);
        var context = new FailedResultToActionResultTransformationContext(result);

        // Act
        var actionResult = _sut.TransformFailedResultToActionResult(context);

        // Assert
        actionResult.Should().BeOfType<ObjectResult>();
        var throttledResult = (ObjectResult)actionResult;
        throttledResult.StatusCode.Should().Be(StatusCodes.Status429TooManyRequests);
    }

    [Fact]
    public void TransformFailedResult_WithInternalServerError_ReturnsInternalServerError()
    {
        // Arrange
        var error = new InternalServerError("An unexpected error occurred");
        var result = Result.Fail(error);
        var context = new FailedResultToActionResultTransformationContext(result);

        // Act
        var actionResult = _sut.TransformFailedResultToActionResult(context);

        // Assert
        actionResult.Should().BeOfType<ObjectResult>();
        var serverErrorResult = (ObjectResult)actionResult;
        serverErrorResult.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
    }

    [Fact]
    public void TransformOkValueResult_WithPostMethodAndLocation_ReturnsCreated()
    {
        // Arrange
        var penguin = new PenguinResponseDto(Guid.NewGuid(), "Happy", "Emperor", 5);
        var result = Result.Ok(penguin);
        _httpContext.Request.Method = HttpMethods.Post;
        _httpContext.Items["Location"] = "/api/v1/penguins/1";
        var context = new OkResultToActionResultTransformationContext<Result<PenguinResponseDto>>(result);

        // Act
        var actionResult = _sut.TransformOkValueResultToActionResult(context);

        // Assert
        actionResult.Should().BeOfType<CreatedResult>();
        var createdResult = (CreatedResult)actionResult;
        createdResult.StatusCode.Should().Be(StatusCodes.Status201Created);
        createdResult.Location.Should().Be("/api/v1/penguins/1");
    }

    [Theory]
    [InlineData("PUT")]
    [InlineData("DELETE")]
    [InlineData("PATCH")]
    public void TransformOkNoValueResult_WithNoContentMethods_ReturnsNoContent(string method)
    {
        // Arrange
        var result = Result.Ok();
        _httpContext.Request.Method = method;
        var context = new OkResultToActionResultTransformationContext<Result>(result);

        // Act
        var actionResult = _sut.TransformOkNoValueResultToActionResult(context);

        // Assert
        actionResult.Should().BeOfType<NoContentResult>();
        var noContentResult = (NoContentResult)actionResult;
        noContentResult.StatusCode.Should().Be(StatusCodes.Status204NoContent);
    }
}