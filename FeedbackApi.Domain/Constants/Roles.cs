namespace FeedbackApi.Domain.Constants
{
    public static class Roles
    {
        public const string GestorONG = nameof(GestorONG);
        public const string Doador = nameof(Doador);

        public const string GestorAccess = $"{GestorONG}";
        public const string DoadorAccess = $"{Doador}";
        public const string Todos = $"{GestorONG},{Doador}";
    }
}
