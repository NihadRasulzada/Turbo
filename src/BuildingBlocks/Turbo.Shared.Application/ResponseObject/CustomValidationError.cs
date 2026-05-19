namespace Turbo.Shared.Application.ResponseObject;

public class CustomValidationError
{
    public string ErrorMessage { get; set; }
    public string PropertyName { get; set; }

    public CustomValidationError() { }

    public CustomValidationError(string propertyName, string errorMessage)
    {
        PropertyName = propertyName;
        ErrorMessage = errorMessage;
    }
}