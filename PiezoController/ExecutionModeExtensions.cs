namespace PiezoController
{
    public static class ExecutionModeExtensions
    {
        public static string ToString(this ExecutionMode value)
        {
            return value switch
            {
                ExecutionMode.SquareWave => "Square Wave",
                ExecutionMode.SineWave => "Sine Wave",
                ExecutionMode.Pulse => "Pulse",
                _ => string.Empty,
            };
        }

        public static ExecutionMode ToExecutionMode(this string value)
        {
            return value switch
            {
                "square wave" => ExecutionMode.SquareWave,
                "Square Wave" => ExecutionMode.SquareWave,
                "SquareWave" => ExecutionMode.SquareWave,
                "squareWave" => ExecutionMode.SquareWave,
                "squarewave" => ExecutionMode.SquareWave,
                "square" => ExecutionMode.SquareWave,
                "Square" => ExecutionMode.SquareWave,
                "sine wave" => ExecutionMode.SineWave,
                "Sine Wave" => ExecutionMode.SineWave,
                "SineWave" => ExecutionMode.SineWave,
                "pulse" => ExecutionMode.Pulse,
                "Pulse" => ExecutionMode.Pulse,
                _ => throw new ArgumentException(null, nameof(value)),
            };
        }
    }
}