using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using WebApplication1.Interfaces;
using WebApplication1.Models;
using WebApplication1.Repository;

namespace WebApplication1.Controllers
{
    
    [ApiController]

    public class DiffController : Controller
    {
        private IDiffRepository diffRepository;

        public DiffController(IDiffRepository diffRepository)
        {
            this.diffRepository = diffRepository;
        }

        [HttpGet]
        [Microsoft.AspNetCore.Mvc.Route("v1/diff/{id}")]
        public IActionResult GetDiff(int id)
        {
            var diff = diffRepository.GetDiff(id);

            if (diff == null || diff.Left == null || diff.Right == null) 
                return NotFound();

            return Ok(CalculateDifference(diff));
        }

        [HttpPut]
        [Microsoft.AspNetCore.Mvc.Route("v1/diff/{id}/left")]
        public IActionResult PutLeft(int id, [FromBody] JsonData jd)
        {
            return Put(id, jd, left: true);
        }

        [HttpPut]
        [Microsoft.AspNetCore.Mvc.Route("v1/diff/{id}/right")]
        public IActionResult PutRight(int id, [FromBody] JsonData jd)
        {
            return Put(id, jd, left: false);           
        }

        private IActionResult Put(int id, JsonData jd, bool left)
        {
            Byte[] dataByte;
            try // Try converting the string to byte array
            {
                dataByte = Convert.FromBase64String(jd.data);
            }
            catch // If the string isn't a proper representation of bytes
            {
                return BadRequest();
            }

            if (left)
                diffRepository.PutLeft(id, dataByte);
            else
                diffRepository.PutRight(id, dataByte);
            return Created(id.ToString(), jd); // Return the original data, since an empty 201 cannot be returned. 
        }

        /// <summary>
        /// Returns the required Json file that describes the difference between diff.Left and diff.Right.
        /// 
        /// If they are the same - "diffResultType": "Equals"
        /// If they are of different lengths - "diffResultType": "SizeDoNotMatch"
        /// If they are of same length but different - array "diffs" with tuples "offset" and "length" 
        /// that describe the differences. 
        /// </summary>
        /// <param name="diff">Diff object that needs to be checked. diff.Left and diff.Right must not be null. </param>
        /// <returns>Returns a Json file that describes the difference between diff.Left and diff.Right.</returns>
        private JsonResult CalculateDifference(Diff diff)
        {
            var l = diff.Left;
            var r = diff.Right;

            JsonResult j;

            if(l.Length != r.Length) //Lengths are not the same
            {
                j = new JsonResult(new { diffResultType = "SizeDoNotMatch" });
            }
            else if(l.SequenceEqual(r)) //Contents are the same
            {
                j = new JsonResult(new { diffResultType = "Equals" });
            }
            else //Lengths are the same, but the contents arent. Find diffs.
            {
                List<OffsetLengthPair> diffs = new List<OffsetLengthPair>();
                int offset = 0;
                int length = 0;
                for(var i = 0; i <= l.Length; i++)
                {
                    if (i < l.Length && l[i] != r[i]) //Current bytes don't match. 
                    {
                        if (length > 0) //At least one previous byte already didn't match
                            length++;
                        else //This is the first of potentially several consecutive mismatched bytes
                        {
                            length++; //Start counting
                            offset = i; //Set the offset to the first mismatched byte
                        }
                    }
                    else if (length > 0) //The bytes match and because length is more than 0, we need to save 
                                        // an offset/length pair and reset the length to 0. 
                    {
                        diffs.Add(new OffsetLengthPair(offset, length));
                        length = 0;
                    }
                }

                j = new JsonResult(new { diffResultType = "ContentDoNotMatch",  diffs = diffs });
            }

            return j;
        }

    }
}
