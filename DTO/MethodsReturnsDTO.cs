using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sounds_New.DTO
{
    public class DefaultMethodResponseDTO : IDefaultMethodResponseDTO
    {
        public required bool IsOk { get; set; }
        public required string Message { get; set; }
        public required int StatusCode { get; set; }
    }

    public interface IDefaultMethodResponseDTO
    {
        public bool IsOk { get; set; }
        public string Message { get; set; }
        public int StatusCode { get; set; }
    }
}