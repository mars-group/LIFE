using Hik.Communication.ScsServices.Service;

namespace PhoneBookCommonLib
{
    [ScsService(Version="0.1")]
    public interface IYetAnotherService
    {
        string GetInformation();
    }
}
