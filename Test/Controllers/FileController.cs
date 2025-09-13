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
        private static Dictionary<string, string[]> tmp = [];

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
                        tmp.Clear();
                        string entry;
                        while ((entry = await reader.ReadLineAsync()) != null)
                        {
                            var str = entry.Split(':');
                            var key = str[0];
                            var values = str[1].Split(",", StringSplitOptions.RemoveEmptyEntries);
                            tmp.Add(key, values);   
                        }
                    }
                }
            }
            catch
            {
                return BadRequest("Файл содержит некорректные данные");
            }

            platforms.Clear();
            foreach (var pair in tmp)
            {
                platforms.Add(pair.Key, pair.Value);
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

            return response.Count == 0 ? NotFound("Платформы не найдены") : Ok(response);
        }
    }
}
