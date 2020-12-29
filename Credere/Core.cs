using System;
using System.Web;

namespace Credere
{
    public class Core
    {
        /// <summary>
        /// Será necessário configurar este módulo no arquivo Web.config do seu
        /// web e registrá-lo no IIS para que ele possa ser usado. Para obter mais informações,
        /// consulte o seguinte link: https://go.microsoft.com/?linkid=8101007
        /// </summary>

        public string Teste()
        {
            return "teste OK";
        }
    }
}
