using System.Threading.Tasks;

namespace AnalyseAudio_PInfo.Contracts.Services
{
    public interface IActivationService
    {
        Task ActivateAsync(object activationArgs);
    }
}
