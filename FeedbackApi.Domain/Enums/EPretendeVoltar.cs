using System.ComponentModel;

namespace FeedbackApi.Domain
{
    public enum EPretendeVoltar
    {
        [Description("Sim")]
        Sim = 1,
        [Description("Talvez")]
        Talvez = 2,
        [Description("Não")]
        Nao = 3,
    }
}
