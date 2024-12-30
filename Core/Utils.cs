
namespace Ordo.Core
{
    internal class Utils
    {
        private static readonly char[] Spinner = { '-', '\\', '|', '/' };
        private static int Counter = 0;

        /// <summary>
        /// Returns a backspace character followed by the next spinner character.
        /// </summary>
        /// <returns>A string containing "\b" and the next spinner character.</returns>
        public static string SpinChar()
        {
            string result = "\b" + Spinner[Counter % Spinner.Length];
            Counter++;
            return result;
        }
    }
}
