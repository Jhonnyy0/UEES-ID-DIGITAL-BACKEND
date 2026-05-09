using System;
using System.Collections.Generic;
using System.Text;

namespace UeesDigital.Domain.Interfaces
{
    public interface IJwtService
    {
        string GenerateToken(string userId, string email, IList<string> roles);
    }
}