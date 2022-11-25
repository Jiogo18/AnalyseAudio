using System.Threading.Tasks;

namespace AnalyseAudio.Activation
{
    public interface IActivationHandler
    {
        bool CanHandle(object args);

        Task HandleAsync(object args);
    }
}
