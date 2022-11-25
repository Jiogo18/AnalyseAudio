using System.Threading.Tasks;

namespace AnalyseAudio.Contracts.Services
{
    public interface IActivationService
    {
        Task ActivateAsync(object activationArgs);
    }
}
