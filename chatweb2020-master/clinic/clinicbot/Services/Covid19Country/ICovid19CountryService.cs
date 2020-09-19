using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace clinicbot.Services.Covid19Country

    //INTERFAS
{
    public interface ICovid19CountryService
    {
        Task<Covid19CountryModel> Execute(string country);
    }
}
