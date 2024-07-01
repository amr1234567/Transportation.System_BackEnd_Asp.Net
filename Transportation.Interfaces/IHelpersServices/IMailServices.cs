using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Transportation.Core.Dto.Email;

namespace Transportation.Interfaces.IHelpersServices
{
    public interface IMailServices
    {
        void SendEmail(Message message);
    }
}
