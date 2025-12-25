using System.Diagnostics;
using System.Text;
using Atbash.Api.Models;

namespace Atbash.Api.Services
{
    public interface IAtbashService
    {
        OperationResponse Encrypt(string text);
        OperationResponse Decrypt(string text);
    }

    public class AtbashService : IAtbashService
    {
        private readonly Dictionary<char, char> _englishAtbashMap;
        private readonly Dictionary<char, char> _russianAtbashMap;

        public AtbashService()
        {
            // Инициализация словаря для английского алфавита
            _englishAtbashMap = new Dictionary<char, char>();
            string englishLower = "abcdefghijklmnopqrstuvwxyz";
            string englishUpper = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

            for (int i = 0; i < 26; i++)
            {
                _englishAtbashMap[englishLower[i]] = englishLower[25 - i];
                _englishAtbashMap[englishUpper[i]] = englishUpper[25 - i];
            }

            // Инициализация словаря для русского алфавита
            _russianAtbashMap = new Dictionary<char, char>();
            string russianLower = "абвгдеёжзийклмнопрстуфхцчшщъыьэюя";
            string russianUpper = "АБВГДЕЁЖЗИЙКЛМНОПРСТУФХЦЧШЩЪЫЬЭЮЯ";

            for (int i = 0; i < 33; i++)
            {
                _russianAtbashMap[russianLower[i]] = russianLower[32 - i];
                _russianAtbashMap[russianUpper[i]] = russianUpper[32 - i];
            }
        }

        public OperationResponse Encrypt(string text)
        {
            var sw = Stopwatch.StartNew();

            if (string.IsNullOrEmpty(text))
            {
                sw.Stop();
                return new OperationResponse
                {
                    Result = string.Empty,
                    ProcessedChars = 0,
                    ElapsedMilliseconds = sw.ElapsedMilliseconds,
                    Message = "текст пуст"
                };
            }

            var result = new StringBuilder(text.Length);
            int processedChars = 0;

            foreach (char c in text)
            {
                char transformedChar = c;

                // английский алфавит
                if (_englishAtbashMap.ContainsKey(c))
                {
                    transformedChar = _englishAtbashMap[c];
                    processedChars++;
                }
                // русский алфавит
                else if (_russianAtbashMap.ContainsKey(c))
                {
                    transformedChar = _russianAtbashMap[c];
                    processedChars++;
                }
                // остальные символы 

                result.Append(transformedChar);
            }

            sw.Stop();
            return new OperationResponse
            {
                Result = result.ToString(),
                ProcessedChars = processedChars,
                ElapsedMilliseconds = sw.ElapsedMilliseconds,
                Message = $"Готово {processedChars} символов"
            };
        }

        public OperationResponse Decrypt(string text) => Encrypt(text);
    }
}