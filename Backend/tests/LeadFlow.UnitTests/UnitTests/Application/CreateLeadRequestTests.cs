using System.ComponentModel.DataAnnotations;
using LeadFlow.Application.DTOs.Leads;

namespace LeadFlow.UnitTests.Application;

public class CreateLeadRequestTests
{
    private static CreateLeadRequest ValidRequest() => new()
    {
        Name = "Anna Andersson",
        Email = "anna@example.com",
        Company = null,
        Message = "Would like more information."
    };

    private static List<ValidationResult> Validate(CreateLeadRequest request)
    {
        var results = new List<ValidationResult>();

        Validator.TryValidateObject(
            request,
            new ValidationContext(request),
            results,
            validateAllProperties: true);

        return results;
    }

    [Fact]
    public void ValidInput_WithoutCompany_PassesValidation()
    {
        Assert.Empty(Validate(ValidRequest()));
    }

    [Fact]
    public void InputAtMaximumLengths_PassesValidation()
    {
        var request = new CreateLeadRequest
        {
            Name = new string('a', 200),
            Email = "anna@example.com",
            Company = new string('a', 200),
            Message = new string('a', 4000)
        };

        Assert.Empty(Validate(request));
    }

    public static IEnumerable<object[]> InvalidRequests()
    {
        foreach (var value in new[] { "", "   " })
        {
            yield return new object[]
            {
                new CreateLeadRequest
                {
                    Name = value,
                    Email = "anna@example.com",
                    Message = "Hello"
                },
                nameof(CreateLeadRequest.Name)
            };

            yield return new object[]
            {
                new CreateLeadRequest
                {
                    Name = "Anna",
                    Email = value,
                    Message = "Hello"
                },
                nameof(CreateLeadRequest.Email)
            };

            yield return new object[]
            {
                new CreateLeadRequest
                {
                    Name = "Anna",
                    Email = "anna@example.com",
                    Message = value
                },
                nameof(CreateLeadRequest.Message)
            };
        }

        yield return new object[]
        {
            new CreateLeadRequest
            {
                Name = "Anna",
                Email = "invalid-email",
                Message = "Hello"
            },
            nameof(CreateLeadRequest.Email)
        };

        yield return new object[]
        {
            new CreateLeadRequest
            {
                Name = new string('a', 201),
                Email = "anna@example.com",
                Message = "Hello"
            },
            nameof(CreateLeadRequest.Name)
        };

        yield return new object[]
        {
            new CreateLeadRequest
            {
                Name = "Anna",
                Email = new string('a', 309) + "@example.com",
                Message = "Hello"
            },
            nameof(CreateLeadRequest.Email)
        };

        yield return new object[]
        {
            new CreateLeadRequest
            {
                Name = "Anna",
                Email = "anna@example.com",
                Company = new string('a', 201),
                Message = "Hello"
            },
            nameof(CreateLeadRequest.Company)
        };

        yield return new object[]
        {
            new CreateLeadRequest
            {
                Name = "Anna",
                Email = "anna@example.com",
                Message = new string('a', 4001)
            },
            nameof(CreateLeadRequest.Message)
        };
    }

    [Theory]
    [MemberData(nameof(InvalidRequests))]
    public void InvalidInput_ProducesErrorForExpectedProperty(
        CreateLeadRequest request,
        string propertyName)
    {
        var errors = Validate(request);

        Assert.Contains(
            errors,
            error => error.MemberNames.Contains(propertyName));
    }
}