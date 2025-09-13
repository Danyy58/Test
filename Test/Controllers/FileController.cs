using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Threading.Tasks;

namespace Тестовое.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FileController : ControllerBase
    {
        private static Dictionary<string, string[]> platforms = [];

        [HttpPost]
        public async Task<ActionResult<Dictionary<string, string[]>>> UploadFile(IFormFile file)
        {
            if (file is null || file.Length == 0 || Path.GetExtension(file.FileName).ToLower() != ".txt")
                return BadRequest("Загружен некорректный файл");

            try
            {
                using (var stream = file.OpenReadStream())
                {
                    using (var reader = new StreamReader(stream))
                    {
                        platforms.Clear();
                        string entry;
                        while ((entry = await reader.ReadLineAsync()) != null)
                        {
                            var str = entry.Split(':');
                            var key = str[0];
                            var values = str[1].Split(",", StringSplitOptions.RemoveEmptyEntries);
                            platforms.Add(key, values);  
                        }
                    }
                }
            }
            catch
            {
                return BadRequest("Файл содержит некорректные данные");
            }

            return Ok(platforms);
        }

        [HttpGet]
        public ActionResult<List<string>> GetPlatforms(string location)
        {
            if (string.IsNullOrEmpty(location))
                return BadRequest();

            var response = new List<string>();
            foreach (var p in platforms)
            {
                foreach (var l in p.Value)
                {
                    if (location.StartsWith(l))
                    {
                        response.Add(p.Key);
                        break;
                    }
                }
            }

            return Ok(response);
        }
    }
}
