using System.ComponentModel;

namespace FeedbackApi.Domain
{
    public enum ERole
    {
        [Description("Gestor da ONG")]
        GestorONG = 1,
        [Description("Doador")]
        Doador = 2,
    }
}
