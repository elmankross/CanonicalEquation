namespace App
{
    public enum Mode
    {
        Interactive,
        File
    }

    public class ConsoleArgs
    {
        public Mode? Mode { get; set; }
        public string FilePath { get; set; }
    }
}
