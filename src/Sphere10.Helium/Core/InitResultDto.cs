namespace Sphere10.Helium.Core
{
    public record InitResultDto
    {
        public InitResultDto(bool isInitSuccessful, string errorList)
        {
            IsInitSuccessful = isInitSuccessful;
            ErrorList = errorList;
        }

        public bool IsInitSuccessful { get; init; }

        public string ErrorList { get; init; }
    }
}