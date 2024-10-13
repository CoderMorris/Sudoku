namespace Sudoku.Constants
{
    //Класс Constants для определения констант,
    //используемых в программе
    public class Constants
    {
        //Режимы сетки
        public const string Easy = "Easy";
        public const string Medium = "Medium";
        public const string Hard = "Hard";

        //Сообщения
        public const string PuzzleGenerated = "Сгенерирована головоломка судоку!";
        public const string PuzzleSolved = "Головоломка судоку решена!";
        public const string PuzzleCleared = "Сетка головоломки судоку очищена.";
        public const string PuzzleGridEmpty = "Сетка головоломки судоку пуста.";
        public const string PuzzleInvalidSolve = "Извините, головоломка судоку решена неправильно.";
        public const string PuzzleValidButNotCompleted = "Текущее состояние головоломки судоку правильное, но еще не завершено.";
        public const string PuzzleInvalidSolveState = "Извините, текущее состояние головоломки судоку неверно.";
        public const string PuzzleNoSolution = "Нет решения для этой головоломки судоку.";


        public const string ModErrorOn = "Включен режим без ошибок";
        public const string ModErrorOff = "Режим без ошибок выключен";

        public const string NoteOn = "Включен режим заметок";
        public const string NoteOff = "Режим заметок выключен";

        public const string Notes = "Для выполнения действия уберите с поля все заметки";

        public const string Error_Mod_Error = "Для режима без ошибок отключите режим заметок";
        public const string Error_Note = "Для режима заметок отключите режим без ошибок";

        //Шрифт
        public const string FontFamily = "Maiandra GD";
    }
}