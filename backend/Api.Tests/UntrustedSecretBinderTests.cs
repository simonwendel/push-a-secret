using System;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.Xunit2;
using Domain;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Moq;
using Validation;
using Xunit;

namespace Api.Tests;

public class UntrustedSecretBinderTests
{
    private readonly Fixture fixture = new();
    private UntrustedValue<Secret>? model;
    private readonly UntrustedValue<Secret> expected;
    private readonly string expectedSerialized;

    public UntrustedSecretBinderTests()
    {
        var secret = new Secret("A128GCM", "xMY6HokU51VT8g02", "zb8HP7LFqYr+1fWZA5ZFAfIHz3Y=");
        expected = new UntrustedValue<Secret>(secret);
        expectedSerialized = JsonSerializer.Serialize(secret);
    }

    [Theory, AutoData]
    public void BindModelAsync_GivenNullBindingContext_ThrowsException(UntrustedSecretBinder sut)
    {
        Func<Task> binding = async () => await sut.BindModelAsync(null);
        binding.Should().ThrowAsync<ArgumentNullException>();
    }

    [Theory, AutoData]
    public async Task BindModelAsync_WhenBodyIsEmpty_DoesNotSetModel(UntrustedSecretBinder sut)
    {
        var context = CreateBinderContextWithValue(string.Empty);
        await sut.BindModelAsync(context);
        sut.BindModelAsync(context).IsCompleted.Should().BeTrue();
        model.Should().BeNull();
    }
    
    [Theory, AutoData]
    public void BindModelAsync_WhenBodyIsNotSerializedSecret_DoesNotSetModel(object value, UntrustedSecretBinder sut)
    {
        var context = CreateBinderContextWithValue(JsonSerializer.Serialize(value));
        sut.BindModelAsync(context).IsCompleted.Should().BeTrue();
        model.Should().NotBe(expected);
    }
    
    [Theory, AutoData]
    public async Task BindModelAsync_WhenBodyIsSerializedSecret_ReturnsModel(UntrustedSecretBinder sut)
    {
        var context = CreateBinderContextWithValue(expectedSerialized);
        await sut.BindModelAsync(context);
        sut.BindModelAsync(context).IsCompleted.Should().BeTrue();
        model.Should().Be(expected);
    }

    private ModelBindingContext CreateBinderContextWithValue(string value)
    {
        var fieldName = fixture.Create<string>();
        var state = new ModelStateDictionary();
        var bindingContext = new Mock<ModelBindingContext>();
        var httpContext = new Mock<HttpContext>();
        var request = new Mock<HttpRequest>();
        var body = new MemoryStream(Encoding.UTF8.GetBytes(value));
        
        httpContext.Setup(x => x.Request).Returns(request.Object);
        bindingContext.Setup(x => x.HttpContext).Returns(httpContext.Object);
        request.Setup(x => x.Body).Returns(body);
        bindingContext.Setup(x => x.FieldName).Returns(fieldName);
        bindingContext.Setup(x => x.ModelState).Returns(state);

        bindingContext.SetupSet(x => x.Result = It.IsAny<ModelBindingResult>())
            .Callback((ModelBindingResult x) => model = x.Model as UntrustedValue<Secret>);

        return bindingContext.Object;
    }
}
