using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ValdymoSistema.Services
{
    public interface IEmailSender
    {
        void SendEmailAsync(string email, string subject, string htmlMessage);
    }
}
