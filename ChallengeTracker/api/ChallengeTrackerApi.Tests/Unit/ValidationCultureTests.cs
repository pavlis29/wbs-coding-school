using System.ComponentModel.DataAnnotations;
using System.Globalization;
using ChallengeTrackerApi.Dtos;

namespace ChallengeTrackerApi.Tests.Unit;

public class ValidationCultureTests
{
    [Theory]
    [InlineData("de-DE")]
    [InlineData("cs-CZ")]
    [InlineData("en-US")]
    public void Decimal_validation_works_with_dot_and_comma_system_cultures(string cultureName)
    {
        var previous = CultureInfo.CurrentCulture;
        try
        {
            CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo(cultureName);
            var challenge = new ChallengeWrite("Read a book", null, "Learning", 0.01m, "pages",
                new DateOnly(2026, 11, 1), new DateOnly(2026, 11, 30));
            AssertValid(challenge);
            AssertValid(new CreateProgressRequest(Guid.NewGuid(), 0.01m));
            AssertValid(new CreateProgressRequest(Guid.NewGuid(), 0.001m));
            var invalid = new CreateProgressRequest(Guid.NewGuid(), 0m);
            Assert.False(Validator.TryValidateObject(invalid, new ValidationContext(invalid), [], true));
        }
        finally
        {
            CultureInfo.CurrentCulture = previous;
        }
    }

    private static void AssertValid(object value)
    {
        var results = new List<ValidationResult>();
        Assert.True(Validator.TryValidateObject(value, new ValidationContext(value), results, true),
            string.Join("; ", results.Select(x => x.ErrorMessage)));
    }
}
