namespace BoysheO.ProcessSystem
{
    public readonly struct Log
    {
        public readonly LogLevel Level;
        public readonly string Text;

        public Log(LogLevel level, string text)
        {
            Level = level;
            Text = text;
        }
    }
}