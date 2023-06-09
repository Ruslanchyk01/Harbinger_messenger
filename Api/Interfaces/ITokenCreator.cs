using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Api.Models;

namespace Api.Interfaces
{
    public interface ITokenCreator
    {
        string CreateToken(AppUser appUser);
    }
}