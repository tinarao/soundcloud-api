using Sounds_New.DTO;

namespace Sounds_New.Utils
{
    public class DefaultReturnFabric
    {
        public static DefaultMethodResponseDTO Created(string message)
        {
            return new DefaultMethodResponseDTO
            {
                IsOk = true,
                StatusCode = 201,
                Message = message,
            };
        }

        public static DefaultMethodResponseDTO BadRequest(string message)
        {
            return new DefaultMethodResponseDTO
            {
                IsOk = false,
                StatusCode = 400,
                Message = message,
            };
        }

        public static DefaultMethodResponseDTO NotFound(string message)
        {
            return new DefaultMethodResponseDTO
            {
                IsOk = false,
                StatusCode = 404,
                Message = message,
            };
        }

        public static DefaultMethodResponseDTO Forbidden(string message)
        {
            return new DefaultMethodResponseDTO
            {
                IsOk = false,
                StatusCode = 403,
                Message = message,
            };
        }

        public static DefaultMethodResponseDTO Unauthorized(string message)
        {
            return new DefaultMethodResponseDTO
            {
                IsOk = false,
                StatusCode = 401,
                Message = message,
            };
        }

        public static DefaultMethodResponseDTO Ok()
        {
            return new DefaultMethodResponseDTO
            {
                IsOk = true,
                StatusCode = 200,
                Message = "Ok"
            };
        }

        public static DefaultMethodResponseDTO Ok(string message)
        {
            return new DefaultMethodResponseDTO
            {
                IsOk = true,
                StatusCode = 200,
                Message = message
            };
        }

        public static DefaultMethodResponseDTO NoContent()
        {
            return new DefaultMethodResponseDTO
            {
                IsOk = true,
                StatusCode = 204,
                Message = "Ok"
            };
        }
    }
}