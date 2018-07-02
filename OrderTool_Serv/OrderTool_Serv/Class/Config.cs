using System.Configuration;

namespace OrderTool_Serv.Class
{
    class Conf
    {
        /// <summary>DB接続情報</summary>
        public static readonly ConnectionStringSettings _settings = ConfigurationManager.ConnectionStrings["DBRemote"];

        public static readonly string _randStr = "0123456789ABCDEFGHIJKLNMOPQRSTUVWXYZabcdefghijklnmopqrstuvwxyz!+-*=,._";
    }
}
